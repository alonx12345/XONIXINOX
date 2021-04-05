using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float se_StartMoveSpeed = 5f;

    public event Action TouchedPlayerAction;
    public event Action<Vector3, bool, bool> TouchedPlayerPathAction;
    public event Action<eSoundID> PlaySoundAction;

    private float m_CurrentMoveSpeed;

    private Rigidbody2D m_Rigidbody;

    private Vector3 m_CurrentVelocity;

    private Vector2 m_InitialDirection;

    private bool m_TouchedPlayer = false;

    private RigidbodyConstraints2D m_EnemyRigidConstraints;

    private bool m_Spin = true;

    private int m_SpinDirection;

    private GameObjectDestroyer m_Destroyer;

    [SerializeField] private float se_MinTimeToSwitchDirection = 3f;
    [SerializeField] private float se_MaxTimeToSwitchDirection = 7f;

    private eDirection m_CurrentDirection;
    private ChopperEnemy m_ChopperComp;
    private float m_TimeTillDirectionSwitch;
    private float m_ElapsedTimeTillDirectionSwitch = 0;

    
    
    public float CurrentMoveSpeed
    {
        get { return m_CurrentMoveSpeed; }
        set { m_CurrentMoveSpeed = value; }
    }

    public GameObjectDestroyer Destroyer
    {
        get { return m_Destroyer; }
    }

    private void Awake()
    {
        m_CurrentMoveSpeed = se_StartMoveSpeed;
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_EnemyRigidConstraints = m_Rigidbody.constraints;
        m_Destroyer = GetComponent<GameObjectDestroyer>();
        m_ChopperComp = GetComponent<ChopperEnemy>();

        m_TimeTillDirectionSwitch = Random.Range(se_MinTimeToSwitchDirection, se_MaxTimeToSwitchDirection);
    }

    private void Start()
    {
        initEnemyMovement();
    }

    private void initEnemyMovement()
    {
        if (m_ChopperComp == null)
        {
            float angle = UnityEngine.Random.Range(0f, 1f) * Mathf.PI * 2;
            float xDir = Mathf.Cos(angle) * 1f;
            float yDir = Mathf.Sin(angle) * 1f;

            m_InitialDirection = new Vector2(xDir, yDir);
            m_SpinDirection = UnityEngine.Random.Range(0, 2) * 2 - 1;

            m_Rigidbody.AddForce(m_InitialDirection * (se_StartMoveSpeed / 100));
        }
        else
        {
            int directionInt = UnityEngine.Random.Range(1, 5);
            m_CurrentDirection  = (eDirection)directionInt;
        }
    }

    public void Move(Vector2 i_Direction, float i_Delta)
    {
        int xDir = Mathf.RoundToInt(i_Direction.x);
        int yDir = Mathf.RoundToInt(i_Direction.y);

        Vector2 moveToVector = new Vector3(xDir, yDir, 0);

        transform.position = Vector3.MoveTowards(transform.position, moveToVector, m_CurrentMoveSpeed * i_Delta);
    }

    private void Update()
    {
        if (m_ChopperComp != null && m_Spin)
        {
            m_ElapsedTimeTillDirectionSwitch += Time.deltaTime;

            if (m_ElapsedTimeTillDirectionSwitch >= m_TimeTillDirectionSwitch)
            {
               changeDirection();
            }

            switch (m_CurrentDirection)
            {
                case eDirection.Up:
                    if (!(transform.position.y + 1 >= Utils.s_GridHeight - 1.02f))
                    {
                        Move(transform.position + Vector3.up, Time.deltaTime);
                    }
                    else
                    {
                        changeDirection();
                    }

                    break;
                case eDirection.Down:
                    if (!(transform.position.y - 1 <= 0.02f))
                    {
                        Move(transform.position + Vector3.down, Time.deltaTime);
                    }
                    else
                    {
                        changeDirection();
                    }

                    break;
                case eDirection.Left:
                    if (!(transform.position.x - 1 <= 0.02f))
                    {
                        Move(transform.position + Vector3.left, Time.deltaTime);
                    }
                    else
                    {
                        changeDirection();
                    }

                    break;
                case eDirection.Right:
                    if (!(transform.position.x + 1 >= Utils.s_GridWidth - 1.02f))
                    {
                        Move(transform.position + Vector3.right, Time.deltaTime);
                    }
                    else
                    {
                        changeDirection();
                    }

                    break;
            }
        }
    }

    private void changeDirection()
    {
        eDirection newDirection = (eDirection)Random.Range(1, 5);

        while (newDirection == m_CurrentDirection
        || (m_CurrentDirection == eDirection.Up && newDirection == eDirection.Down)
        || (m_CurrentDirection == eDirection.Down && newDirection == eDirection.Up)
        || (m_CurrentDirection == eDirection.Left && newDirection == eDirection.Right)
        || (m_CurrentDirection == eDirection.Right && newDirection == eDirection.Left))
        {
            newDirection = (eDirection)Random.Range(1, 5);
        }

        m_CurrentDirection = newDirection;

        m_ElapsedTimeTillDirectionSwitch = 0;
        m_TimeTillDirectionSwitch = Random.Range(se_MinTimeToSwitchDirection, se_MaxTimeToSwitchDirection);
    }

    private void FixedUpdate()
    {
        if (m_ChopperComp == null)
        {
            m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * m_CurrentMoveSpeed;
            SpinEffect();
        }
    }

    private void SpinEffect()
    {
        if (m_Spin)
        {
            transform.Rotate(0, 0, m_CurrentMoveSpeed * 30f * m_SpinDirection * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null && !m_TouchedPlayer && player.CanBeHit)
        {
            m_TouchedPlayer = true;

            TouchedPlayerAction?.Invoke();
            FindObjectOfType<GameManager>()?.HandleDeath();
            PlaySoundAction?.Invoke(eSoundID.EnemyHitsPlayerSound);

            StartCoroutine(resetTouch());
            //FindObjectOfType<AudioManager>().Play("EnemyHitsPlayerSFX");
        }

        if (other.GetComponent<PlayerProjectile>() != null)
        {
            PlaySoundAction?.Invoke(eSoundID.PlayerProjectileHitsEnemySound);
            FindObjectOfType<GridManager>().FloodWithDelayHandler(transform.position, .5f);
            Destroy(other.gameObject);
            Destroy(gameObject);
            //FindObjectOfType<AudioManager>().Play("PlayerProjectileHitsEnemySFX"); // TODO
        }

        if (other.GetComponent<Powerup>() != null)
        {
            PlaySoundAction?.Invoke(eSoundID.EnemyHitsPowerupSound);
            Destroy(other.gameObject);
        }
    }

    private IEnumerator resetTouch()
    {
        yield return new WaitForSeconds(1f);
        m_TouchedPlayer = false;
    }

    private void OnCollisionEnter2D(Collision2D i_Other)
    {
        Vector2 tweak = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
        m_Rigidbody.velocity += tweak;

        Waypoint point = i_Other.gameObject.GetComponent<Waypoint>();

        if (point != null && point.ColoredByPlayer && !m_TouchedPlayer && FindObjectOfType<PlayerController>().CanBeHit)
        {
            m_TouchedPlayer = true;

            TouchedPlayerPathAction?.Invoke(point.transform.position, false, true);
            PlaySoundAction?.Invoke(eSoundID.EnemyHitsWaypointSound);

            StartCoroutine(resetTouch());

            //FindObjectOfType<AudioManager>().Play("EnemyHitsWaypointSFX");
        }
    }

    //public void SubscribeToTouchEvent(Action i_Listener)
    //{
    //    TouchedPlayerAction += i_Listener;
    //}

    //public void SubscribeToTouchPathEvent(Action<Vector3, bool, bool> i_Listener)
    //{
    //    TouchedPlayerPathAction += i_Listener;
    //}

    public void Pause()
    {
        m_CurrentVelocity = m_Rigidbody.velocity;
        m_Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        m_Spin = false;

        toggleSplitterAndBombers();
    }

    public void Resume()
    {
        m_Rigidbody.constraints = m_EnemyRigidConstraints;
        m_Rigidbody.velocity = m_CurrentVelocity;
        m_Spin = true;

        toggleSplitterAndBombers();
    }

    private void toggleSplitterAndBombers()
    {
        SplitterEnemy splitterComp = GetComponent<SplitterEnemy>();
        if (splitterComp != null)
        {
            splitterComp.m_Shrink = m_Spin;
        }

        EnemyBomber bomberComp = GetComponent<EnemyBomber>();
        if (bomberComp != null)
        {
            bomberComp.m_Shrink = m_Spin;
        }
    }
}
