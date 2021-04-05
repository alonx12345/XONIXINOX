using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int se_GridWidth = 31;
    [SerializeField] private int se_GridHeight = 31;
    [SerializeField] private PlayerController se_PlayerPrefab = null;

    [SerializeField] private GameObject se_LoseLabel = null;
    [SerializeField] private TextMeshProUGUI se_ScoreText = null;
    [SerializeField] private TextMeshProUGUI se_HighScoreText = null;
    [SerializeField] private GameObject se_JoystickCanvas = null;

    [SerializeField] private float se_PercentToNextLevel = .8f;

    [SerializeField] private TextMeshProUGUI se_CurrentLevelText = null;
    [SerializeField] private TextMeshProUGUI se_NextLevelText = null;

    [SerializeField] private GameObject[] se_ProgressBarForegrounds = null;
    [SerializeField] private TextMeshProUGUI[] se_LevelsTexts = null;

    [SerializeField] private float se_SpawnDelay = 1f;
    [SerializeField] private float se_GameOverDelay = 1f;
    [SerializeField] private float se_DestroyObjectsDelay = 1f;
    [SerializeField] private float se_ColorAllGridDelay = 1f;
    [SerializeField] private float se_TimeBetweenRingsColor = .01f;

    [SerializeField] private GameObject[] se_BoxLevelFills = null;
    [SerializeField] private eVisualEffectType se_StartLevelEffect = eVisualEffectType.PlayerExplosion;
    [SerializeField] private eVisualEffectType se_LevelCompleteVFX = eVisualEffectType.LevelComplete;
    [SerializeField] private bool se_ResetLevels = false;

    private GridSpawner m_GridSpawner;
    private GridManager m_GridManager;
    private ColorsManager m_ColorsManager;
    private StageManager m_StageManager;
    private ScoreManager m_ScoreManager;
    private PowerupManager m_PowerupManager;
    private PowerupSpawner m_PowerupSpawner;
    private VFXManager m_VFXManager;
    private SoundManager m_SoundManager;
    private TipManager m_TipManager;

    private Color m_CurrentFillColor = Color.black;
    private Color m_CurrentBackgroundColor = Color.black;
    private Color m_CurrentPlayerColor;
    private Color m_FillAllColor;

    private PlayerController m_Player;
    private List<Enemy> m_Enemies;

    private bool[][] m_GridFillPositions;
    private Waypoint[,] m_TileGrid;
    private List<Waypoint> m_AllWaypointsList;
    private List<Waypoint> m_CurrentTilesToFill;
    private float m_NumberTileFilled;

    private int m_CurrentLevel;
    private int m_NextLevel;

    private Coroutine m_FillBarRoutine;
    private float m_LastAmountFilled = 0;
    private float m_LevelBonus = 0;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        Utils.s_GridWidth = se_GridWidth;
        Utils.s_GridHeight = se_GridHeight;
        m_AllWaypointsList = new List<Waypoint>();

        m_GridSpawner = GetComponent<GridSpawner>();
        m_GridManager = GetComponent<GridManager>();
        m_ColorsManager = GetComponent<ColorsManager>();
        m_StageManager = GetComponent<StageManager>();
        m_ScoreManager = GetComponent<ScoreManager>();
        m_PowerupSpawner = GetComponent<PowerupSpawner>();
        m_PowerupManager = GetComponent<PowerupManager>();
        m_VFXManager = GetComponent<VFXManager>();
        m_SoundManager = FindObjectOfType<SoundManager>();
        m_TipManager = GetComponent<TipManager>();
    }

    private void Start()
    {
        Utils.s_IsGamePaused = false;

        if (se_PlayerPrefab == null)
        {
            Debug.LogError("No player prefab set");
            return;
        }

        if (se_ResetLevels)
        {
            GameData.SaveLevel(0);
            GameData.SaveHighScore(0);
        }

        int currentLevel = GameData.LoadLevel();

        for (int i = 1; i < currentLevel; i++)
        {
            m_LevelBonus += 0.1f;
        }


        m_CurrentLevel = currentLevel == 0 ? 1 : currentLevel;
        m_NextLevel = m_CurrentLevel + 1;

        Utils.s_CurrentLevel = m_CurrentLevel;
       

        setLevelsText();

        spawnAndSetPlayer();
        setColors();
        initGrid();

        m_FillAllColor = m_Player.Color;
        colorAllGrid();
        //StartCoroutine(spiralColor(0, 0, Utils.s_GridWidth, Utils.s_GridHeight));

        m_GridManager.Player = m_Player;
        m_GridManager.DeathAction += HandleDeath;

        m_CurrentTilesToFill = m_AllWaypointsList.Where(point => !point.Flooded).ToList();

        handleProgress(0,false);
        m_GridManager.ProgressAction += handleProgress;
        m_GridManager.PlaySoundAction += HandleSound;
        m_PowerupManager.PlaySoundAction += HandleSound;

        spawnEnemiesByLevel();
        initUI();
        
        StartCoroutine(pauseStart());

    }

    private IEnumerator pauseStart()
    {
        togglePlayer();
        yield return new WaitForSeconds(.1f);
        togglePlayer();
        TogglePause();
    }

    public void StartEffect()
    {
        m_FillAllColor = m_Player.Color;
        colorAllGrid();
        StartCoroutine(StartEffectDelayed());
    }

    public IEnumerator StartEffectDelayed()
    {
        yield return new WaitForSeconds(.2f);
        StartCoroutine(spiralColor(0, 0, Utils.s_GridWidth, Utils.s_GridHeight));
        StartCoroutine(playPlayerVFXWithDelay());
    }

    private IEnumerator playPlayerVFXWithDelay()
    {
        yield return new WaitForSeconds(.3f);
        m_VFXManager.PlayVFXWithColor(se_StartLevelEffect, m_Player.transform.position, m_Player.Color);
    }

    private void setColors()
    {
        Color playerColorToAvoid = m_CurrentFillColor != Color.black ? m_CurrentFillColor : m_ColorsManager.GenerateColor();

        m_CurrentPlayerColor = playerColorToAvoid;
        m_CurrentPlayerColor.a = 1f;
        m_Player.Color = m_CurrentPlayerColor;

        while ((m_CurrentFillColor = m_ColorsManager.GenerateColor()) == playerColorToAvoid)
        {
        }

        m_GridManager.PlayerColor = playerColorToAvoid;
        m_GridManager.FillColor = m_CurrentFillColor;
        m_GridManager.BackgroundColor = m_CurrentBackgroundColor;
    }

    private void initGrid()
    {
        loadNextGridPositions();

        m_TileGrid = m_GridSpawner.CreateTileGrid(m_GridFillPositions, m_CurrentFillColor, m_CurrentBackgroundColor);

        m_GridManager.TileGrid = m_TileGrid;
        m_GridManager.AllWaypoints = m_AllWaypointsList;

        for (int i = 0; i < se_GridWidth; i++)
        {
            for (int j = 0; j < se_GridHeight; j++)
            {
                m_AllWaypointsList.Add(m_TileGrid[i, j]);
            }
        }
    }

    private void loadNextGridPositions()
    {
        if (GameData.GameGrids == null ||
            GameData.GameGrids.Count == 0 ||
            GameData.GameGrids[0].Length != se_GridWidth ||
            GameData.GameGrids[0][0].Length != se_GridHeight)
        {
            GameData.GameGrids = new List<bool[][]>();
            m_GridFillPositions = Utils.Init2DArrayRows<bool>(se_GridWidth, se_GridHeight);
        }
        else
        {
            int currentGridIndex = Random.Range(0, GameData.GameGrids.Count);

            switch (m_CurrentLevel)
            {
                case 1:
                    currentGridIndex = 30;
                    break;
                case 2:
                    currentGridIndex = 30;
                    break;
                case 3:
                    currentGridIndex = 31;
                    break;
                case 4:
                    currentGridIndex = 33;
                    break;
                case 5:
                    currentGridIndex = 33;
                    break;
                case 9:
                    currentGridIndex = 25;
                    break;
                case 19:
                    currentGridIndex = 22;
                    break;
                case 32:
                    currentGridIndex = 23;
                    break;
                case 42:
                    currentGridIndex = 26;
                    break;
                case 55:
                    currentGridIndex = 24;
                    break;
            }

            m_GridFillPositions = GameData.GameGrids[currentGridIndex];
        }
    }

    private void spawnEnemiesByLevel()
    {
        if (m_CurrentLevel <= 62)
        {
            m_Enemies = m_StageManager.SpawnEnemiesByLevel(m_CurrentLevel);
        }
        else
        {
            m_Enemies = m_StageManager.SpawnEnemiesByLevel(Random.Range(6, 63));
        }
        
        m_GridManager.Enemies = m_Enemies;

        foreach (Enemy enemy in m_Enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            enemy.TouchedPlayerAction += HandleDeath;
            enemy.Destroyer.DestroyVFXAction += m_VFXManager.PlayVFX;
            enemy.PlaySoundAction += HandleSound;

            SplitterEnemy splitterEnemy = enemy.GetComponent<SplitterEnemy>();

            if (splitterEnemy != null)
            {
                splitterEnemy.SubscribeToSplit(UpdateEnemies);
            }
        }
    }

    private void UpdateEnemies()
    {
        m_Enemies = FindObjectsOfType<Enemy>().ToList();
        m_GridManager.Enemies = m_Enemies;

        foreach (Enemy enemy in m_Enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            enemy.TouchedPlayerAction += HandleDeath;
            enemy.Destroyer.DestroyVFXAction += m_VFXManager.PlayVFX;
            enemy.PlaySoundAction += HandleSound;
        }
    }

    private void initUI()
    {
        if (se_CurrentLevelText != null && se_NextLevelText != null)
        {
            se_CurrentLevelText.text = m_CurrentLevel.ToString();
            se_NextLevelText.text = m_NextLevel.ToString();
        }

        //if (se_BoxLevelFills != null)
        //{
            //foreach (GameObject box in se_BoxLevelFills)
            //{
            //    box.SetActive(false);
            //}

            //se_BoxLevelFills[0].SetActive(true);

            for (int i = 0; i < (m_CurrentLevel - 1) % 5; i++)
            {
                foreach (Transform childTransform in se_ProgressBarForegrounds[i].transform)
                {
                    childTransform.localScale = new Vector2(1f, childTransform.localScale.y);
                }

                se_ProgressBarForegrounds[i].transform.localScale = new Vector2(1f, se_ProgressBarForegrounds[i].transform.localScale.y);
                //print(se_ProgressBarForegrounds[i].transform.GetChild(0).transform.localScale.x);
            }
        //}

        if (se_LoseLabel != null)
        {
            se_LoseLabel.SetActive(false);
        }
    }

    private IEnumerator handleNextLevel()
    {
        m_VFXManager.PlayVFX(se_LevelCompleteVFX, se_LevelsTexts[(m_CurrentLevel - 1) % 5].transform.position);

        m_CurrentLevel++;
        m_NextLevel++;
        Utils.s_CurrentLevel = m_CurrentLevel;
        
        m_LevelBonus += 0.1f;

        m_TipManager.TipText.alpha = 0;
        //TODO
        //if (m_CurrentLevel % 5 == 1)
        //{
        GameData.SaveLevel(m_CurrentLevel);
        //}

        m_FillAllColor = m_CurrentFillColor;

        destroyAllObjects();

        m_PowerupManager.Reset();

        yield return new WaitForSeconds(se_DestroyObjectsDelay);

        foreach (Transform childTransform in se_ProgressBarForegrounds[(m_CurrentLevel - 2) % 5].transform)
        {
            StartCoroutine(fillOverTime(childTransform));
        }

        colorAllGrid();

        yield return new WaitForSeconds(se_ColorAllGridDelay);

        //se_PlayerPrefab.GetComponent<SpriteRenderer>().color = m_CurrentFillColor;

        se_PlayerPrefab.GetComponent<PlayerController>().Color = m_CurrentFillColor;

        spawnAndSetPlayer();

        //m_Player.GetComponent<SpriteRenderer>().enabled = false;

        //foreach (SpriteRenderer childSpriteRenderer in m_Player.GetComponentsInChildren<SpriteRenderer>())
        //{
        //    childSpriteRenderer.enabled = false;
        //}
        //HandleSound(eSoundID.RespawnPlayerSound);

        togglePlayerLook(false);
        setColors();
        togglePlayer(false);
        loadNextGridPositions();

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(spiralColor(0, 0, Utils.s_GridWidth, Utils.s_GridHeight));

        yield return new WaitForSeconds(.1f);
        StartCoroutine(setPlayerColors());

        m_CurrentTilesToFill = m_AllWaypointsList.Where(point => !point.Flooded).ToList();

        yield return new WaitForSeconds(.1f);

        m_LastAmountFilled = 0;

        //togglePlayer();
        m_VFXManager.PlayVFXWithColor(se_StartLevelEffect, m_Player.transform.position, m_Player.Color);
        yield return new WaitForSeconds(.1f);
        spawnEnemiesByLevel();
        yield return new WaitForSeconds(.3f);
        
        handleProgress(0, false);

        if (m_CurrentLevel % 5 == 1)
        {
            foreach (GameObject progressBarForeground in se_ProgressBarForegrounds)
            {
                progressBarForeground.transform.localScale = new Vector3(0, progressBarForeground.transform.localScale.y);

                foreach (Transform childTransform in progressBarForeground.transform)
                {
                    childTransform.localScale = new Vector3(0 , childTransform.localScale.y);
                }
            }

            setLevelsText();
        }


        //StartCoroutine(handleProgressUI(0));

        //se_CurrentLevelText.text = m_CurrentLevel.ToString();
        //se_NextLevelText.text = (m_NextLevel).ToString();

        Utils.s_IsLevelFinished = false;
    }

    private void setLevelsText()
    {
        for (var i = 0; i < se_LevelsTexts.Length; i++)
        {
            TextMeshProUGUI levelsText = se_LevelsTexts[i];

            int balancer = (m_CurrentLevel - 1) % 5;

            string levelToShow = (m_CurrentLevel + i - balancer).ToString();

            levelsText.text = levelToShow;

            switch (levelToShow.Length)
            {
                case 1:
                    levelsText.fontSize = 50;
                    break;
                case 2:
                    levelsText.fontSize = 46;
                    break;
                case 3:
                    levelsText.fontSize = 25;
                    break;
                case 4:
                    levelsText.fontSize = 18;
                    break;
            }
        }
    }

    private void destroyAllObjects()
    {
        if (m_Player != null)
        {
            Destroy(m_Player.gameObject);
        }

        m_Enemies = FindObjectsOfType<Enemy>().ToList();

        foreach (var enemy in m_Enemies)
        {
            if (enemy == null)
            {
                continue;
            }
            Destroy(enemy.gameObject);
        }

        Powerup[] powerups = FindObjectsOfType<Powerup>();

        foreach (Powerup powerup in powerups)
        {
            if (powerup == null)
            {
                continue;
            }
            Destroy(powerup.gameObject);
        }

        EnemyProjectile[] enemyProjectiles = FindObjectsOfType<EnemyProjectile>();

        foreach (EnemyProjectile enemyProjectile in enemyProjectiles)
        {
            if (enemyProjectile == null)
            {
                continue;
            }
            Destroy(enemyProjectile.gameObject);
        }

        PlayerProjectile[] playerProjectiles = FindObjectsOfType<PlayerProjectile>();

        foreach (PlayerProjectile playerProjectile in playerProjectiles)
        {
            if (playerProjectile == null)
            {
                continue;
            }
            Destroy(playerProjectile.gameObject);
        }
    }

    private void colorAllGrid()
    {
        foreach (var waypoint in m_AllWaypointsList)
        {
            waypoint.Color = m_FillAllColor;
            waypoint.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        }
    }

    private void spawnAndSetPlayer()
    {
        if (se_PlayerPrefab != null)
        {
            m_Player = Instantiate(se_PlayerPrefab, new Vector3(Utils.s_GridWidth / 2, Utils.s_GridHeight / 2, 0), Quaternion.identity);
            m_GridManager.Player = m_Player;
            m_PowerupManager.Player = m_Player;

            //PlayerInputHandler playerInputHandler = m_Player.GetComponent<PlayerInputHandler>();
            //playerInputHandler.enabled = false;
            //playerInputHandler.CurrentDirection = eDirection.Default;
            //playerInputHandler.MovingDirection = eDirection.Default;
        }
        else
        {
            Debug.LogError("No player prefab found!");
        }
    }

    private void togglePlayer(bool i_Toggle)
    {
        PlayerInputHandler playerInputHandler = m_Player.GetComponent<PlayerInputHandler>();
        playerInputHandler.enabled = i_Toggle;
        playerInputHandler.CurrentDirection = eDirection.Default;
        playerInputHandler.MovingDirection = eDirection.Default;
    }

    private void togglePlayerLook(bool i_Look)
    {
        foreach (SpriteRenderer childSpriteRenderer in m_Player.GetComponentsInChildren<SpriteRenderer>())
        {
            childSpriteRenderer.enabled = i_Look;
        }
    }

    // NOT IN USE
    private IEnumerator colorRings()
    {
        int currentIndex = 0;
        int cols = Utils.s_GridWidth;
        int rows = Utils.s_GridHeight;

        int currentRow = 0;
        int currentCol = 0;
        Waypoint currentWaypointToReset = null;

        while (currentRow < rows && currentCol < cols)
        {
            for (currentIndex = currentCol; currentIndex < cols; ++currentIndex)
            {
                currentWaypointToReset = m_TileGrid[currentRow, currentIndex];
                ColorSingleTile(currentWaypointToReset);
            }

            currentRow++;

            for (currentIndex = currentRow; currentIndex < rows; ++currentIndex)
            {
                currentWaypointToReset = m_TileGrid[currentIndex, cols - 1];
                ColorSingleTile(currentWaypointToReset);
            }
            cols--;

            if (currentRow < rows)
            {
                for (currentIndex = cols - 1; currentIndex >= currentCol; --currentIndex)
                {
                    currentWaypointToReset = m_TileGrid[rows - 1, currentIndex];
                    ColorSingleTile(currentWaypointToReset);
                }
                rows--;
            }

            if (currentCol < cols)
            {
                for (currentIndex = rows - 1; currentIndex >= currentRow; --currentIndex)
                {
                    currentWaypointToReset = m_TileGrid[currentIndex, currentCol];
                    ColorSingleTile(currentWaypointToReset);
                }

                currentCol++;
            }

            yield return new WaitForSeconds(se_TimeBetweenRingsColor);
        }
    }

    private IEnumerator spiralColor(int i, int j, int m, int n)
    {
        if (i >= m || j >= n)
        {
            togglePlayer(true);
            yield return new WaitForSeconds(.3f);
            yield break;
        }

        for (int p = i; p < n; p++)
        {
            ColorSingleTile(m_TileGrid[i, p]);
        }

        for (int p = i + 1; p < m; p++)
        {
            ColorSingleTile(m_TileGrid[p, n - 1]);
        }

        if ((m - 1) != i)
        {
            for (int p = n - 2; p >= j; p--)
            {
                ColorSingleTile(m_TileGrid[m - 1, p]);
            }
        }

        if ((n - 1) != j)
        {
            for (int p = m - 2; p > i; p--)
            {
                ColorSingleTile(m_TileGrid[p, j]);
            }
        }

        yield return new WaitForSeconds(se_TimeBetweenRingsColor);

        StartCoroutine(spiralColor(i + 1, j + 1, m - 1, n - 1));
    }

    private void ColorSingleTile(Waypoint i_Tile)
    {
        if (Utils.OnBorder(i_Tile.transform.position.x, i_Tile.transform.position.y) ||
            Utils.OnMiddle(i_Tile.transform.position.x, i_Tile.transform.position.y) ||
            m_GridFillPositions[Mathf.RoundToInt(i_Tile.transform.position.x)][Mathf.RoundToInt(i_Tile.transform.position.y)])
        {
            Color color = m_CurrentFillColor;

            //if (Utils.OnBorder(i_Tile.transform.position.x, i_Tile.transform.position.y))
            //{
            //    color = new Color(1f, 1f, 0, 0.7f);
            //}

            i_Tile.ColorPoint(color, true);
        }
        else
        {
            i_Tile.Color = m_CurrentBackgroundColor;
            i_Tile.ResetPoint();
        }

        if (i_Tile.transform.position.x == Utils.s_GridWidth / 2 && i_Tile.transform.position.y == Utils.s_GridHeight / 2)
        {
            //m_Player.GetComponent<SpriteRenderer>().enabled = true;
            if (!Utils.s_IsGamePaused)
            {
                togglePlayerLook(true);
                m_TipManager.ActivateTipText();
                m_TipManager.FadeOverTimeWithDelay(15f);
            }
          
        }

        i_Tile.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
    }

    private IEnumerator setPlayerColors()
    {
        while (m_FillAllColor.a < 1f)
        {
            m_FillAllColor.a += Time.deltaTime;
            m_Player.Color = m_FillAllColor;
            yield return new WaitForSeconds(.1f);
        }


        se_PlayerPrefab.GetComponent<PlayerController>().Color = m_FillAllColor;
        //se_PlayerPrefab.GetComponent<SpriteRenderer>().color = m_FillAllColor;
    }

    private void handleProgress(int i_NumberOfTilesFilled, bool i_Score = true)
    {
        //float numberOfTilesFilled = m_CurrentTilesToFill.Count(point => point.Flooded);
        float numberOfTilesFilled = i_NumberOfTilesFilled;
        float percentFilled = Mathf.Min(m_CurrentTilesToFill.Count(point => point.Flooded) / (m_CurrentTilesToFill.Count * se_PercentToNextLevel), 1f);

        if (m_FillBarRoutine != null)
        {
            StopCoroutine(m_FillBarRoutine);
        }

        m_FillBarRoutine = StartCoroutine(handleProgressUI(percentFilled));

        if (i_Score)
        {
            //float newNumberOfTilesFilled = numberOfTilesFilled - m_LastAmountFilled;

            float newNumberOfTilesFilled = i_NumberOfTilesFilled;
            float bonusPerecent = newNumberOfTilesFilled / m_CurrentTilesToFill.Count;

            m_ScoreManager.AddScoreByFill((int)((newNumberOfTilesFilled * (bonusPerecent + 1f)) * (m_LevelBonus + 1f)), m_CurrentTilesToFill.Count);

            print(newNumberOfTilesFilled);
        }

        if (percentFilled >= 1f && !Utils.s_IsLevelFinished)
        {
            HandleSound(eSoundID.LevelFinishedSound);
            StartCoroutine(handleNextLevel());
            Utils.s_IsLevelFinished = true;
        }

        

        m_LastAmountFilled = numberOfTilesFilled;

    }

    private IEnumerator handleProgressUI(float i_Percent)
    {
        if (se_ProgressBarForegrounds != null)
        {
            GameObject currentBar = se_ProgressBarForegrounds[(m_CurrentLevel - 1) % 5];

            if (currentBar.transform.localScale.x < i_Percent)
            {
                while (currentBar.transform.localScale.x < i_Percent)
                {
                    currentBar.transform.localScale = new Vector2(Mathf.Min(Mathf.Min(currentBar.transform.localScale.x + 0.03f, i_Percent), 1f), 1);
                    yield return new WaitForSeconds(0);
                }
            }
            else
            {
                if (currentBar.transform.localScale.x > i_Percent && !Utils.s_IsLevelFinished)
                {
                    //FindObjectOfType<AudioManager>().Play("PercentFilledDecreaseSFX");
                }

                while (currentBar.transform.localScale.x > i_Percent)
                {
                    currentBar.transform.localScale = new Vector2(Mathf.Max(Mathf.Max(currentBar.transform.localScale.x - 0.03f, i_Percent), 0), 1);
                    yield return new WaitForSeconds(0);
                }
            }
        }
    }

    private IEnumerator fillOverTime(Transform i_Transform)
    {
        while (i_Transform.localScale.x < 1)
        {
            i_Transform.localScale = new Vector2(Mathf.Min(i_Transform.localScale.x + 0.03f, 1f), i_Transform.localScale.y);
            yield return new WaitForSeconds(0f);
        }
    }

    public static void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void HandleDeath()
    {
        Destroy(m_Player.gameObject);
        m_GridManager.ResetPlayerPath(Vector3.zero, true, true);
        StartCoroutine(HandleGameOver());
    }

    public IEnumerator HandleGameOver()
    {
        yield return new WaitForSeconds(se_GameOverDelay);

        if (se_LoseLabel != null)
        {
            se_ScoreText.text = "Score\n" + m_ScoreManager.CurrentScore;
            se_HighScoreText.text = "High Score\n" + GameData.LoadHighScore();
            se_LoseLabel.SetActive(true);
        }
        
        HandleSound(eSoundID.GameOverSound);
    }

    // Activated by a button


    public void TogglePauseWithDelay()
    {
        StartCoroutine(togglePauseWithDelay());
    }

    private IEnumerator togglePauseWithDelay()
    {
        yield return new WaitForSeconds(.3f);
        TogglePause();
        togglePlayer();
    }

    public void TogglePause()
    {
        Utils.s_IsGamePaused = !Utils.s_IsGamePaused;

        if (Utils.s_IsGamePaused)
        {
            HandleSound(eSoundID.PauseModeInSound);
            PauseEnemies();
        }
        else
        {
            HandleSound(eSoundID.PauseModeOutSound);
            ResumeEnemies();
        }

        if (se_JoystickCanvas != null)
        {
            //se_JoystickCanvas.SetActive(Utils.s_IsGamePaused);
        }
        
        //m_Pause = !m_Pause;

        togglePlayer();
    }

    private void togglePlayer()
    {
        m_Player.GetComponent<PlayerInputHandler>().enabled = !m_Player.GetComponent<PlayerInputHandler>().enabled;
    }

    
    private void PauseEnemies()
    {
        if (m_Enemies == null)
        {
            return;
        }

        foreach (Enemy enemy in m_Enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            enemy.GetComponent<Enemy>().Pause();
        }

        foreach (var projectile in FindObjectsOfType<EnemyProjectile>())
        {
            projectile.StopProjectile();
        }
    }

    private void ResumeEnemies()
    {
        if (m_Enemies == null)
        {
            return;
        }
        foreach (Enemy enemy in m_Enemies)
        {
            if (enemy == null)
            {
                continue;
            }
            enemy.GetComponent<Enemy>().Resume();
        }

        foreach (var projectile in FindObjectsOfType<EnemyProjectile>())
        {
            projectile.SendProjectile();
        }
    }

    public void HandleSound(eSoundID i_SoundID)
    {
        m_SoundManager.Play(i_SoundID);
    }
}