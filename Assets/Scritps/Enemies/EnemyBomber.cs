using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBomber : MonoBehaviour
{
    [SerializeField] private float se_TimeToExplode = 20f;
    [SerializeField] private float se_TimeToScale = 0.1f;
    [SerializeField] private int se_ExplosionRadius = 8;

    public event Action ExplodeAction;

    public float m_Scale;
    private float m_ElapsedTimeToExplode;
    private float m_ElapsedTimeToScale;

    public bool m_Shrink = true;


    private float m_TimeToExplodeSpeed = 1f;
    public float m_TimeToScaleSpeed = 0.3f;

    public float TimeToExplodeSpeed
    {
        get { return m_TimeToExplodeSpeed; }

        set { m_TimeToExplodeSpeed = value; }
    }

    void Awake()
    {
        m_Scale = transform.localScale.x;
        m_ElapsedTimeToScale = se_TimeToScale;
        m_ElapsedTimeToExplode = se_TimeToExplode;
    }

    private void Update()
    {
        if (m_Scale < 1f || !m_Shrink)
        {
            return;
        }

        m_ElapsedTimeToExplode -= Time.deltaTime * m_TimeToExplodeSpeed;

        if (m_ElapsedTimeToExplode <= 0)
        {
            m_Scale /= 1.4f;
            FindObjectOfType<GridManager>()?.ExplodeAroundPoint(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), se_ExplosionRadius);
            FindObjectOfType<SoundManager>().Play(eSoundID.GameOverSound);
            Destroy(gameObject);
        }

        float speed = 0;

        if (m_ElapsedTimeToExplode > se_TimeToExplode / 2f)
        {
            speed = 0.5f;
        }
        else if (m_ElapsedTimeToExplode <= se_TimeToExplode / 2f && m_ElapsedTimeToExplode > se_TimeToExplode / 4)
        {
            speed = 0.8f;
        }
        else
        {
            speed = 2f;
        }

        float scale = 0.8f + Mathf.PingPong(Time.time * speed, 0.3f);
        scale = Mathf.Clamp(scale, 0.7f, 1.5f);

        transform.localScale = new Vector2(scale, scale);
    }
}
