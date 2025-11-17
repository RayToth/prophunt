using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkCursorPitch : NetworkBehaviour
{
    [Header("Target to rotate (head, object, etc.)")]
    [SerializeField] private Transform targetTransform;

    [Header("Settings")]
    [SerializeField] private float sensitivity = 100f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private float smoothing = 10f;
    [SerializeField] private float sendThreshold = 0.2f; // foknyi változás

    // NetworkVariable: csak az Owner írhatja, mindenki olvassa
    private NetworkVariable<float> networkPitch = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private float localPitch = 0f;    // helyi számított pitch
    private float smoothPitch = 0f;   // interpolált pitch megjelenítéshez
    private float lastSentPitch = 0f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (targetTransform == null)
        {
            Debug.LogError("NetworkCursorPitch: targetTransform nincs beállítva!");
        }

        // Ha ez a helyi játékos, lockoljuk a kurzort (FPS jelleg)
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        // 1) Owner kezeli a bemenetet
        if (IsOwner)
        {
            HandleLocalInput();
        }

        // 2) Minden kliens — owner is! — lerpeli a pitch-et, és alkalmazza
        ApplyRotation();
    }

    private void HandleLocalInput()
    {
        // Egér Y tengely lekérése
        float mouseY = Input.GetAxisRaw("Mouse Y");

        // Egér mozgás konvertálása pitch értékké
        localPitch -= mouseY * sensitivity * Time.deltaTime;
        localPitch = Mathf.Clamp(localPitch, minPitch, maxPitch);

        // Csak akkor küldjük a hálózatra, ha érdemben változott (sávszélesség spórolás)
        if (Mathf.Abs(Mathf.DeltaAngle(lastSentPitch, localPitch)) > sendThreshold)
        {
            networkPitch.Value = localPitch;
            lastSentPitch = localPitch;
        }
    }

    private void ApplyRotation()
    {
        // A hálózati pitch a hiteles érték mindenki számára
        float targetPitch = networkPitch.Value;

        // Interpoláció, hogy ne rángasson
        smoothPitch = Mathf.LerpAngle(smoothPitch, targetPitch, Time.deltaTime * smoothing);

        // Alkalmazzuk a head/object X forgatását
        Vector3 e = targetTransform.localEulerAngles;
        e.x = smoothPitch;
        targetTransform.localEulerAngles = e;
    }
}
