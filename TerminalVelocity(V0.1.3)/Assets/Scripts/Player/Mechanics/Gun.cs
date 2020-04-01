using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    //Gun Info. 
    [SerializeField] GameObject m_components;
    [SerializeField] GameObject m_GFX; 

    [SerializeField] private string m_name; //Name of the weapon.
    [SerializeField] private int m_id; 

    [SerializeField] private int m_ammoRemaining; //Total number of bullets remaining. 
    [SerializeField] private int m_clipSize; //The maximum ammo capacity of the guns magazines.
    [SerializeField] private int m_fireType; //Whether the gun is single shot, semi, or fully automatic. 
    [SerializeField] private int m_slot; 

    [SerializeField] private float m_base_damage; //The amount of damage it deals at level 1 with no attachments
    [SerializeField] private float m_reloadTime; //The amount of time it to takes to reload the gun.
    [SerializeField] private float m_yKickBase; //The base vertical recoil amount.
    [SerializeField] private float m_xKickBase; //The base horizontal recoil amount. 
    [SerializeField] private float m_shotDelay; //How many seconds between each shot (fire rate);
    [SerializeField] private float m_adsSpeed; //How fast the gun moves from the hip-fire position to the ADS position;

    //Gun Child Objects. 
    [SerializeField] private ParticleSystem m_muzzleFlash;

    [SerializeField] private Transform m_hipPos;
    [SerializeField] private Transform m_aimPos;

    //Getters.
    public string Name { get => m_name; set => m_name = value; }
    public Transform HipPos { get => m_hipPos; set => m_hipPos = value; }
    public Transform AimPos { get => m_aimPos; set => m_aimPos = value; }
    public float AdsSpeed { get => m_adsSpeed; set => m_adsSpeed = value; }
    public ParticleSystem MuzzleFlash { get => m_muzzleFlash; set => m_muzzleFlash = value; }
    public float YKickBase { get => m_yKickBase; set => m_yKickBase = value; }
    public float XKickBase { get => m_xKickBase; set => m_xKickBase = value; }
    public float ReloadTime { get => m_reloadTime; set => m_reloadTime = value; }
    public float Base_damage { get => m_base_damage; set => m_base_damage = value; }
    public int ClipSize { get => m_clipSize; set => m_clipSize = value; }
    public float ShotDelay { get => m_shotDelay; set => m_shotDelay = value; }
    public GameObject GFX { get => m_GFX; set => m_GFX = value; }
    public GameObject Components { get => m_components; set => m_components = value; }
    public int AmmoRemaining { get => m_ammoRemaining; set => m_ammoRemaining = value; }
    public int Slot { get => m_slot; set => m_slot = value; }
    public int Id { get => m_id; set => m_id = value; }
}
