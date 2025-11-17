using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
   public int damage = 20;

   private void OnCollisionEnter(Collider other)
   {
        Debug.Log("Collidál");
        PlayerHP player = other.GetComponent<PlayerHP>();
        if(player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
   }
}
