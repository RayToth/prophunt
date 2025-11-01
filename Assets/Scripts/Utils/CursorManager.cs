using UnityEngine;

public class CursorManager : MonoBehaviour
{
    void Start()
    {
        // Unlock and show cursor so we can use the UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
