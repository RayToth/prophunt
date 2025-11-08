using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pistol_bullet : MonoBehaviour
{
   private void OnCollisionEnter(Collision collision)
   {
        Destroy(gameObject);
   }
}
