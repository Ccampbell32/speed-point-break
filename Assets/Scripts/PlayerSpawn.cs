using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public MazeGenerator mazeGenerator;
    public GameObject playerPrefab; // Assign your player prefab here
    public float cellSize = 1f; // Match your cell size

    void Start()
    {
        if (mazeGenerator == null || playerPrefab == null)
        {
            Debug.LogError("MazeGenerator or PlayerPrefab not assigned!");
            return;
        }

        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        // Find a random, valid starting position
        Vector2Int randomCell = GetRandomValidCell();

        // Calculate the world position based on the cell coordinates
        Vector3 spawnPosition = new Vector3(randomCell.x * cellSize + cellSize / 2f, 0.5f, randomCell.y * cellSize + cellSize / 2f);

        // Instantiate the player at the calculated position
        Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
    }

    Vector2Int GetRandomValidCell()
    {
        int width = mazeGenerator.mazeWidth;
        int height = mazeGenerator.mazeHeight;

        Vector2Int randomCell;
        do
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            randomCell = new Vector2Int(x, y);
        } while (mazeGenerator.maze[randomCell.x, randomCell.y] == null); // Ensure the cell isn't null

        return randomCell;
    }
}
