using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;
    [SerializeField]
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage()
    {
        maxHealth -= 5;
        if (maxHealth <= 0)
            Dead();
    }

    void Dead()
    {
        Destroy(gameObject);
    }

    private void OncollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("bullet"))
            TakeDamage();
    }
}
