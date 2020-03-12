using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GunManager : MonoBehaviour
{
    private Player_Input_Handler m_inputHandler;
    [SerializeField] private Gun_Controller m_gunController; 

    //Gun Elements. 
    [SerializeField] private GameObject activeGun;
    [SerializeField] private Gun gunInfo;

    [SerializeField] private int primary;
    [SerializeField] private int secondary;
    [SerializeField] private int equipped; 

    //Gun Control.
    bool hasEquippedChanged;

    private bool m_canFire;
    private bool m_canAim;

    private int m_bulletsRemaining;

    //Avatar Components.
    private GameObject m_rotationHelper; 

    //UI Elements.
    private Text m_ammoCount;
    private GameObject m_crosshair;

    private void Start()
    {
        m_inputHandler = GetComponent<Player_Input_Handler>();
        m_crosshair = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/InGame/Crosshair").gameObject;

        primary = 0;
        secondary = 1;
        equipped = 1;

        m_canAim = true;
        m_canFire = true;
    }

    private void Update()
    {
        UpdateUI();

        if (m_bulletsRemaining > 0)
        {
            Fire();
        }

        Aim();

        StartReload();
        SwitchWeaponInput(); 
    }

    //Called after avatar is instantiated.
    public void SetupForShooting()
    {
        m_gunController = GetComponentInChildren<Gun_Controller>();

        activeGun = m_gunController.GetGunObject(primary);
        gunInfo = activeGun.GetComponent<Gun>(); 

        m_rotationHelper = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/RotationHelper").gameObject;
        gunInfo.Components.SetActive(true); 
    }

    private void SwitchWeaponInput()
    {
        if (m_inputHandler.GetSwitchWeapon() && !m_inputHandler.GetRightMousePressed())
        {
            SwitchWeapon();
        }
    }

    private void SwitchWeapon()
    {
        if (equipped == 1) //If the primary is active and a secondary is equipped. 
        {
            equipped = 2;
            gunInfo.Components.SetActive(false); //Disable components of currently equipped gun.
            activeGun = m_gunController.GetGunObject(secondary); //Set active gun to secondary gun.
            gunInfo = activeGun.GetComponent<Gun>(); //Get the gun info.
            gunInfo.Components.SetActive(true); //Activate the components of the newly equipped gun.
        }
        else if(equipped == 2)
        {
            equipped = 1;
            gunInfo.Components.SetActive(false);
            activeGun = m_gunController.GetGunObject(primary);
            gunInfo = activeGun.GetComponent<Gun>();
            gunInfo.Components.SetActive(true);
        }
    }

    public void SwitchPrimary(int weaponID)
    {
        primary = weaponID; 
    }

    public void SwitchSecondary(int weaponID)
    {
        secondary = weaponID; 
    }

    
    //------------------------------------------------//
    //Gun Functions.
    //------------------------------------------------//

    private void Aim()
    {
        if (m_inputHandler.GetRightMousePressed() && m_canAim)
        {
            m_crosshair.gameObject.SetActive(false);

            float step = gunInfo.AdsSpeed * Time.deltaTime;
            gunInfo.GFX.transform.position = Vector3.Slerp(gunInfo.GFX.transform.position, gunInfo.AimPos.position, step);
        }
        if (!m_inputHandler.GetRightMousePressed())
        {
            m_crosshair.gameObject.SetActive(true);
            float step = gunInfo.AdsSpeed * Time.deltaTime;
            gunInfo.GFX.transform.position = Vector3.Slerp(gunInfo.GFX.transform.position, gunInfo.HipPos.position, step);
        }
    }

    private void Fire()
    {
        if (m_inputHandler.GetLeftMousePressed())
        {
            if (m_canFire)
            {
                gunInfo.MuzzleFlash.Play();

                m_rotationHelper.transform.Rotate(new Vector3(gunInfo.YKickBase, 0, 0), Space.Self); //Vertical recoil.
                transform.Rotate(new Vector3(0, gunInfo.XKickBase, 0), Space.Self); //Horizontal recoil.

                m_bulletsRemaining--; //Reduce number of bullets remaining.

                StartCoroutine(WaitForShot()); //Delay next shot.
            }
        }
    }

    //If the player has fired any bullets begin reloading.
    private void StartReload()
    {
        if (m_bulletsRemaining < gunInfo.ClipSize && m_inputHandler.GetReloadPressed())
        {
            StartCoroutine(WaitForReload()); //Delay reload.
        }
    }

    private void FinishReload()
    {
        m_bulletsRemaining = gunInfo.ClipSize; //Replenish bullets.
        m_canFire = true; //Allow the player to shoot again.
    }

    private void UpdateUI()
    {
        //m_ammoCount.text = m_bulletsRemaining + "/" + gunInfo.ClipSize;
    }

    private IEnumerator WaitForReload()
    {
        m_canFire = false; //Prevent player from shooting.

        yield return new WaitForSeconds(gunInfo.ReloadTime); //Wait...

        FinishReload(); //Replenish ammo and allow player to fire again. 
    }

    private IEnumerator WaitForShot()
    {
        m_canFire = false; //Stop player from shooting.

        yield return new WaitForSeconds(gunInfo.ShotDelay); //Wait for time delay.

        m_canFire = true; //Allow shooting again.
    }
}

