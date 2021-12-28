using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject _menuScreen;
    [SerializeField] GameObject _optionsScreen;
    [SerializeField] TMPro.TextMeshProUGUI _scoreText;

    private void Start()
    {
        _scoreText.text = "HIGHSCORE -" + PlayerPrefs.GetInt("highscore").ToString();
        GameManager.Instance.PlayMusic();
    }
    public void ShowOptions()
    {
        _menuScreen.SetActive(false);
        _optionsScreen.SetActive(true);
    }

    public void ShowMenu()
    {
        _menuScreen.SetActive(true);
        _optionsScreen.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }

    public void SetMusicVolume(float value)
    {
        GameManager.Instance.SetMusicVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        GameManager.Instance.SetSFXVolume(value);
    }
}
