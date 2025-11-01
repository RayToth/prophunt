using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class RelayUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField joinCodeInput;
    public TMP_Text joinCodeDisplay;
    public GameObject mainPanel; // the whole menu panel
    private RelayManager relayManager;

    void Start()
    {
        relayManager = FindObjectOfType<RelayManager>();

        // Make sure cursor is unlocked and visible for menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    void TransitionToGameplay()
    {
        // Hide menu
        if (mainPanel != null)
            mainPanel.SetActive(false);

        // Lock and hide cursor for FPS gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Press ESC to bring menu/cursor back (for debugging or pause menu)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool isActive = mainPanel.activeSelf;
            mainPanel.SetActive(!isActive);
            Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !isActive;
        }
    }
}
