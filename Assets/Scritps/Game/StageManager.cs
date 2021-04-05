using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : 
    MonoBehaviour
{
    private EnemySpawner m_EnemySpawner;

    private void Awake()
    {
        m_EnemySpawner = GetComponent<EnemySpawner>();
    }

    public List<Enemy> SpawnEnemiesByLevel(int i_level)
    {
        switch (i_level)
        {
            // Beginner
            case 1:
                m_EnemySpawner.SpawnEnemyInLocation(eEnemyType.Grunt, 5, new Vector3(Utils.s_GridWidth - 2, 2));
                break;
            case 2:
                m_EnemySpawner.SpawnEnemyInLocation(eEnemyType.Grunt, 5, new Vector3(Utils.s_GridWidth - 2, 2));
                m_EnemySpawner.SpawnEnemyInLocation(eEnemyType.Grunt, 5, new Vector3(3, 5));
                break;
            case 3:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 5);
                break;
            case 4:
                m_EnemySpawner.SpawnEnemyInLocation(eEnemyType.Grunt, 5, new Vector3(Utils.s_GridWidth - 2, 2));
                m_EnemySpawner.SpawnEnemyInLocation(eEnemyType.Grunt, 5, new Vector3(3, 25));
                break;
            case 5:
                m_EnemySpawner.SpawnEnemyInLocation(eEnemyType.Grunt, 5, new Vector3(Utils.s_GridWidth - 2, 2));
                m_EnemySpawner.SpawnEnemyInLocation(eEnemyType.Grunt, 5, new Vector3(2, 2));
                break;
            // Spike
            case 6:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 5);
                break;
            case 7:
                m_EnemySpawner.SpawnEnemies(3, eEnemyType.Grunt, 5);
                break;
            case 8:
                m_EnemySpawner.SpawnEnemies(3, eEnemyType.Grunt, 5);
                break;
            case 9:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 5);
                break;
            case 10:
                m_EnemySpawner.SpawnEnemies(4, eEnemyType.Grunt, 5);
                break;
            case 11:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Destroyer, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Grunt, 6);
                break;
            case 12:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Destroyer, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 5);
                break;
            case 13:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Destroyer, 5);
                break;
            case 14:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Destroyer, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Grunt, 5);
                break;
            case 15:
                m_EnemySpawner.SpawnEnemies(3, eEnemyType.Grunt, 6);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Destroyer, 5);
                break;
            case 16:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.EnemyOnColor, 5);
                break;
            case 17:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.EnemyOnColor, 5);
                m_EnemySpawner.SpawnEnemies(3, eEnemyType.Grunt, 5);
                break;
            case 18:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.EnemyOnColor, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Destroyer, 5);
                break;
            case 19:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Destroyer, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Grunt, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.EnemyOnColor, 5);
                break;
            case 20:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.EnemyOnColor, 5);
                m_EnemySpawner.SpawnEnemies(4, eEnemyType.Grunt, 6);
                break;
            case 21:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.EnemyOnColor, 5);
                m_EnemySpawner.SpawnEnemies(3, eEnemyType.Destroyer, 5);
                break;
            case 22:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.EnemyOnColor, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 6);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Destroyer, 5);
                break;
            case 23:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Chopper, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 5);
                break;
            case 24:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Chopper, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Grunt, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Destroyer, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.EnemyOnColor, 5);
                break;
            case 25:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.EnemyOnColor, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Chopper, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Grunt, 6);
                break;
            case 26:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.EnemyOnColor, 5);
                m_EnemySpawner.SpawnEnemies(3, eEnemyType.Grunt, 6);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Destroyer, 5);
                break;
            case 27:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Spike, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 6);
                break;
            case 28:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Spike, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Grunt, 6);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Destroyer, 5);
                break;
            case 29:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Spike, 6);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Chopper, 5);
                break;
            case 30:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Spike, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 6);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Chopper, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.EnemyOnColor, 5);
                break;
            case 31:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Spike, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Destroyer, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.EnemyOnColor, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Grunt, 6);
                break;
            case 32:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Spike, 5);
                break;
            case 33:
                m_EnemySpawner.SpawnEnemies(3, eEnemyType.EnemyOnColor, 5);
                m_EnemySpawner.SpawnEnemies(3, eEnemyType.Grunt, 6);
                break;
            case 34:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Destroyer, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Spike, 6);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.EnemyOnColor, 5);
                break;
            case 35:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Destroyer, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Chopper, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Grunt, 6);
                break;
            case 36:
                m_EnemySpawner.SpawnEnemies(3, eEnemyType.Spike, 6);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.EnemyOnColor, 5);
                break;
            case 37:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Bomber, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 6);
                break;
            case 38:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Bomber, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Spike, 6);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Destroyer, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.EnemyOnColor, 5);
                break;
            case 39:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Bomber, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 6);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Chopper, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.EnemyOnColor, 5);
                break;
            case 40:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Bomber, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 6);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.EnemyOnColor, 5);
                break;
            case 41:
                m_EnemySpawner.SpawnEnemies(3, eEnemyType.Spike, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Destroyer, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.EnemyOnColor, 5);
                break;
            case 42:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Spike, 6);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 6);
                break;
            case 43:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Chopper, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Grunt, 6);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Destroyer, 6);
                break;
            case 44:
                m_EnemySpawner.SpawnEnemies(3, eEnemyType.Grunt, 6);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Bomber, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.EnemyOnColor, 5);
                break;
            case 45:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Spike, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Destroyer, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Bomber, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Chopper, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Grunt, 5);
                break;
            case 46:
                m_EnemySpawner.SpawnEnemies(4, eEnemyType.Spike, 7);
                break;
            case 47:
                m_EnemySpawner.SpawnEnemies(3, eEnemyType.Grunt, 7);
                m_EnemySpawner.SpawnEnemies(4, eEnemyType.EnemyOnColor, 6);
                break;
            case 48:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Splitter, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 6);
                break;
            case 49:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Splitter, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Grunt, 6);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Chopper, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.EnemyOnColor, 5);
                break;
            case 50:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Splitter, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Spike, 6);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.EnemyOnColor, 5);
                break;
            case 51:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Splitter, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Bomber, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Grunt, 5);
                break;
            case 52:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Spike, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.EnemyOnColor, 5);
                m_EnemySpawner.SpawnEnemies(3, eEnemyType.Chopper, 5);
                break;
            case 53:
                m_EnemySpawner.SpawnEnemies(4, eEnemyType.Grunt, 6);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Splitter, 5);
                break;
            case 54:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Bomber, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Splitter, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.EnemyOnColor, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Chopper, 5);
                break;
            case 55:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Spike, 6);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 6);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.EnemyOnColor, 5);
                break;
            case 56:
                m_EnemySpawner.SpawnEnemies(3, eEnemyType.EnemyOnColor, 5);
                m_EnemySpawner.SpawnEnemies(3, eEnemyType.Spike, 6);
                break;
            case 57:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.EnemyOnColor, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Bomber, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 6);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Chopper, 5);
                break;
            case 58:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Splitter, 5);
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Spike, 7);
                break;
            case 59:
                m_EnemySpawner.SpawnEnemies(3, eEnemyType.Chopper, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.EnemyOnColor, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Destroyer, 5);
                break;
            case 60:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Shooter, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Grunt, 6);
                break;
            case 61:
                m_EnemySpawner.SpawnEnemies(1, eEnemyType.Shooter, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.EnemyOnColor, 5);
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Destroyer, 5);
                break;
            case 62:
                m_EnemySpawner.SpawnEnemies(2, eEnemyType.Shooter, 5);
                break;
        }

        return FindObjectsOfType<Enemy>().ToList();
    }
}
