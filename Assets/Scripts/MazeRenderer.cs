using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeRenderer : MonoBehaviour
{
    [SerializeField] MazeGenerator mazeGenerator; // Reference to the maze generator
    [SerializeField] GameObject MazeCellPrefab; // Prefab for maze cells

    public float CellSize = 1f; // Size of each cell

    // Start is called before the first frame update
    public void Start()
    {
        // Ensure the maze is generated correctly
        MazeGenerator.MazeCell[,] maze = mazeGenerator.GetMaze(); // Adjusted type

        // Loop through the maze dimensions
        for (int x = 0; x < mazeGenerator.mazeWidth; x++)
        {
            for (int y = 0; y < mazeGenerator.mazeHeight; y++)
            {
                // Instantiate a new maze cell at the calculated position
                GameObject newCell = Instantiate(MazeCellPrefab, new Vector3((float)x * CellSize, 0f, (float)y * CellSize), Quaternion.identity, transform);

                // Get the MazeCellObject component from the instantiated prefab
                MazeCellObject mazeCell = newCell.GetComponent<MazeCellObject>();

                // Check if the maze cell is valid before accessing its properties
                if (maze[x, y] != null)
                {
                    // Retrieve wall information from the maze cell
                    bool top = maze[x, y].topWall; // Ensure topWall exists
                    bool left = maze[x, y].leftWall; // Ensure leftWall exists
                    bool right = (x == mazeGenerator.mazeWidth - 1); // Right wall is true if it's the last column
                    bool bottom = (y == 0); // Bottom wall is true if it's the first row

                    // Initialize the maze cell with wall information
                    mazeCell.Init(top, bottom, right, left);
                }
                else
                {
                    Debug.LogWarning($"Maze cell at ({x}, {y}) is null.");
                }
            }
        }
    }
}

