using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    PlayerCharacterController _player;

    void Start()
    {
        if (gameObject.tag == "Player")
        {
            _player = GetComponent<PlayerCharacterController>();
        }
    }
    void Update()
    {
        if(currentHealth <= 0)
        {
            _player.SetState(_player.deathstate);
        }
    }
}
