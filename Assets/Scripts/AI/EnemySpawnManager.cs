using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public int EnemiesPerWave;
    public GameObject Enemy;
}

[System.Serializable]
public class FakeTransform
{
    public Vector3 position;
    public Quaternion rotation;
}

public class EnemySpawnManager : MonoBehaviour
{
    public GameObject mapGenerator;
    public Wave[] Waves; // class to hold information per wave
    public float TimeBetweenEnemies = 2f;
    public GameObject[] currentEnemies;

    private int _totalEnemiesInCurrentWave;
    private int _enemiesInWaveLeft;
    private int _spawnedEnemies;
    public float distanceAway = 400f;
    public float distanceBetween = 50f;

    private int _currentWave;
    private int _totalWaves;

    private TerrainGenerator terrainGenerator;
    private MeshSettings meshSettings;
    private HeightMapSettings heightMapSettings;

    private FakeTransform[] SpawnPoints;

	void Start ()
	{
        terrainGenerator = mapGenerator.GetComponent<TerrainGenerator>();
        meshSettings = terrainGenerator.meshSettings;
        heightMapSettings = terrainGenerator.heightMapSettings;

	    _currentWave = -1; // avoid off by 1
	    _totalWaves = Waves.Length - 1; // adjust, because we're using 0 index

	    StartNextWave();
	}

    void StartNextWave()
    {

        _currentWave++;

        // win
        if (_currentWave > _totalWaves)
        {
            return;
        }

        _totalEnemiesInCurrentWave = Waves[_currentWave].EnemiesPerWave;
        _enemiesInWaveLeft = 0;
        _spawnedEnemies = 0;

        StartCoroutine(SpawnEnemies());
    }

    void SetSpawnPoints()
    {
        int enemiesPerCurrentWave = Waves[_currentWave].EnemiesPerWave;
        SpawnPoints = new FakeTransform[enemiesPerCurrentWave];


        Transform obj = terrainGenerator.viewer;
        Vector3 center = obj.position + distanceAway * Vector3.Scale(obj.forward, new Vector3(1, 0, 1));


        center.y = heightMapSettings.maxHeight + 200f; 
       

        Vector3 dir = (center - obj.position).normalized;
        Vector3 perpendicular = new Vector3(dir.z, 0, -dir.x);

        int offset = -Mathf.FloorToInt((enemiesPerCurrentWave - 1)/ 2);
        
        for (int i = 0; i < enemiesPerCurrentWave; i++)
        {
            SpawnPoints[i] = new FakeTransform();
            SpawnPoints[i].position = center + perpendicular * distanceBetween * (i + offset);
            SpawnPoints[i].rotation = Quaternion.LookRotation(-dir);
            Debug.Log("AIPos[" + i.ToString() + ": " + SpawnPoints[i].position);
        }
    }

    // Coroutine to spawn all of our enemies
    IEnumerator SpawnEnemies()
    {
        GameObject currentEnemyPrefab = Waves[_currentWave].Enemy;

        currentEnemies = new GameObject[_totalEnemiesInCurrentWave];

        
        SetSpawnPoints();



        while (_spawnedEnemies < _totalEnemiesInCurrentWave)
        {
            _spawnedEnemies++;
            _enemiesInWaveLeft++;

            currentEnemies[_spawnedEnemies - 1] = Instantiate(
                                        currentEnemyPrefab, 
                                        SpawnPoints[_spawnedEnemies - 1].position, 
                                        SpawnPoints[_spawnedEnemies - 1].rotation);

            currentEnemies[_spawnedEnemies - 1].gameObject.name = "AircraftAI_" + 
                                                    (_spawnedEnemies - 1).ToString();
            yield return new WaitForSeconds(TimeBetweenEnemies);
        }
        yield return null;
    }
    
    // called by an enemy when they're defeated
    public void EnemyDefeated()
    {
        _enemiesInWaveLeft--;
        
        // We start the next wave once we have spawned and defeated them all
        if (_enemiesInWaveLeft == 0 && _spawnedEnemies == _totalEnemiesInCurrentWave)
        {
            StartNextWave();
        }
    }
}
