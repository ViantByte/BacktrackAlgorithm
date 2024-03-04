using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCreation : MonoBehaviour
{
    [SerializeField]
    [Range(1, 50)]
    private int width = 10;

    [SerializeField]
    [Range(1, 50)]
    private int height = 10;

    [SerializeField]
    private float size = 1f;

    [SerializeField]
    private Transform wallPrefab = null;

    [SerializeField]
    private Transform floorPrefab = null;

    [SerializeField]
    [Range(1, 10)]
    private int numExits = 1; 

    void Start()
    {
        MazeGeneration();
    }

    private void MazeGeneration()
    {
        // Generate maze with specified number of exits
        var maze = MazeGenerator.Generate(width, height, numExits); 
        // Draw the generated maze
        DrawMaze(maze);
    }

    // Method to draw the maze
    private void DrawMaze(WallState[,] maze)
    {
        // Create the floor of the maze
        var floor = Instantiate(floorPrefab, transform);
        floor.localScale = new Vector3(width, 1, height);

        // Loop through each cell in the maze
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                var cell = maze[i, j];
                var position = new Vector3(-width / 2 + i, 0, -height / 2 + j);

                // Check for walls and create them accordingly
                if (cell.HasFlag(WallState.UP))
                {
                    CreateWall(position + new Vector3(0, 0, size / 2), Vector3.zero);
                }

                if (cell.HasFlag(WallState.LEFT))
                {
                    CreateWall(position + new Vector3(-size / 2, 0, 0), new Vector3(0, 90, 0));
                }

                if (i == width - 1 && cell.HasFlag(WallState.RIGHT))
                {
                    CreateWall(position + new Vector3(size / 2, 0, 0), new Vector3(0, 90, 0));
                }

                if (j == 0 && cell.HasFlag(WallState.DOWN))
                {
                    CreateWall(position + new Vector3(0, 0, -size / 2), Vector3.zero);
                }
            }
        }
    }

    // Method to create walls
    private void CreateWall(Vector3 position, Vector3 rotation)
    {
        var wall = Instantiate(wallPrefab, transform) as Transform;
        wall.position = position;
        wall.localScale = new Vector3(size, wall.localScale.y, wall.localScale.z);
        wall.eulerAngles = rotation;
    }
}
