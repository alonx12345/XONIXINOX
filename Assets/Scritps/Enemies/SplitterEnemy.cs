using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SplitterEnemy : MonoBehaviour
{
    [SerializeField] private float se_TimeToSplit = 20f;
    [SerializeField] private float se_TimeToScale = 0.1f;

    [SerializeField] private GameObject m_EnemyToSplit = null;

    public event Action SplitAction;

    public float m_Scale;
    private float m_ElapsedTimeToSplit;
    private float m_ElapsedTimeToScale;

    public bool m_Shrink = true;

    private float m_TimeToSplitSpeed = 1f;
    public float m_TimeToScaleSpeed = 0.3f;

    public float TimeToSplitSpeed
    {
        get { return m_TimeToSplitSpeed;}

        set { m_TimeToSplitSpeed = value; }
    }

    void Awake()
    {
        m_Scale = transform.localScale.x;
        m_ElapsedTimeToScale = se_TimeToScale;
        m_ElapsedTimeToSplit = se_TimeToSplit;
    }

    private void Update()
    {
        if (m_Scale < 1f || !m_Shrink)
        {
            return;
        }

        m_ElapsedTimeToSplit -= Time.deltaTime * m_TimeToSplitSpeed;

        if (m_ElapsedTimeToSplit <= 0)
        {
            m_Scale /= 1.4f;

            int numberToSpawn = Random.Range(4, 5);

            for (int i = 0; i < numberToSpawn; i++)
            {
                GameObject clone = Instantiate(m_EnemyToSplit, transform.position, Quaternion.identity);
                clone.GetComponent<Enemy>().CurrentMoveSpeed = GetComponent<Enemy>().CurrentMoveSpeed * 1.5f;
                FindObjectOfType<SoundManager>().Play(eSoundID.GameOverSound);
                SplitAction?.Invoke();
            }
            
            Destroy(gameObject);
        }

        float speed = 0;

        if (m_ElapsedTimeToSplit > se_TimeToSplit / 2f)
        {
            speed = 0.5f;
        }
        else if (m_ElapsedTimeToSplit <= se_TimeToSplit / 2f && m_ElapsedTimeToSplit > se_TimeToSplit / 4)
        {
            speed = 0.8f;
        }
        else
        {
            speed = 2f;
        }

        float scale = 0.9f + Mathf.PingPong(Time.time * speed, 0.6f);
        transform.localScale = new Vector2(scale, scale);
    }

    public void SubscribeToSplit(Action i_Listener)
    {
        SplitAction += i_Listener;
    }
}
