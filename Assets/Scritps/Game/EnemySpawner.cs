using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum eEnemyType
{
    Grunt,
    Destroyer,
    Spike,
    Splitter,
    Shooter,
    Bomber,
    EnemyOnColor,
    Chopper
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Enemy> se_enemyTypes = null;

    public void SpawnEnemies(int i_numberToSpawn, eEnemyType i_type, float i_speed)
    {
        List<Waypoint> placesToSpawn;

        if (i_type == eEnemyType.EnemyOnColor)
        {
            placesToSpawn = FindObjectsOfType<Waypoint>()
                .Where(point => (point.Colored || point.Flooded) && !point.ColoredByPlayer &&
                                Vector2.Distance(point.transform.position, new Vector2(Utils.s_GridWidth / 2f, Utils.s_GridWidth / 2f)) > 5f).ToList();
        }
        else
        {
            placesToSpawn = FindObjectsOfType<Waypoint>()
                .Where(point => !point.Flooded && !point.ColoredByPlayer).ToList();
        }

        for (int i = 0; i < i_numberToSpawn; i++)
        {
            Waypoint spawnPoint = placesToSpawn[Random.Range(0, placesToSpawn.Count)];

            if (i_type == eEnemyType.Destroyer || i_type == eEnemyType.Chopper)
            {
                while (Utils.IsFloatInRange(spawnPoint.transform.position.x, Utils.s_GridWidth / 2f - 7f, Utils.s_GridWidth / 2f + 7f) &&
                       Utils.IsFloatInRange(spawnPoint.transform.position.y, Utils.s_GridHeight / 2f - 7f, Utils.s_GridHeight / 2f + 7f))
                {
                    spawnPoint = placesToSpawn[Random.Range(0, placesToSpawn.Count)];
                }
            }

            placesToSpawn.Remove(spawnPoint);
            Enemy clone = Instantiate(se_enemyTypes[(int)i_type], spawnPoint.transform.position, Quaternion.identity);
            clone.CurrentMoveSpeed = i_speed;
        }
    }

    public void SpawnEnemyInLocation(eEnemyType i_type, float i_speed, Vector3 i_Pos)
    {
        Enemy clone = Instantiate(se_enemyTypes[(int)i_type], i_Pos, Quaternion.identity);
        clone.CurrentMoveSpeed = i_speed;
    }
}
