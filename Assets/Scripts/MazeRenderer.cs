using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    [SerializeField] MazeGenerator mazeGenerator;
    [SerializeField] GameObject MazeCellPrefab;
    public float CellSize = 1f;

    public void Start()
    {
        MazeGenerator.MazeCell[,] maze = mazeGenerator.GenerateMaze(); // Fully qualified name here

        for (int x = 0; x < mazeGenerator.mazeWidth; x++)
        {
            for (int y = 0; y < mazeGenerator.mazeHeight; y++)
            {
                GameObject newCell = Instantiate(MazeCellPrefab, new Vector3(x * CellSize, 0f, y * CellSize), Quaternion.identity, transform);
                MazeCellObject mazeCell = newCell.GetComponent<MazeCellObject>();

                if (mazeCell == null)
                {
                    Debug.LogError("MazeCellObject component missing from prefab!");
                    continue;
                }

                if (maze[x, y] != null)
                {
                    // Correctly use all wall properties
                    bool top = maze[x, y].topWall;
                    bool left = maze[x, y].leftWall;
                    bool right = maze[x, y].rightWall;
                    bool bottom = maze[x, y].bottomWall;

                    mazeCell.Init(top, bottom, left, right);

                    //Debug log for verification (remove later if not needed)
                    //Debug.Log($"Cell ({x}, {y}): Top={top}, Bottom={bottom}, Left={left}, Right={right}");
                }
                else
                {
                   // Debug.LogError($"Maze data is null at ({x}, {y})!");
                }
            }
        }
    }
}



