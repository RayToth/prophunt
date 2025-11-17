using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
   public int damage = 20;


   private void OnTriggerEnter(Collider other)
   {
        PlayerHP player = other.GetComponent<PlayerHP>();
        if(player != null)
        {
            Debug.Log(player.currentHealth);
        }

        

        if (player != null)
        {
            player.TakeDamage(damage);
        }

        Destroy(gameObject); // Destroy bullet
   }
}
