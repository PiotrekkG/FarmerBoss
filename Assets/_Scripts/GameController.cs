using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static bool isGamePaused = false;
    public static GameObject currentDialogue = null;
    public static GameObject PauseMenu;

    private void Start()
    {
        if (PauseMenu != null)
            PauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (currentDialogue != null)
            {
                currentDialogue.SetActive(false);
                currentDialogue = null;
            }
            else
                ToggleSettingsMenu();
        }
    }

    public static void ToggleSettingsMenu()
    {
        if (currentDialogue != null)
        {
            currentDialogue.SetActive(false);
            currentDialogue = null;
        }

        if (PauseMenu != null)
        {
            if (isGamePaused)
            {
                isGamePaused = false;
                PauseMenu.SetActive(false);
            }
            else
            {
                isGamePaused = true;
                PauseMenu.SetActive(true);
            }
        }
    }
}