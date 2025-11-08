using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Camera/Simple Anchor Camera")]
public class SimpleAnchorCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;                 // Player root
    public Transform neckAnchor;             // optional: üres GameObject a nyaknál (child)
    public Vector3 neckLocalOffset = new Vector3(0f, 1.6f, 0f); // ha nincs neckAnchor

    [Header("Eye offset (local relative to anchor)")]
    public Vector3 eyeOffset = new Vector3(0.2f, 0.15f, 0.15f); // X=jobb+, Y=fel+, Z=elõre (a kamera nézete szerint)

    [Header("Mouse look")]
    public bool enableMouseLook = true;
    public float sensitivityX = 120f;
    public float sensitivityY = 100f;
    public float minPitch = -60f;
    public float maxPitch = 60f;

    [Header("Smoothing")]
    public float posSmooth = 10f;
    public float rotSmooth = 12f;

    [Header("Collision")]
    public bool preventClipping = true;
    public LayerMask obstructionMask = ~0;
    public float minDistFromAnchor = 0.05f;

    // internals
    private float yaw = 0f;
    private float pitch = 0f;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("SimpleAnchorCamera: Target nincs beállítva!");
            enabled = false;
            return;
        }

        Vector3 e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1) anchor pozíció deriválása
        Vector3 anchorWorld = (neckAnchor != null) ? neckAnchor.position : target.TransformPoint(neckLocalOffset);

        // 2) egér input
        if (enableMouseLook && (Input.GetMouseButton(1) || Input.GetMouseButton(0)))
        {
            yaw += Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
            pitch -= Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        // 3) desired cam pozíció: anchor + rotált eyeOffset
        Quaternion lookRot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPos = anchorWorld + lookRot * eyeOffset;

        // 4) collision check (ha be van kapcsolva)
        if (preventClipping)
        {
            Vector3 dir = desiredPos - anchorWorld;
            float dist = dir.magnitude;
            if (dist > 0.001f)
            {
                Ray r = new Ray(anchorWorld, dir.normalized);
                if (Physics.Raycast(r, out RaycastHit hit, dist, obstructionMask))
                {
                    float newDist = Mathf.Max(hit.distance - 0.02f, minDistFromAnchor);
                    desiredPos = anchorWorld + dir.normalized * newDist;
                }
            }
        }

        // 5) sima mozgatás
        transform.position = Vector3.Lerp(transform.position, desiredPos, 1f - Mathf.Exp(-posSmooth * Time.deltaTime));

        // 6) kamera nézzen az anchor felé
        Quaternion targetRot = Quaternion.LookRotation(anchorWorld - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 1f - Mathf.Exp(-rotSmooth * Time.deltaTime));
    }

    void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Vector3 anchor = (neckAnchor != null) ? neckAnchor.position : target.TransformPoint(neckLocalOffset);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(anchor, 0.03f);

        Quaternion lookRot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desired = anchor + lookRot * eyeOffset;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(anchor, desired);
        Gizmos.DrawSphere(desired, 0.03f);
    }
}
