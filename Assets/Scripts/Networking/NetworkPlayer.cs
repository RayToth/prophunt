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

        // Disable camera for remote players
        if (!IsOwner && playerCamera != null)
            playerCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!IsOwner) return; // Only the local player can move this

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;
        controller.Move(move * speed * Time.deltaTime);
    }
}
