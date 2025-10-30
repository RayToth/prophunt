using UnityEngine;

[RequireComponent(typeof(Transform))]
public class WeaponFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform anchor;                // Usually a child of the camera
    public float positionSmooth = 12f;
    public float rotationSmooth = 15f;

    [Header("Sway Settings")]
    [Tooltip("How much the weapon rotates when moving the mouse.")]
    public float swayAmount = 1.5f;
    [Tooltip("How fast the weapon returns to its center after sway.")]
    public float swaySmooth = 8f;

    [Header("Bob Settings")]
    public bool enableBob = true;
    public float bobSpeed = 6f;
    public float bobAmount = 0.03f;

    [Header("Recoil Settings")]
    public float recoilRecoverSpeed = 6f;
    private Vector3 recoilOffset = Vector3.zero;

    private Vector3 startLocalPos;
    private Vector3 bobOffset;
    private Vector3 targetPos;

    void Start()
    {
        if (anchor == null)
            anchor = transform.parent;

        startLocalPos = transform.localPosition;
    }

    void LateUpdate()
    {
        if (anchor == null) return;

        // ---------- FOLLOW POSITION ----------
        Vector3 desiredPos = anchor.position + recoilOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * positionSmooth);

        // ---------- FOLLOW ROTATION ----------
        transform.rotation = Quaternion.Slerp(transform.rotation, anchor.rotation, Time.deltaTime * rotationSmooth);

        // ---------- MOUSE SWAY ----------
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Quaternion targetRot = Quaternion.Euler(-mouseY * swayAmount, mouseX * swayAmount, 0f);
        transform.localRotation = Quaternion.Slerp(Quaternion.identity, targetRot, Time.deltaTime * swaySmooth);

        // ---------- WALK BOB ----------
        if (enableBob)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            bool isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f;

            if (isMoving)
                bobOffset.y = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            else
                bobOffset = Vector3.Lerp(bobOffset, Vector3.zero, Time.deltaTime * 5f);

            targetPos = startLocalPos + bobOffset;
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * 8f);
        }

        // ---------- RECOIL RECOVERY ----------
        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * recoilRecoverSpeed);
    }

    // Call this from your gun firing script
    public void AddRecoil(Vector3 recoil)
    {
        recoilOffset += recoil;
    }
}
