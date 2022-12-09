using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography;


public enum GameState { GS_PAUSEMENU, GS_GAME, GS_LEVELCOMPLETED, GS_GAME_OVER }

public class GameManager : MonoBehaviour
{
    public GameState currentGameState = GameState.GS_PAUSEMENU;

    public Canvas inGameCanvas;

    public static GameManager instance;

    public TMP_Text scoreText;

    private int score = 0;

    public Image[] keysTab;

    private int keysFound = 0;

    private int keysNumber = 3;

    public TMP_Text healthText;

    private int health = 3;

    public TMP_Text enemyText;

    private int enemyCount = 0;

    public TMP_Text timeText;

    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(currentGameState == GameState.GS_PAUSEMENU)
            {
                InGame();
                Time.timeScale = 1f;
            }
            else
            {
                PauseMenu();
                Time.timeScale = 0f;
            }
        }
        if (currentGameState == GameState.GS_GAME)
        {
            timer += Time.deltaTime;
            timeText.text = FormatTime(timer);
        }
    }

    private void Awake()
    {
        instance = this;

        foreach (Image key in keysTab)
        {
            key.color = Color.black;
        }

        healthText.text = health.ToString();
        Time.timeScale = 0f;

    }

    void SetGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        if (currentGameState == GameState.GS_GAME)
        {
            inGameCanvas.enabled = true;
        }
    }

    public void PauseMenu()
    {
        SetGameState(GameState.GS_PAUSEMENU);
        Debug.Log("PAUSE!");
    }

    public void InGame()
    {
        SetGameState(GameState.GS_GAME);
    }

    public bool LevelCompleted()
    {
        if (keysNumber == keysFound)
        {
            SetGameState(GameState.GS_LEVELCOMPLETED);
            Debug.Log("LEVEL COMPLETED!");
            return true;
        }
        else
            Debug.Log("Find all keys! Remaining: " + (keysNumber - keysFound));

        return false;
    }

    public void GameOver()
    {
        SetGameState(GameState.GS_GAME_OVER);
        Debug.Log("GAME OVER!");
    }

    public void AddPoints(int points)
    {
        score += points;
        scoreText.text = score.ToString();
    }

    public void AddKeys(Color color)
    {
        if (keysFound < keysNumber)
        {
            keysTab[keysFound].color = color;
            keysFound++;
        }
    }

    public void AddHealth(int lives)
    {
        health += lives;
        healthText.text = health.ToString();
        if (health == 0) GameOver();
    }

    public void AddKilledEnemy()
    {
        enemyCount ++;
        enemyText.text = enemyCount.ToString();
    }

    private string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
