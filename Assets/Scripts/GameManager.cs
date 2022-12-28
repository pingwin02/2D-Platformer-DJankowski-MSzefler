using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;


public enum GameState { GS_PAUSEMENU, GS_GAME, GS_LEVELCOMPLETED, GS_GAME_OVER, GS_OPTIONS, GS_DIALOGUE }

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

    public Canvas pauseMenuCanvas;

    public Canvas levelCompletedCanvas;

    const string keyHighScore = "HighScoreLevel1";

    public TMP_Text ScoreText;

    public TMP_Text HighScoreText;

    public Canvas optionsCanvas;

    public TMP_Text volumeText;

    public TMP_Text qualityText;

    public Canvas gameoverCanvas;

    // Dialogue variables

    public Canvas dialogueCanvas;

    public TextMeshProUGUI dialogueText;

    public string[] dialogueLines;

    public float dialogueTextSpeed;

    private int dialogueIndex;

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
            }
            else if (currentGameState == GameState.GS_GAME)
            {
                PauseMenu();
            }
        }
        if (currentGameState == GameState.GS_GAME)
        {
            timer += Time.deltaTime;
            timeText.text = FormatTime(timer);
        }

        if (Input.GetMouseButtonDown(0) && currentGameState == GameState.GS_DIALOGUE)
        {
            if (dialogueText.text == dialogueLines[dialogueIndex])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                dialogueText.text = dialogueLines[dialogueIndex];
            }
        }
    }

    private void Awake()
    {
        instance = this;

        InGame();

        foreach (Image key in keysTab)
        {
            key.color = Color.black;
        }

        healthText.text = health.ToString();
        if (!PlayerPrefs.HasKey(keyHighScore))
        {
            PlayerPrefs.SetInt(keyHighScore, 0);
        }

    }

    public void OnResumeButtonClicked()
    {
        InGame();
    }

    public void OnRestartButtonClicked()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void OnReturntoMainMenuButtonClicked()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    void SetGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        if (currentGameState == GameState.GS_GAME)
        {
            inGameCanvas.enabled = true;
        }
        if (currentGameState == GameState.GS_LEVELCOMPLETED)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.name == "Level1")
            {
                int highScore = PlayerPrefs.GetInt(keyHighScore);
                if (highScore < score)
                {
                    PlayerPrefs.SetInt(keyHighScore, score);
                }
            }
        }
        pauseMenuCanvas.enabled = (currentGameState == GameState.GS_PAUSEMENU);
        levelCompletedCanvas.enabled = (currentGameState == GameState.GS_LEVELCOMPLETED);
        optionsCanvas.enabled = (currentGameState == GameState.GS_OPTIONS);
        gameoverCanvas.enabled = (currentGameState == GameState.GS_GAME_OVER);
        dialogueCanvas.enabled = (currentGameState == GameState.GS_DIALOGUE);
    }

    public void PauseMenu()
    {
        SetGameState(GameState.GS_PAUSEMENU);
        Time.timeScale = 0f;
    }

    public void Options()
    {
        SetGameState(GameState.GS_OPTIONS);
        Time.timeScale = 0f;
    }

    public void QualityUp()
    {
        QualitySettings.IncreaseLevel();
        qualityText.text = "Quality: " + QualitySettings.names[QualitySettings.GetQualityLevel()];
    }

    public void QualityDown()
    {
        QualitySettings.DecreaseLevel();
        qualityText.text = "Quality: " + QualitySettings.names[QualitySettings.GetQualityLevel()];
    }

    public void SetVolume(Slider vol)
    {
        AudioListener.volume = vol.value / 100;
        volumeText.text = "Master volume: " + vol.value + " %";
    }

    public void InGame()
    {
        SetGameState(GameState.GS_GAME);
        Time.timeScale = 1f;
    }

    public void Dialogue()
    {
        SetGameState(GameState.GS_DIALOGUE);
    }

    public void LevelCompleted()
    {
        if (keysFound == keysNumber)
        {
            score += health;
            score += enemyCount;
            SetGameState(GameState.GS_LEVELCOMPLETED);
            ScoreText.text = "Your score: " + score;
            HighScoreText.text = "Your best score: " + PlayerPrefs.GetInt(keyHighScore);
            Time.timeScale = 0f;
        }
        else
        {
            dialogueLines[1] = " Find all keys! Remaining: " + (keysNumber - keysFound);
            StartDialogue();
            Dialogue();
            //Debug.Log("Find all keys! Remaining: " + (keysNumber - keysFound));
        }
    }

    public void GameOver()
    {
        SetGameState(GameState.GS_GAME_OVER);
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

    void StartDialogue()
    {
        if (currentGameState != GameState.GS_DIALOGUE)
        {
            dialogueIndex = 0;
            StartCoroutine(TypeLine());
        }
    }

    IEnumerator TypeLine()
    {
        // Type each character one by one
        foreach (char c in dialogueLines[dialogueIndex].ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(dialogueTextSpeed);
        }
    }

    void NextLine()
    {
        if (dialogueIndex < dialogueLines.Length - 1)
        {
            dialogueIndex++;
            StartCoroutine(TypeLine());
        }
        else
        {
            InGame();         
        }
        dialogueText.text = string.Empty;
    }
}
