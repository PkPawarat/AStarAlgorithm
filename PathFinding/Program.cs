using System;
using System.Collections.Generic;

public class Cell
{
    public int Row { get; set; }
    public int Col { get; set; }
}

public class AStar
{
    private int[,] grid;
    private int rows;
    private int columns;

    public AStar(int[,] grid)
    {
        this.grid = grid;
        rows = grid.GetLength(0);
        columns = grid.GetLength(1);
    }

    private List<Cell> GetNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>
        {
            new Cell { Row = cell.Row + 1, Col = cell.Col },
            new Cell { Row = cell.Row - 1, Col = cell.Col },
            new Cell { Row = cell.Row, Col = cell.Col + 1 },
            new Cell { Row = cell.Row, Col = cell.Col - 1 }
        };

        // Filter out cells that are out of bounds or obstacles
        neighbors.RemoveAll(n => n.Row < 0 || n.Row >= rows || n.Col < 0 || n.Col >= columns || grid[n.Row, n.Col] == 1);

        return neighbors;
    }

    private double CalculateHeuristic(Cell cell, Cell goal)
    {
        return Math.Sqrt(Math.Pow(cell.Row - goal.Row, 2) + Math.Pow(cell.Col - goal.Col, 2));
    }

    public List<Cell> FindPath(Cell start, Cell goal)
    {
        var openSet = new PriorityQueue<Cell>();
        var cameFrom = new Dictionary<Cell, Cell>();
        var gScore = new Dictionary<Cell, double>();
        var fScore = new Dictionary<Cell, double>();
        var closedSet = new HashSet<Cell>();

        openSet.Enqueue(start, 0);
        gScore[start] = 0;
        fScore[start] = CalculateHeuristic(start, goal);

        while (openSet.Count > 0)
        {
            Cell current = openSet.Dequeue();

            if (current.Row == goal.Row && current.Col == goal.Col)
            {
                return ReconstructPath(cameFrom, current);
            }

            closedSet.Add(current);

            foreach (Cell neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                double tentativeGScore = gScore[current] + CalculateHeuristic(current, neighbor);

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + CalculateHeuristic(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }

        return null;
    }

    private List<Cell> ReconstructPath(Dictionary<Cell, Cell> cameFrom, Cell current)
    {
        List<Cell> path = new List<Cell> { current };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }

        path.Reverse();
        return path;
    }
}

public class PriorityQueue<T>
{
    private List<(T item, double priority)> elements = new List<(T item, double priority)>();

    public int Count => elements.Count;

    public void Enqueue(T item, double priority)
    {
        elements.Add((item, priority));
        int index = elements.Count - 1;

        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;

            if (elements[parentIndex].priority <= elements[index].priority)
                break;

            (T tempItem, double tempPriority) = elements[index];
            elements[index] = elements[parentIndex];
            elements[parentIndex] = (tempItem, tempPriority);

            index = parentIndex;
        }
    }

    public T Dequeue()
    {
        T topItem = elements[0].item;
        elements[0] = elements[elements.Count - 1];
        elements.RemoveAt(elements.Count - 1);

        int index = 0;

        while (true)
        {
            int child1Index = 2 * index + 1;
            int child2Index = 2 * index + 2;

            if (child1Index >= elements.Count)
                break;

            int minChildIndex = child2Index < elements.Count && elements[child2Index].priority < elements[child1Index].priority ? child2Index : child1Index;

            if (elements[index].priority <= elements[minChildIndex].priority)
                break;

            (T tempItem, double tempPriority) = elements[index];
            elements[index] = elements[minChildIndex];
            elements[minChildIndex] = (tempItem, tempPriority);

            index = minChildIndex;
        }

        return topItem;
    }

    public bool Contains(T item)
    {
        foreach (var (existingItem, _) in elements)
        {
            if (EqualityComparer<T>.Default.Equals(existingItem, item))
                return true;
        }

        return false;
    }
}

