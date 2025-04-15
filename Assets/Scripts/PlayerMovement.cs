using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public MazeGenerator mazeGenerator;
    public float speed = 5f;
    public float cellSize = 1f; // Adjust to match your cell size

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized * speed * Time.deltaTime;
        Vector3 newPosition = transform.position + movement;

        if (IsMoveAllowed(transform.position, newPosition))
        {
            transform.position = newPosition;
        }
    }

    bool IsMoveAllowed(Vector3 currentPos, Vector3 newPos)
    {
        Vector2Int currentCell = GetCellFromPosition(currentPos);
        Vector2Int nextCell = GetCellFromPosition(newPos);

        if (currentCell == nextCell) return true; //Same cell

        int xDiff = nextCell.x - currentCell.x;
        int yDiff = nextCell.y - currentCell.y;

        if (xDiff == 1) return !mazeGenerator.maze[currentCell.x, currentCell.y].rightWall;
        if (xDiff == -1) return !mazeGenerator.maze[currentCell.x, currentCell.y].leftWall;
        if (yDiff == 1) return !mazeGenerator.maze[currentCell.x, currentCell.y].topWall;
        if (yDiff == -1) return !mazeGenerator.maze[currentCell.x, currentCell.y].bottomWall;

        return false; // Invalid move
    }

    Vector2Int GetCellFromPosition(Vector3 position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x / cellSize), Mathf.FloorToInt(position.z / cellSize));
    }
}