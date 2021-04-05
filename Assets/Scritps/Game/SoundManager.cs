using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public enum eSoundID
{
    BackgroundMusic,
    LaserShootSound,
    PlayerProjectileHitsEnemySound,
    EnemyHitsPowerupSound,
    EnemyHitsWaypointSound,
    EnemyHitsPlayerSound,
    ExtraLifePowerupPickupSound,
    EnemySlowDownPowerupPickupSound,
    PlayerSpeedUpPowerupPickupSound,
    MiniPlayerPowerupPickupSound,
    ShootPowerupPickupSound,
    ShootPowerupSound,
    PlayerComeOutOtherSideSound,
    LevelFinishedSound,
    PercentFilledIncreaseSound,
    PercentFilledDecreaseSound,
    PauseModeInSound,
    PauseModeOutSound,
    RespawnPlayerSound,
    GameOverSound,
    Shield
}

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public eSoundID m_SoundID;
        public AudioClip m_AudioClip;

        [Range(0f, 1f)]
        public float m_Volume;
        [Range(.1f, 3f)]
        public float m_Pitch;

        public bool m_Loop;
    
        [HideInInspector]
        public AudioSource m_AudioSource;

        public void PlaySound()
        {
            if (m_AudioSource == null)
            {
                return;
            }
            this.m_AudioSource.Play();
        }
    }
    
    [SerializeField]
    private Sound[] se_Sounds = new Sound[20];

    public static bool m_IsBGMusicMuted;
    public static bool m_IsSFXMuted;

    private int m_IsSoundMuted;
    private int m_IsMusicMuted;



    //public static SoundManager instance;

    public static SoundManager instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        m_IsSoundMuted = PlayerPrefs.GetInt("SoundMuted");
        m_IsMusicMuted = PlayerPrefs.GetInt("MusicMuted");

        foreach (Sound sound in se_Sounds)
        {
            sound.m_AudioSource = gameObject.AddComponent<AudioSource>();
            sound.m_AudioSource.clip = sound.m_AudioClip;
            if (m_IsSoundMuted == 0)
            {
                sound.m_AudioSource.volume = sound.m_Volume;
            }
            else
            {
                sound.m_AudioSource.volume = 0;
            }
            sound.m_AudioSource.pitch = sound.m_Pitch;
            sound.m_AudioSource.loop = sound.m_Loop;
        }

        se_Sounds[0].PlaySound();
    }

    /*
    private void initSoundsArray()
    {
        this.m_Sounds = new Sound[m_NumberOfSounds];
        
        for (int i = 0; i < m_NumberOfSounds; i++)
        {
            //this.m_Sounds[i].clip = getAudioClipBySoundID(i);
        }
    }
    */

    /*
    private AudioClip getAudioClipBySoundID(int i_SoundID)
    {
        
        AudioClip audioSource = new AudioClip();
        audioSource
        
        switch (i_SoundID)
        {
            case Sound.eSoundID.BackgroundMusic:
                
        }
    }
    */

    public void Play(eSoundID i_SoundID)
    {
        /*
        foreach (var sound in se_Sounds.ToList().Where(sound => sound.m_SoundID == i_SoundID))
        {
            sound.PlaySound();
        }
        */
        
        
        Sound soundToPlay = Array.Find(se_Sounds, sound => sound.m_SoundID == i_SoundID);
        if (soundToPlay == null)
        {
            Debug.LogWarning("Sound: " + i_SoundID.ToString("G") + " not found!");
            return;
        }
        
        soundToPlay.PlaySound();
        
        //soundToPlay.m_AudioSource.Play();
    }

    public void ToggleSound()
    {
        if (m_IsSoundMuted == 0)
        {
            MuteAllSounds();
            m_IsSoundMuted = 1;
            PlayerPrefs.SetInt("SoundMuted", 1);
        }
        else
        {
            MakeSoundsPlay();
            m_IsSoundMuted = 0;
            PlayerPrefs.SetInt("SoundMuted", 0);
        }

    }

    private void MuteAllSounds()
    {
        foreach (Sound sound in se_Sounds)
        {
            sound.m_AudioSource.volume = 0;
        }
    }

    private void MakeSoundsPlay()
    {
        foreach (Sound sound in se_Sounds)
        {
            sound.m_AudioSource.volume = sound.m_Volume;
        }
    }

    public void ToggleMusic()
    {
        if (m_IsMusicMuted == 0)
        {
            MuteMusic();
            m_IsMusicMuted = 1;
            PlayerPrefs.SetInt("MusicMuted", 1);
        }
        else
        {
            MakeMusicPlay();
            m_IsMusicMuted = 0;
            PlayerPrefs.SetInt("MusicMuted", 0);
        }
    }

    private void MuteMusic()
    {

    }

    private void MakeMusicPlay()
    {

    }
}
