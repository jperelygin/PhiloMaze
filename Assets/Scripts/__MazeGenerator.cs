using System.IO;
using System.Linq;
using UnityEngine;

public enum Behaviour
{
    Picture,
    Mesh,
    Prim
}

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private int cols = 0;
    [SerializeField]
    private int rows = 0;
    [SerializeField]
    private Behaviour behaviour = Behaviour.Picture;
    [SerializeField]
    private GameObject wall = null;
    [SerializeField]
    private GameObject floor = null;

    private byte[,] matrix;

    private readonly byte DEFAULT = 15;
    private readonly byte VISITED =  16;

    private byte[,] CreateMatrix(int rows, int cols, byte defaultValue)
    {
        byte[,] matrix = new byte[rows, cols];
        for(int row = 0; row < rows; row++)
        {
            for(int col = 0; col < cols; col++)
            {
                matrix[row, col] = defaultValue;
            }
        }
        return matrix;
    }

    private void BacktrackFillCells(int[] start)
    {
        string[] vector = {"U", "D", "L", "R"};
        matrix[start[0], start[1]] += VISITED;

        while (vector.Length > 0)
        {
            int num = Random.Range(0, vector.Length - 1); // NOT SURE HOW IT WILL WORK WITH 1 ELEMENT
            string chosenVector = vector[num];
            vector = vector.Where(val => val != chosenVector).ToArray();

            int[] nextCell = {0 , 0};
            byte removeInCurrent = 0;
            byte removeInNext = 0;
            if (chosenVector == "U")
            {
                nextCell[0] = start[0];
                nextCell[1] = start[1] - 1;
                removeInCurrent = 1;
                removeInNext = 4;
            }
            if (chosenVector == "D")
            {
                nextCell[0] = start[0];
                nextCell[1] = start[1] + 1;
                removeInCurrent = 4;
                removeInNext = 1;
            }
            if (chosenVector == "L")
            {
                nextCell[0] = start[0] - 1;
                nextCell[1] = start[1];
                removeInCurrent = 8;
                removeInNext = 2;
            }
            if (chosenVector == "R")
            {
                nextCell[0] = start[0] + 1;
                nextCell[1] = start[1];
                removeInCurrent = 2;
                removeInNext = 8;
            }
            if (nextCell[0] < 0 || nextCell[0] >= cols || nextCell[1] < 0 || nextCell[1] >= rows || matrix[nextCell[0], nextCell[1]] >= 16)
            {
                continue;
            }
            matrix[start[0], start[1]] -= removeInCurrent;
            matrix[nextCell[0], nextCell[1]] -= removeInNext;
            BacktrackFillCells(nextCell);
        }
        // Clean up visited cells
        // for (int row = 0; row < rows; row++)
        // {
        //     for (int col = 0; col < cols; col++)
        //     {
        //         matrix[row, col] = (byte)(matrix[row, col] & ~VISITED);
        //     }
        // }
    }

    private void PrimGenerateMaze(int[] start)
    {
        // List of walls to consider
        var walls = new System.Collections.Generic.List<(int[] cell, string direction)>();
        
        // Mark the starting cell as visited
        matrix[start[0], start[1]] += VISITED;
        
        // Add walls of the starting cell
        if (start[1] > 0) walls.Add((start, "U"));           // Up
        if (start[1] < rows - 1) walls.Add((start, "D"));    // Down
        if (start[0] > 0) walls.Add((start, "L"));          // Left
        if (start[0] < cols - 1) walls.Add((start, "R"));   // Right

        while (walls.Count > 0)
        {
            // Pick a random wall
            int wallIndex = Random.Range(0, walls.Count);
            var (cell, direction) = walls[wallIndex];
            walls.RemoveAt(wallIndex);

            int[] nextCell = {0, 0};
            byte removeInCurrent = 0;
            byte removeInNext = 0;

            // Calculate the cell on the opposite side of the wall
            if (direction == "U")
            {
                nextCell[0] = cell[0];
                nextCell[1] = cell[1] - 1;
                removeInCurrent = 1;
                removeInNext = 4;
            }
            else if (direction == "D")
            {
                nextCell[0] = cell[0];
                nextCell[1] = cell[1] + 1;
                removeInCurrent = 4;
                removeInNext = 1;
            }
            else if (direction == "L")
            {
                nextCell[0] = cell[0] - 1;
                nextCell[1] = cell[1];
                removeInCurrent = 8;
                removeInNext = 2;
            }
            else if (direction == "R")
            {
                nextCell[0] = cell[0] + 1;
                nextCell[1] = cell[1];
                removeInCurrent = 2;
                removeInNext = 8;
            }

            // If the next cell hasn't been visited
            if (matrix[nextCell[0], nextCell[1]] < VISITED)
            {
                // Remove walls between cells
                matrix[cell[0], cell[1]] -= removeInCurrent;
                matrix[nextCell[0], nextCell[1]] -= removeInNext;
                
                // Mark the new cell as visited
                matrix[nextCell[0], nextCell[1]] += VISITED;

                // Add new walls to consider
                if (nextCell[1] > 0 && matrix[nextCell[0], nextCell[1] - 1] < VISITED)
                    walls.Add((nextCell, "U"));
                if (nextCell[1] < rows - 1 && matrix[nextCell[0], nextCell[1] + 1] < VISITED)
                    walls.Add((nextCell, "D"));
                if (nextCell[0] > 0 && matrix[nextCell[0] - 1, nextCell[1]] < VISITED)
                    walls.Add((nextCell, "L"));
                if (nextCell[0] < cols - 1 && matrix[nextCell[0] + 1, nextCell[1]] < VISITED)
                    walls.Add((nextCell, "R"));
            }
        }

        // Clean up visited flags
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                matrix[row, col] = (byte)(matrix[row, col] & ~VISITED);
            }
        }
    }

    private int[] GetRandomStart(int rows, int cols)
    {
        return new int[] {Random.Range(0, rows), Random.Range(0, cols)};
    }

    private Texture2D GenerateMazeImage()
    {
        int cellSize = 10;
        Texture2D texture = new Texture2D(cols * cellSize, rows * cellSize, TextureFormat.RGB24, false);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                float colorValue = matrix[row, col] / 255f;
                Color color = new Color(colorValue, colorValue, colorValue);
                for (int y = 0; y < cellSize; y++)
                {
                    for (int x = 0; x < cellSize; x++)
                    {
                        texture.SetPixel(col * cellSize + x, row * cellSize + y, color);
                    }
                }
            }
        }
        texture.Apply();
        return texture;
    }

    private void SaveTexture(Texture2D texture2D, string filePath)
    {
        byte[] bytes = texture2D.EncodeToJPG(90);
        File.WriteAllBytes(filePath, bytes);
    }

    private void GenerateWalls(byte cell, float x, float z)
    {
        float y = 0.4f;
        int up = 0;
        int right = 1;
        int down = 2;
        int left = 3;
        if ((cell & (1 << up)) != 0)
        {
            var up_wall = Instantiate(wall, new Vector3(x, y, z), Quaternion.Euler(90, 90, 0));
            up_wall.name = $"Wall Up {x}|{z}";
        }
        if ((cell & (1 << right)) != 0)
        {
            var right_wall = Instantiate(wall, new Vector3(x + 3.6f, y, z + 0.4f), Quaternion.Euler(90, 0, 0));
            right_wall.name = $"Wall Right {x}|{z}";
        }
        if ((cell & (1 << down)) != 0)
        {
            var down_wall = Instantiate(wall, new Vector3(x, y, z + 3.6f), Quaternion.Euler(90, 90, 0));
            down_wall.name = $"Wall Down {x}|{z}";
        }
        if ((cell & (1 << left)) != 0)
        {
            var left_wall = Instantiate(wall, new Vector3(x, y, z), Quaternion.Euler(90, 0, 0));
            left_wall.name = $"Wall Left {x}|{z}";
        }
    }

    private void GenerateMazeMesh()
    {

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int x = row * 4;
                int z = col * 4;
                Instantiate(floor, new Vector3(x, 0, z), Quaternion.identity);
                GenerateWalls(matrix[row, col], x, z);
            }
        }
    }

    private void ExportMatrixToCSV()
    {
        string filePath = Path.Combine(Application.dataPath, "maze_matrix.csv");
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int row = 0; row < rows; row++)
            {
                string line = string.Join(",", Enumerable.Range(0, cols).Select(col => matrix[row, col].ToString()));
                writer.WriteLine(line);
            }
        }
        Debug.Log($"Matrix exported to: {filePath}");
    }

    // Start is called before the first frame update
    void Start()
    {
        matrix = CreateMatrix(rows, cols, DEFAULT);
        if (behaviour == Behaviour.Picture)
        {
            BacktrackFillCells(GetRandomStart(rows, cols));
            Texture2D image = GenerateMazeImage();
            string path = Path.Combine(Application.dataPath, "mazeImage.jpg");
            SaveTexture(image, path);
            Debug.Log($"Image saved to {path}");
        }
        else if (behaviour == Behaviour.Mesh)
        {
            BacktrackFillCells(GetRandomStart(rows, cols));
            GenerateMazeMesh();
        }
        else if (behaviour == Behaviour.Prim)
        {
            PrimGenerateMaze(GetRandomStart(rows, cols));
            GenerateMazeMesh();
        }
        ExportMatrixToCSV();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
