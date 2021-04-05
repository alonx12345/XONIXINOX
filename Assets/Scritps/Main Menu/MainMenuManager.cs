using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject settingsLabel = null;
    // [SerializeField] GameObject topChartsLabel = null;
    public Button sound;
    public Button music;
    public Sprite soundOff;
    public Sprite soundOn;
    public Sprite musicOff;
    public Sprite musicOn;
    int currentSceneIndex;
    bool pressed = true;

    [SerializeField] private GameObject se_MainCanvas = null;

    public AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        //sound = GetComponent<Button>();
        //music = GetComponent<Button>();
        settingsLabel.SetActive(false);
        // topChartsLabel.SetActive(false);
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        audio = GetComponent<AudioSource>();
        updateSoundSprite();

        
    }

    public static MainMenuManager instance;

    private void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}

        //DontDestroyOnLoad(gameObject);
    }

    public void settingsOn()
    {
        settingsLabel.SetActive(true);
    }

    public void settingsOff()
    {
        settingsLabel.SetActive(false);
    }

    // public void TopChartsOn()
    // {
    //     topChartsLabel.SetActive(true);
    // }

    // public void TopChartsOff()
    // {
    //     topChartsLabel.SetActive(false);
    // }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void updateSoundSprite()
    {
        if (sound != null)
        {
            if (PlayerPrefs.GetInt("SoundMuted") == 1)
            {
                sound.image.overrideSprite = soundOff;
                pressed = false;
                //audio.mute = !audio.mute;
            }
            else
            {
                sound.image.overrideSprite = soundOn;
                pressed = true;
                //audio.mute = !audio.mute;
            }
        }

       
    }

    public void updateMusicSprite()
    {
        if (pressed == true)
        {
            music.image.overrideSprite = musicOff;
            pressed = false;
        }
        else
        {
            music.image.overrideSprite = musicOn;
            pressed = true;
        }
    }

    public void HideCanvas()
    {
        if (se_MainCanvas != null)
        {
            se_MainCanvas.SetActive(false);
        }
    }
}

