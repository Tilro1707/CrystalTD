using UnityEngine;

public class Enemy : MonoBehaviour
{
    private WaveManager waveManager;

    [SerializeField] private float health = 10f;
    [SerializeField] private Transform[] waypoints;

    private BeatManager beatManager;
    private bool isFrozen;
    private bool isMoving;
    private Vector2 targetPos;

    [SerializeField] private float moveSpeed = 8f;

    private double nextFreezeAllowed;
    private double freezeEnd;
    private int currentWaypointIndex;
    private bool isDead;

    [SerializeField] private int beatsPerMove = 1;
    [SerializeField] private int stepsPerMove = 1;

    [SerializeField] private GameObject rootVisual;

    private int beatCounter;

    private void Start()
    {
        if (rootVisual != null)
        {
            rootVisual.SetActive(false);
        }

        beatManager = FindFirstObjectByType<BeatManager>();

        if (beatManager != null)
        {
            beatManager.OnBeat += HandleBeat;
        }
    }

    private void Update()
    {
        if (isFrozen && AudioSettings.dspTime >= freezeEnd)
        {
            isFrozen = false;

            if (rootVisual != null)
            {
                rootVisual.SetActive(false);
            }
        }

        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );

            if (Vector2.Distance(transform.position, targetPos) < 0.02f)
            {
                transform.position = targetPos;
                isMoving = false;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        health -= amount;

        if (health <= 0f)
        {
            AudioController.Instance.PlayEnemyDeath();
            Die();
        }
    }

    private void HandleBeat()
    {
        if (isFrozen || isDead)
            return;

        beatCounter++;

        if (beatCounter >= beatsPerMove)
        {
            beatCounter = 0;
            int nextIndex = currentWaypointIndex + stepsPerMove;

            if (nextIndex < waypoints.Length)
            {
                currentWaypointIndex = nextIndex;
                targetPos = waypoints[currentWaypointIndex].position;
                isMoving = true;
            }
            else
            {
                waveManager.LoseLife();
                Die();
            }
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        waveManager.EnemyDied();
        Destroy(gameObject);
    }

    public bool Freeze(double beatLength, int cooldownBeats)
    {
        if (isDead || AudioSettings.dspTime < nextFreezeAllowed)
            return false;

        isFrozen = true;
        freezeEnd = AudioSettings.dspTime + beatLength;
        nextFreezeAllowed = AudioSettings.dspTime + cooldownBeats * beatLength;

        if (rootVisual != null)
        {
            rootVisual.SetActive(true);
        }

        return true;
    }

    public void SetWaypoints(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
        currentWaypointIndex = 0;
        transform.position = waypoints[0].position;
    }

    public void SetWaveManager(WaveManager manager)
    {
        waveManager = manager;
    }

    private void OnDestroy()
    {
        if (beatManager != null)
        {
            beatManager.OnBeat -= HandleBeat;
        }
    }

    public void ScaleHealth(float multiplier)
    {
        health *= multiplier;
    }
}
