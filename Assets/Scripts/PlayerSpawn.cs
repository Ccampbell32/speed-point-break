using UnityEngine;
using System.Collections;

public class PlayerSpawn : MonoBehaviour
{
    public MazeGenerator mazeGenerator;
    public GameObject playerPrefab;
    public float cellSize = 1f;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => mazeGenerator.isMazeReady); // Wait until maze is ready
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        Vector2Int randomCell = GetRandomValidCell();
        Vector3 spawnPosition = new Vector3(randomCell.x * cellSize + cellSize / 2f, 0.5f, randomCell.y * cellSize + cellSize / 2f);
        Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
    }

    Vector2Int GetRandomValidCell()
    {
        if (mazeGenerator == null || mazeGenerator.maze == null)
        {
            Debug.LogError("MazeGenerator or Maze array is not initialized!");
            return Vector2Int.zero; // Handle the error appropriately
        }

        int width = mazeGenerator.mazeWidth;
        int height = mazeGenerator.mazeHeight;

        if (width <= 0 || height <= 0)
        {
            Debug.LogError("Maze width or height is invalid!");
            return Vector2Int.zero; // Handle the error appropriately
        }

        Vector2Int randomCell;
        int attempts = 0; // Counter to prevent infinite loops
        const int maxAttempts = 100; // Limit on attempts

        do
        {
            if (attempts >= maxAttempts)
            {
                Debug.LogError("Could not find a valid cell after " + maxAttempts + " attempts.");
                return Vector2Int.zero; // Handle the error appropriately
            }

            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            randomCell = new Vector2Int(x, y);

            attempts++;
        } while (mazeGenerator.maze[randomCell.x, randomCell.y] == null); // Ensure the cell isn't null

        return randomCell;
    }

}
