using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePowerupType
{
    SLOW_DOWN,
    SPEED_UP,
    TIME,
    SHOOT,
    MINI_PLAYER,
    EXTRA_LIFE,
    SHIELD,
    BOMB
}

public class Powerup : MonoBehaviour
{
    [SerializeField] private ePowerupType se_powerupID = ePowerupType.SLOW_DOWN;
    [SerializeField] private eVisualEffectType se_VFXType;
    [SerializeField] private eSoundID se_SoundID = eSoundID.EnemyHitsPowerupSound;

    public event Action<ePowerupType> PowerupActivate;
    public event Action DestroyAction;

    private float m_TimeToDestroy;
    private SelfDestroyScript m_SelfDestroyScript;
    private float m_ElapsedTimeToDestroy = 0;
    private SpriteRenderer m_SpriteRenderer;

    private void Awake()
    {
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_SelfDestroyScript = GetComponent<SelfDestroyScript>();
    }

    private void Start()
    {
        if (m_SelfDestroyScript != null)
        {
            m_SelfDestroyScript.TimeToDestroyChanged += onSelfDestroyScript_TimeToDestroyChanged;
            m_TimeToDestroy = m_SelfDestroyScript.TimeToDestroy;
        }
    }

    private void Update()
    {
       handleBlinkBeforeDestroy();
    }

    private void handleBlinkBeforeDestroy()
    {
        if (m_SelfDestroyScript != null && m_SpriteRenderer != null && !Utils.s_IsGamePaused)
        {
            m_ElapsedTimeToDestroy += Time.deltaTime;

            if (m_ElapsedTimeToDestroy >= 0.5f * m_TimeToDestroy && m_ElapsedTimeToDestroy < 0.75f * m_TimeToDestroy)
            {
                float alpha = Mathf.PingPong(Time.time * 2f, 1f);
                Color color = m_SpriteRenderer.color;
                color.a = alpha;
                m_SpriteRenderer.color = color;
            }
            else if (m_ElapsedTimeToDestroy >= 0.75f * m_TimeToDestroy)
            {
                float alpha = Mathf.PingPong(Time.time * 4f, 1f);
                Color color = m_SpriteRenderer.color;
                color.a = alpha;
                m_SpriteRenderer.color = color;
            }
        }
    }

    private void onSelfDestroyScript_TimeToDestroyChanged()
    {
        m_TimeToDestroy = m_SelfDestroyScript.TimeToDestroy;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            PowerupActivate?.Invoke(se_powerupID);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        DestroyAction?.Invoke();

        VFXManager vfxManager = FindObjectOfType<VFXManager>();

        if (vfxManager != null)
        {
            vfxManager.PlayVFX(se_VFXType, transform.position);
        }

        SoundManager soundManager = FindObjectOfType<SoundManager>();

        if (soundManager != null)
        {
            soundManager.Play(se_SoundID);
        }

    }
}
