using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEnemy : MonoBehaviour
{
    [SerializeField] private EnemyProjectile se_ProjectilePrefab = null;
    [SerializeField] private float se_TimeToShoot = 1f;
    [SerializeField] private float se_TimeToReload = 1f;
    [SerializeField] private GameObject[] se_ProjectilesViews = null;


    private float m_TimeToShoot;
    private float m_ElapsedTimeToShoot;
    private float m_ElapsedTimeToReload;

    private int m_CurrentView = 0;

    public float TimeToShoot
    {
        get { return m_TimeToShoot;}

        set { m_TimeToShoot = value; }
    }

    public EnemyProjectile Projectile
    {
        get { return se_ProjectilePrefab; }
    }

    private void Awake()
    {
        m_TimeToShoot = se_TimeToShoot;
        m_ElapsedTimeToShoot = m_TimeToShoot;
        m_ElapsedTimeToReload = se_TimeToReload;
    }

    private void Start()
    {
        if (se_ProjectilesViews != null)
        {
            foreach (var view in se_ProjectilesViews)
            {
                view.SetActive(true);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!Utils.s_IsGamePaused)
        {
            handleShoot();
            handleReload();
        }
    }

    private void handleShoot()
    {
        m_ElapsedTimeToShoot -= Time.deltaTime;

        if (m_ElapsedTimeToShoot <= 0 && m_CurrentView < se_ProjectilesViews.Length)
        {
            EnemyProjectile proj = Instantiate(se_ProjectilePrefab, se_ProjectilesViews[m_CurrentView].transform.position, Quaternion.Euler(0, 0, 0));
            proj.SendProjectile();
            se_ProjectilesViews[m_CurrentView].SetActive(false);
            m_CurrentView++;
            m_ElapsedTimeToShoot = m_TimeToShoot;
            FindObjectOfType<SoundManager>().Play(eSoundID.LaserShootSound);
        }
    }

    private void handleReload()
    {
        if (m_CurrentView >= se_ProjectilesViews.Length)
        {
            m_ElapsedTimeToReload -= Time.deltaTime;

            if (m_ElapsedTimeToReload <= 0)
            {
                foreach (var view in se_ProjectilesViews)
                {
                    view.SetActive(true);
                }

                m_ElapsedTimeToReload = se_TimeToReload;
                m_CurrentView = 0;
            }
        }
    }
}
