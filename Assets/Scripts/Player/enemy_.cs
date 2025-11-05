using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health = 100;

    void TakeDamage()
    {
        health -= 5;
        if (health <= 0)
            Dead();
    }

    void Dead()
    {
        Destroy(gameObject);
    }

    private void OncollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("pistol_bullet"))
            TakeDamage();
    }
}
