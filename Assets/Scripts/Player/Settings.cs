using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void QuitGame()
    {
        Application.Quit();

        UnityEditor.EditorApplication.isPlaying = false;
    }
}
