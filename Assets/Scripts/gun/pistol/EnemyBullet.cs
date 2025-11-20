using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
   public int damageAmount = 20;


   private void OnTriggerEnter(Collider collider)
   {
        Health targetPlayerHP = collider.GetComponent<Health>();

        if (targetPlayerHP != null)
        {
            Debug.Log(targetPlayerHP.HP);
            targetPlayerHP.TakeDamageServerRpc(damageAmount);
        }

        Destroy(gameObject); // Destroy bullet
   }
}
