using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class GunManager : MonoBehaviourPunCallbacks
{
    private Player_Input_Handler m_inputHandler;
    private Player_Stats m_playerStats;
    [SerializeField] private Gun_Controller m_gunController;
    private InventoryUI UI;

    //Gun Elements. 
    public struct GunInfo
    {
        public GameObject gunComponents;
        public Gun gunController;
    }

    private GunInfo primary;
    private GunInfo secondary;
    private GunInfo activeGun;

    private int equipped;
    private Gun equippedGun;

    //GFX
    private GameObject bloodSplatterObject;
    private ParticleSystem bloodSplatterEffect;

    //Gun Control.
    bool hasEquippedChanged; //Has the player switched from primary to secondary. 

    private bool m_canFire; //False if no gun equipped or reloading. 
    private bool m_canAim; //False if already aiming or in menus. 

    private int m_bulletsLeftInClip;

    //Avatar Components.
    private GameObject m_rotationHelper; 

    //UI Elements.
    private Text m_ammoCount;
    private Text m_gunName;

    private GameObject m_reloadingText;
    private GameObject m_crosshair;

    [SerializeField] private Camera m_camera; 
    private Vector3 m_centreOfScreen; 

    private void Start()
    {
        m_inputHandler = GetComponent<Player_Input_Handler>();
        m_playerStats = GetComponent<Player_Stats>(); 

        m_ammoCount = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/InGame/AmmoCounter").GetComponent<Text>();
        m_gunName = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/InGame/GunName").GetComponent<Text>();        

        m_reloadingText = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/InGame/ReloadingText").gameObject;
        m_crosshair = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/InGame/Crosshair").gameObject;

        m_camera = GetComponentInChildren<Camera>(); 

        m_centreOfScreen = new Vector3(0.5f, 0.5f, 0);

        UI = GetComponent<InventoryUI>(); 

        bloodSplatterObject = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/BloodSplatter").gameObject; 
        bloodSplatterEffect = bloodSplatterObject.GetComponent<ParticleSystem>();

        equipped = 0;

        m_canAim = true;
        m_canFire = true;
    }

    private void Update()
    {
        UpdateUI();

        if (m_bulletsLeftInClip > 0)
        {
            Fire();
        }

        Aim();

        StartReload();
        SwitchWeaponInput(); 
    }

    private void SwitchWeaponInput()
    {
        if (m_inputHandler.GetSwitchWeapon() && !m_inputHandler.GetRightMousePressed())
        {
            if (equipped == 1 && secondary.gunController != null) //If the primary is active and a secondary is equipped. 
            {
                equipped = 2;
                SwapWeapon(equipped);
            }
            else if (equipped == 2 && primary.gunComponents != null)
            {
                equipped = 1;
                SwapWeapon(equipped); 
            }
            else if (equipped == 0)
            {
                Debug.Log("[INFO] You have no weapons.");
            }
        }       
    }

    private void SwapWeapon(int weapon)
    {       
        if(activeGun.gunController != null)
        {
            activeGun.gunComponents.SetActive(false); 
        }
        if(weapon == 1)
        {
            activeGun = primary;
            activeGun.gunComponents.SetActive(true);
            equippedGun = activeGun.gunController; 
            m_bulletsLeftInClip = equippedGun.ClipSize;
        }
        else if (weapon == 2)
        {
            activeGun = secondary;
            activeGun.gunComponents.SetActive(true);
            equippedGun = activeGun.gunController;
            m_bulletsLeftInClip = equippedGun.ClipSize;
        }              
    }

    public void SwitchPrimary(GameObject components, Gun controller)
    {
        GunInfo temp;

        temp.gunComponents = components;
        temp.gunController = controller;

        primary = temp; 

        if (equipped == 0 || equipped == 1)
        {
            equipped = 1;
            SwapWeapon(equipped); 
        }
    }

    public void SwitchSecondary(GameObject components, Gun controller)
    {
        GunInfo temp;

        temp.gunComponents = components;
        temp.gunController = controller;

        secondary = temp; 

        if (equipped == 0 || equipped == 2)
        {
            equipped = 2;
            SwapWeapon(equipped); 
        }
    }

    
    //------------------------------------------------//
    //Gun Functions.
    //------------------------------------------------//

    private void Aim()
    {
        if(equipped != 0)
        {
            if (m_inputHandler.GetRightMousePressed() && m_canAim)
            {
                m_crosshair.gameObject.SetActive(false);

                float step = equippedGun.AdsSpeed * Time.deltaTime;
                equippedGun.GFX.transform.position = Vector3.Slerp(equippedGun.GFX.transform.position, equippedGun.AimPos.position, step);
            }
            if (!m_inputHandler.GetRightMousePressed())
            {
                m_crosshair.gameObject.SetActive(true);
                float step = equippedGun.AdsSpeed * Time.deltaTime;
                equippedGun.GFX.transform.position = Vector3.Slerp(equippedGun.GFX.transform.position, equippedGun.HipPos.position, step);
            }
        }
    }

    private void Fire()
    {
        if (m_inputHandler.GetLeftMousePressed() && equipped != 0)
        {
            if (m_canFire)
            {             
                equippedGun.MuzzleFlash.Play();

                Ray ray = m_camera.ViewportPointToRay(m_centreOfScreen);
                RaycastHit hit;
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.green);

                if (Physics.Raycast(ray, out hit))
                {
                    if(hit.transform.gameObject.tag == "Monster")
                    {
                        m_playerStats.Money += 10; 

                        var rotation = Vector3.up * 180.0f; 

                        bloodSplatterObject.transform.position = hit.transform.position;
                        bloodSplatterObject.transform.Rotate(rotation, Space.World); 
                        bloodSplatterEffect.Play();

                        ZombieController zombieController = hit.transform.gameObject.GetComponent<ZombieController>();

                        zombieController.Health -= equippedGun.Base_damage;
                        zombieController.LastPlayer = m_playerStats; 
                    }
                }

                m_rotationHelper.transform.Rotate(new Vector3(equippedGun.YKickBase, 0, 0), Space.Self); //Vertical recoil.
                transform.Rotate(new Vector3(0, equippedGun.XKickBase, 0), Space.Self); //Horizontal recoil.

                m_bulletsLeftInClip--; //Reduce number of bullets remaining.

                StartCoroutine(WaitForShot()); //Delay next shot.
            }
        }
    }

    //If the player has fired any bullets begin reloading.
    private void StartReload()
    {
        if(equipped != 0)
        {
            if (m_bulletsLeftInClip < equippedGun.ClipSize && equippedGun.AmmoRemaining > 0 && m_inputHandler.GetReloadPressed())
            {
                if(equippedGun.AmmoRemaining > (equippedGun.ClipSize - m_bulletsLeftInClip))
                {               
                    StartCoroutine(WaitForReload()); 
                }
                else if(equippedGun.AmmoRemaining < (equippedGun.ClipSize - m_bulletsLeftInClip)) 
                {
                    m_bulletsLeftInClip = equippedGun.AmmoRemaining + m_bulletsLeftInClip;
                    equippedGun.AmmoRemaining = 0; 
                }         
            }
        }
    }

    private void FinishReload()
    {
        equippedGun.AmmoRemaining -= equippedGun.ClipSize - m_bulletsLeftInClip; //Subtract from bullet reserves.
        m_bulletsLeftInClip = equippedGun.ClipSize; //Replenish bullets.
        m_canFire = true; //Allow the player to shoot again.
    }

    
    private void UpdateUI() //Update the player HUD. 
    {
        if (equipped != 0)
        {
            m_ammoCount.text = m_bulletsLeftInClip + "/" + equippedGun.AmmoRemaining;
            m_gunName.text = equippedGun.Name;
        }
        else
        {
            m_gunName.text = "fist";
            m_ammoCount.text = "n/a"; 
        }
    }

    //------------------------------------------------------------------------------
    //Public Functions.
    //------------------------------------------------------------------------------
    public void SetupForShooting()  //Called after avatar is instantiated.
    {        
        m_rotationHelper = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/RotationHelper").gameObject;       
    }

    //------------------------------------------------------------------------------
    //Photon Functions.
    //------------------------------------------------------------------------------
    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        stream.SendNext()
    //    }
    //    else
    //    {
    //        stream.ReceiveNext()
    //    }
    //}

    //------------------------------------------------------------------------------
    //Co-Routines.
    //------------------------------------------------------------------------------
    private IEnumerator WaitForReload()
    {
        m_reloadingText.gameObject.SetActive(true); 

        m_canFire = false; //Prevent player from shooting.

        yield return new WaitForSeconds(equippedGun.ReloadTime); //Wait...

        m_reloadingText.gameObject.SetActive(false);

        FinishReload(); //Replenish ammo and allow player to fire again. 
    }

    private IEnumerator WaitForShot()
    {
        m_canFire = false; //Stop player from shooting.

        yield return new WaitForSeconds(equippedGun.ShotDelay); //Wait for time delay.

        m_canFire = true; //Allow shooting again.
    }
}

