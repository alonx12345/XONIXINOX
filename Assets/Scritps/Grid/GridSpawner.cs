using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    [SerializeField] private Waypoint se_TilePrefab = null;
    [SerializeField] private GameObject se_OutlinePrefab = null;

    private GameObject m_TilesParent;
    private GameObject m_OutlineParent;
    private GameObject[,] m_Outlines;

    public Waypoint[,] CreateTileGrid(bool[][] i_FilledTilePositions, Color i_FillColor, Color i_BackgroundColor)
    {
        m_TilesParent = new GameObject {name = "Tiles"};

        Waypoint[,] tileGrid = new Waypoint[Utils.s_GridWidth,Utils.s_GridHeight];

        for (int i = 0; i < Utils.s_GridWidth; i++)
        {
            for (int j = 0; j < Utils.s_GridHeight; j++)
            {
                Waypoint newWaypoint = Instantiate(se_TilePrefab, new Vector3(i, j), Quaternion.identity);

                tileGrid[i, j] = newWaypoint;

                newWaypoint.transform.parent = m_TilesParent.transform;

                if (i_FilledTilePositions[i][j] || Utils.OnBorder(i, j) || Utils.OnMiddle(i, j))
                {
                    newWaypoint.ColorPoint(i_FillColor, true);
                }
                else
                {
                    newWaypoint.Color = i_BackgroundColor;
                }
            }
        }

        return tileGrid;
    }

    public void DestroyGrid(Waypoint[,] i_Grid)
    {
        Destroy(m_TilesParent);

        foreach (Waypoint waypoint in i_Grid)
        {
            if (waypoint != null)
            {
                Destroy(waypoint.gameObject);
            }
        }
    }

    public void CreateOutline()
    {
        if (se_OutlinePrefab == null || m_Outlines != null)
        {
            return;
        }

        m_OutlineParent = new GameObject { name = "Outlines" };
        m_Outlines = new GameObject[Utils.s_GridWidth, Utils.s_GridHeight];

        for (int i = 0; i < Utils.s_GridWidth; i++)
        {
            for (int j = 0; j < Utils.s_GridHeight; j++)
            {
                GameObject newOutline = Instantiate(se_OutlinePrefab, new Vector3(i, j), Quaternion.identity);
                m_Outlines[i, j] = newOutline;
                newOutline.transform.parent = m_OutlineParent.transform;
            }
        }
    }

    public void DestroyOutline()
    {
        if (m_Outlines == null)
        {
            return;
        }

        foreach (GameObject outline in m_Outlines)
        {
            Destroy(outline);
        }

        m_Outlines = null;
        Destroy(m_OutlineParent);
    }
}
