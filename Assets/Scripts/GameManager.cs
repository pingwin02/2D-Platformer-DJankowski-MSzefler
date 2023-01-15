using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using static Unity.VisualScripting.Member;
using UnityEditor;

public enum GameState { GS_PAUSEMENU, GS_GAME, GS_LEVELCOMPLETED, GS_GAME_OVER, GS_OPTIONS, GS_DIALOGUE, GS_START }

public enum Season { Winter, Spring, Summer }

public class GameManager : MonoBehaviour
{
    public GameState currentGameState = GameState.GS_START;

    public Season currentSeason = Season.Winter;

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

    private int temperature = -10;

    public Canvas pauseMenuCanvas;

    public Canvas levelCompletedCanvas;

    const string keyHighScore = "HighScoreLevel1";

    const string keyVolume = "VolumeSetting";

    const string keyQuality = "QualitySetting";

    public TMP_Text ScoreText;

    public TMP_Text HighScoreText;

    public Canvas optionsCanvas;

    public TMP_Text volumeText;

    public Slider volumeSlider;

    public TMP_Text qualityText;

    public Canvas gameoverCanvas;

    public Image fadeImageLose;

    public Image fadeImageWin;

    [SerializeField] AudioClip gameOverMusic;

    [SerializeField] AudioClip levelCompletedMusic;

    private AudioSource source;

    // Dialogue variables

    public Canvas dialogueCanvas;

    public TextMeshProUGUI dialogueText;

    private string[] dialogueLines;

    public float dialogueTextSpeed;

    private int dialogueIndex;

    public Light2D DayLight;

    public Canvas startScreenCanvas;

    public bool immortalMode = false;

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
                if (offsetTimer > 5 || (immortalMode && offsetTimer > 1))
                {
                    offsetTimer = 0;
                    temperature++;
                    SetTemperature();
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

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.G))
            {
                immortalMode = !immortalMode;
            }
#endif
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

        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt(keyQuality, QualitySettings.GetQualityLevel()));

        qualityText.text = "Quality: " + QualitySettings.names[QualitySettings.GetQualityLevel()];

        AudioListener.volume = (float)PlayerPrefs.GetInt(keyVolume, 10) / 100;

        volumeSlider.value = PlayerPrefs.GetInt(keyVolume, 10);

        SetVolume(volumeSlider);

        dialogueLines = new string[2];

        source = GetComponent<AudioSource>();

        source.volume = 1f;
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
            Cursor.visible = false;
        } else
        {
            Cursor.visible = true;
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
        SetGameState(GameState.GS_OPTIONS);
        Time.timeScale = 0f;
    }

    public void QualityUp()
    {
        QualitySettings.IncreaseLevel();
        qualityText.text = "Quality: " + QualitySettings.names[QualitySettings.GetQualityLevel()];
        PlayerPrefs.SetInt(keyQuality, QualitySettings.GetQualityLevel());
    }

    public void QualityDown()
    {
        QualitySettings.DecreaseLevel();
        qualityText.text = "Quality: " + QualitySettings.names[QualitySettings.GetQualityLevel()];
        PlayerPrefs.SetInt(keyQuality, QualitySettings.GetQualityLevel());
    }

    public void SetVolume(Slider vol)
    {
        AudioListener.volume = vol.value / 100;
        volumeText.text = "Master volume: " + vol.value + " %";
        PlayerPrefs.SetInt(keyVolume, (int)vol.value);
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
    public void SignInfo(int nr)
    {
        switch (nr)
        {
            case 1:
                dialogueLines[0] = "Global warming is the long-term warming";
                dialogueLines[1] = "of the planet's overall temperature.";
                break;
            case 2:
                dialogueLines[0] = "The greenhouse effect is when the sun's rays penetrate the atmosphere,";
                dialogueLines[1] = "but when that heat is reflected off the surface cannot escape back into space.";
                break;
            case 3:
                dialogueLines[0] = "Global warming causes climate change, which poses a serious threat to life on Earth";
                dialogueLines[1] = "in the forms of widespread flooding and extreme weather.";
                break;
            default: return;

        }

        StartDialogue();
    }

    public void StartScreen()
    {
        SetGameState(GameState.GS_START);
        inGameCanvas.enabled = false;
    }
    public void LevelCompleted()
    {
        if (keysFound == keysNumber || immortalMode)
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
            source.clip = null;
            source.PlayOneShot(levelCompletedMusic, 1);
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
        source.PlayOneShot(gameOverMusic, 1);

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
        enemyCount++;
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

    void SetTemperature()
    {
        temperatureText.text = temperature + "°C";
        //Winter
        if (temperature < 0)
        {
            temperatureText.color = new Color(0.03f, 0.5f, 0.6f);
            DayLight.color = new Color(0.7f, 1f, 1f);
            currentSeason = Season.Winter;
            GetComponent<SeasonChanger>().changeSeason(0);
        }
        //Spring
        else if (temperature >= 0 && temperature < 20)
        {
            temperatureText.color = new Color(0.03f, 0.6f, 0.03f);
            DayLight.color = new Color(1f, 1f, 1f);
            currentSeason = Season.Spring;
            GetComponent<SeasonChanger>().changeSeason(1);
        }
        //Summer
        else if (temperature >= 20 && temperature < 30)
        {
            temperatureText.color = new Color(1f, 0f, 0);
            DayLight.color = new Color(0.75f, 0.75f, 0.75f);
            currentSeason = Season.Summer;
            GetComponent<SeasonChanger>().changeSeason(2);
        }
        else if (temperature >= 30)
        {
            //FoxMaterial.color = new Color(250/255f,68/255f,65/255f);
            temperatureText.color = new Color(0, 0, 0);
            GameOver();
        }
    }
}
