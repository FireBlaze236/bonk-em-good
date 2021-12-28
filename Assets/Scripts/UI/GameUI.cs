using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _countdownText;
    [SerializeField] TextMeshProUGUI _killsText;
    [SerializeField] TextMeshProUGUI _savesText;
    [SerializeField] TextMeshProUGUI _waveText;
    

    [SerializeField] Animator _ingameUIAnimator;

    private void OnEnable()
    {
        GameManager.Instance.OnWaveSpawnCounterUpdate += UpdateCountdownText;
        GameManager.Instance.OnKillUpdate += UpdateKillText;
        GameManager.Instance.OnSavesUpdate += UpdateSaveText;

        GameManager.Instance.OnWaveUpdate += UpdateWaveText;

        GameManager.Instance.OnPauseGame += ShowPauseScreen;
        GameManager.Instance.OnUnpauseGame += HidePauseScreen;

        GameManager.Instance.OnGameOver += ShowGameOverScreen;

        GameManager.Instance.OnGameFinish += ShowCongratsScreen;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnWaveSpawnCounterUpdate -= UpdateCountdownText;
        GameManager.Instance.OnKillUpdate -= UpdateKillText;
        GameManager.Instance.OnSavesUpdate-= UpdateSaveText;

        GameManager.Instance.OnWaveUpdate -= UpdateWaveText;

        GameManager.Instance.OnPauseGame -= ShowPauseScreen;
        GameManager.Instance.OnUnpauseGame -= HidePauseScreen;

        GameManager.Instance.OnGameOver -= ShowGameOverScreen;

        GameManager.Instance.OnGameFinish -= ShowCongratsScreen;
    }

    public void QuitToMenu()
    {
        GameManager.Instance.ResetGame();
        SceneManager.LoadScene(0);
    }
    public void UnpauseGame()
    {
        GameManager.Instance.UnpauseGame();
    }

    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }

    private void UpdateCountdownText(int count)
    {
        if(count <= 0)
        {
            _countdownText.gameObject.SetActive(false);
            return;
        }
        else
        {
            _countdownText.gameObject.SetActive(true);
            _countdownText.text = count.ToString();
        }
    }

    private void UpdateKillText(int val)
    {
        _killsText.text = val.ToString();
    }

    private void UpdateSaveText(int val)
    {
        _savesText.text = val.ToString();
    }

    private void UpdateWaveText(int val)
    {
        _waveText.text = val.ToString();
    }


    private void ShowPauseScreen()
    {
        _ingameUIAnimator.SetBool("gamePaused", true);
    }
    
    private void HidePauseScreen()
    {
        _ingameUIAnimator.SetBool("gamePaused", false);
    }

    private void ShowGameOverScreen()
    {
        _ingameUIAnimator.SetTrigger("gameOver");
    }

    private void ShowCongratsScreen()
    {
        _ingameUIAnimator.SetTrigger("gameEnd");
    }
}
