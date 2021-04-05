using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlooderBFS : MonoBehaviour
{
    private Dictionary<Vector3, Waypoint> m_WaypointGrid;
    private List<Waypoint> m_AllWaypointsList;
    private List<Waypoint> m_AllReachableWaypoints;
    private Queue<Waypoint> m_BFSQueue;
    private Waypoint m_SearchCenter;
    private bool m_IsRunning;
    private int _numberOfTiles;
    private Waypoint m_StartWaypoint, _endWaypoint;
    private Color m_BackgroundColor;


    public List<Waypoint> getAllReachableWaypoints
    {
        get { return m_AllReachableWaypoints; }
    }

    public void RunBFS(Waypoint i_StartWaypoint, List<Waypoint> i_AllWaypointsList)
    {
        m_StartWaypoint = i_StartWaypoint;
        m_AllReachableWaypoints = new List<Waypoint>();
        m_WaypointGrid = new Dictionary<Vector3, Waypoint>();
        m_AllWaypointsList = i_AllWaypointsList;
        m_BFSQueue = new Queue<Waypoint>();
        m_IsRunning = true;
        m_BackgroundColor = Color.black; // TODO

        CreatePath();
    }

    private void CreatePath()
    {
        LoadTiles();
        BreadthFirstSearch();
    }

    private void LoadTiles()
    {
        foreach (var waypoint in m_AllWaypointsList)
        {
            Vector2 gridPos = waypoint.transform.position;
            waypoint.IsExploredByBFS = false;

            if (!m_WaypointGrid.ContainsKey(gridPos))
            {
                m_WaypointGrid.Add(gridPos, waypoint);
            }
        }
    }

    private void BreadthFirstSearch()
    {
        m_BFSQueue.Enqueue(m_StartWaypoint);
        m_StartWaypoint.IsExploredByBFS = true;
        m_AllReachableWaypoints.Add(m_StartWaypoint);

        while (m_BFSQueue.Count > 0 && m_IsRunning)
        {
            m_SearchCenter = m_BFSQueue.Dequeue();
            m_SearchCenter.IsExploredByBFS = true;
            HaltIfEndFound();
            ExploreNeighbors();
        }
    }

    private void ExploreNeighbors()
    {
        if (!m_IsRunning)
            return;

        foreach (var direction in Utils.s_Directions)
        {
            var exploreCoordinates = m_SearchCenter.transform.position + direction;
            QueueNewNeighbor(exploreCoordinates);
        }
    }

    private void QueueNewNeighbor(Vector3 exploreCoordinates)
    {
        if (m_WaypointGrid.ContainsKey(exploreCoordinates))
        {
            Waypoint neighbor = m_WaypointGrid[exploreCoordinates];
            if (!neighbor.IsExploredByBFS &&
                !neighbor.IsExploredByEnemy &&
                m_SearchCenter.Color == neighbor.Color &&
                neighbor.Color == m_BackgroundColor)
            {
                m_BFSQueue.Enqueue(neighbor);
                neighbor.ExploredFrom = m_SearchCenter;
                neighbor.IsExploredByBFS = true;
                m_AllReachableWaypoints.Add(neighbor);
                neighbor.IsExploredByEnemy = true;
            }
        }
    }

    private void HaltIfEndFound()
    {
        if (m_WaypointGrid.Count > 0) return;
        m_IsRunning = false;
    }
}
