using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using StarterAssets;

public class SnakeAI : MonoBehaviour
{
    public enum SnakeState { Patrol, Alert, Chase, Lost }
    public SnakeState currentState = SnakeState.Patrol;

    public AudioSource miBombo, snakeRattle, alarm;

    public Transform player;
    public float viewDistance = 15f;
    public float viewAngle = 60f;
    public float interactsHearingDistance = 10f;
    public float playerHearingDistance = 30f;

    public float patrolWaitTime = 2f;
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    private NavMeshAgent agent;
    private float lostTimer = 0f;
    private float lostTimeLimit = 5f;

    private Vector3 alertTarget; // cel do którego w¹¿ idzie w Alert
    private bool reachedAlertTarget = false;

    [Header("Patrol Points Generation")]
    public GameObject patrolPointPrefab;  // Prefab patrol pointa (np. pusty GameObject z gizmem)
    public int patrolCount = 5;

    [Header("Camera Shake")]
    [SerializeField] CinemachineVirtualCamera vcam; // przypisz w Inspectorze
    [SerializeField] NoiseSettings chaseNoiseProfile; // np. 6D Wobble
    [SerializeField] NoiseSettings defaultNoiseProfile; // Handheld_normal_mild
    [SerializeField] float chaseAmplitude = 2f;
    [SerializeField] float chaseFrequency = 2f;
    [SerializeField] float shakeTransitionSpeed = 2f; // jak szybko wchodzi/wychodzi

    private CinemachineBasicMultiChannelPerlin noise;
    private bool isChasingLastFrame = false;

    private bool isWaiting = false;

    [HideInInspector] public MazeFieldObject[,] mazeFields;

    [SerializeField] Transform eyes;
    FirstPersonController playerController;

    bool hasJustLostChase = false;
    float afterChaseTimer = 0f;
    float afterChaseTime = 4f;


    void Start()
    {
        vcam = FindFirstObjectByType<CinemachineVirtualCamera>();
        SetupCameraNoise();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
        playerController = player.gameObject.GetComponentInParent<FirstPersonController>();

    }
    public void Initialize(MazeFieldObject[,] fields)
    {
        mazeFields = fields;
        GeneratePatrolPoints(mazeFields);

        foreach (var p in patrolPoints)
        {
            Debug.Log("Patrol point: " + (p == null ? "null" : p.name));
        }

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            foreach (var point in patrolPoints)
            {
                if (point == null)
                {
                    Debug.LogError("[SnakeAI] Jeden z patrolPoints jest null!");
                    return;
                }
            }

            currentPatrolIndex = 0;
            GoToNextPatrolPoint();
        }
        else
        {
            Debug.LogError("[SnakeAI] patrolPoints[] jest puste po GeneratePatrolPoints()");
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case SnakeState.Patrol:
                if (patrolPoints == null || patrolPoints.Length == 0)
                {
                    Debug.LogError("[SnakeAI] patrolPoints[] is null or empty, cannot patrol.");
                    return;
                }
                Patrol();
                break;
            case SnakeState.Alert:
                Alert();
                break;
            case SnakeState.Chase:
                Chase();
                break;
            case SnakeState.Lost:
                Lost();
                break;
        }
        
