using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform lefteyeTransform;
    [SerializeField] private Transform righteyeTransform;
    [SerializeField] private Transform gunTransform;
    [SerializeField] private Transform cam_model_Transform;
    [SerializeField] private float maxHeadAngle = 6f; // korlát, hogy ne törjön ki a nyak :)
    [SerializeField] private float followSpeed = 1f;  // smooth követés

    private void LateUpdate()
    {
        // Csak a függõleges (pitch) rotációt vesszük a kamerából
        float cameraPitch = cameraTransform.localEulerAngles.x;
        if (cameraPitch > 180) cameraPitch -= 360; // Unity rotációs fix

        // Limitáljuk a fej mozgását
        cameraPitch = Mathf.Clamp(cameraPitch, -maxHeadAngle, maxHeadAngle);

        // bal_szem jelenlegi rotációja
        Vector3 lefteyeEuler = lefteyeTransform.localEulerAngles;
        lefteyeEuler.x = Mathf.LerpAngle(lefteyeEuler.x, cameraPitch, Time.deltaTime * followSpeed);
        lefteyeTransform.localEulerAngles = lefteyeEuler;

        // job_szem jelenlegi rotációja
        Vector3 righteyeEuler = righteyeTransform.localEulerAngles;
        righteyeEuler.x = Mathf.LerpAngle(righteyeEuler.x, cameraPitch, Time.deltaTime * followSpeed);
        righteyeTransform.localEulerAngles = righteyeEuler;

        // gun jelenlegi rotációja
        Vector3 gunEuler = gunTransform.localEulerAngles;
        gunEuler.x = Mathf.LerpAngle(gunEuler.x, cameraPitch, Time.deltaTime * followSpeed);
        gunTransform.localEulerAngles = gunEuler;

        // cam_model jelenlegi rotációja
        Vector3 cam_model_Euler = cam_model_Transform.localEulerAngles;
        cam_model_Euler.x = Mathf.LerpAngle(cam_model_Euler.x, cameraPitch, Time.deltaTime * followSpeed);
        cam_model_Transform.localEulerAngles = cam_model_Euler;
    }
}
