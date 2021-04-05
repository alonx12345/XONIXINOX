using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float se_PlayerDefaultSpeed = 20f;
    [SerializeField] private GameObject se_ProjectilePrefab = null;
    [SerializeField] private float se_ProjectileSpeed = 5f;
    [SerializeField] private eVisualEffectType se_DeathVFXType = eVisualEffectType.PlayerExplosion;
    [SerializeField] private Transform[] se_PlayerBodyParts = null;
    [SerializeField] public Transform[] se_PlayerShieldParts = null;

    private SpriteRenderer m_SpriteRenderer;
    private float m_CurrentMoveSpeed;
    private GameObjectDestroyer m_Destroyer;

    public Vector3 MoveDirection { get; set; }
    public bool CanBeHit { get; set; } = true;

    public float CurrentMoveSpeed
    {
        set { m_CurrentMoveSpeed = value; }
    }

    public float DefaultMoveSpeed
    {
        get { return se_PlayerDefaultSpeed; }
    }

    public GameObjectDestroyer Destroyer
    {
        get { return m_Destroyer; }
    }

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_CurrentMoveSpeed = se_PlayerDefaultSpeed;
        m_Destroyer = GetComponent<GameObjectDestroyer>();
    }

    public Color Color
    {
        get
        {
            //return m_SpriteRenderer.color;

            return transform.GetChild(0).GetComponent<SpriteRenderer>().color;


            //return GetComponentInChildren<SpriteRenderer>().color;
        }
        set
        {
            //if (m_SpriteRenderer != null)
            //{
            //    m_SpriteRenderer.color = value;
            //}



            foreach (Transform part in se_PlayerBodyParts)
            {
                SpriteRenderer childSpriteRenderer = part.GetComponent<SpriteRenderer>();

                if (childSpriteRenderer != null)
                {
                    childSpriteRenderer.color = value;
                }
            }
        }
    }

    public void Move(Vector2 i_Direction, float i_Delta)
    {
        int xDir = Mathf.RoundToInt(i_Direction.x);
        int yDir = Mathf.RoundToInt(i_Direction.y);

        Vector2 moveToVector =  new Vector3(xDir, yDir, 0);

        transform.position = Vector3.MoveTowards(transform.position, moveToVector, m_CurrentMoveSpeed * i_Delta);
    }

    private void Update()
    {
        if (!Utils.s_IsGamePaused)
        {
            float scale = 0.9f + Mathf.PingPong(Time.time * .5f, 0.2f);
            transform.localScale = new Vector2(scale, scale);
        }
    }

    public void Shoot()
    {
        if (se_ProjectilePrefab != null && this != null)
        {
            GameObject projectileClone = Instantiate(se_ProjectilePrefab, transform.position, Quaternion.identity);
            if (MoveDirection != Vector3.zero)
            {
                projectileClone.GetComponent<Rigidbody2D>().AddForce(MoveDirection * (se_ProjectileSpeed / 100f));
                //Destroy(projectileClone, 3f);
            }
        }
    }

    private void OnDestroy()
    {
        VFXManager vfxManager = FindObjectOfType<VFXManager>();
        if (vfxManager != null)
        {
            vfxManager.PlayVFXWithColor(se_DeathVFXType, transform.position, Color);
        }
    }

    public void ActivateShields(bool i_Active)
    {
        foreach (Transform playerShieldPart in se_PlayerShieldParts)
        {
            playerShieldPart.gameObject.SetActive(i_Active);
        }
    }
}
