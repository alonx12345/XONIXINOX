using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    [SerializeField] private float se_TimeBetweenSpawns = 2f;
    [SerializeField] private Powerup[] se_PowerupPrefabs = null;
    [SerializeField] private float se_MaxNumberOfPowerups = 3;
    [SerializeField] private float se_PowerupsTomeToDestroy = 10f;

    private PowerupManager m_PowerupManager;

    private float m__TimeLeftToSpawn;

    public int m_NumberOfPowerupSpawned { get; set; } = 0;

    private void Awake()
    {
        m_PowerupManager = GetComponent<PowerupManager>();
        m__TimeLeftToSpawn = se_TimeBetweenSpawns;
    }

    private void Update()
    {
        if (!Utils.s_IsGamePaused &&
            !Utils.s_IsLevelFinished &&
            Utils.s_CurrentLevel >= 9)
        {
            handleSpawns();
        }
    }

    private void handleSpawns()
    {
        m__TimeLeftToSpawn -= Time.deltaTime;

        if (se_PowerupPrefabs != null &&
            se_PowerupPrefabs.Length > 0  &&
            m__TimeLeftToSpawn < 0 &&
            m_NumberOfPowerupSpawned < se_MaxNumberOfPowerups)
        {
            spwan();
            m__TimeLeftToSpawn = se_TimeBetweenSpawns;
            m_NumberOfPowerupSpawned++;
        }
        else if (m_NumberOfPowerupSpawned >= se_MaxNumberOfPowerups)
        {
            m__TimeLeftToSpawn = se_TimeBetweenSpawns;
        }
    }

    private void spwan()
    {
        int maxIndex = 1;

        if (Utils.IsFloatInRange(Utils.s_CurrentLevel, 19, 31))
        {
            maxIndex = 2;
        }
        else if (Utils.IsFloatInRange(Utils.s_CurrentLevel, 32, 41))
        {
            maxIndex = 3;
        }
        else if (Utils.IsFloatInRange(Utils.s_CurrentLevel, 42, 54))
        {
            maxIndex = 4;
        }
        else if (Utils.s_CurrentLevel >= 55)
        {
            maxIndex = 5;
        }


        int randomPowerupIndex = Random.Range(0, maxIndex);

        float rowToSpawn = Random.Range(2f, Utils.s_GridWidth - 2f);
        float colToSpawn = Random.Range(2f, Utils.s_GridHeight - 2f);

        Vector3 spawnPos = new Vector3(rowToSpawn, colToSpawn, 0);

        Powerup newPowerup = Instantiate(se_PowerupPrefabs[randomPowerupIndex], spawnPos, Quaternion.identity);

        newPowerup.PowerupActivate += m_PowerupManager.ActivatePowerup;
        newPowerup.DestroyAction += handlePowerupDestroy;
        SelfDestroyScript powerupDestroyScript = newPowerup.GetComponent<SelfDestroyScript>();

        if (powerupDestroyScript != null)
        {
            powerupDestroyScript.TimeToDestroy = se_PowerupsTomeToDestroy;
        }
    }

    private void handlePowerupDestroy()
    {
        m_NumberOfPowerupSpawned--;
    }
}
