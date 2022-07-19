using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button startButton;

    private void Start()
    {
        startButton.Select();
    }

    public void LoadeScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetScore()
    {
        PlayerPrefs.DeleteKey(ScoreSystem.highscoreBloodKey);
        PlayerPrefs.DeleteKey(ScoreSystem.highscoreGhostCatchesKey);
        PlayerPrefs.DeleteKey(ScoreSystem.highscoreGuestKey);
        PlayerPrefs.DeleteKey(ScoreSystem.highscoreMoneyKey);
    }

    public void SetLevelMode(int modeIndex)
    {
        PlayerPrefs.SetInt(GameManager.modeKey, modeIndex);
        PlayerPrefs.Save();
    }
}
