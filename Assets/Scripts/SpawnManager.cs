using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public GameObject speedBreakPointPrefab;
    public GameObject enemyPrefab; // Assign the enemy prefab here
    public MazeGenerator mazeGenerator;
    public int numberOfBreakPoints = 5;
    public float cellSize = 1f;
    public float enemySpawnInterval = 30f; // Time between enemy spawns
    private List<SpeedBreakPoint> speedBreakPoints = new List<SpeedBreakPoint>();
    private float timeSinceLastEnemySpawn = 0f;
    private float enemySpawnSpeedIncrease = 1f; // Speed increase for each new enemy

    void Start()
    {
        if (speedBreakPointPrefab == null || mazeGenerator == null || enemyPrefab == null)
        {
            Debug.LogError("Prefabs or MazeGenerator not assigned!");
            return;
        }

        SpawnBreakPoints();
    }

    void Update()
    {
        timeSinceLastEnemySpawn += Time.deltaTime;
        if (timeSinceLastEnemySpawn >= enemySpawnInterval)
        {
            SpawnEnemy();
            timeSinceLastEnemySpawn = 0f;
        }
    }

    void SpawnBreakPoints()
    {
        for (int i = 0; i < numberOfBreakPoints; i++)
        {
            SpawnSpeedBreakPoint();
        }
    }

    void SpawnSpeedBreakPoint()
    {
        Vector2Int randomCell = GetRandomValidCell();
        Vector3 spawnPosition = new Vector3(randomCell.x * cellSize + cellSize / 2f, 0.5f, randomCell.y * cellSize + cellSize / 2f);
        GameObject newBreakPoint = Instantiate(speedBreakPointPrefab, spawnPosition, Quaternion.identity);

        SpeedBreakPoint speedBreakPointScript = newBreakPoint.GetComponent<SpeedBreakPoint>();
        if (speedBreakPointScript != null)
        {
            speedBreakPointScript.mazeGenerator = mazeGenerator;
            speedBreakPointScript.cellSize = cellSize;
        }
        else
        {
            Debug.LogError("SpeedBreakPoint script not found on prefab!");
            Destroy(newBreakPoint); // Destroy the instance if the script is missing
        }
        speedBreakPoints.Add(speedBreakPointScript);
    }

    void SpawnEnemy()
    {
        Vector2Int randomCell = GetRandomValidCell();
        Vector3 spawnPosition = new Vector3(randomCell.x * cellSize + cellSize / 2f, 0.5f, randomCell.y * cellSize + cellSize / 2f);
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        EnemyAI enemyAI = newEnemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.mazeGenerator = mazeGenerator;
            enemyAI.cellSize = cellSize;
            enemyAI.roamingSpeed += enemySpawnSpeedIncrease;
            enemyAI.chaseSpeed += enemySpawnSpeedIncrease;
            enemySpawnSpeedIncrease += 1f;
        }
        else
        {
            Debug.LogError("EnemyAI script not found on prefab!");
            Destroy(newEnemy);
        }
    }

    Vector2Int GetRandomValidCell()
    {
        if (mazeGenerator == null || mazeGenerator.maze == null)
        {
            Debug.LogError("MazeGenerator or Maze array is not initialized!");
            return Vector2Int.zero; // Or handle the error appropriately
        }

        int width = mazeGenerator.mazeWidth;
        int height = mazeGenerator.mazeHeight;

        Vector2Int randomCell;
        int attempts = 0;
        const int maxAttempts = 100;

        do
        {
            if (attempts >= maxAttempts)
            {
                Debug.LogError("Could not find a valid cell after " + maxAttempts + " attempts.");
                return Vector2Int.zero;
            }

            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            randomCell = new Vector2Int(x, y);
            attempts++;
        } while (mazeGenerator.maze[randomCell.x, randomCell.y] == null); // Ensure the cell isn't null

        return randomCell;
    }
}
