using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class MazeGeneratior2 : MonoBehaviour
{
    [SerializeField] private GameObject cell;
    [SerializeField] private GameObject cellsParent;
    [SerializeField] private GameObject doorWall;
    [SerializeField] private GameObject ceiling;
    [SerializeField] private GameObject ceilingParent;
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private GameObject player;
    [SerializeField] private Camera mapCamera;
    [SerializeField] private int rows;
    [SerializeField] private int cols;
    [SerializeField] private int numberOfKeys;

    private List<List<Cell>> matrix = new List<List<Cell>>();
    private GameObject door;

    public int GetNumberOfKeys()
    {
        return numberOfKeys;
    }

    public void GenerateMaze(int rows, int cols, int numberOfKeys)
    {
        this.rows = rows;
        this.cols = cols;
        this.numberOfKeys = numberOfKeys;
        GenerateMatrix(rows, cols);
        int[] startCell = GetBorderStart();
        Debug.Log($"Start cell: {startCell[0]}, {startCell[1]}");
        BacktrackFillCells(startCell);
        InstantiateDoor(startCell);
        InstantiateKeys(startCell, numberOfKeys);
        InstantiatePlayer(startCell);
        FixMapCamera();
        InstantiateCeiling();
    }

    private void GenerateMatrix(int rows, int cols)
    {
        for (int i = 0; i < rows; i++)
        {
            matrix.Add(new List<Cell>());
            for (int j = 0; j < cols; j++)
            {
                int x = i * 4;
                int z = j * 4;
                matrix[i].Add(new Cell(cell, new Vector3(x, 0, z), new int[] {i, j}, cellsParent));
            }
        }
    }
    private int[] GetRandomStart()
    {
        return new int[] {UnityEngine.Random.Range(0, rows), UnityEngine.Random.Range(0, cols)};
    }

    private int[] GetBorderStart()
    {
        int[] coordinates;

        string[] borders = { "r", "c" };
        string chosenBorder = borders[Random.Range(0, borders.Length)];
        if (chosenBorder == "r")
        {
            int[] borderRow = { 0, rows - 1 };
            coordinates = new int[] { borderRow[Random.Range(0, borderRow.Length)], Random.Range(0, cols) };
        }
        else
        {
            int[] borderCol = { 0, cols - 1 };
            coordinates = new int[] { Random.Range(0, rows), borderCol[Random.Range(0, borderCol.Length)] };
        }

        return coordinates;
    }

    private void RemoveWallByName(GameObject cellObject, string wallName)
    {
        Transform child = cellObject.transform.Find(wallName);
        if (child != null)
        {
            child.gameObject.tag = "Removed";
            UnityEngine.Object.Destroy(child.gameObject);
        }
    }

    private void RemoveWall(Cell cell, string direction)
    {
        switch(direction)
        {
            case "U":
                RemoveWallByName(cell.gameObject, "UpWall");
                cell.vectors.Remove("U");
                break;
            case "D":
                RemoveWallByName(cell.gameObject, "DownWall");
                cell.vectors.Remove("D");
                break;
            case "L":
                RemoveWallByName(cell.gameObject, "LeftWall");
                cell.vectors.Remove("L");
                break;
            case "R":
                RemoveWallByName(cell.gameObject, "RightWall");
                cell.vectors.Remove("R");
                break;
        }
    }

    private void BacktrackFillCells(int[] start)
    {
        Cell cell = matrix[start[0]][start[1]];
        cell.visited = true;
        Cell nextCell;
        int[] nextCellCoordinates = {-1, -1};
        while (cell.vectors.Count > 0)
        {
            string vector = cell.SelectRandomVector();
            if (vector == "U")
            {
                nextCellCoordinates = new int[] {start[0] - 1, start[1]};
            }
            if (vector == "D")
            {
                nextCellCoordinates = new int[] {start[0] + 1, start[1]};
            }
            if (vector == "L")
            {
                nextCellCoordinates = new int[] {start[0], start[1] - 1};
            }
            if (vector == "R")
            {
                nextCellCoordinates = new int[] {start[0], start[1] + 1};
            }
            cell.vectors.Remove(vector);

            if (!(nextCellCoordinates[0] < 0 || nextCellCoordinates[0] >= rows || nextCellCoordinates[1] < 0 || nextCellCoordinates[1] >= cols))
            {
                nextCell = matrix[nextCellCoordinates[0]][nextCellCoordinates[1]];
                if (nextCell.visited == false)
                {
                    if (vector == "U")
                    {
                        RemoveWall(cell, "U");
                        RemoveWall(nextCell, "D");
                    }
                    if (vector == "D")
                    {
                        RemoveWall(cell, "D");
                        RemoveWall(nextCell, "U");
                    }
                    if (vector == "L")
                    {
                        RemoveWall(cell, "L");
                        RemoveWall(nextCell, "R");
                    }
                    if (vector == "R")
                    {
                        RemoveWall(cell, "R");
                        RemoveWall(nextCell, "L");
                    }
                    BacktrackFillCells(nextCellCoordinates);
                } else {
                    continue;
                }
            }
        }
        return;
    }

    private void InstantiateCeiling()
    {
        for (int i = 1; i < rows + 1; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                UnityEngine.GameObject.Instantiate(ceiling, new Vector3(j * 4, 4.4f , i * 4), Quaternion.identity, ceilingParent.GetComponent<Transform>());
            }
        }
    }


    private void InstantiateDoor(int[] start)
    {
        GameObject cell = GameObject.Find($"Cell {start[0]} {start[1]}");
        string nameOfWall;
        if (start[0] == 0)
        {
            nameOfWall = "UpWall";
        }
        else if (start[0] == rows - 1)
        {
            nameOfWall = "DownWall";
        }
        else if (start[1] == 0)
        {
            nameOfWall = "LeftWall";
        }
        else
        {
            nameOfWall = "RightWall";
        }
        var t = cell.transform.Find(nameOfWall);
        door = UnityEngine.GameObject.Instantiate(doorWall, t.position, t.rotation);
        RemoveWallByName(cell, nameOfWall);
    }

    public void InstantiateKeys(int[] start, int numberOfKeys)
    {
        Dictionary<(int, int), float> distances = new Dictionary<(int, int), float>();
        List<(int, int)> selectedPositions = new List<(int, int)> ();
        List<Color> availableColors = new List<Color>() { Color.red, Color.blue, Color.green, Color.cyan, Color.magenta };

        float minDistanceBetweenKeys = Mathf.Sqrt(rows * cols) / 2f;
        Debug.Log($"Min distance between keys = {minDistanceBetweenKeys}");

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                float distance = Mathf.Sqrt(Mathf.Pow(i - start[0], 2) + Mathf.Pow(j - start[1], 2));
                distances.Add((i, j), distance);
            }
        }
        var sortedCells = distances.OrderByDescending(x  => x.Value).ToList();

        foreach (var cell in sortedCells)
        {
            if (selectedPositions.Count >= numberOfKeys) break;

            var pos = cell.Key;
            bool isFarEnough = true;

            foreach (var selectedPos in selectedPositions)
            {
                float distance = Mathf.Sqrt(Mathf.Pow(pos.Item1 - selectedPos.Item1, 2) + Mathf.Pow(pos.Item2 - selectedPos.Item2, 2));
                if (distance < minDistanceBetweenKeys * 1.5f)
                {
                    isFarEnough = false;
                    break;
                }
            }

            float distanceFromStart = Mathf.Sqrt(Mathf.Pow(pos.Item1 - start[0], 2) + Mathf.Pow(pos.Item2 - start[1], 2));

            if (distanceFromStart < minDistanceBetweenKeys) isFarEnough = false;

            if (selectedPositions.Count > 0)
            {
                foreach (var selectedPos in selectedPositions)
                {
                    if (pos.Item1 == selectedPos.Item1 || pos.Item2 == selectedPos.Item2) isFarEnough = false;
                }
            }

            if (isFarEnough)
            {
                selectedPositions.Add(pos);
                Vector3 keyPosition = new Vector3((pos.Item1 * 4) + 2, 1, (pos.Item2 * 4) + 2);
                var key = UnityEngine.GameObject.Instantiate(keyPrefab, keyPosition, Quaternion.identity);
                Color color = availableColors[Random.Range(0, availableColors.Count)];
                availableColors.Remove(color);
                SetKeyColors(key, color);
                Debug.Log($"Key placed at: {pos.Item1}, {pos.Item2}. Color: {color.GetHashCode()}");
            }
        }
    }

    private void SetKeyColors(GameObject key, Color color)
    {
        Transform sphere = key.transform.Find("MapSphere");
        Transform light = key.transform.Find("Highlight");
        if (sphere != null)
        {
            Renderer renderer = sphere.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
            else
            {
                Debug.LogError("Renderer or Material is missing on the GameObject: " + key.name);
            }
        }
        if (light != null)
        {
            Light l = light.GetComponent<Light>();
            l.color = color;
        }
    }

    private void InstantiatePlayer(int[] start)
    {
        Vector3 position = new Vector3((start[0] * 4) + 2, 1, (start[1] * 4) + 2);
        if (door == null)
        {
            UnityEngine.GameObject.Instantiate(player, position, Quaternion.identity);
        }
        else
        {
            Vector3 doorPosition = door.transform.position;
            UnityEngine.GameObject.Instantiate(player, position, Quaternion.LookRotation(- new Vector3(position.x - doorPosition.x, 0, position.z - doorPosition.z)));
        }
    }

    private void FixMapCamera() 
    {
        int coordinates = rows + cols;
        mapCamera.transform.position = new Vector3(coordinates, coordinates, coordinates);
        mapCamera.orthographicSize = coordinates + 1;
    }

    void Start()
    {
        //GenerateMatrix(rows, cols);
        //int[] startCell = GetBorderStart();
        //Debug.Log($"Start cell: {startCell[0]}, {startCell[1]}");
        //BacktrackFillCells(startCell);
        //InstantiateDoor(startCell);
        //InstantiateKeys(startCell, numberOfKeys);
        //InstantiatePlayer(startCell);
        //FixMapCamera();
        //InstantiateCeiling();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class Cell
{
    public bool visited = false;
    public GameObject gameObject;
    private Vector3 position;

    public List<string> vectors = new List<string> {"U", "D", "L", "R"};

    public Cell(GameObject prefab, Vector3 position, int[] number, GameObject parent)
    {
        this.position = position;
        this.gameObject = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity, parent.GetComponent<Transform>());
        this.gameObject.name = $"Cell {number[0]} {number[1]}";
    }
    public string SelectRandomVector()
    {
        return vectors[UnityEngine.Random.Range(0, vectors.Count)];
    }
    
}
