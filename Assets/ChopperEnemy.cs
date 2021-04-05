using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopperEnemy : MonoBehaviour
{
    //private void OnTriggerEnter2D(Collider2D other)
    //{
        
    //}

    void OnTriggerStay2D(Collider2D other)
    {
        Waypoint waypointCollided = other.gameObject.GetComponent<Waypoint>();
        
        if (waypointCollided != null && (waypointCollided.Colored || waypointCollided.Flooded) &&
            !Utils.OnBorder(waypointCollided.transform.position.x,waypointCollided.transform.position.y) && !waypointCollided.ColoredByPlayer)
        {
            waypointCollided.Color = Color.black;
            waypointCollided.ResetPoint();
            FindObjectOfType<GridManager>().SetCollidersAroundPoint((int)waypointCollided.transform.position.x, (int)waypointCollided.transform.position.y);
            FindObjectOfType<GridManager>().ProgressActionInvoke();
        }
    }
}
