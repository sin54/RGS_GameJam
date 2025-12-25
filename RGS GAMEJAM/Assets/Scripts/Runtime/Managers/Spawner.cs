using Mirror;
using UnityEngine;
using System.Collections;

public class Spawner : NetworkBehaviour
{
    [Header("Spawn Settings")]
    public float radius = 5f; 
    public SO_WaveData[] waves;

    private int currentWave = 0;

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        yield return new WaitForSeconds(1f);

        while (currentWave < waves.Length)
        {
            yield return StartCoroutine(SpawnWave(waves[currentWave]));
            currentWave++;
        }
    }

    private IEnumerator SpawnWave(SO_WaveData wave)
    {
        Debug.Log($"[Spawner] Wave {currentWave + 1} Start");

        int totalCount = 0;
        foreach (var e in wave.randomEnemies)
            totalCount += e.count;

        float interval = wave.waveLength / Mathf.Max(totalCount, 1);

        float timer = 0f;
        int[] spawned = new int[wave.randomEnemies.Length];

        while (timer < wave.waveLength)
        {
            for (int i = 0; i < wave.randomEnemies.Length; i++)
            {
                EnemyData data = wave.randomEnemies[i];

                if (spawned[i] < data.count)
                {
                    SpawnSingleEnemy(data.enemyType);
                    spawned[i]++;
                }
            }

            yield return new WaitForSeconds(interval);
            timer += interval;
        }

        for (int i = 0; i < wave.randomEnemies.Length; i++)
        {
            EnemyData data = wave.randomEnemies[i];
            while (spawned[i] < data.count)
            {
                SpawnSingleEnemy(data.enemyType);
                spawned[i]++;
                yield return null; 
            }
        }

    }


    [Server]
    private void SpawnSingleEnemy(EnemyType type)
    {
        float theta = Random.Range(0f, Mathf.PI * 2f);

        float a = 2f;
        float b = 1f;

        Vector2 pos = new Vector2(
            a * Mathf.Cos(theta)*radius,
            b * Mathf.Sin(theta)*radius
        );

        GameManager.Instance.spawnManager.SpawnEnemy(type, pos);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
