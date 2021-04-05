using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TipManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI se_TipText = null;
    [SerializeField] private float se_TimeToFade = 2.5f;

    private float m_TimeToStartFade = 5f;
    private float m_ElapsedTimeToFade = 0;
    private float m_ElapsedStartTimeToFade = 0;
    

    private const string k_SwipeString = "Fill the requied amount of\n area by swiping in any of the\n4 directions to move";
    private const string k_FillString = "At the top, you can see how\nhow much area is left to filled\nfor the level";
    private const string k_EnemyString = "Don't let moving enemies hit\nyou or the trail you leave";
    private const string k_ScoreStringTip1 = "Filling areas is rewarded\nwith points, try get as much\n as you can";
    private const string k_ScoreStringTip2 = "The larger the area filled\nat once, the more points will\nbe recieved";
    private const string k_SlowDownPowerupText = "The slow down powerup will\n make all enemies move slower";
    private const string k_SpeedPowerupText = "The speed boost powerup\nwill make you go faster";
    private const string k_ShieldPowerupText = "The shield powerup will\nprotect you and your trail\nfrom getting hit";
    private const string k_ShootPowerupText = "The target powerup will\nlet you destroy enemies\n with projectiles";
    private const string k_BombPowerupText = "The bomb powerup will instantly\nfill an area around you";
    private const string k_DestroyerEnemyText = "The Destroyer enemy type\nwill break colored areas,\nhindering your progress";
    private const string k_SplitterEnemyText = "The Splitter enemy type\nwill split into 4 new\nregular enemies after some time";
    private const string k_SpikeEnemyText = "The Cross enemy is a bigger\nvarient of the regular enemy";
    private const string k_CropperEnemyText = "The Cropper enemy type will go\nthrough any colored areas,\nbreaking it";
    private const string k_OnColorEnemyText = "The Spike enemy type is like\na regular enemy but travels\non colored areas";
    private const string k_ShooterEnemyText = "The Shooter enemy type will\nhurl projectiles at you\ntry to avoid them";
    private const string k_BomberEnemyText = "The Bomber enemy will explode\nafter some time, destroying\ncolored area around it";
    public bool Fade { get; set; }
    public bool StartFade { get; set; }

    public TextMeshProUGUI TipText
    {
        get { return se_TipText; }
    }

    private void Awake()
    {
        se_TipText.alpha = 0;
    }

    private void Update()
    {
        if (!Utils.s_IsGamePaused)
        {
            handleFade();
        }
    }

    private void handleFade()
    {
        if (StartFade)
        {
            if (m_ElapsedStartTimeToFade >= m_TimeToStartFade)
            {
                FadeTextOverTime();
                StartFade = false;
            }

            m_ElapsedStartTimeToFade += Time.deltaTime;
        }

        if (Fade)
        {
            if (se_TipText.alpha <= 0)
            {
                Fade = false;
            }
            else
            {
                se_TipText.alpha -= Time.deltaTime / se_TimeToFade;
            }
        }
    }

    public void ActivateTipText()
    {
        switch (Utils.s_CurrentLevel)
        {
            case 1:
                se_TipText.text = k_SwipeString;
                break;
            case 2:
                se_TipText.text = k_EnemyString;
                break;
            case 3:
                se_TipText.text = k_FillString;
                break;
            case 4:
                se_TipText.text = k_ScoreStringTip1;
                break;
            case 5:
                se_TipText.text = k_ScoreStringTip2;
                break;
            case 9:
                se_TipText.text = k_SlowDownPowerupText;
                break;
            case 11:
                se_TipText.text = k_DestroyerEnemyText;
                break;
            case 16:
                se_TipText.text = k_OnColorEnemyText;
                break;
            case 19:
                se_TipText.text = k_SpeedPowerupText;
                break;
            case 23:
                se_TipText.text = k_CropperEnemyText;
                break;
            case 27:
                se_TipText.text = k_SpikeEnemyText;
                break;
            case 32:
                se_TipText.text = k_BombPowerupText;
                break;
            case 37:
                se_TipText.text = k_BomberEnemyText;
                break;
            case 42:
                se_TipText.text = k_ShootPowerupText;
                break;
            case 48:
                se_TipText.text = k_SplitterEnemyText;
                break;
            case 55:
                se_TipText.text = k_ShieldPowerupText;
                break;
            case 60:
                se_TipText.text = k_ShooterEnemyText;
                break;
            default:
                return;
        }

        Fade = false;
       
        se_TipText.alpha = 1f;
        
    }

    public void FadeTextOverTime()
    {
        Fade = true;
    }

    public void FadeOverTimeWithDelay(float i_Delay)
    {
        m_TimeToStartFade = i_Delay;
        m_ElapsedStartTimeToFade = 0;
        StartFade = true;
    }
}
