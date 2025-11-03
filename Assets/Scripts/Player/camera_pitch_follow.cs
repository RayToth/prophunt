using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform eyeTransform;
    [SerializeField] private Transform gunTransform;
    //[SerializeField] private float maxHeadAngle = 60f; // korlát, hogy ne törjön ki a nyak :)
    [SerializeField] private float followSpeed = 1f;  // smooth követés

    private void LateUpdate()
    {
        // Csak a függõleges (pitch) rotációt vesszük a kamerából
        float cameraPitch = cameraTransform.localEulerAngles.x;
        if (cameraPitch > 180) cameraPitch -= 360; // Unity rotációs fix

        // Limitáljuk a fej mozgását
        //cameraPitch = Mathf.Clamp(cameraPitch, -maxHeadAngle, maxHeadAngle);

        // Szem jelenlegi rotációja
        Vector3 eyeEuler = eyeTransform.localEulerAngles;
        eyeEuler.x = Mathf.LerpAngle(eyeEuler.x, cameraPitch, Time.deltaTime * followSpeed);
        eyeTransform.localEulerAngles = eyeEuler;

        // Gun jelenlegi rotációja
        Vector3 gunEuler = gunTransform.localEulerAngles;
        gunEuler.x = Mathf.LerpAngle(gunEuler.x, cameraPitch, Time.deltaTime * followSpeed);
        gunTransform.localEulerAngles = gunEuler;
    }
}
