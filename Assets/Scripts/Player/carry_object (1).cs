using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CarryObject : MonoBehaviour
{
    [Header("Pick settings")]
    [Tooltip("Which layers can be picked up")]
    public LayerMask pickableLayers = ~0;
    [Tooltip("Max distance from camera to be able to pick an object")]
    public float pickRange = 5f;

    [Header("Hold distance")]
    public float minHoldDistance = 1f;
    public float maxHoldDistance = 6f;
    [Tooltip("Multiplier for scroll wheel distance change")]
    public float scrollDistanceSpeed = 2f;

    [Header("Controls")]
    [Tooltip("Mouse button to pick/drop (0=left,1=right)")]
    public int pickupMouseButton = 1;
    [Tooltip("Multiplier for scroll rotation when holding Shift")]
    public float scrollRotationSpeed = 30f;

    [Header("Physics-follow settings (mass-dependent)")]
    [Tooltip("Mass <= this -> nearly instant (uses MovePosition/MoveRotation)")]
    public float massInstantThreshold = 70f;
    [Tooltip("Mass <= this and > instantThreshold -> moderate follow")]
    public float massSlowThreshold = 100f;

    [Tooltip("Follow strength when in slow mass range (higher = faster following)")]
    public float slowFollowKp = 10f;
    [Tooltip("Follow strength when in heavy mass range (higher = faster following)")]
    public float heavyFollowKp = 4f;
    [Tooltip("Damping for velocity smoothing (higher = snappier)")]
    public float velocityDamping = 8f;

    [Header("Throw / Camera-jerk behavior")]
    [Tooltip("If true, a sharp camera jerk can auto-release heavy objects")]
    public bool autoReleaseOnJerk = true;
    [Tooltip("Only auto-release if object's mass is >= this value")]
    public float autoReleaseMassThreshold = 100f;
    [Tooltip("Angular jerk threshold in degrees per second to trigger auto-release")]
    public float autoReleaseJerkThreshold = 400f;
    [Tooltip("Multiplier applied to camera linear velocity when auto-releasing (imparts throw) ")]
    public float releaseVelocityMultiplier = 1.2f;
    [Tooltip(@"Extra forward impulse applied on release (adds ""throw"" feel)")]
    public float releaseForwardImpulse = 1.5f;

    [Header("Collision & general behavior")]
    [Tooltip("If true, object will have gravity disabled while carried (but still collides)")]
    public bool disableGravityWhileCarried = true;
    [Tooltip("Whether to make the carried object kinematic while carried (default false to allow collisions)")]
    public bool makeKinematicOnCarry = false;

    // runtime
    Camera cam;
    Rigidbody carriedRb;
    Transform carriedT;
    bool isCarrying = false;

    // stored original rigidbody settings to restore on drop
    bool origUseGravity;
    bool origIsKinematic;
    RigidbodyInterpolation origInterpolation;
    CollisionDetectionMode origCollisionMode;

    // hold state
    float holdDistance;
    Quaternion rotationOffsetFromCamera;
    float extraPitch = 0f;

    // camera motion tracking for jerk detection
    Vector3 prevCamPos;
    Quaternion prevCamRot;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;

        prevCamPos = cam.transform.position;
        prevCamRot = cam.transform.rotation;
    }

    void Update()
    {
        HandlePickupInput();
        if (isCarrying)
        {
            HandleScrollAndRotation();
        }

        // store camera orientation for use in FixedUpdate (physics)
        prevCamPos = cam.transform.position;
        prevCamRot = cam.transform.rotation;
    }

    void FixedUpdate()
    {
        // camera velocities for jerk detection and release velocity calculation (approx)
        Vector3 camVel = (cam.transform.position - prevCamPos) / Time.fixedDeltaTime; // note: prevCamPos updated in Update() so this approximates last frame
        float camAngularSpeed = Quaternion.Angle(prevCamRot, cam.transform.rotation) / Time.fixedDeltaTime; // deg/s

        if (isCarrying && carriedRb != null)
        {
            UpdateCarriedPhysics(camVel, camAngularSpeed);

            // auto-release on big jerks if enabled and mass condition met
            if (autoReleaseOnJerk && carriedRb.mass >= autoReleaseMassThreshold && camAngularSpeed >= autoReleaseJerkThreshold)
            {
                // impart velocity based on camera motion and forward impulse
                Vector3 impart = camVel * releaseVelocityMultiplier + cam.transform.forward * releaseForwardImpulse;
                DropWithVelocity(impart);
            }
        }
    }

    void HandlePickupInput()
    {
        if (Input.GetMouseButtonDown(pickupMouseButton))
        {
            if (!isCarrying)
                TryPickup();
            else
                Drop();
        }
    }

    void TryPickup()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, pickRange, pickableLayers))
        {
            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb != null)
            {
                StartCarry(rb, hit.point);
            }
            else
            {
                rb = hit.collider.GetComponentInParent<Rigidbody>();
                if (rb != null)
                    StartCarry(rb, hit.point);
            }
        }
    }

    void StartCarry(Rigidbody rb, Vector3 hitPoint)
    {
        carriedRb = rb;
        carriedT = rb.transform;
        isCarrying = true;

        // save original physics settings
        origUseGravity = rb.useGravity;
        origIsKinematic = rb.isKinematic;
        origInterpolation = rb.interpolation;
        origCollisionMode = rb.collisionDetectionMode;

        // configure rigidbody so it collides while we move it
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.useGravity = !disableGravityWhileCarried ? origUseGravity : false;
        rb.isKinematic = makeKinematicOnCarry; // default false so collisions happen

        // initial hold distance
        holdDistance = Vector3.Distance(cam.transform.position, carriedT.position);
        holdDistance = Mathf.Clamp(holdDistance, minHoldDistance, maxHoldDistance);

        rotationOffsetFromCamera = Quaternion.Inverse(cam.transform.rotation) * carriedT.rotation;
        extraPitch = 0f;

        // store camera states for jerk detection baseline
        prevCamPos = cam.transform.position;
        prevCamRot = cam.transform.rotation;
    }

    void Drop()
    {
        if (carriedRb != null)
        {
            // restore rigidbody settings
            carriedRb.useGravity = origUseGravity;
            carriedRb.isKinematic = origIsKinematic;
            carriedRb.interpolation = origInterpolation;
            carriedRb.collisionDetectionMode = origCollisionMode;
        }

        carriedRb = null;
        carriedT = null;
        isCarrying = false;
    }

    void DropWithVelocity(Vector3 velocity)
    {
        if (carriedRb != null)
        {
            // restore before applying velocity
            carriedRb.useGravity = origUseGravity;
            carriedRb.isKinematic = origIsKinematic;
            carriedRb.interpolation = origInterpolation;
            carriedRb.collisionDetectionMode = origCollisionMode;

            carriedRb.velocity = velocity;
        }

        carriedRb = null;
        carriedT = null;
        isCarrying = false;
    }

    void HandleScrollAndRotation()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Approximately(scroll, 0f)) return;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            extraPitch += scroll * scrollRotationSpeed;
            extraPitch = Mathf.Clamp(extraPitch, -90f, 90f);
        }
        else
        {
            holdDistance -= scroll * scrollDistanceSpeed;
            holdDistance = Mathf.Clamp(holdDistance, minHoldDistance, maxHoldDistance);
        }
    }

    void UpdateCarriedPhysics(Vector3 camVel, float camAngularSpeed)
    {
        if (carriedRb == null) return;

        // target position and rotation in front of camera
        Vector3 targetPos = cam.transform.position + cam.transform.forward * holdDistance;
        Quaternion baseRot = cam.transform.rotation * rotationOffsetFromCamera;
        if (!Mathf.Approximately(extraPitch, 0f))
            baseRot = Quaternion.AngleAxis(extraPitch, cam.transform.right) * baseRot;

        float mass = Mathf.Max(0.0001f, carriedRb.mass);

        // choose behavior by mass ranges
        if (mass <= massInstantThreshold)
        {
            // almost instant: directly move the object to target using MovePosition/MoveRotation for minimal jitter
            carriedRb.MovePosition(targetPos);
            carriedRb.MoveRotation(baseRot);

            // zero velocity to avoid residual motion
            carriedRb.velocity = Vector3.zero;
            carriedRb.angularVelocity = Vector3.zero;
        }
        else if (mass <= massSlowThreshold)
        {
            // moderate follow: set velocity toward target scaled by slowFollowKp and mass
            Vector3 desiredVel = (targetPos - carriedRb.position) * slowFollowKp;
            // slow down heavier objects proportionally
            desiredVel /= (mass / massInstantThreshold);

            // smooth the velocity change to avoid snapping
            carriedRb.velocity = Vector3.Lerp(carriedRb.velocity, desiredVel, Mathf.Clamp01(Time.fixedDeltaTime * velocityDamping));

            // rotate smoothly
            Quaternion newRot = Quaternion.Slerp(carriedRb.rotation, baseRot, Mathf.Clamp01(Time.fixedDeltaTime * slowFollowKp * 0.5f));
            carriedRb.MoveRotation(newRot);
        }
        else
        {
            // heavy: very sluggish follow
            Vector3 desiredVel = (targetPos - carriedRb.position) * heavyFollowKp;
            desiredVel /= (mass / massSlowThreshold);
            carriedRb.velocity = Vector3.Lerp(carriedRb.velocity, desiredVel, Mathf.Clamp01(Time.fixedDeltaTime * (velocityDamping * 0.5f)));

            Quaternion newRot = Quaternion.Slerp(carriedRb.rotation, baseRot, Mathf.Clamp01(Time.fixedDeltaTime * heavyFollowKp * 0.25f));
            carriedRb.MoveRotation(newRot);
        }

        // ensure collisions are enabled — do not change layer or use kinematic unless explicitly requested
        // gravity already controlled on pickup
    }

    void OnDrawGizmosSelected()
    {
        if (cam == null) cam = GetComponent<Camera>();
        if (cam == null) return;
        Gizmos.color = Color.cyan;
        Vector3 origin = cam.transform.position;
        Vector3 dir = cam.transform.forward;
        Gizmos.DrawLine(origin, origin + dir * pickRange);

        if (isCarrying && carriedT != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(cam.transform.position + cam.transform.forward * holdDistance, 0.05f);
        }
    }
}
