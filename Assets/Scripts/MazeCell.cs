using UnityEngine;

public class MazeCell
{
    public bool visited;
    public int x, y;
    public bool topWall;
    public bool leftWall;
    public bool rightWall;
    public bool bottomWall;
    public Vector2Int position => new Vector2Int(x, y);

    public MazeCell(int x, int y)
    {
        this.x = x;
        this.y = y;
        visited = false;
        topWall = leftWall = rightWall = bottomWall = true;
    }
}