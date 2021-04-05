using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float se_ProjectileSpeed = 5f;
    [SerializeField] private eVisualEffectType se_VFXType;

    private Vector3 m_PlayerPos = Vector3.zero;

    private float m_ProjectileSpeed;

    public float ProjectileSpeed
    {
        get { return m_ProjectileSpeed;}
        set
        {
            m_ProjectileSpeed = value;
            SendProjectile();
        }
    }

    private void Awake()
    {
        m_ProjectileSpeed = se_ProjectileSpeed;

        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            m_PlayerPos = player.transform.position;
        }
    }

    public void StopProjectile()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    public void SendProjectile()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().AddForce((m_PlayerPos - transform.position).normalized * ProjectileSpeed);
    }

    private void Update()
    {
        if (Utils.s_IsGamePaused)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null && player.CanBeHit)
        {
            FindObjectOfType<GameManager>().HandleDeath();
            //FindObjectOfType<AudioManager>().Play("EnemyHitsPlayerSFX");
            Destroy(gameObject);
        }

        Waypoint point = other.GetComponent<Waypoint>();

        if (point != null && point.ColoredByPlayer && FindObjectOfType<PlayerController>().CanBeHit)
        {
            FindObjectOfType<GridManager>().ResetPlayerPath(point.transform.position, false, true);
            //FindObjectOfType<AudioManager>().Play("EnemyHitsWaypointSFX");
        }

        if (other.GetComponent<PlayerProjectile>() != null)
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }

        if (other.GetComponent<Powerup>() != null)
        {
            Destroy(other.gameObject);
            //FindObjectOfType<AudioManager>().Play("EnemyHitsPowerupSFX");
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        FindObjectOfType<VFXManager>()?.PlayVFX(se_VFXType, transform.position);
    }
}
