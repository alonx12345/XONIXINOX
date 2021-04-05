using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    [SerializeField] private float se_PlayerSpeedPowerupTime = 5f;
    [SerializeField] private float se_EnemyPowerupTime = 5f;
    [SerializeField] private float se_PlayerShootPowerupTime = 5f;
    //[SerializeField] private float se_MiniPlayerPowerupTime = 5f;
    [SerializeField] private float se_ShieldPowerupTime = 5f;

    [SerializeField] private float se_TimeBetweenAlphaChanges = .1f;
    [SerializeField] private float se_AlphaChange = .05f;

    [SerializeField] private float se_TimeBetweenShots = 1f;
    [SerializeField] private float se_TimeBetweenColorChanges = .1f;

    public event Action<eSoundID> PlaySoundAction;
    public bool m_ShootPowerupActive { get; set; } = false;
    public bool m_EnemyPowerupActive { get; set; } = false;
    public bool m_PlayMovePowerupActive { get; set; } = false;
    public bool m_MiniPlayerPowerupActive { get; set; } = false;
    public bool m_ShieldPowerupActive { get; set; } = false;

    private bool m_AlphaGoesDown = true;

    private float m_elapsedMove;
    private float m_ElapsedSlowEnemyTime;
    private float m_ElapsedShoot;
    private float m_elapsedMini;
    private float m_ElapsedTimeBetweenShots;
    private float m_ElapsedBetweenColorChange;

    private float m_ElapsedAlphaWave;
    private float m_ElapsedTimeBetweenAlphaChanges;

    private float m_ElapsedShieldTime;

    private int m_ColorIndex = 0;
    private Color m_CurrentPlayerColor;

    private float m_PlayerDefaultMoveSpeed;

    private PlayerController m_Player;

    public PlayerController Player
    {
        set
        {
            m_Player = value;
            m_PlayerDefaultMoveSpeed = m_Player.DefaultMoveSpeed;
            m_CurrentPlayerColor = m_Player.Color;
        }
    }

    public void ActivatePowerup(ePowerupType i_PowerupType)
    {
        switch (i_PowerupType)
        {
            case ePowerupType.EXTRA_LIFE:
                PlaySoundAction?.Invoke(eSoundID.ExtraLifePowerupPickupSound);
                //FindObjectOfType<AudioManager>().Play("ExtraLifePowerupPickupSFX");
                break;
            case ePowerupType.SLOW_DOWN:
                PlaySoundAction?.Invoke(eSoundID.EnemySlowDownPowerupPickupSound);
                HandleEnemySlowdown();
                //FindObjectOfType<AudioManager>().Play("EnemySlowDownPowerupPickupSFX");
                break;
            case ePowerupType.SPEED_UP:
                PlaySoundAction?.Invoke(eSoundID.PlayerSpeedUpPowerupPickupSound);
                HandleSpeedup();
                //FindObjectOfType<AudioManager>().Play("PlayerSpeedUpPowerupPickupSFX");
                break;
            case ePowerupType.MINI_PLAYER:
                PlaySoundAction?.Invoke(eSoundID.MiniPlayerPowerupPickupSound);
                // FindObjectOfType<AudioManager>().Play("PlayerSpeedUpPowerupPickupSFX");
                break;
            case ePowerupType.SHOOT:
                PlaySoundAction?.Invoke(eSoundID.ShootPowerupPickupSound);

                //FindObjectOfType<SoundManager>().Play(eSoundID.ShootPowerupPickupSound);
                HandleShoot();
                // FindObjectOfType<AudioManager>().Play("ShootPowerupPickupSFX");
                break;
            case ePowerupType.TIME:
                break;
            case ePowerupType.SHIELD:
                PlaySoundAction?.Invoke(eSoundID.Shield);
                HandleShield();
                break;
            case ePowerupType.BOMB:
                PlaySoundAction?.Invoke(eSoundID.GameOverSound);
                HandleBomb();
                break;
        }
    }

    private void HandleBomb()
    {
        int row = Mathf.RoundToInt(m_Player.transform.position.x);
        int col = Mathf.RoundToInt(m_Player.transform.position.y);

        FindObjectOfType<GridManager>().FillAroundPoint(row, col, 7);
    }

    private void Update()
    {
        if (!Utils.s_IsGamePaused)
        {
            SpeedUpTimer();
            EnemySlowdownTimer();
            ShootTimer();
            shieldTimer();
        }
        
        //MiniPlayerTimer();
    }

    public void HandleShield()
    {
        m_ElapsedShieldTime = se_ShieldPowerupTime;

        m_CurrentPlayerColor = m_Player.Color;

        m_Player.ActivateShields(true);
        m_Player.CanBeHit = false;

        foreach (Transform shieldPart in m_Player.se_PlayerShieldParts)
        {
            shieldPart.GetComponent<SpriteRenderer>().color = Color.cyan;
        }

        //if (!m_ShieldPowerupActive)
        //{
            
        //}

        m_ShieldPowerupActive = true;
    }

    private void shieldTimer()
    {
        if (m_ShieldPowerupActive)
        {
            m_ElapsedShieldTime -= Time.deltaTime;

            if (m_ElapsedShieldTime <= 0.5f * se_ShieldPowerupTime)
            {
                float speed = 1f;

                if (m_ElapsedShieldTime < 0.7f * se_ShieldPowerupTime)
                {
                    speed = 2f;
                }

                float alpha = Mathf.PingPong(speed * Time.time, 1f);
                Color shieldColor = Color.cyan;
                shieldColor.a = alpha;

                foreach (Transform shieldPart in m_Player.se_PlayerShieldParts)
                {
                    
                    shieldPart.GetComponent<SpriteRenderer>().color = shieldColor;
                }
            }

            if (m_ElapsedShieldTime <= 0)
            {
                m_ShieldPowerupActive = false;
                m_Player.ActivateShields(false);
                m_Player.CanBeHit = true;
            }
        }
    }

    public void HandleSpeedup()
    {
        m_elapsedMove = se_PlayerSpeedPowerupTime;

        if (!m_PlayMovePowerupActive)
        {
            m_Player.CurrentMoveSpeed = m_PlayerDefaultMoveSpeed * 1.5f;
        }

        m_PlayMovePowerupActive = true;
    }

    private void SpeedUpTimer()
    {
        if (m_PlayMovePowerupActive)
        {
            m_elapsedMove -= Time.deltaTime;
            if (m_elapsedMove <= 0)
            {
                m_PlayMovePowerupActive = false;
                m_Player.CurrentMoveSpeed = m_PlayerDefaultMoveSpeed;
            }
        }
    }

    // TODO
    //public void HandleMiniPlayerPowerup()
    //{
    //    m_elapsedMini = se_miniPlayerPowerupTime;
    //    m_MiniPlayerPowerupActive = true;
    //}

    //// TODO
    //private void MiniPlayerTimer()
    //{
    //    if (m_MiniPlayerPowerupActive)
    //    {
    //        m_elapsedMini -= Time.deltaTime;
    //        if (m_elapsedMini <= 0)
    //        {
    //            m_MiniPlayerPowerupActive = false;
    //        }
    //    }
    //}

    //TODO
    public void HandleShoot()
    {
        m_ElapsedShoot = se_PlayerShootPowerupTime;

        if (!m_ShootPowerupActive)
        {
            m_ElapsedTimeBetweenShots = se_TimeBetweenShots;
            m_ElapsedBetweenColorChange = se_TimeBetweenColorChanges;
            m_CurrentPlayerColor = m_Player.Color;
        }

        m_ShootPowerupActive = true;
    }

    //TODO
    private void ShootTimer()
    {
        if (m_ShootPowerupActive)
        {
            m_ElapsedShoot -= Time.deltaTime;
            m_ElapsedTimeBetweenShots -= Time.deltaTime;
            m_ElapsedBetweenColorChange -= Time.deltaTime;

            if (m_ElapsedTimeBetweenShots <= 0)
            {
                PlaySoundAction?.Invoke(eSoundID.LaserShootSound);
                m_Player.Shoot();
                m_ElapsedTimeBetweenShots = se_TimeBetweenShots;
                //FindObjectOfType<AudioManager>().Play("ShootPowerupSFX");
            }

            if (m_ElapsedBetweenColorChange <= 0)
            {
                if (m_ColorIndex >= Utils.s_GameColors.Length)
                {
                    m_ColorIndex = 0;
                }

                Color playerColor = Utils.s_GameColors[m_ColorIndex];
                playerColor.a = 1f;
                m_ColorIndex++;
                m_Player.Color = playerColor;
                m_ElapsedBetweenColorChange = se_TimeBetweenColorChanges;
            }

            if (m_ElapsedShoot <= 0)
            {
                m_Player.Color = m_CurrentPlayerColor;
                m_ShootPowerupActive = false;
            }
        }
    }

    public void HandleEnemySlowdown()
    {
        m_ElapsedSlowEnemyTime = se_EnemyPowerupTime;
        if (!m_EnemyPowerupActive)
        {
            m_ElapsedTimeBetweenAlphaChanges = se_TimeBetweenAlphaChanges;
            m_ElapsedAlphaWave = 1f;
            m_AlphaGoesDown = true;

            Enemy[] enemies = FindObjectsOfType<Enemy>();

            foreach (var enemy in enemies)
            {
                enemy.CurrentMoveSpeed /= 3f;

                SplitterEnemy splitter = enemy.GetComponent<SplitterEnemy>();

                if (splitter != null)
                {
                    splitter.TimeToSplitSpeed /= 3f;
                }

                ShootEnemy shooter = enemy.GetComponent<ShootEnemy>();

                if (shooter != null)
                {
                    shooter.TimeToShoot *= 3f;
                    shooter.Projectile.ProjectileSpeed /= 3f;
                }

                EnemyBomber bomber = enemy.GetComponent<EnemyBomber>();

                if (bomber != null)
                {
                    bomber.TimeToExplodeSpeed /= 3f;
                }
            }
        }

        m_EnemyPowerupActive = true;
    }

    private void EnemySlowdownTimer()
    {
        if (m_EnemyPowerupActive)
        {
            m_ElapsedSlowEnemyTime -= Time.deltaTime;
            m_ElapsedTimeBetweenAlphaChanges -= Time.deltaTime;

            if (m_ElapsedTimeBetweenAlphaChanges <= 0)
            {
                if (m_ElapsedAlphaWave > 0.7f && m_AlphaGoesDown)
                {
                    m_ElapsedAlphaWave = Mathf.Max(m_ElapsedAlphaWave - se_AlphaChange, 0.7f);
                    if (m_ElapsedAlphaWave <= 0.7f)
                    {
                        m_AlphaGoesDown = false;
                    }
                }
                else if (m_ElapsedAlphaWave < 1f && !m_AlphaGoesDown)
                {
                    m_ElapsedAlphaWave = Mathf.Min(m_ElapsedAlphaWave + se_AlphaChange, 1f);
                    if (m_ElapsedAlphaWave >= 1f)
                    {
                        m_AlphaGoesDown = true;
                    }
                }

                Enemy[] enemies = FindObjectsOfType<Enemy>();
                foreach (var enemy in enemies)
                {
                    Color enemyColor = enemy.GetComponent<SpriteRenderer>().color;

                    enemy.GetComponent<SpriteRenderer>().color = new Color(enemyColor.r, enemyColor.g, enemyColor.b, m_ElapsedAlphaWave);

                    List<SpriteRenderer> enemyRenderers = enemy.GetComponentsInChildren<SpriteRenderer>().ToList();
                    foreach (var enemyRenderer in enemyRenderers)
                    {
                        enemyRenderer.color = new Color(enemyRenderer.color.r, enemyRenderer.color.g, enemyRenderer.color.b, m_ElapsedAlphaWave);
                    }
                }

                m_ElapsedTimeBetweenAlphaChanges = se_TimeBetweenAlphaChanges;
            }

            if (m_ElapsedSlowEnemyTime <= 0)
            {
                m_EnemyPowerupActive = false;
                Enemy[] enemies = FindObjectsOfType<Enemy>();
                foreach (var enemy in enemies)
                {
                    Color enemyColor = enemy.GetComponent<SpriteRenderer>().color;
                    enemy.GetComponent<SpriteRenderer>().color = new Color(enemyColor.r, enemyColor.g, enemyColor.b, 1f);
                    List<SpriteRenderer> enemyRenderers = enemy.GetComponentsInChildren<SpriteRenderer>().ToList();
                    foreach (var enemyRenderer in enemyRenderers)
                    {
                        enemyRenderer.color = new Color(enemyRenderer.color.r, enemyRenderer.color.g, enemyRenderer.color.b, 1f);

                    }
                    enemy.CurrentMoveSpeed *= 3f;

                    SplitterEnemy splitter = enemy.GetComponent<SplitterEnemy>();

                    if (splitter != null)
                    {
                        splitter.TimeToSplitSpeed *= 3f;
                    }

                    ShootEnemy shooter = enemy.GetComponent<ShootEnemy>();

                    if (shooter != null)
                    {
                        shooter.TimeToShoot /= 3f;
                        shooter.Projectile.ProjectileSpeed *= 3f;
                    }

                    EnemyBomber bomber = enemy.GetComponent<EnemyBomber>();

                    if (bomber != null)
                    {
                        bomber.TimeToExplodeSpeed *= 3f;
                    }
                }
            }
        }
    }
   
    public void Reset()
    {
        m_ShootPowerupActive = false;
        m_EnemyPowerupActive = false;
        m_PlayMovePowerupActive = false;
        m_ShieldPowerupActive = false;
    }
}
