using System.Collections;
using System.Collections.Generic;
/*using UnityEngine;

public class ClickToToggleCanvas : MonoBehaviour
{
    public GameObject canvasToShow;

    private bool isOpen = false;

    private void OnMouseDown()
    {
        if (!isOpen)
        {
            OpenCanvas();
        }
    }

    public void CloseCanvas()
    {
        isOpen = false;
        canvasToShow.SetActive(false);
    }

    private void OpenCanvas()
    {
        isOpen = true;
        canvasToShow.SetActive(true);
    }

    private void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCanvas();
        }
    }
}*/
using UnityEngine;

public class ClickToToggleCanvas : MonoBehaviour
{
    public GameObject canvasToShow;
    private bool isOpen = false;

    private void Start()
    {
        LockCursor(); // játék induláskor legyen lockolva
    }

    private void OnMouseDown()
    {
        if (!isOpen)
        {
            OpenCanvas();
        }
    }

    private void Update()
    {
        // ESC -> bezár és lock vissza
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCanvas();
        }
    }

    private void OpenCanvas()
    {
        isOpen = true;
        canvasToShow.SetActive(true);
        UnlockCursor();
    }

    public void CloseCanvas()
    {
        isOpen = false;
        canvasToShow.SetActive(false);
        LockCursor();
    }

    // --- Cursor kezekése ---

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;   // egér nincs lockolva
        Cursor.visible = true;                    // látható
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // képernyõ közepére zárolva
        Cursor.visible = false;                   // láthatatlan
    }
}

