using System;
using UnityEngine;
using System.Collections;

public class SelfDestroyScript : MonoBehaviour
{
    [SerializeField] private float se_TimeToDestroy = 3f;

    public Action TimeToDestroyChanged;

    public float TimeToDestroy
    {
        get { return se_TimeToDestroy; }
        set
        {
            se_TimeToDestroy = value;
            TimeToDestroyChanged?.Invoke();
        }
    }

    private float m_ElapsedTimeToDestroy = 0;

    private void Update()
    {
        if (!Utils.s_IsGamePaused)
        {
            m_ElapsedTimeToDestroy += Time.deltaTime;

            if (m_ElapsedTimeToDestroy >= se_TimeToDestroy)
            {
                Destroy(gameObject);
            }

        }
    }
}
