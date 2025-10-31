using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crought : MonoBehaviour
{
    CharacterController characterCollider;
    void Start()
    {
        characterCollider = gameObject.GetComponent<CharacterController>  ();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            characterCollider.height = 0.5f;
        }
        else
        {
            characterCollider.height = 1.8f;
        }
    }
}
