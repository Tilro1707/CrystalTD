using System.Collections;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private GameObject nextWaveButton;
    [SerializeField] private GameObject normalEnemyPrefab;
    [SerializeField] private GameObject fastEnemyPrefab;
    [SerializeField] private GameObject tankEnemyPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private RewardManager rewardManager;
    [SerializeField] private BeatManager beatManager;

    [SerializeField] private Transform[] waypoints;

    [SerializeField] private int currentWave = 0;
    [SerializeField] private int enemiesToSpawn;
    [SerializeField] private int baseEnemies = 5;

    [SerializeField] private TMP_Text waveText;
    [SerializeField] private int maxWaves = 10;

    [SerializeField] private int lives = 10;
    [SerializeField] private TMP_Text livesText;

    [SerializeField] private GameObject WinPanel;
    [SerializeField] private GameObject GameOverPanel;

    private bool isBuildPhase;
    private int enemiesAlive;
    private bool isWaveRunning;
    private bool allEnemiesSpawned;
    private bool gameEnded;

    public bool IsBuildPhase => isBuildPhase && !gameEnded;

    private void Start()
    {
        enemiesToSpawn = baseEnemies;
        UpdateWaveUI();

        isBuildPhase = true;
        nextWaveButton.SetActive(false);

        livesText.text = $"Lives: {lives}";
        WinPanel.SetActive(false);
        GameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (isWaveRunning && allEnemiesSpawned && enemiesAlive <= 0)
        {
            EndWave();
        }
    }

    private void StartWave()
    {
        isWaveRunning = true;
        isBuildPhase = false;
        allEnemiesSpawned = false;

        nextWaveButton.SetActive(false);
        AudioController.Instance.PlayWaveStart();
        StartCoroutine(SpawnWave());
    }

    private void SpawnEnemy()
    {
        GameObject prefabToSpawn = GetEnemyPrefab();

        GameObject enemyObject = Instantiate(
            prefabToSpawn,
            spawnPoint.position,
            Quaternion.identity
        );

        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemy.SetWaypoints(waypoints);
        enemy.SetWaveManager(this);

        float healthMultiplier = 1f + (currentWave - 1) * 0.05f;
        enemy.ScaleHealth(healthMultiplier);

        enemiesAlive++;
    }

    private GameObject GetEnemyPrefab()
    {
        int roll = Random.Range(0, 100);

        switch (currentWave)
        {
            case 1:
                return normalEnemyPrefab;

            case 2:
                return roll < 80 ? normalEnemyPrefab : fastEnemyPrefab;

            case 3:
                return roll < 65 ? normalEnemyPrefab : fastEnemyPrefab;

            case 4:
                if (roll < 60) return normalEnemyPrefab;
                if (roll < 90) return fastEnemyPrefab;
                return tankEnemyPrefab;

            case 5:
                if (roll < 50) return normalEnemyPrefab;
                if (roll < 80) return fastEnemyPrefab;
                return tankEnemyPrefab;

            case 6:
                if (roll < 40) return normalEnemyPrefab;
                if (roll < 75) return fastEnemyPrefab;
                return tankEnemyPrefab;

            case 7:
                if (roll < 35) return normalEnemyPrefab;
                if (roll < 70) return fastEnemyPrefab;
                return tankEnemyPrefab;

            case 8:
                if (roll < 30) return normalEnemyPrefab;
                if (roll < 65) return fastEnemyPrefab;
                return tankEnemyPrefab;

            case 9:
                if (roll < 25) return normalEnemyPrefab;
                if (roll < 60) return fastEnemyPrefab;
                return tankEnemyPrefab;

            case 10:
                if (roll < 20) return normalEnemyPrefab;
                if (roll < 55) return fastEnemyPrefab;
                return tankEnemyPrefab;
        }

        return normalEnemyPrefab;
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (gameEnded)
                yield break;

            SpawnEnemy();
            int beatDelay = Random.Range(1, 4);

            yield return new WaitForSeconds(beatDelay * beatManager.BeatLength);
        }

        allEnemiesSpawned = true;
    }

    public void EnemyDied()
    {
        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
    }

    public void StartNextWave()
    {
        if (gameEnded || isWaveRunning || !isBuildPhase)
            return;

        currentWave++;
        UpdateWaveUI();

        enemiesToSpawn = baseEnemies + 2 * (currentWave - 1);
        StartWave();
    }

    private void EndWave()
    {
        isWaveRunning = false;
        isBuildPhase = false;

        if (currentWave >= maxWaves)
        {
            gameEnded = true;
            isBuildPhase = false;
            beatManager.PlayWinEnding();

            WinPanel.SetActive(true);
            nextWaveButton.SetActive(false);
            return;
        }

        nextWaveButton.SetActive(false);
        rewardManager.ShowRewards();
    }

    public void RewardChosen()
    {
        if (gameEnded)
            return;

        isBuildPhase = true;
        nextWaveButton.SetActive(true);
    }

    public void LoseLife()
    {
        if (gameEnded)
            return;

        lives--;
        livesText.text = $"Lives: {lives}";
        AudioController.Instance.PlayLoseLife();

        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void GainLife(int count)
    {
        if (gameEnded)
            return;

        lives += count;
        livesText.text = $"Lives: {lives}";
    }

    private void UpdateWaveUI()
    {
        waveText.text = $"Wave {currentWave} / {maxWaves}";
    }

    private void GameOver()
    {
        if (gameEnded)
            return;

        gameEnded = true;
        isWaveRunning = false;
        isBuildPhase = false;

        StopAllCoroutines();

        beatManager.PlayLoseEnding();

        nextWaveButton.SetActive(false);

        GameOverPanel.SetActive(true);
    }

    public void EnableFirstWaveButton()
    {
        if (!gameEnded && isBuildPhase)
        {
            nextWaveButton.SetActive(true);
        }
    }
}
