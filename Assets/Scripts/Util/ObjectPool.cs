using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [SerializeField] GameObject _bloodEffect;
    [SerializeField] GameObject _hitEffect;
    [SerializeField] GameObject _enemyMinion;
    [SerializeField] GameObject _sparkleEffect;
    [SerializeField] GameObject _bossPrefab;
    [SerializeField] GameObject _bombPrefab;

    [Header("Pickups")]
    [SerializeField] GameObject _healthPickup;
    [SerializeField] GameObject _speedPickup;
    [Header("AudioSFX")]
    [SerializeField] GameObject _sfxPlayer;

    ObjectPooler _bloodEffectPool;
    ObjectPooler _hitEffectPool;
    ObjectPooler _enemyMinionPool;
    ObjectPooler _sparkleEffectPool;
    ObjectPooler _sfxPlayerPool;
    ObjectPooler _healthPickupPool;
    ObjectPooler _speedPickupPool;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(this);
        }


    }


    private void OnEnable()
    {

        _bloodEffectPool = new ObjectPooler(_bloodEffect, this.gameObject);
        _hitEffectPool = new ObjectPooler(_hitEffect, this.gameObject);
        _enemyMinionPool = new ObjectPooler(_enemyMinion, this.gameObject);
        _sparkleEffectPool = new ObjectPooler(_sparkleEffect, this.gameObject);

        _sfxPlayerPool = new ObjectPooler(_sfxPlayer, this.gameObject);

        _healthPickupPool = new ObjectPooler(_healthPickup, this.gameObject);

        _speedPickupPool = new ObjectPooler(_speedPickup, this.gameObject);
    }




    public GameObject GetBloodEffect()
    {
        return _bloodEffectPool.GetObject();
    }

    public GameObject GetHitEffect()
    {
        return _hitEffectPool.GetObject();
    }

    public GameObject GetEnemyMinion()
    {
        return _enemyMinionPool.GetObject();
    }

    public GameObject GetSparkleEffect()
    {
        return _sparkleEffectPool.GetObject();
    }

    public SFXPlayer GetSFXPlayer()
    {
        return _sfxPlayerPool.GetObject().GetComponent<SFXPlayer>();
    }

    public GameObject GetRandomPickup()
    {
        int rand = Random.Range(0, 2);
        switch (rand)
        {
            case 0:
                return GetHealthPickup();
            case 1:
                return GetSpeedPickup();
        }

        return GetHealthPickup();
    }
    private GameObject GetHealthPickup()
    {
        return _healthPickupPool.GetObject();
    }

    private GameObject GetSpeedPickup()
    {
        return _speedPickupPool.GetObject();
    }

    public GameObject GetBoss()
    {
        return Instantiate(_bossPrefab);
    }

}
