using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool gameIsRunning = false;

    private int _saves = 0;
    private int _kills = 0;

    [SerializeField] bool _waveSpawning = true;
    [SerializeField] bool _spawnCounter = false;
    [SerializeField] int _waveNumber = 0;
    [SerializeField] List<EnemyMinion> _spawnedMinions = new List<EnemyMinion>();
    [SerializeField] float _radius = 5f;

    [SerializeField] int _waveDelay = 10;

    [SerializeField] LayerMask _enemyLayer;

   

    public Action<int> OnWaveSpawnCounterUpdate;
    public Action<int> OnSavesUpdate;
    public Action<int> OnKillUpdate;
    public Action<int> OnWaveUpdate;


    public Action OnPauseGame;
    public Action OnUnpauseGame;
    public Action OnGameOver;
    public Action OnGameFinish;

    [SerializeField] GameObject boss;


    [SerializeField] AudioMixer _master;

    public static int highestScore = 0;

    private void Update()
    {
        

        if(_waveSpawning)
            CheckWave();

        if(Input.GetKeyDown(KeyCode.Escape) && _waveSpawning) 
        {
            PauseGame();
        }
        
    }
    private void Awake()
    {
        highestScore = PlayerPrefs.GetInt("highscore");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    
    }

    public void SetMusicVolume(float value)
    {
        _master.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        _master.SetFloat("SFXVolume", value);
    }
    private void SetHighscore(int score)
    {
        highestScore = score;
        int prevHighestScore = PlayerPrefs.GetInt("highscore");
        if (highestScore > prevHighestScore)
        {
            PlayerPrefs.SetInt("highscore", highestScore);
        }
    }
    public void QuitGame()
    {
        Application.Quit();
        
    }

    private void CheckWave()
    {
        if(_spawnedMinions.Count == 0 && !_spawnCounter)
        {
            StartCoroutine(WaveSpawnRoutine());
        }
    }

    IEnumerator WaveSpawnRoutine()
    {
        int waveCounter = _waveDelay;
        _spawnCounter = true;
        while(_spawnCounter)
        {
            OnWaveSpawnCounterUpdate?.Invoke(waveCounter);
            yield return new WaitForSeconds(1);
            waveCounter--;
            _spawnCounter = waveCounter >= 0;
        }


        SpawnEnemies();

        if (_waveNumber == 6)
        {
            SpawnBoss();
        }
    }
    public void SpawnEnemies()
    {
        int enemiesTotal = Mathf.RoundToInt(_waveNumber * 1.5f) + 1;
        enemiesTotal = Mathf.Clamp(enemiesTotal, 0, 12);
        

        for(int i =0; i < enemiesTotal; i++)
        {
            GameObject enemy = ObjectPool.Instance.GetEnemyMinion();

            bool emptyPlace = false;
            Vector3 randomPoint = transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)).normalized * UnityEngine.Random.Range(3f, _radius);
            while (!emptyPlace)
            {
                emptyPlace = Physics.OverlapSphere(randomPoint, 2f, _enemyLayer).Length == 0 && enemy.GetComponent<NavMeshAgent>().isActiveAndEnabled;
                randomPoint = transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)).normalized * UnityEngine.Random.Range(3f, _radius);
            }

            

            enemy.transform.position = randomPoint;

            _spawnedMinions.Add(enemy.GetComponent<EnemyMinion>());

        }
        

        _waveNumber++;

        OnWaveUpdate?.Invoke(_waveNumber);

    }

    public void SavedOne()
    {
        _saves++;

        OnSavesUpdate?.Invoke(_saves);

        SetHighscore(_saves - _kills);
    }

    public void KilledOne(EnemyMinion minion)
    {
        _kills++;
        if(!minion.isGood)
        {
            _spawnedMinions.Remove(minion);
        }
        OnKillUpdate?.Invoke(_kills);

        SetHighscore(_saves - _kills);
    }

    public void TurnGoodOne(EnemyMinion minion)
    {
        _spawnedMinions.Remove(minion);

        
    }

    public void PlayMusic()
    {
        GetComponent<AudioSource>().Play();
    }

    public void GameOver()
    {
        Time.timeScale = 0.0f;
        gameIsRunning = false;
        _waveSpawning = false;

        OnGameOver?.Invoke();

        Debug.Log("Game OVER");
        //TODO: Save player highscore

        SetHighscore(_saves - _kills);
    }

    public void PauseGame()
    {
        
        OnPauseGame?.Invoke();
        Time.timeScale = 0;
        gameIsRunning = false;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        gameIsRunning = true;

        OnUnpauseGame?.Invoke();

    }

    public void ResetGame()
    {
        _saves = 0;
        _kills = 0;
        _waveNumber = 0;
        highestScore = PlayerPrefs.GetInt("highscore");

        gameIsRunning = false;
        _waveSpawning = false;
        
        StopAllCoroutines();
        _spawnedMinions.Clear();
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        gameIsRunning = true;
        _waveSpawning = true;
        
        StartCoroutine(WaveSpawnRoutine());

        PlayMusic();
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ResetGame();

    }

    private void SpawnBoss()
    {
        boss = ObjectPool.Instance.GetBoss();

        GameObject.FindGameObjectWithTag("HomePortal").SetActive(false);

        boss.GetComponent<Boss>().OnBossDead += FinishGame;
    }

    private void FinishGame()
    {
        Time.timeScale = 0.0f;
        gameIsRunning = false;
        _waveSpawning = false;
        boss.GetComponent<Boss>().OnBossDead -= FinishGame;

        OnGameFinish?.Invoke();
    }
}
