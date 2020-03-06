using Photon.Pun; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun_Controller : MonoBehaviour
{
    //Networking Components.
    private PhotonView PV;

    //Gun Settings.
    [SerializeField] private string m_name; //Name of the weapon.

    [SerializeField] private float m_weight; //How it affects movement speed.

    [SerializeField] private float m_base_damage; //The amount of damage it deals at level 1 with no attachments

    [SerializeField] private int m_clipSize; //The maximum ammo capacity of the guns magazines.
    [SerializeField] private float m_reloadTime; //The amount of time it to takes to reload the gun.
    private int m_bulletsRemaining; //The number of bullets remaining in the clip. 

    [SerializeField] private int m_fireType; //Whether the gun is single shot, semi, or fully automatic. 

    [SerializeField] private float m_yKickBase; //The base vertical recoil amount.
    [SerializeField] private float m_xKickBase; //The base horizontal recoil amount. 

    [SerializeField] private float m_shotDelay; //How many seconds between each shot (fire rate);
    [SerializeField] private float m_adsSpeed; //How fast the gun moves from the hip-fire position to the ADS position;

    [SerializeField] private float m_maxDamageRange; //The maximum range before bullet damage decreases.

    [SerializeField] private float m_currentXP; //How much progress has been made towards levelling up. 
    [SerializeField] private float m_xpToLevel; //How much XP is required for the next level.

    [SerializeField] private GameObject m_rotationHelper; //Handle to the camera rotation point for recoil.
    [SerializeField] private GameObject m_player; //Handle to the player for horizontal recoil.

    [SerializeField] private ParticleSystem m_muzzleFlash;

    [SerializeField] private Transform m_hipPos;
    [SerializeField] private Transform m_aimPos;
    
    private bool m_isLerpingToAim;
    private bool m_isLerpingToHip; 

    private float m_distance;

    [SerializeField] private Text m_ammoCount;
    [SerializeField] private Image m_crosshair;

    [SerializeField] private List<GameObject> attachmentList;

    private Player_Input_Handler m_inputHandler; //Handle to the input manager.

    private bool m_canFire;

    private bool m_canAim;

    private int m_level;

    private void Start()
    {
        m_inputHandler = GetComponentInParent<Player_Input_Handler>();

        m_bulletsRemaining = m_clipSize;
        m_canAim = true;
        m_canFire = true;
        m_distance = Vector3.Distance(m_aimPos.position, m_hipPos.position);
        m_isLerpingToAim = false;
        m_isLerpingToHip = false;
    }

    void Update()
    {
        UpdateUI();

        if (m_bulletsRemaining > 0)
        {
            Fire();
        }

        Aim();

        StartReload();
    }

    private void Aim()
    {
        if (m_inputHandler.GetRightMousePressed() && m_canAim)
        {
            m_crosshair.gameObject.SetActive(false);

            float step = m_adsSpeed * Time.deltaTime; 
            transform.position = Vector3.Slerp(transform.position, m_aimPos.position, step);
        }
        if (!m_inputHandler.GetRightMousePressed())
        {
            m_crosshair.gameObject.SetActive(true);
            float step = m_adsSpeed * Time.deltaTime;
            transform.position = Vector3.Slerp(transform.position, m_hipPos.position, step);
        }
    }

    private void Fire()
    {
        if (m_inputHandler.GetLeftMousePressed())
        {
            if (m_canFire)
            {
                m_muzzleFlash.Play();

                m_rotationHelper.transform.Rotate(new Vector3(-m_yKickBase, 0, 0), Space.Self); //Vertical recoil.
                m_player.transform.Rotate(new Vector3(0, m_xKickBase, 0), Space.Self); //Horizontal recoil.

                m_bulletsRemaining--; //Reduce number of bullets remaining.

                StartCoroutine(WaitForShot()); //Delay next shot.
            }
        }
    }

    //If the player has fired any bullets begin reloading.
    private void StartReload()
    {
        if (m_bulletsRemaining < m_clipSize && m_inputHandler.GetReloadPressed())
        {
            StartCoroutine(WaitForReload()); //Delay reload.
        }
    }

    private void FinishReload()
    {
        m_bulletsRemaining = m_clipSize; //Replenish bullets.
        m_canFire = true; //Allow the player to shoot again.
    }

    private void UpdateUI()
    {
        m_ammoCount.text = m_bulletsRemaining + "/" + m_clipSize;
    }

    private IEnumerator WaitForReload()
    {
        m_canFire = false; //Prevent player from shooting.

        yield return new WaitForSeconds(m_reloadTime); //Wait...

        FinishReload(); //Replenish ammo and allow player to fire again. 
    }

    private IEnumerator WaitForShot()
    {
        m_canFire = false; //Stop player from shooting.

        yield return new WaitForSeconds(m_shotDelay); //Wait for time delay.

        m_canFire = true; //Allow shooting again.
    }
}
