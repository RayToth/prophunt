using UnityEngine;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine.UI;

public class RelayManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI joinCodeText;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    public async void StartRelay()
    {
        string joinCode = await StartHostWithRelay();
        joinCodeText.text = joinCode;
    }

    public async void JoinRelay()
    {
        await StartClientWithRelay(joinCodeInputField.text);
    }

    private async Task<string> StartHostWithRelay(int maxConnections = 3)
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);


        PlayerPrefs.SetString("PlayerName", playerNameInputField.text);
        PlayerPrefs.Save();

        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }
    private async Task<bool> StartClientWithRelay(string joinCode)
    {
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

        PlayerPrefs.SetString("PlayerName", playerNameInputField.text);
        PlayerPrefs.Save();

        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }
}

/*public class RelayManager : MonoBehaviour
{
    public async Task<string> CreateRelay()
    {
        await UnityServices.InitializeAsync();
        Allocation alloc = await RelayService.Instance.CreateAllocationAsync(4);

        string joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);
        Debug.Log($"Relay join code: {joinCode}");

        var relayServerData = new RelayServerData(alloc, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartHost();
        return joinCode;
    }

    public async Task JoinRelay(string joinCode)
    {
        await UnityServices.InitializeAsync();
        JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode);

        var relayServerData = new RelayServerData(joinAlloc, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartClient();
    }
}
*/