public class Program
{
    public static void Main()
    {
        int[,] grid = new int[,]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 1, 0, 1, 1, 1, 0, 1, 1, 0 },
            { 0, 1, 0, 1, 1, 1, 0, 1, 1, 0 },
            { 0, 0, 0, 1, 1, 1, 0, 1, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 1, 0, 1, 1, 1, 0, 1, 1, 0 },
            { 0, 1, 0, 1, 1, 1, 0, 1, 1, 0 },
            { 0, 1, 0, 1, 1, 1, 0, 1, 1, 0 },
            { 0, 0, 0, 1, 1, 1, 0, 1, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        AStar aStar = new AStar(grid);
        List<Cell> path = aStar.FindPath(new Cell { Row = 0, Col = 0 }, new Cell { Row = 9, Col = 9 });

        if (path != null)
        {
            Console.WriteLine("Path found.");

            // Create a copy of the grid to mark the path
            string[,] pathGrid = new string[grid.GetLength(0), grid.GetLength(1)];

            for (int i = 0; i < pathGrid.GetLength(0); i++)
            {
                for (int j = 0; j < pathGrid.GetLength(1); j++)
                {
                    pathGrid[i, j] = grid[i, j] == 0 ? "0" : "1"; // Initialize with obstacles or empty spaces
                }
            }

            foreach (Cell cell in path)
            {
                pathGrid[cell.Row, cell.Col] = "*"; // Mark path cells with asterisk
            }

            // Print the path-marked grid
            for (int i = 0; i < pathGrid.GetLength(0); i++)
            {
                for (int j = 0; j < pathGrid.GetLength(1); j++)
                {
                    Console.Write(pathGrid[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("No path found.");
        }
    }
}

//using System;
//using System.Collections.Generic;

//class Node : IComparable<Node>
//{
//    public int X { get; }
//    public int Y { get; }

//    public Node(int x, int y)
//    {
//        X = x;
//        Y = y;
//    }

//    public int CompareTo(Node other)
//    {
//        int xComparison = X.CompareTo(other.X);
//        if (xComparison != 0)
//            return xComparison;

//        return Y.CompareTo(other.Y);
//    }
//}

//class DijkstraAlgorithm
//{
//    public Dictionary<Node, List<Tuple<Node, int>>> graph;

//    public DijkstraAlgorithm()
//    {
//        graph = new Dictionary<Node, List<Tuple<Node, int>>>();
//    }

//    public void AddEdge(Node from, Node to, int cost)
//    {
//        if (!graph.ContainsKey(from))
//            graph[from] = new List<Tuple<Node, int>>();

//        graph[from].Add(new Tuple<Node, int>(to, cost));
//    }

//    public List<Node> ShortestPath(Node start, Node destination)
//    {
//        var distance = new Dictionary<Node, int>();
//        var previous = new Dictionary<Node, Node>();
//        var visited = new HashSet<Node>();
//        var queue = new PriorityQueue<Node>();

//        foreach (var node in graph.Keys)
//        {
//            distance[node] = int.MaxValue;
//        }

//        distance[start] = 0;
//        queue.Enqueue(start, 0);

//        while (!queue.IsEmpty())
//        {
//            var current = queue.Dequeue();

//            if (visited.Contains(current))
//                continue;

//            visited.Add(current);

//            if (current == destination)
//                break;

//            if (!graph.ContainsKey(current))
//                continue;

//            foreach (var neighborTuple in graph[current])
//            {
//                var neighbor = neighborTuple.Item1;
//                var weight = neighborTuple.Item2;

//                var altDistance = distance[current] + weight;
//                if (!distance.ContainsKey(neighbor) || altDistance < distance[neighbor])
//                {
//                    distance[neighbor] = altDistance;
//                    previous[neighbor] = current;
//                    queue.Enqueue(neighbor, altDistance);
//                }
//            }
//        }

//        // Reconstruct the path
//        var path = new List<Node>();
//        Node currentPathNode = destination;
//        while (currentPathNode != null)
//        {
//            path.Insert(0, currentPathNode);
//            currentPathNode = previous.ContainsKey(currentPathNode) ? previous[currentPathNode] : null;
//        }

//        return path;
//    }

//}

//class PriorityQueue<T> where T : IComparable<T>
//{
//    private List<Tuple<T, int>> elements = new List<Tuple<T, int>>();

//    public void Enqueue(T item, int priority)
//    {
//        elements.Add(Tuple.Create(item, priority));
//        int i = elements.Count - 1;

//        while (i > 0)
//        {
//            int parentIndex = (i - 1) / 2;
//            if (elements[i].Item2 >= elements[parentIndex].Item2)
//                break;

//            Tuple<T, int> temp = elements[i];
//            elements[i] = elements[parentIndex];
//            elements[parentIndex] = temp;

//            i = parentIndex;
//        }
//    }

//    public T Dequeue()
//    {
//        T result = elements[0].Item1;
//        int lastIndex = elements.Count - 1;
//        elements[0] = elements[lastIndex];
//        elements.RemoveAt(lastIndex);

//        int currentIndex = 0;
//        while (true)
//        {
//            int leftChildIndex = currentIndex * 2 + 1;
//            int rightChildIndex = currentIndex * 2 + 2;
//            int smallestIndex = currentIndex;

//            if (leftChildIndex < elements.Count && elements[leftChildIndex].Item2 < elements[smallestIndex].Item2)
//                smallestIndex = leftChildIndex;

//            if (rightChildIndex < elements.Count && elements[rightChildIndex].Item2 < elements[smallestIndex].Item2)
//                smallestIndex = rightChildIndex;

//            if (smallestIndex == currentIndex)
//                break;

//            Tuple<T, int> temp = elements[currentIndex];
//            elements[currentIndex] = elements[smallestIndex];
//            elements[smallestIndex] = temp;

//            currentIndex = smallestIndex;
//        }

//        return result;
//    }

//    public bool IsEmpty()
//    {
//        return elements.Count == 0;
//    }
//}

//class Program
//{
//    static void Main(string[] args)
//    {
//        DijkstraAlgorithm dijkstra = new DijkstraAlgorithm();

//        Node A = new Node(0, 0);
//        Node B = new Node(1, 2);
//        Node C = new Node(3, 1);
//        Node D = new Node(2, 4);
//        Node E = new Node(4, 3);
//        Node F = new Node(5, 6);
//        Node G = new Node(7, 5);
//        Node H = new Node(6, 8);
//        Node I = new Node(9, 7);
//        Node J = new Node(8, 10);
//        Node K = new Node(11, 9);
//        Node L = new Node(10, 12);
//        Node M = new Node(13, 11);
//        Node N = new Node(12, 14);
//        Node O = new Node(15, 13);
//        Node P = new Node(14, 16);
//        Node Q = new Node(17, 15);
//        Node R = new Node(16, 18);
//        Node S = new Node(19, 17);

//        dijkstra.AddEdge(A, B, 1);
//        dijkstra.AddEdge(A, C, 1);
//        dijkstra.AddEdge(B, D, 1);
//        dijkstra.AddEdge(C, D, 1);
//        dijkstra.AddEdge(C, E, 1);
//        dijkstra.AddEdge(D, E, 1);
//        dijkstra.AddEdge(E, F, 1);
//        dijkstra.AddEdge(F, G, 1);
//        dijkstra.AddEdge(G, H, 1);
//        dijkstra.AddEdge(H, I, 1);
//        dijkstra.AddEdge(I, J, 1);
//        dijkstra.AddEdge(J, K, 1);
//        dijkstra.AddEdge(K, L, 1);
//        dijkstra.AddEdge(L, M, 1);
//        dijkstra.AddEdge(M, N, 1);
//        dijkstra.AddEdge(N, O, 1);
//        dijkstra.AddEdge(O, P, 1);
//        dijkstra.AddEdge(P, Q, 1);
//        dijkstra.AddEdge(Q, R, 1);
//        dijkstra.AddEdge(R, S, 1);


//        List<Node> shortestPath = dijkstra.ShortestPath(A, G);

//        foreach (var node in shortestPath)
//        {
//            Console.WriteLine($"Shortest path: ({node.X}, {node.Y})");
//        }
//        Console.WriteLine("Map with All Locations:");
//        DrawMap(dijkstra.graph.Keys);

//        Console.WriteLine("\nMap with Shortest Path:");
//        DrawMapWithShortestPath(dijkstra.graph.Keys, shortestPath);


//    }
//    static void DrawMap(IEnumerable<Node> locations)
//    {
//        int maxX = locations.Max(node => node.X);
//        int maxY = locations.Max(node => node.Y);

//        for (int y = 0; y <= maxY; y++)
//        {
//            for (int x = 0; x <= maxX; x++)
//            {
//                Node node = new Node(x, y);
//                if (locations.Any(x => x.X == node.X && x.Y == node.Y))
//                {
//                    Console.Write("* ");
//                }
//                else
//                {
//                    Console.Write(". ");
//                }
//            }
//            Console.WriteLine();
//        }
//    }


//    static void DrawMapWithShortestPath(IEnumerable<Node> locations, List<Node> shortestPath)
//    {
//        int maxX = int.MinValue;
//        int maxY = int.MinValue;

//        // Find the maximum X and Y values among all locations
//        foreach (var location in locations)
//        {
//            maxX = Math.Max(maxX, location.X);
//            maxY = Math.Max(maxY, location.Y);
//        }

//        string[,] grid = new string[maxY + 1, maxX + 1];

//        // Initialize the grid with spaces
//        for (int y = 0; y <= maxY; y++)
//        {
//            for (int x = 0; x <= maxX; x++)
//            {
//                grid[y, x] = " . ";
//            }
//        }

//        // Mark the nodes in the grid
//        foreach (var location in locations)
//        {
//            grid[location.Y, location.X] = " * ";
//        }

//        // Mark the nodes in the shortest path with 'P'
//        foreach (var location in shortestPath)
//        {
//            grid[location.Y, location.X] = " P ";
//        }

//        // Display the grid
//        for (int y = 0; y <= maxY; y++)
//        {
//            for (int x = 0; x <= maxX; x++)
//            {
//                Console.Write(grid[y, x]);
//            }
//            Console.WriteLine();
//        }
//    }


//}

