using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public GameObject speedBreakPointPrefab;
    public MazeGenerator mazeGenerator;
    public int numberOfBreakPoints = 5;
    public float cellSize = 1f;
    private List<SpeedBreakPoint> speedBreakPoints = new List<SpeedBreakPoint>();

    void Start()
    {
        if (speedBreakPointPrefab == null || mazeGenerator == null)
        {
            Debug.LogError("Speed Break Point Prefab or MazeGenerator not assigned!");
            return;
        }

        SpawnBreakPoints();
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

    Vector2Int GetRandomValidCell()
    {
        if (mazeGenerator == null || mazeGenerator.maze == null)
        {
            Debug.LogError("MazeGenerator or Maze array is not initialized!");
            return Vector2Int.zero; // Or handle the error appropriately
        }

        int width = mazeGenerator.mazeWidth;
        int height = mazeGenerator.mazeHeight;

        if (width <= 0 || height <= 0)
        {
            Debug.LogError("Maze width or height is invalid!");
            return Vector2Int.zero; // Or handle the error appropriately
        }

        Vector2Int randomCell;
        int attempts = 0; // Counter to prevent infinite loops
        const int maxAttempts = 100; // Limit on attempts

        do
        {
            if (attempts >= maxAttempts)
            {
                Debug.LogError("Could not find a valid cell after " + maxAttempts + " attempts.");
                return Vector2Int.zero; // Or handle the error appropriately
            }

            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            randomCell = new Vector2Int(x, y);

            attempts++;
        } while (mazeGenerator.maze[randomCell.x, randomCell.y] == null); // Ensure the cell isn't null

        return randomCell;
    }
}
