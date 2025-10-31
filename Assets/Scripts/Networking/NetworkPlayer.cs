using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NetworkPlayer : NetworkBehaviour
{
    public Camera playerCamera;
    public float speed = 6f;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null)
        {
            Debug.LogError("PlayerCamera not assigned in NetworkPlayer on " + gameObject.name);
            return;
        }

        // Local player: ensure camera is ON
        if (IsOwner)
        {
            playerCamera.gameObject.SetActive(true);
        }
        // Remote player: ensure camera is OFF
        else
        {
            playerCamera.gameObject.SetActive(false);
            AudioListener listener = playerCamera.GetComponent<AudioListener>();
            if (listener) listener.enabled = false;
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;
        controller.Move(move * speed * Time.deltaTime);
    }
}
