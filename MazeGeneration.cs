using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Flags]
public enum WallState
{
    LEFT = 1,   
    RIGHT = 2,  
    UP = 4,     
    DOWN = 8,   
    VISITED = 128   
}

public struct Position
{
    public int X;
    public int Y;
}

public struct Neighbour
{
    public Position Position;
    public WallState SharedWall;
}

public static class MazeGenerator
{
    // Method to get opposite wall
    private static WallState GetOppositeWall(WallState wall)
    {
        switch (wall)
        {
            case WallState.RIGHT: return WallState.LEFT;
            case WallState.LEFT: return WallState.RIGHT;
            case WallState.UP: return WallState.DOWN;
            case WallState.DOWN: return WallState.UP;
            default: return WallState.LEFT;
        }
    }

    // Recursive Backtracker algorithm to generate maze paths
    private static WallState[,] ApplyRecursiveBacktracker(WallState[,] maze, int width, int height)
    {
        var rng = new System.Random();  
        var positionStack = new Stack<Position>();  // Stack to keep track of visited positions
        var position = new Position { X = rng.Next(0, width), Y = rng.Next(0, height) };  // Starting position

        maze[position.X, position.Y] |= WallState.VISITED;  // Mark starting position as visited
        positionStack.Push(position);  // Push starting position onto the stack

        // Main loop for generating maze paths
        while (positionStack.Count > 0)
        {
            var current = positionStack.Pop();  // Get the current position
            var neighbours = GetUnvisitedNeighbours(current, maze, width, height);  // Get unvisited neighbours

            if (neighbours.Count > 0)  // If there are unvisited neighbours
            {
                positionStack.Push(current);  // Push current position back onto the stack

                var randIndex = rng.Next(0, neighbours.Count);  // Randomly select a neighbour
                var randomNeighbour = neighbours[randIndex];

                var nPosition = randomNeighbour.Position;
                maze[current.X, current.Y] &= ~randomNeighbour.SharedWall;  // Remove wall between current and neighbour
                maze[nPosition.X, nPosition.Y] &= ~GetOppositeWall(randomNeighbour.SharedWall);  // Remove wall between neighbour and current
                maze[nPosition.X, nPosition.Y] |= WallState.VISITED;  // Mark neighbour as visited

                positionStack.Push(nPosition);  // Push neighbour onto the stack
            }
        }

        return maze;
    }

    // Method to get unvisited neighbours of a position
    private static List<Neighbour> GetUnvisitedNeighbours(Position p, WallState[,] maze, int width, int height)
    {
        var list = new List<Neighbour>();  // List to store unvisited neighbours

        // Check left neighbour
        if (p.X > 0)
        {
            if (!maze[p.X - 1, p.Y].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X - 1,
                        Y = p.Y
                    },
                    SharedWall = WallState.LEFT
                });
            }
        }

        // Check bottom neighbour
        if (p.Y > 0)
        {
            if (!maze[p.X, p.Y - 1].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X,
                        Y = p.Y - 1
                    },
                    SharedWall = WallState.DOWN
                });
            }
        }

        // Check top neighbour
        if (p.Y < height - 1)
        {
            if (!maze[p.X, p.Y + 1].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X,
                        Y = p.Y + 1
                    },
                    SharedWall = WallState.UP
                });
            }
        }

        // Check right neighbour
        if (p.X < width - 1)
        {
            if (!maze[p.X + 1, p.Y].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X + 1,
                        Y = p.Y
                    },
                    SharedWall = WallState.RIGHT
                });
            }
        }

        return list;  // Return list of unvisited neighbours
    }

    // Method to generate the maze
    public static WallState[,] Generate(int width, int height, int numExits)
    {
        WallState[,] maze = new WallState[width, height];
        WallState initial = WallState.RIGHT | WallState.LEFT | WallState.UP | WallState.DOWN;

        // Initialize maze with all walls intact
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                maze[i, j] = initial;
            }
        }

        var rng = new System.Random();  
        var availableCells = new List<(int, int)>();  // List to store available cells

        // Add boundary cells to availableCells list
        for (int i = 0; i < width; i++)
        {
            availableCells.Add((i, 0));             // Top boundary
            availableCells.Add((i, height - 1));    // Bottom boundary
        }
        for (int j = 1; j < height - 1; j++)
        {
            availableCells.Add((0, j));             // Left boundary
            availableCells.Add((width - 1, j));     // Right boundary
        }

        // Randomly select cells to mark as exits
        for (int i = 0; i < numExits; i++)
        {
            int index = rng.Next(availableCells.Count);
            var cell = availableCells[index];
            int x = cell.Item1;
            int y = cell.Item2;

            availableCells.RemoveAt(index);  // Remove the selected cell from availableCells

            maze[x, y] = WallState.VISITED;  // Mark the cell as visited
        }

        // Apply recursive backtracking algorithm to generate maze paths
        maze = ApplyRecursiveBacktracker(maze, width, height);

        return maze;  
    }
}
