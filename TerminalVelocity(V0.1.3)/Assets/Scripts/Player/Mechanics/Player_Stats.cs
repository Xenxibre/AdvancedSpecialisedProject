using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Stats : MonoBehaviour
{
    private bool isAlive;
    private float health;
    private float money;
    private Transform position; 

    [SerializeField] private ZombieManager zombManager;

    private void Start()
    {
        isAlive = true;

        Position = GetComponent<Transform>(); 
        zombManager = GameObject.FindGameObjectWithTag("ZombieManager").GetComponent<ZombieManager>();
        zombManager.AddToPlayerList(this);     
    }

    public void TakeDamage(float amount)
    {
        health = health - amount; 
        if (health <= 0)
        {
            IsAlive = false; 
        }
    }

    //Getters & Setters
    public float Health { get => health; set => health = value; }
    public float Money { get => money; set => money = value; }
    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public Transform Position { get => position; set => position = value; }
}
