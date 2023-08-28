# AStar-Algorithm
AStar Algorithm using C# with finding paths as grid in X and Y plane


# A* Pathfinding Algorithm Implementation
This code provides an implementation of the A* (A-star) pathfinding algorithm. The A* algorithm is used to find the shortest path between two points on a grid while considering the terrain and obstacles in the grid. This implementation includes the necessary classes and methods to create a pathfinding solution using A*.

# Classes and Key Components
- Cell Class

The Cell class represents a cell on the grid. Each cell has Row and Col properties that indicate its position on the grid.

- AStar Class

The AStar class contains the core functionality of the A* algorithm. It takes a 2D grid as input, where each cell is either passable (0) or an obstacle (1). This class provides methods for finding the shortest path between two cells on the grid.

- GetNeighbors Method

This method calculates the valid neighboring cells of a given cell. It considers cells in the up, down, left, and right directions, excluding cells that are out of bounds or contain obstacles.

- CalculateHeuristic Method

The heuristic function used in the A* algorithm is implemented here. It calculates the Euclidean distance (straight-line distance) between two cells. This heuristic estimate helps guide the search towards the goal.

- FindPath Method

This method implements the A* algorithm to find the shortest path between a start cell and a goal cell. It maintains open and closed sets, calculates scores, and explores neighboring cells. The resulting path is returned as a list of cells if a path is found, otherwise, it returns null.

- ReconstructPath Method

This method reconstructs the path from the start cell to the goal cell using the cameFrom dictionary generated during the A* algorithm.

- PriorityQueue Class

The PriorityQueue class is used to manage cells with priority. It is used to efficiently select the next cell to explore in the A* algorithm.

## Program Class
The Program class contains the Main method where the pathfinding algorithm is applied to a sample grid. A sample grid is defined with passable cells and obstacles. The A* algorithm is used to find the shortest path from the top-left corner to the bottom-right corner of the grid. If a path is found, the grid is printed with the path marked by asterisks (*).

## Usage
Define your grid by specifying passable cells (0) and obstacles (1).
Create an instance of the AStar class, passing your grid to the constructor.
Call the FindPath method on the AStar instance, providing the start and goal cells.
If a path is found, the method will return a list of cells representing the path.
Display the path by marking the path cells on the grid and printing the grid.

## Example
{

    // Define the grid
    int[,] grid = new int[,]
    {
        // Define your grid here
    };

    // Create AStar instance
    AStar aStar = new AStar(grid);

    // Find path
    List<Cell> path = aStar.FindPath(new Cell { Row = startRow, Col = startCol }, new Cell { Row = goalRow, Col = goalCol });

    // Display path
    if (path != null)
    {
        // Display grid with path marked
    }
    else
    {
        Console.WriteLine("No path found.");
    }
}
## Note

This code provides a basic implementation of the A* algorithm and may require modifications or enhancements for specific use cases. It is intended to serve as a starting point for understanding and implementing the A* pathfinding algorithm in C#.
