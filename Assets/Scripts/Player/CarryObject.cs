using UnityEngine;
using System.Collections;

public class CarryObject : MonoBehaviour
{
    public float interactDistance = 3;
    public float carryDistance = 2;
    public LayerMask interactLayer;

    private Transform carryObject;
    private bool haveObject;

    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            if (Input.GetMouseButtonDown(1))
            {
                carryObject = hit.transform;
                carryObject.GetComponent<Rigidbody>().useGravity = false;
                haveObject = true;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (haveObject)
            {
                haveObject = false;
                carryObject.GetComponent<Rigidbody>().useGravity = true;
                carryObject = null;
            }
        }

        if (haveObject)
        {
            carryObject.position = Vector3.Lerp(carryObject.position, Camera.main.transform.position + Camera.main.transform.forward * carryDistance, Time.deltaTime * 8);
        }
    }
}