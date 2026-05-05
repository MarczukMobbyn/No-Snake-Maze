using StarterAssets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SnakeMovement : MonoBehaviour
{
    public Transform[] segments;           // Przeci¹gnij segmenty w Inspectorze
    public float followDistance = 0.5f;    // Dystans miêdzy segmentami
    public float smoothSpeed = 10f;        // P³ynnoœæ pod¹¿ania

    [SerializeField] float WaitTime = 0.5f; // How much time to wait at the beginning of game
    float timePassed = 0;

    private List<Vector3> positionHistory = new List<Vector3>();

    private NavMeshAgent agent;
    Transform player;

    [SerializeField] Transform headBone; // przeci¹gnij w Inspectorze
    [SerializeField] Vector3 headRotationOffset = new Vector3(145.491f, 0, 0); // lub dopasuj rêcznie

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        positionHistory.Add(transform.position); // Pocz¹tkowa pozycja
        player = FindAnyObjectByType<FirstPersonController>().GetComponent<Transform>();
       
        agent.updateUpAxis = false; // Wy³¹cz aktualizacjê osi Y, jeœli nie jest potrzebna


    }

    void Update()
    {
        if (timePassed < WaitTime)
        {
            timePassed += Time.deltaTime;
            return; // Czekaj, a¿ up³ynie czas oczekiwania
        }
        //FollowPlayer();
        MoveBodyParts();
    }

    void LateUpdate()
    {
        if (headBone == null || agent == null)
            return;

        Vector3 direction = agent.velocity;

        if (direction.sqrMagnitude > 0.001f)
        {
            // Obróæ g³owê w kierunku ruchu
            Quaternion targetRot = Quaternion.LookRotation(direction.normalized, Vector3.up);

            // Dodaj poprawkê modelu, np. jeœli g³owa mia³a defaultowo 145 stopni
            Quaternion correctionOffset = Quaternion.Euler(headRotationOffset);

            // Ustaw rotacjê globalnie lub lokalnie, zale¿nie od sytuacji
            headBone.rotation = Quaternion.Lerp(headBone.rotation,targetRot * correctionOffset, 20f * Time.deltaTime);

            // Jeœli model dalej patrzy nie tak: zamieñ kolejnoœæ mno¿enia
            // headBone.rotation = correctionOffset * targetRot;
        }
    }

    void FollowPlayer()
    {
        if (agent != null)
        {
            // Ustaw cel agenta na pozycjê g³owy wê¿a
            agent.SetDestination(player.position);
        }
        else
        {
            Debug.LogWarning("NavMeshAgent not found on the snake object.");
        }
    }

    void MoveBodyParts()
    {
        // Zapisujemy pozycjê g³owy do historii
        if (Vector3.Distance(transform.position, positionHistory[0]) > 0.1f)
        {
            positionHistory.Insert(0, transform.position);
        }

        for (int i = 0; i < segments.Length; i++)
        {
            int index = Mathf.Min((i + 1) * Mathf.RoundToInt(followDistance * 10f), positionHistory.Count - 1);
            Vector3 targetPos = positionHistory[index];

            segments[i].position = Vector3.Lerp(segments[i].position, targetPos, Time.deltaTime * smoothSpeed);

            Vector3 direction;

            if (i == 0)
            {
                direction = transform.position - segments[i].position;
            }
            else
            {
                direction = segments[i - 1].position - segments[i].position;
            }

            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
                segments[i].rotation = lookRotation * Quaternion.Euler(90, 0, 0);
            }
        }

        // Ogranicz wielkoœæ historii
        if (positionHistory.Count > 1000)
        {
            positionHistory.RemoveAt(positionHistory.Count - 1);
        }
    }
}
