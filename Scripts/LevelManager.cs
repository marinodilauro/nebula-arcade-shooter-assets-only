using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] float sceneLoadDelay = 2f;
    [SerializeField] int prevSceneIndex;
    [SerializeField] GameObject creditCanvas;
    [SerializeField] GameObject backButton;
    [SerializeField] GameObject mainMenuCanvas;
    [SerializeField] GameObject startButton;

    AudioPlayer audioPlayer;
    ScoreKeeper scoreKeeper;
    PlayerController player;
    EventSystem eventSystem;

    private void Awake()
    {
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        player = FindObjectOfType<PlayerController>();
        audioPlayer = FindObjectOfType<AudioPlayer>();
        eventSystem = FindObjectOfType<EventSystem>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu" || scene.name == "GameOver")
        {
            AudioPlayer.PlayMenuMusic(prevSceneIndex);
            audioPlayer.GetComponent<AudioSource>().volume = 0.3f;
        }
        else if (scene.name == "Game")
        {
            AudioPlayer.PlayGameMusic();
            audioPlayer.GetComponent<AudioSource>().volume = 0.3f;
        }
        prevSceneIndex = scene.buildIndex;
    }

    public void LoadTutorial()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("HowToPlay");
    }

    public void LoadGame()
    {
        Time.timeScale = 1f;
        scoreKeeper.ResetScore();
        SceneManager.LoadScene("Game");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
        /*creditCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
        eventSystem.firstSelectedGameObject = backButton;*/
    }

    /*
    public void Back()
    {
        creditCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
        eventSystem.firstSelectedGameObject = startButton;
    }*/

    public void LoadGameOver()
    {
        StartCoroutine(WaitAndLoad("GameOver", sceneLoadDelay));
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    IEnumerator WaitAndLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
