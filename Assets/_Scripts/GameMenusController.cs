using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenusController : MonoBehaviour
{
    public Button settingsButton;
    public GameObject PauseMenu;

    private void Awake()
    {
        settingsButton.onClick.AddListener(GameController.ToggleSettingsMenu);

        GameController.PauseMenu = PauseMenu;
    }
}