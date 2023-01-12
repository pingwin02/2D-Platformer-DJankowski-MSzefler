using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public Canvas mainMenuCanvas;

    public Canvas howToPlayCanvas;

    const string keyVolume = "VolumeSetting";

    const string keyQuality = "QualitySetting";

    private void Awake()
    {
        howToPlayCanvas.enabled = false;

        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt(keyQuality, QualitySettings.GetQualityLevel()));

        AudioListener.volume = (float)PlayerPrefs.GetInt(keyVolume, 10) / 100;
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

    public void OnResetPressed()
    {
        PlayerPrefs.DeleteAll();
    }

    public void OnMainMenuButtonPressed()
    {
        mainMenuCanvas.enabled = true;
        howToPlayCanvas.enabled = false;
    }
}
