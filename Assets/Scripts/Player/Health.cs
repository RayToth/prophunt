using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public NetworkVariable<float> HP = new NetworkVariable<float>(
        100f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public float MaxHealth = 100f;
    [SerializeField] private HealthBarUI healthBar;

    public override void OnNetworkSpawn()
    {
        // Subscribe to health changes
        HP.OnValueChanged += OnHealthChanged;
    }

    private void OnHealthChanged(float oldValue, float newValue)
    {
        // Update UI anytime health changes (local OR remote player)
        healthBar.UpdateBar(newValue, MaxHealth);
    }

    // Called by server when the player is hit
    [ServerRpc]
    public void TakeDamageServerRpc(float damage)
    {
        HP.Value = Mathf.Clamp(HP.Value - damage, 0, MaxHealth);
    }


}