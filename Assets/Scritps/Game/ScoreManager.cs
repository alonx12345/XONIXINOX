using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI se_ScoreText = null;
    [SerializeField] private TextMeshProUGUI se_ScoreAddText = null;
    [SerializeField] private float[] se_ScoreMultiplier = null;
    [SerializeField] private float[] se_FillPercents = null;
    [SerializeField] private float se_TimeToStartFade = 2f;
    [SerializeField] private float se_TimeToFade = 2f;

    private Dictionary<float, float> m_ScoreMatrix = null;

    private int m_CurrentScore = 0;

    private float m_ElapsedTimeTillStartFade = 0;
    private float m_ElapsedTimeTillFade = 0;
    private bool m_StartFadeTimeout = false;
    private bool m_StartFade = false;

    public float CurrentScore
    {
        get { return m_CurrentScore; }
    }

    private void Awake()
    {
        m_CurrentScore = 0;
        se_ScoreText.text = m_CurrentScore.ToString();
        loadScoreMatrix();

        se_ScoreAddText.alpha = 0;
    }

    private void Update()
    {
        if (m_StartFadeTimeout)
        {
            m_ElapsedTimeTillStartFade += Time.deltaTime;

            if (m_ElapsedTimeTillStartFade >= se_TimeToStartFade)
            {
                m_StartFade = true;
                m_StartFadeTimeout = false;
            }
        }

        if (m_StartFade)
        {
            se_ScoreAddText.alpha -= Time.deltaTime / se_TimeToFade;

            if (se_ScoreAddText.alpha <= 0)
            {
                m_StartFade = false;
            }
        }
    }

    private void loadScoreMatrix()
    {
        m_ScoreMatrix = new Dictionary<float, float>();
        if (se_ScoreMultiplier.Length != se_FillPercents.Length)
        {
            Debug.LogError("Cannot load scoring matrix");
            return;
        }

        for (int i = 0; i < se_FillPercents.Length; i++)
        {
            m_ScoreMatrix[se_FillPercents[i]] = se_ScoreMultiplier[i];
        }
    }

    public void AddScoreByFill(int i_numberOfTileFilled, int i_totalNumberOfTiles)
    {
        //float fillPercent = i_numberOfTileFilled / (i_totalNumberOfTiles * 1f);

        //if (fillPercent < se_FillPercents[0])
        //{
        //    addScore(i_numberOfTileFilled);
        //    return;
        //}

        //for (int i = 0; i < se_FillPercents.Length - 1; i++)
        //{
        //    if (fillPercent >= se_FillPercents[i] && fillPercent <= se_FillPercents[i + 1])
        //    {
        //        addScore((int)(i_numberOfTileFilled * m_ScoreMatrix[se_FillPercents[i]]));
        //        return;
        //    }
        //}

        //addScore((int)(i_numberOfTileFilled * se_ScoreMultiplier[se_ScoreMultiplier.Length - 1]));

        addScore(i_numberOfTileFilled);
    }

    private void addScore(int i_score)
    {
        m_CurrentScore += i_score;
        se_ScoreText.text = m_CurrentScore.ToString();
        se_ScoreAddText.text = "+" + i_score.ToString();

        m_StartFade = false;
        m_StartFadeTimeout = true;
        se_ScoreAddText.alpha = 1;
        m_ElapsedTimeTillStartFade = 0;

        if (m_CurrentScore > GameData.LoadHighScore())
        {
            GameData.SaveHighScore(m_CurrentScore);
        }
    }
}
