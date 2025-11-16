using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using UnityEditor.Analytics;
using UnityEngine.Timeline;

public class RelayUI : MonoBehaviour
{
    public static bool isMenuOpen { get; set; }

    [Header("UI Elements")]
    public TMP_InputField joinCodeInput;
    public TMP_Text joinCodeDisplay;
    public GameObject mainPanel; // the whole menu panel
    public GameObject playerPanel;
    private RelayManager relayManager;

    void Start()
    {
        ShowMenu();
    }


    public void TransitionToGameplay()
    {
        HideMenu();
    }

    void Update()
    {
        // Press ESC to bring menu/cursor back (for debugging or pause menu)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool isActive = mainPanel.activeSelf;
            mainPanel.SetActive(!isActive);

            isMenuOpen = !isMenuOpen;

            Cursor.lockState = isActive ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isActive;

            Debug.Log($"Cursor: {Cursor.lockState}, visible: {Cursor.visible}");
            Debug.Log(isActive ? "Hide menu" : "Show menu");
        }
    }

    private void ShowMenu()
    {
        isMenuOpen = true;
        if(mainPanel != null) mainPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HideMenu()
    {
        isMenuOpen = false;
        if(mainPanel != null) mainPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
