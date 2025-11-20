using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public GameObject el;
    public GameObject halt;
    public Rigidbody mozgas;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // currentHealth = currentHealth - damage;
        Debug.Log("Player HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }

    }

    void Die()
    /*{
        Debug.Log("Játékos meghalt!");
        Transform target = transform.Find("alive_player_Capsule");
        if (target != null)
            Destroy(target.gameObject);
    }*/

    /*{
        Debug.Log("Játékos meghalt!");

        Transform el = transform.Find("el");
        Transform halt = transform.Find("halt");

        if (el != null)
            el.gameObject.SetActive(false);

        if (halt != null)
            halt.gameObject.SetActive(true);
    }*/
    {
        Debug.Log("Játékos meghalt!");

        el.SetActive(false);
        halt.SetActive(true);
        Object.Destroy(mozgas);
    }
}
