using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager m_BackgroundMusicInstance;
    [SerializeField]
    private SoundManager.Sound m_BackgroundMusic;

    private void Awake()
    {
        if (m_BackgroundMusicInstance is null)
        {
            m_BackgroundMusicInstance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        //DontDestroyOnLoad(gameObject);
        /*
        m_BackgroundMusic = new SoundManager.Sound();
        m_BackgroundMusic.m_AudioSource.gameObject.AddComponent<AudioSource>();
        m_BackgroundMusic.m_AudioSource.clip = m_BackgroundMusic.m_AudioClip;
        m_BackgroundMusic.m_AudioSource.volume = m_BackgroundMusic.m_Volume;
        m_BackgroundMusic.m_AudioSource.pitch = m_BackgroundMusic.m_Pitch;
        m_BackgroundMusic.m_AudioSource.loop = m_BackgroundMusic.m_Loop;
        
        Debug.LogWarning(m_BackgroundMusic.m_AudioSource.gameObject);
        Debug.LogWarning(m_BackgroundMusic.m_AudioSource.clip);
        Debug.LogWarning(m_BackgroundMusic.m_AudioSource.volume);
        Debug.LogWarning(m_BackgroundMusic.m_AudioSource.pitch);
        Debug.LogWarning(m_BackgroundMusic.m_AudioSource.loop);
        
        if (m_BackgroundMusic is null)
        {
            Debug.LogWarning("Background music was not found!");
            return;
        }
        */
    }

    private void Start()
    {
        //m_BackgroundMusic.m_AudioSource.Play();
    }
    
}
