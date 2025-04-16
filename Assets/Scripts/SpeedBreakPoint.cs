using UnityEngine;
using System.Collections;

public class SpeedBreakPoint : MonoBehaviour
{
    public MazeGenerator mazeGenerator; // Reference to your MazeGenerator script
    public float cellSize = 1f; // Match your cell size
    public float respawnDelay = 15f; // Respawn delay in seconds
    public GameObject speedBreakPointPrefab; // Assign the prefab in the Inspector
    private bool isDestroyed = false;

    void Start()
    {
        // Ensure the prefab and maze generator are assigned
        if (speedBreakPointPrefab == null || mazeGenerator == null)
        {
            Debug.LogError("Speed Break Point Prefab or MazeGenerator not assigned!");
            Destroy(gameObject); // Destroy this instance to prevent further errors
            return;
        }

        // Initial placement
        PlaceAtRandomLocation();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDestroyed)
        {
            isDestroyed = true;
            // Increase player speed
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.speed += 1f; // Increase speed
            }
            else
            {
                Debug.LogWarning("PlayerMovement script not found on the player!");
            }

            // Respawn after a delay
            StartCoroutine(RespawnAfterDelay());
        }
    }

    void PlaceAtRandomLocation()
    {
        Vector2Int randomCell = GetRandomValidCell();
        Vector3 spawnPosition = new Vector3(randomCell.x * cellSize + cellSize / 2f, 0.5f, randomCell.y * cellSize + cellSize / 2f);
        transform.position = spawnPosition;
    }

    IEnumerator RespawnAfterDelay()
    {
        // Hide the speed break point (e.g., disable the renderer)
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        // Respawn
        isDestroyed = false;
        PlaceAtRandomLocation();

        // Show the speed break point (e.g., enable the renderer)
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
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

