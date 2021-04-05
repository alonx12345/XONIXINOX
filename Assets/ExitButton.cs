using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void SettingsOff()
    {
        MainMenuManager mainMenuManager = FindObjectOfType<MainMenuManager>();

        if (mainMenuManager != null)
        {
            mainMenuManager.settingsOff();
        }
    }
}
