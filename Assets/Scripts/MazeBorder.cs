using UnityEngine;

public class MazeBorder : MonoBehaviour
{
    public MazeGenerator mazeGenerator; // Reference to your MazeGenerator
    public GameObject wallPrefab; // Prefab for your wall
    public float wallHeight = 2f; // Adjust the height as needed
    public float wallThickness = 0.1f; // Adjust the thickness as needed

    void Start()
    {
        // Get the maze dimensions
        int width = mazeGenerator.mazeWidth;
        int height = mazeGenerator.mazeHeight;
        float cellSize = 1f; // Assuming cell size is 1

        // Create top wall
        for (int x = 0; x < width; x++)
        {
            CreateWall(new Vector3(x * cellSize + cellSize / 2f, wallHeight / 2f, height * cellSize), new Vector3(wallThickness, wallHeight, cellSize));
        }

        // Create bottom wall
        for (int x = 0; x < width; x++)
        {
            CreateWall(new Vector3(x * cellSize + cellSize / 2f, wallHeight / 2f, -wallThickness), new Vector3(wallThickness, wallHeight, cellSize));
        }

        // Create left wall
        for (int y = 0; y < height; y++)
        {
            CreateWall(new Vector3(-wallThickness, wallHeight / 2f, y * cellSize + cellSize / 2f), new Vector3(wallThickness, wallHeight, cellSize));
        }

        // Create right wall
        for (int y = 0; y < height; y++)
        {
            CreateWall(new Vector3(width * cellSize, wallHeight / 2f, y * cellSize + cellSize / 2f), new Vector3(wallThickness, wallHeight, cellSize));
        }
    }

    void CreateWall(Vector3 position, Vector3 scale)
    {
        GameObject newWall = Instantiate(wallPrefab, position, Quaternion.identity, transform);
        newWall.transform.localScale = scale;
    }
}

