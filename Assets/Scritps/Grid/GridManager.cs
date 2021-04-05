using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private float se_playerPathDestroyDelay = 0.01f;
    [SerializeField] private eVisualEffectType se_PathDestroyVFXType;
    [SerializeField] private eVisualEffectType se_TileDestroyVFXType;
    [SerializeField] private eVisualEffectType se_FillVFX = eVisualEffectType.FillEffect;
 
    public event Action<int, bool> ProgressAction;
    public event Action DeathAction;
    public event Action<eSoundID> PlaySoundAction;

    private Waypoint[,] m_TileGrid;
    private List<Waypoint> m_AllWaypointsList;
    private PlayerController m_Player;
    private List<Enemy> m_Enemies;

    public  Color FillColor { get; set; }
    public Color BackgroundColor { get; set; }
    public Color PlayerColor { get; set; }

    private FlooderBFS m_Flooder;

    public Waypoint[,] TileGrid
    {
        get { return m_TileGrid; }
        set { m_TileGrid = value; }
    }

    public List<Waypoint> AllWaypoints
    {
        set { m_AllWaypointsList = value; }
    }

    public PlayerController Player
    {
        set { m_Player = value; }
    }

    public List<Enemy> Enemies
    {
        set
        {
            m_Enemies = value;
            foreach (Enemy enemy in m_Enemies)
            {
                if (enemy == null)
                {
                    continue;
                }

                enemy.TouchedPlayerPathAction += ResetPlayerPath;
            }
        }
    }

    public void Awake()
    {
        m_Flooder = GetComponent<FlooderBFS>();
    }

    private void Update()
    {
        if (m_Player != null)
        {
            handlePlayerMovementOverGrid();
        }
    }

    //public void SubscribeForFlood(Action i_Listener)
    //{
    //    m_ProgressAction += i_Listener;
    //}

    //public void SubscribeForDeathEvent(Action i_Listener)
    //{
    //    m_DeathAction += i_Listener;
    //}

    private void handlePlayerMovementOverGrid()
    {
        int playerPosX = Mathf.RoundToInt(m_Player.transform.position.x);
        int playerPosY = Mathf.RoundToInt(m_Player.transform.position.y);

        if (!Utils.IsFloatInRange(playerPosX, 0, Utils.s_GridWidth - 1) ||
            !Utils.IsFloatInRange(playerPosY, 0, Utils.s_GridHeight - 1))
        {
            return;
        }

        Vector3 moveDir = m_Player.MoveDirection;
        moveDir = new Vector3(Mathf.RoundToInt(moveDir.x), Mathf.RoundToInt(moveDir.y));

        Waypoint lastPlayerWaypoint = null;

        if (Utils.IsFloatInRange(playerPosX - moveDir.x, 0, Utils.s_GridWidth - 1) &&
            Utils.IsFloatInRange(playerPosY - moveDir.y, 0, Utils.s_GridHeight - 1))
        {
            lastPlayerWaypoint = m_TileGrid[playerPosX - Mathf.RoundToInt(moveDir.x), playerPosY - Mathf.RoundToInt(moveDir.y)];

        }

        Waypoint currentPlayerWaypoint = m_TileGrid[playerPosX, playerPosY];

        if (!currentPlayerWaypoint.Colored)
        {
            currentPlayerWaypoint.ColoredByPlayer = true;

            //TEST
            currentPlayerWaypoint.BorderColliderActive = true;

            if (!m_Player.CanBeHit)
            {
                currentPlayerWaypoint.Color = Color.cyan;
            }
            else
            {
                currentPlayerWaypoint.Color = new Color(m_Player.Color.r, m_Player.Color.g, m_Player.Color.b, .7f);
            }
           
            //StartCoroutine(colorPointByPlayer(currentPlayerWaypoint));
            colorPointByPlayer(currentPlayerWaypoint);
        }
        else if (currentPlayerWaypoint.Flooded)
        {
            if (lastPlayerWaypoint != null && lastPlayerWaypoint.ColoredByPlayer)
            {
                currentPlayerWaypoint.ColoredByPlayer = true;
                floodAreaByEnemiesLocation();
            }

            m_Player.GetComponent<PlayerInputHandler>().CurrentDirection = eDirection.Default;
        }
    }

    private void colorPointByPlayer(Waypoint i_Waypoint)
    {
        i_Waypoint.IsExploredByPlayer = true;
    }

    private void floodAreaByEnemiesLocation()
    {
        if (Utils.s_IsLevelFinished)
        {
            return;
        }

        List<Waypoint> waypointsToColor = m_AllWaypointsList.Where(point => !point.Colored).ToList();

        List<Waypoint> waypointsToIterateOver = null;

        m_Enemies = FindObjectsOfType<Enemy>().ToList();

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

            waypointsToIterateOver = m_AllWaypointsList
                .Where(point => !point.Colored)
                .OrderByDescending(point => Vector3.Distance(enemy.transform.position, point.transform.position))
                .ToList();

            int enemyPosX = Mathf.RoundToInt(enemy.transform.position.x);
            int enemyPosY = Mathf.RoundToInt(enemy.transform.position.y);

            floodFrom(m_TileGrid[enemyPosX, enemyPosY],waypointsToColor);
        }

        if (Utils.s_IsLevelFinished)
        {
            return;
        }

        floodTiles(waypointsToColor, 0.55f);

        colorPlayerPath();

        resetTiles(waypointsToIterateOver);

        int playerPathCount = m_AllWaypointsList.
            Where(point => point.ColoredByPlayer).ToList().Count;

        ProgressAction?.Invoke(waypointsToColor.Count + playerPathCount, true);
        PlaySoundAction?.Invoke(eSoundID.PercentFilledIncreaseSound);

        FindObjectOfType<VFXManager>().PlayVFXWithColor(se_FillVFX, m_Player.transform.position, m_Player.Color);
    }

    private void floodTiles(List<Waypoint> i_PointToFlood, float i_Alpha)
    {
        foreach (Waypoint waypoint in i_PointToFlood)
        {
            Color tempColor = new Color(FillColor.r, FillColor.g, FillColor.b, i_Alpha);
            waypoint.ColorPoint(tempColor, false);
        }
    }

    public void FloodWithDelayHandler(Vector3 i_Pos, float i_Delay)
    {
        StartCoroutine(floodWithDelay(i_Pos, i_Delay));
    }
    private IEnumerator floodWithDelay(Vector3 i_Pos, float i_Delay)
    {
        yield return new WaitForSeconds(i_Delay);

        floodAreaByEnemiesLocation();
    }

    private void floodFrom(Waypoint i_SourceWaypoint,List<Waypoint> i_WaypointsToColor)
    {
        List<Waypoint> reachableWaypoints = new List<Waypoint>();

        m_Flooder.RunBFS(i_SourceWaypoint, i_WaypointsToColor);
        reachableWaypoints = m_Flooder.getAllReachableWaypoints;

        i_WaypointsToColor.RemoveAll(point => reachableWaypoints.Contains(point));
    }


    private void colorPlayerPath()
    {

        //&& 
        List<Waypoint> playerPath = m_AllWaypointsList.
            Where(point => point.ColoredByPlayer).ToList();

        foreach (var waypoint in playerPath)
        {
            waypoint.ColoredByPlayer = false;
            waypoint.IsExploredByPlayer = false;
            waypoint.Flooded = true;
            waypoint.Colored = true;
            waypoint.BorderColliderActive = true;
        }

        StartCoroutine(colorPath(playerPath));
    }

    private IEnumerator colorPath(List<Waypoint> i_Path)
    {
        yield return new WaitForSeconds(0.01f);
        foreach (var waypoint in i_Path)
        {
            if (!Utils.OnBorder(waypoint.transform.position.x, waypoint.transform.position.y))
            {
                waypoint.Color = FillColor;
            }

            waypoint.ColoredByPlayer = false;
        }
    }

    private void resetTiles(List<Waypoint> i_Tiles)
    {
        if (i_Tiles == null)
        {
            return;
        }
        foreach (var waypoint in i_Tiles)
        {
            waypoint.IsExploredByEnemy = false;
        }
    }

    public void ResetPlayerPath(Vector3 i_enemyPos, bool i_AllImidiate, bool i_VFX)
    {
        Dictionary<Vector3, Waypoint> playerPathDict = LoadPlayerPath();

        if (i_AllImidiate)
        {
            List<Waypoint> playerPath = m_AllWaypointsList.Where(point => point.ColoredByPlayer).ToList();

            if (playerPath.Count > 0)
            {
                i_enemyPos = playerPath[0].transform.position;
                if (playerPathDict.ContainsKey(i_enemyPos))
                {
                    DestroyPathImmediate(playerPathDict, playerPathDict[i_enemyPos], i_VFX);
                }
            }
        }
        else
        {
            StartCoroutine(DestroyPath(playerPathDict, playerPathDict[i_enemyPos]));
        }
    }

    private Dictionary<Vector3, Waypoint> LoadPlayerPath()
    {
        List<Waypoint> currentPlayerPath = m_AllWaypointsList.Where(point => point.ColoredByPlayer).ToList();
        Dictionary<Vector3, Waypoint> playerPathDictionary = new Dictionary<Vector3, Waypoint>();

        foreach (Waypoint waypoint in currentPlayerPath)
        {
            Vector2 gridPos = waypoint.transform.position;
            if (!playerPathDictionary.ContainsKey(gridPos))
            {
                playerPathDictionary.Add(gridPos, waypoint);
            }
        }

        return playerPathDictionary;
    }

    private IEnumerator DestroyPath(Dictionary<Vector3, Waypoint> i_playerPathDict, Waypoint i_SourceWaypoint)
    {
        i_playerPathDict = LoadPlayerPath();
        yield return new WaitForSeconds(se_playerPathDestroyDelay);
        i_playerPathDict = LoadPlayerPath();
        if (m_Player != null)
        {
            Vector3 playerPos = m_Player.transform.position;
            if (Vector3.Distance(i_SourceWaypoint.transform.position, playerPos) < 1f)
            {
                DeathAction?.Invoke();
            }
        }

        FindObjectOfType<VFXManager>().PlayVFX(se_PathDestroyVFXType, i_SourceWaypoint.transform.position);

        i_SourceWaypoint.ResetPoint();
        i_SourceWaypoint.GetComponent<SpriteRenderer>().color = Color.black;

        for (int i = 0; i < Utils.s_Directions.Length && i_playerPathDict.Count > 0; i++)
        {
            if (i_playerPathDict.ContainsKey(i_SourceWaypoint.transform.position + Utils.s_Directions[i]))
            {
                Waypoint neighbor = i_playerPathDict[i_SourceWaypoint.transform.position + Utils.s_Directions[i]];
                i_playerPathDict.Remove(i_SourceWaypoint.transform.position + Utils.s_Directions[i]);
                if (i_playerPathDict.Count > 0)
                {
                    if (neighbor.ColoredByPlayer && !neighbor.Flooded && neighbor.Color != FillColor)
                    {
                        i_playerPathDict = LoadPlayerPath();
                        StartCoroutine(DestroyPath(i_playerPathDict, neighbor));
                    }
                }
            }
        }
    }

    private void DestroyPathImmediate(Dictionary<Vector3, Waypoint> i_PlayerPathDict, Waypoint i_SourceWaypoint, bool i_VFX)
    {
        i_PlayerPathDict = LoadPlayerPath();

        if ( i_VFX)
        {
            FindObjectOfType<VFXManager>().PlayVFX(se_PathDestroyVFXType, i_SourceWaypoint.transform.position);
        }

        i_SourceWaypoint.ResetPoint();
        i_SourceWaypoint.GetComponent<SpriteRenderer>().color = Color.black;

        for (int i = 0; i < Utils.s_Directions.Length && i_PlayerPathDict.Count > 0; i++)
        {
            if (i_PlayerPathDict.ContainsKey(i_SourceWaypoint.transform.position + Utils.s_Directions[i]))
            {
                Waypoint neighbor = i_PlayerPathDict[i_SourceWaypoint.transform.position + Utils.s_Directions[i]];
                i_PlayerPathDict.Remove(i_SourceWaypoint.transform.position + Utils.s_Directions[i]);
                if (i_PlayerPathDict.Count > 0)
                {
                    if (neighbor.ColoredByPlayer && !neighbor.Flooded && neighbor.Color != FillColor)
                    {
                        i_PlayerPathDict = LoadPlayerPath();
                        DestroyPathImmediate(i_PlayerPathDict, neighbor, i_VFX);
                    }
                }
            }
        }
    }

    public void FillAroundPoint(int i_Row, int i_Col, int radius)
    {
        List<Enemy> enemies = FindObjectsOfType<Enemy>().Where(enemy => enemy.gameObject.layer != 11).ToList();
        List<Enemy> enemiesToDestroy = new List<Enemy>();

        List<Waypoint> pointsToFill = new List<Waypoint>();

        for (int i = i_Row - radius; i < i_Row + radius + 1; i++)
        {
            for (int j = i_Col - radius; j < i_Col + radius + 1; j++)
            {
                if (Utils.OnGrid(i, j) && !m_TileGrid[i, j].Flooded && !m_TileGrid[i, j].Colored)
                {
                    foreach (Enemy enemy in enemies)
                    {
                        int enemyX = Mathf.RoundToInt(enemy.transform.position.x);
                        int enemyY = Mathf.RoundToInt(enemy.transform.position.y);
                        Vector3 enemyPos = new Vector3(enemyX, enemyY, m_TileGrid[i, j].transform.position.z);

                        if (enemyPos == m_TileGrid[i, j].transform.position)
                        {
                            enemiesToDestroy.Add(enemy);
                        }
                    }

                    pointsToFill.Add(m_TileGrid[i, j]);
                }
            }
        }

        if (pointsToFill.Count > 0)
        {
            floodTiles(pointsToFill, FillColor.a);
            
            PlaySoundAction?.Invoke(eSoundID.PercentFilledIncreaseSound);

            foreach (Enemy enemy in enemiesToDestroy)
            {
                FloodWithDelayHandler(transform.position, .5f);
                Destroy(enemy.gameObject);
            }

            ProgressAction?.Invoke(pointsToFill.Count, true);
        }
    }

    public void ExplodeAroundPoint(int i_Row, int i_Col)
    {
        int numOfTiles = 0;

        for (int i = i_Row - 1; i <= i_Row + 1; i++)
        {
            for (int j = i_Col - 1; j <= i_Col + 1; j++)
            {
                if ((i == i_Row - 1 && j == i_Col - 1) || (i == i_Row + 1 && j == i_Col + 1))
                {
                    continue;
                }

                if (!Utils.OnBorder(i, j) && Utils.OnGrid(i, j) && m_TileGrid[i, j].Color != Color.black)
                {
                    numOfTiles++;
                }
            }
        }

        DestroyPoint(i_Row, i_Col);
        DestroyPoint(i_Row + 1, i_Col);
        DestroyPoint(i_Row - 1, i_Col);
        DestroyPoint(i_Row, i_Col + 1);
        DestroyPoint(i_Row, i_Col - 1);

        ProgressAction?.Invoke(numOfTiles, false);
        print(numOfTiles);
        PlaySoundAction?.Invoke(eSoundID.PercentFilledDecreaseSound);
    }

    public void ExplodeAroundPoint(int i_Row, int i_Col, int radius)
    {
        int numOfTiles = 0;

        for (int i = i_Row - radius; i <= i_Row + radius; i++)
        {
            for (int j = i_Col - radius; j <= i_Col + radius; j++)
            {
                bool toAdd = DestroyPoint(i, j);
                if (toAdd)
                {
                    numOfTiles++;
                }

                killPlayerIfOnPoint(new Vector3(i, j));
            }
        }

        ProgressAction?.Invoke(numOfTiles, false);
        PlaySoundAction?.Invoke(eSoundID.PercentFilledDecreaseSound);
    }

    public void ProgressActionInvoke()
    {
        ProgressAction?.Invoke(1, false);
    }

    private bool DestroyPoint(int i_Row, int i_Col)
    {
        bool toReturn = false;

        if (!Utils.OnBorder(i_Row, i_Col) && Utils.OnGrid(i_Row, i_Col))
        {
            List<Enemy> enemies = FindObjectsOfType<Enemy>().Where(enemy => enemy.gameObject.layer == 11).ToList();
            List<Enemy> enemiesToDestroy = new List<Enemy>();

            Waypoint pointHit = m_TileGrid[i_Row, i_Col];
            Color explosionColor = pointHit.Color;

            if (pointHit.Color != Color.black)
            {
                toReturn = true;
            }

            pointHit.Color = Color.black;
            pointHit.ResetPoint();
            SetCollidersAroundPoint(i_Row, i_Col);
            FindObjectOfType<VFXManager>().PlayVFXWithColor(se_TileDestroyVFXType, pointHit.transform.position, explosionColor);

            foreach (Enemy enemy in enemies)
            {
                int enemyX = Mathf.RoundToInt(enemy.transform.position.x);
                int enemyY = Mathf.RoundToInt(enemy.transform.position.y);
                Vector3 enemyPos = new Vector3(enemyX, enemyY);

                if (enemyPos == pointHit.transform.position)
                {
                    enemiesToDestroy.Add(enemy);
                }
            }

            foreach (Enemy enemy in enemiesToDestroy)
            {
                Destroy(enemy.gameObject);
            }
        }

        return toReturn;
    }

    public void SetCollidersAroundPoint(int i_Row, int i_Col)
    {
        if (m_TileGrid[i_Row + 1, i_Col].Color != Color.black)
        {
            m_TileGrid[i_Row + 1, i_Col].BorderColliderActive = true;
        }

        if (m_TileGrid[i_Row - 1, i_Col].Color != Color.black)
        {
            m_TileGrid[i_Row - 1, i_Col].BorderColliderActive = true;
        }

        if (m_TileGrid[i_Row, i_Col + 1].Color != Color.black)
        {
            m_TileGrid[i_Row, i_Col + 1].BorderColliderActive = true;
        }

        if (m_TileGrid[i_Row, i_Col - 1].Color != Color.black)
        {
            m_TileGrid[i_Row, i_Col - 1].GetComponent<Waypoint>().BorderColliderActive = true;
        }

        killPlayerIfOnPoint(new Vector3(i_Row, i_Col));
    }

    private void killPlayerIfOnPoint(Vector3 i_Pos)
    {
        if (m_Player != null && m_Player.transform.position == i_Pos && m_Player.CanBeHit)
        {
            DeathAction?.Invoke();
        }
    }
}
