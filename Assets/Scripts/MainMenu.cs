using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public Canvas mainMenuCanvas;

    public Canvas howToPlayCanvas;

    private void Awake()
    {
        howToPlayCanvas.enabled = false;
    }

    public void OnLevel1ButtonPressed()
    {
        SceneManager.LoadSceneAsync("Level1");
    }

    public void OnExitToDesktopButtonPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void OnHowToPlayButtonPressed()
    {
        mainMenuCanvas.enabled = false;
        howToPlayCanvas.enabled = true;
    }

    public void OnResetHighScoreButtonPressed()
    {
        PlayerPrefs.DeleteKey("HighScoreLevel1");
    }

    public void OnMainMenuButtonPressed()
    {
        mainMenuCanvas.enabled = true;
        howToPlayCanvas.enabled = false;
    }
}
