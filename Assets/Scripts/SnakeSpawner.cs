using UnityEngine;

public class SnakeSpawner : MonoBehaviour
{
    public GameObject snakePrefab;
    public Vector3 spawnPoint;
    private GameObject snakeInstance;
    public Transform player;

    MazeGenerator mazeGenerator;
    [HideInInspector] public MazeFieldObject[,] mazeFields;

    [SerializeField] int numOfTriesToSpawnSnake = 100;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        mazeGenerator = FindFirstObjectByType<MazeGenerator>();
        if (mazeGenerator != null)
        {
            mazeFields = mazeGenerator.mazeFields;
            spawnPoint = mazeGenerator.GetRandomSpawnPoint();
            while(player != null && Vector3.Distance(player.position, spawnPoint) < 20f)
            {
                numOfTriesToSpawnSnake--;
                spawnPoint = mazeGenerator.GetRandomSpawnPoint();
                if (numOfTriesToSpawnSnake <= 0)
                {
                    Debug.LogWarning("Cannot find a place to spawnSnake, spawnin at 0,0,0");
                    spawnPoint = Vector3.zero; // Fallback spawn point
                    break;
                }
            }
        }
        else
        {
            Debug.LogError("MazeGenerator not found in the scene.");
        }
    }

    public void SpawnSnake()
    {
        if (snakeInstance == null)
        {
            snakeInstance = Instantiate(snakePrefab, spawnPoint, Quaternion.identity);

            var ai = snakeInstance.GetComponentInChildren<SnakeAI>();

            ai.Initialize(mazeFields);

            Debug.Log("W¹¿ zespawnowany.");
        }
    }

    public void EmitNoise(Vector3 pos)
    {
        if (snakeInstance != null)
        {
            snakeInstance.GetComponentInChildren<SnakeAI>()?.HearNoise(pos);
            Debug.Log("W¹¿ us³ysza³ ha³as w pozycji: " + pos);
        }
    }
}
