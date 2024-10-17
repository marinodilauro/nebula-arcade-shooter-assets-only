using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("General variables")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject pausePanel;
    [SerializeField] bool isLooping;

    [Header("PickUps vaiables")]
    [SerializeField] List<GameObject> pickUps;
    [SerializeField] float timeBetweenPowerUpSpawns = 1f;
    [SerializeField] float powerUpSpawnTimerVariance;
    [SerializeField] float powerUpMinimumSpawnTime = 0.2f;

    [Header("Enemy waves vaiables")]
    [SerializeField] List<WaveConfigSO> waveConfigs;
    [SerializeField] float timeBetweenWaves = 0f;

    WaveConfigSO currentWave;
    Enemy enemyHealth;
    ScoreKeeper scoreKeeper;
    AudioPlayer audioPlayer;

    private float xSpawnRange = 4f;
    private float ySpawn = 10f;

    public bool needPause;
    public bool pressingPause;
    public static bool isPaused;

    private void Awake()
    {
        enemyHealth = GetComponent<Enemy>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        audioPlayer = FindObjectOfType<AudioPlayer>();
    }

    void Start()
    {
        pressingPause = false;
        isPaused = false;

        StartCoroutine("SpawnRandomPowerUP");
        StartCoroutine(SpawnEnemyWaves());
    }

    // Play/Pause methods
    public void PausingGame()
    {
        if (needPause && pressingPause)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        audioPlayer.PlayOpenPauseClip();
        audioPlayer.GetComponent<AudioSource>().volume = 0.1f;
        pausePanel.SetActive(true);
        scoreText.text = "You Scored :\n" + scoreKeeper.GetScore();
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        audioPlayer.GetComponent<AudioSource>().volume = 0.3f;
        audioPlayer.PlayClosePauseClip();
        pausePanel.SetActive(false);
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
        Time.timeScale = 1f;
        isPaused = false;
    }

    // Spawning pickups
    float GetRandomPickUpSpawnTime()
    {
        float spawnTime = Random.Range(timeBetweenPowerUpSpawns - powerUpSpawnTimerVariance,
                                        timeBetweenPowerUpSpawns + powerUpSpawnTimerVariance);

        return Mathf.Clamp(spawnTime, powerUpMinimumSpawnTime, float.MaxValue);
    }

    IEnumerator SpawnRandomPowerUP()
    {
        yield return new WaitForSeconds(20f);

        do
        {
            int randomIndex = Random.Range(0, pickUps.Count);
            float randomX = Random.Range(-xSpawnRange, xSpawnRange);
            Vector2 spawnPos = new Vector2(randomX, ySpawn);

            Instantiate(pickUps[randomIndex], spawnPos, Quaternion.identity, transform);

            yield return new WaitForSeconds(GetRandomPickUpSpawnTime());
        }
        while (isLooping);
    }

    // Enemy waves
    public WaveConfigSO GetCurrentWave()
    {
        return currentWave;
    }

    IEnumerator SpawnEnemyWaves()
    {
        do
        {
            int randomIndex = Random.Range(0, waveConfigs.Count);

            currentWave = waveConfigs[randomIndex];

            for (int i = 0; i < currentWave.GetEnemyCount(); i++)
            {
                Instantiate(currentWave.GetEnemyPrefab(i),
                            currentWave.GetStartingWaypoint().position,
                            Quaternion.Euler(0, 0, 180),
                            transform);

                yield return new WaitForSeconds(currentWave.GetRandomSpawnTime());
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }
        while (isLooping);
    }

}