        CameraShake();
        AlertByPlayerSprint();
        PlayChaseSounds();
        //Debug.Log(CanSeePlayer() ? "Player is visible" : "Player is not visible");
        Debug.Log("current state: " + currentState);
        Debug.Log("after chase timer: " + afterChaseTimer); 
        Debug.Log("Has just lost chase: " + hasJustLostChase);
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !isWaiting)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                StartCoroutine(WaitAndMove(patrolWaitTime));
            }
        }

        if (CanSeePlayer())
        {
            Debug.Log("[SnakeAI] Player detected! Switching to Chase.");
            currentState = SnakeState.Chase;
        }
    }



    IEnumerator WaitAndMove(float wait)
    {
        isWaiting = true;
        yield return new WaitForSeconds(wait);
        GoToNextPatrolPoint();
        isWaiting = false;
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogWarning("[SnakeAI] patrolPoints[] is null or empty");
            return;
        }

        if (patrolPoints[currentPatrolIndex] == null)
        {
            Debug.LogError($"[SnakeAI] patrolPoints[{currentPatrolIndex}] is null!");
            return;
        }

        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void Alert()
    {
        if (CanSeePlayer())
        {
            currentState = SnakeState.Chase;
            return;
        }

        // Jeœli nie doszed³ jeszcze do celu
        if (!reachedAlertTarget)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                reachedAlertTarget = true;
                StartCoroutine(WaitAndReturnToPatrol(1.5f)); // chwilka czuwania
            }
        }
    }

    IEnumerator WaitAndReturnToPatrol(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        currentState = SnakeState.Patrol;
        GoToNextPatrolPoint();
    }

    void Chase()
    {
        agent.destination = player.position;

        if (!CanSeePlayer())
        {
            lostTimer += Time.deltaTime;
            if (lostTimer > lostTimeLimit)
            {
                lostTimer = 0f;
                currentState = SnakeState.Lost;
            }
        }
        else
        {
            lostTimer = 0f;
        }
    }

    void Lost()
    {
        hasJustLostChase = true;
        afterChaseTimer = 0f;
        // go back to patrolling
        currentState = SnakeState.Patrol;
        GoToNextPatrolPoint();
    }

    public bool CanSeePlayer()
    {
        Vector3 dirToPlayer = (player.position + Vector3.up * 0.5f) - (transform.position + Vector3.up * 0.5f);
        float distance = dirToPlayer.magnitude;

        if (distance > viewDistance) return false;

        Debug.DrawRay(eyes.position, eyes.forward * viewDistance, Color.green, 1f);

        float angle = Vector3.Angle(eyes.forward, dirToPlayer);
        if (angle < viewAngle / 2f)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dirToPlayer.normalized, out RaycastHit hit, viewDistance))
            {
                Debug.DrawRay(transform.position + Vector3.up * 0.5f, dirToPlayer.normalized * viewDistance, Color.red, 1f);
                if (hit.collider.CompareTag("Player"))
                {
                    //Debug.Log("[SnakeAI] Player detected via raycast!");
                    return true;
                }
            }
        }
        return false;
    }

    void AlertByPlayerSprint()
    {
        afterChaseTimer += Time.deltaTime;
        if (hasJustLostChase && afterChaseTimer < afterChaseTime)
        {
            // jeœli w³aœnie straci³ chase, ale nie min¹³ czas na powrót do patrolu
            return;
        }

        hasJustLostChase = false;
        if (playerController != null && playerController.IsSprinting() && currentState != SnakeState.Chase)
        {
            // jeœli gracz biega, a w¹¿ nie jest w stanie Chase ani Lost
            if (Vector3.Distance(transform.position, player.position) < playerHearingDistance)
            {
                currentState = SnakeState.Alert;
                alertTarget = player.position;
                reachedAlertTarget = false;
                agent.SetDestination(alertTarget);
            }
        }
    }

    public void HearNoise(Vector3 noisePos)
    {
        if (Vector3.Distance(transform.position, noisePos) < interactsHearingDistance)
        {
            currentState = SnakeState.Alert;
            alertTarget = noisePos;
            reachedAlertTarget = false;
            agent.SetDestination(alertTarget);
        }
    }

    public void SetMazeFields(MazeFieldObject[,] fields)
    {
        mazeFields = fields;
    }

    public void GeneratePatrolPoints(MazeFieldObject[,] mazeFields)
    {
        patrolPoints = new Transform[patrolCount];

        int width = mazeFields.GetLength(0);
        int height = mazeFields.GetLength(1);

        int created = 0;
        int attempts = 0;

        while (created < patrolCount && attempts < 1000)
        {
            attempts++;

            // losuj losowe pole w labiryncie
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            MazeFieldObject field = mazeFields[x, y];

            Vector3 center = field.transform.position;

            // sprawdŸ czy pozycja jest na NavMeshu
            if (NavMesh.SamplePosition(center, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                GameObject point = Instantiate(patrolPointPrefab, hit.position, Quaternion.identity);
                point.name = $"SnakePatrolPoint_{created}";
                patrolPoints[created] = point.transform;
                created++;
            }
        }

        if (created < patrolCount)
        {
            Debug.LogWarning($"[SnakeAI] Nie uda³o siê stworzyæ wszystkich punktów patrolowych ({created}/{patrolCount})");
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

    void PlayChaseSounds()
    {
        if (currentState == SnakeState.Chase)
        {
            if (miBombo != null && !miBombo.isPlaying)
            {
                miBombo.Play();
            }
            if (snakeRattle != null && !snakeRattle.isPlaying)
            {
                snakeRattle.Play();
            }
            if (alarm != null && !alarm.isPlaying)
            {
                alarm.Play();
            }
        }
        else
        {
            miBombo?.Stop();
            snakeRattle?.Stop();
            alarm?.Stop();
        }
    }

    void CameraShake()
    {
        bool nowChasing = currentState == SnakeState.Chase;

        if (noise != null)
        {
            // jeœli zaczêto chase i wczeœniej nie by³o
            if (nowChasing && !isChasingLastFrame)
            {
                noise.m_NoiseProfile = chaseNoiseProfile; // zmieñ profil
            }
            // jeœli zakoñczono chase
            else if (!nowChasing && isChasingLastFrame)
            {
                noise.m_NoiseProfile = defaultNoiseProfile; // przywróæ
            }

            float targetAmp = nowChasing ? chaseAmplitude : 0f;
            float targetFreq = nowChasing ? chaseFrequency : 0f;

            noise.m_AmplitudeGain = Mathf.MoveTowards(noise.m_AmplitudeGain, targetAmp, shakeTransitionSpeed * Time.deltaTime);
            noise.m_FrequencyGain = Mathf.MoveTowards(noise.m_FrequencyGain, targetFreq, shakeTransitionSpeed * Time.deltaTime);
        }

        isChasingLastFrame = nowChasing;
    }

    void SetupCameraNoise()
    {
        if (noise != null) return;

#if UNITY_2023_1_OR_NEWER
        if (vcam == null) vcam = FindFirstObjectByType<CinemachineVirtualCamera>();
#else
    if (vcam == null) vcam = FindObjectOfType<CinemachineVirtualCamera>();
#endif

        if (vcam != null)
        {
            noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise != null)
            {
                // ustaw domyœlny profil i wyzeruj gainy
                defaultNoiseProfile = noise.m_NoiseProfile;// ustaw domyœlny profil
                noise.m_AmplitudeGain = 0f;
                noise.m_FrequencyGain = 0f;
            }
        }
    }


}
