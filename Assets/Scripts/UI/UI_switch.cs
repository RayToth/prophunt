using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject Player_Panel;

    public void OpenLobby()
    {
        MainPanel.SetActive(false);
        Player_Panel.SetActive(true);
    }

    public void BackToMenu()
    {
        Player_Panel.SetActive(false);
        MainPanel.SetActive(true);
    }
}
