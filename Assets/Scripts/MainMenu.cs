using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Canvas mainMenuCanvas;

    public Canvas howToPlayCanvas;

    public Canvas levelSelectCanvas;

    public Button level2Button;

    const string keyVolume = "VolumeSetting";

    const string keyQuality = "QualitySetting";

    const string keyLevel2 = "Level2Unlocked";

    private void Awake()
    {
        howToPlayCanvas.enabled = false;

        levelSelectCanvas.enabled = false;

        Time.timeScale = 1f;

        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt(keyQuality, QualitySettings.GetQualityLevel()));

        AudioListener.volume = (float)PlayerPrefs.GetInt(keyVolume, 10) / 100;

        level2Button.interactable = (PlayerPrefs.GetInt(keyLevel2, 0) == 1);

        StartCoroutine(seasonAnimator());
    }

    public void OnPlayButtonPressed()
    {
        mainMenuCanvas.enabled = false;
        levelSelectCanvas.enabled = true;
    }

    public void OnLevel1ButtonPressed()
    {
        SceneManager.LoadSceneAsync("Level1");
    }

    public void OnLevel2ButtonPressed()
    {
        SceneManager.LoadSceneAsync("Level2");
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
        levelSelectCanvas.enabled = false;
    }

    public void OnResetPressed()
    {
        PlayerPrefs.DeleteAll();
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt(keyQuality, QualitySettings.GetQualityLevel()));
        AudioListener.volume = (float)PlayerPrefs.GetInt(keyVolume, 10) / 100;
        level2Button.interactable = (PlayerPrefs.GetInt(keyLevel2, 0) == 1);
    }

    public void OnMainMenuButtonPressed()
    {
        mainMenuCanvas.enabled = true;
        howToPlayCanvas.enabled = false;
        levelSelectCanvas.enabled = false;
    }

    IEnumerator seasonAnimator()
    {
        int n = 0;
        while (true)
        {
            GetComponent<SeasonChanger>().changeSeason(n);
            n++;
            n %= 3;
            yield return new WaitForSeconds(1.5f);
        }
    }
}
