using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] int width = 10; // Width of the maze
    [SerializeField] int height = 10; // Height of the maze
    [SerializeField] GameObject[] fieldPrefabs; // Prefabs for the field
    [SerializeField] int randomWallsDeleted = 60; // Number of random walls to delete after maze generation

    [SerializeField] GameObject leverPrefab;
    [SerializeField] int leverCount = 3;
    [SerializeField] GameObject hatchPrefab; // Prefab for the hatch that will be spawned after levers are pulled
    [SerializeField] float minDistance = 15f; // Minimum distance between levers to avoid overlap

    public MazeFieldObject[,] mazeFields; // 2D array to hold the maze fields

    [SerializeField] int scale = 5; // Scale of the maze, can be useful for larger mazes

    [SerializeField] WinManager winManager; // Reference to the WinManager to track lever pulls
    [SerializeField] SnakeSpawner snakeSpawner; // Reference to the SnakeSpawner to spawn snakes
    [SerializeField] GameTimer gameTimer; // Reference to the GameTimer to track time
    [SerializeField] SceneManagerObject sceneManager; // Reference to the SceneManagerObject to manage scenes

    public NavMeshSurface surface;


    private void Awake()
    {
        GenerateMaze();
        DepthFirstSearch(0, 0); // Start DFS from the Bottom-Left corner of the maze
        DeleteRandomWallsBetweenRooms(randomWallsDeleted); // Delete random walls between rooms
        ScaleMaze();
        SpawnLeversInRandomFields(); // Spawn levers in random fields

        CombineAllFloorsIntoOneMesh();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(BuildNavMeshDelayed());
    }

    IEnumerator BuildNavMeshDelayed()
    {
        yield return null; // jedna klatka opóŸnienia
        surface.BuildNavMesh();
        Debug.Log("NavMesh zbudowany!");
    }

    void GenerateMaze()
    {
        mazeFields = new MazeFieldObject[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Randomly select a prefab from the array
                GameObject fieldPrefab = fieldPrefabs[Random.Range(0, fieldPrefabs.Length)];
                // Instantiate the prefab at the correct position
                Vector3 position = new Vector3(x, 0, y);
                mazeFields[x, y] = Instantiate(fieldPrefab, position, Quaternion.identity, transform).GetComponent<MazeFieldObject>();
            }
        }
    }


    void DepthFirstSearch(int startX, int startY)
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, List<Vector2Int>> unvisitedNeighboursMap = new Dictionary<Vector2Int, List<Vector2Int>>();

        Vector2Int start = new Vector2Int(startX, startY);
        stack.Push(start);
        visited.Add(start);
        unvisitedNeighboursMap[start] = GetShuffledNeighbours(start, visited);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();

            if (unvisitedNeighboursMap[current].Count == 0)
            {
                // no more unvisited neighbours, backtrack
                stack.Pop();
                continue;
            }

            // we have unvisited neighbours, so we can continue
            Vector2Int next = unvisitedNeighboursMap[current][0];
            unvisitedNeighboursMap[current].RemoveAt(0);

            if (!visited.Contains(next))
            {
                // delete the wall between current and next
                RemoveWalls(current, next);

                // mark next as visited
                visited.Add(next);

                // add next to the map of unvisited neighbours
                unvisitedNeighboursMap[next] = GetShuffledNeighbours(next, visited);

                stack.Push(next);
            }
        }

        Debug.Log($"Maze complete. Visited {visited.Count}/{width * height} fields.");
    }


    List<Vector2Int> GetShuffledNeighbours(Vector2Int cell, HashSet<Vector2Int> visited)
    {
        List<Vector2Int> directions = new List<Vector2Int>
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

        List<Vector2Int> neighbours = new List<Vector2Int>();
        foreach (var dir in directions)
        {
            Vector2Int neighbour = cell + dir;
            if (neighbour.x >= 0 && neighbour.x < width &&
                neighbour.y >= 0 && neighbour.y < height &&
                !visited.Contains(neighbour))
            {
                neighbours.Add(neighbour);
            }
        }

        Shuffle(neighbours);
        return neighbours;
    }

    public Vector3 GetRandomSpawnPoint()
    {
        // Get a random position within the maze bounds
        int x = Random.Range(0, width);
        int y = Random.Range(0, height);
        // Return the position of the field at (x, y)
        if (mazeFields[x, y] != null)
        {
            return mazeFields[x, y].transform.position;
        }
        else
        {
            Debug.LogWarning($"Maze field at ({x}, {y}) is null!");
            return Vector3.zero; // Return zero vector if the field is null
        }
    }


    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }


    void RemoveWalls(Vector2Int a, Vector2Int b)
    {
        int dx = b.x - a.x;
        int dy = b.y - a.y;

        if (dx == 1)
        {
            if (mazeFields[a.x, a.y].RightWall != null)
            {
                Destroy(mazeFields[a.x, a.y].RightWall);
                Destroy(mazeFields[b.x, b.y].LeftWall);
            }
        }
        else if (dx == -1)
        {
            if (mazeFields[a.x, a.y].LeftWall != null)
            {
                Destroy(mazeFields[a.x, a.y].LeftWall);
                Destroy(mazeFields[b.x, b.y].RightWall);
            }
        }
        else if (dy == 1)
        {
            if(mazeFields[a.x, a.y].TopWall != null)
            {
                Destroy(mazeFields[a.x, a.y].TopWall);
                Destroy(mazeFields[b.x, b.y].BottomWall);
            }
        }
        else if (dy == -1)
        {
            if(mazeFields[a.x, a.y].BottomWall != null)
            {
                Destroy(mazeFields[a.x, a.y].BottomWall);
                Destroy(mazeFields[b.x, b.y].TopWall);
            }
        }
    }

    void SpawnLeversInRandomFields()
    {
         
        List<Vector3> leverWorldPositions = new List<Vector3>();
        HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();

        int attemptsPerLever = 100; // limit prób na znalezienie sensownej pozycji

        for (int i = 0; i < leverCount; i++)
        {
            Vector2Int pos = default;
            MazeFieldObject field = null;
            Vector3 spawnPos = Vector3.zero;
            bool found = false;

            for (int attempt = 0; attempt < attemptsPerLever; attempt++)
            {
                pos = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
                if (usedPositions.Contains(pos))
                    continue;

                field = mazeFields[pos.x, pos.y];
                if (field == null)
                    continue;

                spawnPos = field.transform.position;

                bool tooClose = false;
                foreach (var existing in leverWorldPositions)
                {
                    if (Vector3.Distance(existing, spawnPos) < minDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (tooClose)
                    continue;

                // akceptujemy tê pozycjê
                found = true;
                break;
            }

            if (!found)
            {
                Debug.LogWarning($"Nie uda³o siê znaleŸæ pola dla dŸwigni {i} spe³niaj¹cego odstêp, losujê bez ograniczeñ.");
                // fallback: szukaj jedynie unikalnego bez sprawdzania dystansu
                do
                {
                    pos = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
                } while (usedPositions.Contains(pos));

                field = mazeFields[pos.x, pos.y];
                if (field != null)
                    spawnPos = field.transform.position;
            }

            if (field == null)
            {
                Debug.LogWarning($"Pole [{pos.x},{pos.y}] jest null!");
                continue;
            }

            usedPositions.Add(pos);
            leverWorldPositions.Add(spawnPos);

            GameObject lever = Instantiate(leverPrefab, spawnPos, Quaternion.identity, field.transform);
            lever.GetComponent<LeverInteract>().Init(winManager, snakeSpawner);
            Debug.Log($"DŸwignia zespawnowana na polu [{pos.x}, {pos.y}] w pozycji {spawnPos}", lever);
        }

        Debug.Log($"{leverCount} dŸwignie zespawnowane.");

        //spawn the hatch
        Vector2Int hatchPos;

        // Szukaj unikalnej pozycji
        do
        {
            hatchPos = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        }
        while (usedPositions.Contains(hatchPos));

        MazeFieldObject hatchField = mazeFields[hatchPos.x, hatchPos.y];
        if (hatchField == null)
        {
            Debug.LogWarning($"Pole [{hatchPos.x},{hatchPos.y}] jest null!");
        }

        Vector3 hatchSpawnPos = hatchField.transform.position + new Vector3(0, 0.3f, 0);
        GameObject hatch = Instantiate(hatchPrefab, hatchField.transform);
        hatch.GetComponent<HatchInteract>().Init(snakeSpawner, gameTimer, sceneManager);
        hatch.transform.position = hatchSpawnPos;
        Debug.Log($"DŸwignia zespawnowana na polu [{hatchPos.x}, {hatchPos.y}] w pozycji {hatchSpawnPos}", hatch);
    }


    void DeleteRandomWallsBetweenRooms(int wallsToDelete)
    {
        // Lista wszystkich mo¿liwych œcian miêdzy s¹siaduj¹cymi polami
        List<(Vector2Int, Vector2Int)> wallPairs = new List<(Vector2Int, Vector2Int)>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int current = new Vector2Int(x, y);

                // Prawo
                if (x < width - 1)
                    wallPairs.Add((current, new Vector2Int(x + 1, y)));
                // Góra
                if (y < height - 1)
                    wallPairs.Add((current, new Vector2Int(x, y + 1)));
            }
        }

        // Tasujemy listê œcian
        Shuffle(wallPairs);

        // Usuwamy losowe œciany (maksymalnie wallsToDelete lub tyle ile jest dostêpnych)
        int count = Mathf.Min(wallsToDelete, wallPairs.Count);
        for (int i = 0; i < count; i++)
        {
            var pair = wallPairs[i];
            RemoveWalls(pair.Item1, pair.Item2);
        }
    }

    void ScaleMaze()
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    void CombineAllFloorsIntoOneMesh()
    {
        List<MeshFilter> meshFilters = new List<MeshFilter>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var field = mazeFields[x, y];
                if (field == null) continue;

                Transform floorTransform = field.transform.Find("Model/Floor");
                if (floorTransform == null)
                {
                    Debug.LogWarning($"Brak 'Model/Floor' w polu [{x},{y}]");
                    continue;
                }

                MeshFilter meshFilter = floorTransform.GetComponent<MeshFilter>();
                if (meshFilter == null)
                {
                    Debug.LogWarning($"Brak MeshFiltera w 'Floor' [{x},{y}]");
                    continue;
                }

                meshFilters.Add(meshFilter);
            }
        }

        if (meshFilters.Count == 0)
        {
            Debug.LogError(" Nie znaleziono ¿adnych MeshFilterów pod³ogi!");
            return;
        }

        CombineInstance[] combine = new CombineInstance[meshFilters.Count];
        for (int i = 0; i < meshFilters.Count; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        GameObject combinedFloor = new GameObject("CombinedFloor");
        combinedFloor.transform.parent = this.transform;

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);

        MeshFilter mf = combinedFloor.AddComponent<MeshFilter>();
        mf.mesh = combinedMesh;

        MeshRenderer mr = combinedFloor.AddComponent<MeshRenderer>();
        mr.material = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;

        MeshCollider mc = combinedFloor.AddComponent<MeshCollider>();
        mc.sharedMesh = combinedMesh;

        combinedFloor.layer = LayerMask.NameToLayer("Walkable");

        for (int i = 0; i < meshFilters.Count; i++)
        {
            Destroy(meshFilters[i].gameObject);
        }

        Debug.Log($"Po³¹czono {meshFilters.Count} pod³óg w jedn¹ siatkê.");
    }


}
