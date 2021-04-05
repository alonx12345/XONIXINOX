using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DrawerManager : MonoBehaviour
{
    [SerializeField] private int se_GridWidth = 31;
    [SerializeField] private int se_GridHeight = 31;
    [SerializeField] private TextMeshProUGUI se_MouseUI = null;
    [SerializeField] private Transform pointer = null;
    
    private GridSpawner m_GridSpawner;
    private bool[][] m_CurrentGrid;
    private Waypoint[,] m_TileGrid;
    private Color m_FillColor;

    private bool m_ShowOutline = false;
    private int m_CurrentGridIndex = 0;

    private void Awake()
    {
        m_GridSpawner = GetComponent<GridSpawner>();
        Utils.s_GridWidth = se_GridWidth;
        Utils.s_GridHeight = se_GridHeight;
    }

    private void Start()
    {
        m_FillColor = Color.blue;
        m_FillColor.a = .7f;

        m_CurrentGrid = Utils.Init2DArrayRows<bool>(se_GridWidth, se_GridHeight);
        if (GameData.GameGrids == null || GameData.GameGrids.Count == 0 || GameData.GameGrids[0].Length != se_GridWidth)
        {
            GameData.GameGrids = new List<bool[][]>();
        }
        else
        {
            m_CurrentGrid = GameData.GameGrids[0];

            for (int i = 0; i < GameData.GameGrids[0].Length; i++)
            {
                for (int j = 0; j < GameData.GameGrids[0][i].Length; j++)
                {
                    m_CurrentGrid[i][j] = GameData.GameGrids[0][i][j];
                }
            }
        }

        m_TileGrid = m_GridSpawner.CreateTileGrid(m_CurrentGrid, m_FillColor, Color.black);
    }

    private void showGrid(int i_Index)
    {
        if (i_Index < GameData.GameGrids.Count)
        {
            m_GridSpawner.DestroyGrid(m_TileGrid);

            m_CurrentGrid = GameData.GameGrids[i_Index];

            for (int i = 0; i < GameData.GameGrids[i_Index].Length; i++)
            {
                for (int j = 0; j < GameData.GameGrids[i_Index][i].Length; j++)
                {
                    m_CurrentGrid[i][j] = GameData.GameGrids[i_Index][i][j];
                }
            }

            m_TileGrid = m_GridSpawner.CreateTileGrid(m_CurrentGrid, m_FillColor, Color.black);
        }
        else
        {
            m_CurrentGridIndex = 0;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            m_GridSpawner.DestroyGrid(m_TileGrid);
            m_CurrentGrid = Utils.Init2DArrayRows<bool>(se_GridWidth, se_GridHeight);
            m_TileGrid = m_GridSpawner.CreateTileGrid(m_CurrentGrid, m_FillColor, Color.black);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            m_ShowOutline = !m_ShowOutline;

            if (m_ShowOutline)
            {
                m_GridSpawner.CreateOutline();
            }
            else
            {
                m_GridSpawner.DestroyOutline();
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            GameData.GameGrids.Add(m_CurrentGrid);
            print("Grid Added , Press H to save");
            print("Number of grids: " + GameData.GameGrids.Count);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (GameData.GameGrids.Count > 0)
            {
                GameData.GameGrids.RemoveAt(GameData.GameGrids.Count - 1);
                print("Last grid removed, Press H to save");
                print("Number of grids: " + GameData.GameGrids.Count);
            }

        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            GameData.SaveGrids();
            print("Saved Grids");
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            print(m_CurrentGridIndex);
            showGrid(m_CurrentGridIndex++);
        }

        handleMouseOps();
    }

    private void handleMouseOps()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.RoundToInt(mousePos.x);
        int y = Mathf.RoundToInt(mousePos.y);

        if (Input.GetMouseButton(0))
        {
            if (!Utils.IsFloatInRange(y, 0, se_GridHeight - 1) ||
                !Utils.IsFloatInRange(x, 0, Utils.s_GridWidth))
            {
                return;
            }
            else
            {
                Waypoint point = m_TileGrid[x, y];
                point.Color = m_FillColor;
                point.Colored = true;
                m_CurrentGrid[x][y] = true;
            }
        }

        if (Input.GetMouseButton(1))
        {

            if (!Utils.IsFloatInRange(y, 0, se_GridHeight - 1) ||
                !Utils.IsFloatInRange(x, 0, Utils.s_GridWidth))
            {
                return;
            }
            else
            {
                Waypoint point = m_TileGrid[x, y];
                point.Color = Color.black;
                point.Colored = false;
                m_CurrentGrid[x][y] = false;
            }
        }

        if (pointer != null)
        {
            pointer.position = new Vector3(x ,y);
        }

        if (se_MouseUI != null)
        {
            se_MouseUI.text = new Vector2Int(x, y).ToString();
        }
    }
}
