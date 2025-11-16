using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameInputManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private TMP_InputField nameInput;

    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            nameInput.text = PlayerPrefs.GetString("PlayerName");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
