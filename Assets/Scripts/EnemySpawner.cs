using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : PoissonSpawner
{
    private float originalSpawnRate;
    [SerializeField]
    private float spawnRate;
    private float _spawnRate;
    protected override void Start()
    {
        base.Start();
        originalSpawnRate = spawnRate;
        if (DifficultyAdjuster.instance != null)
            DifficultyAdjuster.instance.UpdateSpawnRate += factor => spawnRate = originalSpawnRate * factor;
    }
    private void Update()
    {
        _spawnRate -= Time.deltaTime;
        if (_spawnRate <= 0)
        {
            SpawnEnemies();
            _spawnRate = spawnRate;
        }
    }
#nullable enable
    private void SpawnEnemies()
    {
        if (GetNextPosition(out Vector2 position))
        {
            Enemy? SpawnedZombie = EnemyManager.instance.GetNextEnemyInPool();
            if (SpawnedZombie)
                SpawnedZombie.transform.position = position;
        }
        else
        {
            Debug.LogWarning($"Spawner {name} has no availiable spawn positions");
        }
    }
}
