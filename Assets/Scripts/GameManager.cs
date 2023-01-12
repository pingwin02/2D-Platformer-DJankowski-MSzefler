using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public enum GameState { GS_PAUSEMENU, GS_GAME, GS_LEVELCOMPLETED, GS_GAME_OVER, GS_OPTIONS, GS_DIALOGUE, GS_START }

public class GameManager : MonoBehaviour
{
    public GameState currentGameState = GameState.GS_START;

    public Canvas inGameCanvas;

    public static GameManager instance;

    public TMP_Text scoreText;

    private int score = 0;

    public Image[] keysTab;

    private int keysFound = 0;

    private const int keysNumber = 3;

    public TMP_Text healthText;

    private int health = 3;

    public TMP_Text enemyText;

    private int enemyCount = 0;

    public TMP_Text timeText;

    public float timer = 0;

    private float offsetTimer = 0;

    public TMP_Text temperatureText;

    private int temperature = 20;

    public Canvas pauseMenuCanvas;

    public Canvas levelCompletedCanvas;

    const string keyHighScore = "HighScoreLevel1";

    public TMP_Text ScoreText;

    public TMP_Text HighScoreText;

    public Canvas optionsCanvas;

    public TMP_Text volumeText;

    public TMP_Text qualityText;

    public Canvas gameoverCanvas;

    public Image fadeImageLose;

    public Image fadeImageWin;

    // Dialogue variables

    public Canvas dialogueCanvas;

    public TextMeshProUGUI dialogueText;

    private string[] dialogueLines;

    public float dialogueTextSpeed;

    private int dialogueIndex;

    public Light2D DayLight;

    public Canvas startScreenCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGameState != GameState.GS_START)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (currentGameState == GameState.GS_PAUSEMENU)
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
                offsetTimer += Time.deltaTime;
                if (offsetTimer > 5)
                {
                    offsetTimer = 0;
                    temperature++;
                    SetTemperatureText();
                }
                timeText.text = FormatTime(timer);
                
            }

            if (Input.GetKeyDown(KeyCode.Space) && currentGameState == GameState.GS_DIALOGUE)
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

        AudioListener.volume = 0.05f;

        DayLight.color = new Color(1f, 1f, 1f);

        dialogueLines = new string[2];
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
        startScreenCanvas.enabled = (currentGameState == GameState.GS_START);
    }

    public void PauseMenu()
    {
        SetGameState(GameState.GS_PAUSEMENU);
        Time.timeScale = 0f;
    }

    public void Options()
    {
        if (currentGameState == GameState.GS_GAME)
        {
            SetGameState(GameState.GS_OPTIONS);
            Time.timeScale = 0f;
        }
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
        inGameCanvas.enabled = true;
        Time.timeScale = 1f;
    }

    public void DungeonWarning()
    {
        dialogueLines[0] = "You have entered dark dungeons!";
        dialogueLines[1] = "You are too afraid to double jump. Find a key to escape!";
        StartDialogue();
    }

    public void StartScreen()
    {
        SetGameState(GameState.GS_START);
        inGameCanvas.enabled = false;
    }
    public void LevelCompleted()
    {
        if (keysFound == keysNumber)
        {
            score += health;
            score += enemyCount;
            int beforeBonus = score;
            ScoreText.text = "Your score:\n" + beforeBonus;
            if (120 < timer && timer < 180)
            {
                score += 10;
                ScoreText.text += " + 10 (<180s)";
            }
            else if (60 < timer && timer < 120)
            {
                score += 20;
                ScoreText.text += " + 20 (<120s)";
            }
            else if (timer < 60)
            {
                score += 30;
                ScoreText.text += " + 30 (<60s)";
            }
            else
            {
                ScoreText.text += " + 0 (>180s)";
            }
            SetGameState(GameState.GS_LEVELCOMPLETED);
            StartCoroutine(FadeWin());
            ScoreText.text += " = " + score;
            HighScoreText.text = "Your best score: " + PlayerPrefs.GetInt(keyHighScore);
        }
        else
        {
            dialogueLines[0] = "Collect all keys to finish the level!";
            dialogueLines[1] = "Remaining: " + (keysNumber - keysFound);
            StartDialogue();
        }
    }

    public void GameOver()
    {
        SetGameState(GameState.GS_GAME_OVER);
        StartCoroutine(FadeLose());
        AudioListener.volume = 0f;
    }

    private IEnumerator FadeLose()
    {
        for (float i = 0; i < 1; i += 0.01f)
        {
            fadeImageLose.color = new Color(fadeImageLose.color.r, fadeImageLose.color.g, fadeImageLose.color.b, i);
            yield return new WaitForSeconds(0.02f);
        }

        fadeImageLose.color = new Color(fadeImageLose.color.r, fadeImageLose.color.g, fadeImageLose.color.b, 1);
    }

    private IEnumerator FadeWin()
    {
        for (float i = 0; i < 1; i += 0.01f)
        {
            fadeImageWin.color = new Color(fadeImageWin.color.r, fadeImageWin.color.g, fadeImageWin.color.b, i);
            yield return new WaitForSeconds(0.02f);
        }

        fadeImageWin.color = new Color(fadeImageWin.color.r, fadeImageWin.color.g, fadeImageWin.color.b, 1);
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
        SetGameState(GameState.GS_DIALOGUE);
    }

    IEnumerator TypeLine()
    {
        dialogueText.text = string.Empty;
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
    }

    void SetTemperatureText()
    {
        temperatureText.text = temperature + "°C";
        if (temperature >= 30 && temperature < 45)
        {
            temperatureText.color = new Color(0.9f, 0.7f, 0);
            DayLight.color = new Color(0.75f, 0.75f, 0.75f);
        }
        else if (temperature >= 45 && temperature < 60)
        {
            temperatureText.color = new Color(1, 0, 0);
            DayLight.color = new Color(0.75f, 0.75f, 0.5f);
        }
        else if(temperature >= 60) 
        {    
            temperatureText.color = new Color(0, 0, 0);
            GameOver();
        }
    }
}
