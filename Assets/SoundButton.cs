using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundButton : MonoBehaviour
{
    public void ToggleSoundAndSprite()
    {
        SoundManager soundManager = FindObjectOfType<SoundManager>();

        if (soundManager != null)
        {
            soundManager.ToggleSound();
        }

        MainMenuManager mainMenuManager = FindObjectOfType<MainMenuManager>();

        if (mainMenuManager != null)
        {
            mainMenuManager.updateSoundSprite();
        }
    }
}
