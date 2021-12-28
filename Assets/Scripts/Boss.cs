using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour, IBonkable
{
    [SerializeField] PlayerController _player;

    [SerializeField] float _health = 500f;
    [SerializeField] float _maxHealth = 500f;
    [SerializeField] Transform _bombSpawnLocation;

    public bool _spawningBombs = true;
    public bool _stunned = false;
    [SerializeField] float _stunTime = 10f;
    [SerializeField] float _bombSpawnTime = 10f;

    [SerializeField] AudioClip _eatSound;

    [SerializeField] ParticleSystem _stunParticles;
    [SerializeField] AudioSource _stunAudio;

    public Action<float> OnBossHealthChange;
    public Action OnBossDead;


    
    [SerializeField] GameObject _body;
    [SerializeField] ParticleSystem _deadExplosion;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    private void OnEnable()
    {
        _health = _maxHealth;

        StartCoroutine(SpawnBombTimer());
    }

    private void OnDisable()
    {
        StopAllCoroutines();

    }
    private void Update()
    {
        Vector3 pos = new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z);
        transform.LookAt(pos);
    }

    IEnumerator SpawnBombTimer()
    {
        _spawningBombs = true;
        while(_spawningBombs)
        {
            GameObject newBomb = ObjectPool.Instance.GetRandomPickup();
            newBomb.transform.position = _bombSpawnLocation.position;

            newBomb.AddComponent<RollingBomb>();

            newBomb.GetComponent<Rigidbody>().isKinematic = false;

            yield return new WaitForSeconds(_bombSpawnTime);

        }
    }

    IEnumerator StunLoop()
    {
        _stunned = true;
        _spawningBombs = false;
        _stunAudio.Play();
        _stunParticles.Play();

        yield return new WaitForSeconds(_stunTime);
        _stunAudio.Stop();
        _stunParticles.Stop();
        StartCoroutine(SpawnBombTimer());
        _stunned = false;
    }

    public void Stun()
    {
        if(!_stunned)
            StartCoroutine(StunLoop());

        _health -= 10f;
    }



    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Enemy") && other.TryGetComponent<EnemyMinion>(out EnemyMinion minion))
        {
            if (minion.isGood)
            {
                GameManager.Instance.KilledOne(minion);
                ObjectPool.Instance.GetSFXPlayer().Play(_eatSound, transform.position);

                Stun();
                other.gameObject.SetActive(false);
            }
            else if(minion.knockedDown)
            {
                Heal();

                GameManager.Instance.KilledOne(minion);
                ObjectPool.Instance.GetSFXPlayer().Play(_eatSound, transform.position);
            }
            

        }


    }
    private void Heal()
    {

    }

    public void Bonk(float value, int damage)
    {
        if (!_stunned)
        {
            if (value > Constants.BONK_LOWER && value < Constants.BONK_UPPER)
            {
                _health -= damage;
            }
        }
        else
        {

            if (value > Constants.BONK_LOWER && value < Constants.BONK_UPPER)
            {
                _health -= damage * 2f;
            }
            else
            {
                _health -= damage;
            }
        }

        ClampHealth();
    }

    public void Hit(float value, int damage)
    {

        Bonk(value, damage);

    }

    private void ClampHealth()
    {
        _health = Mathf.Clamp(_health, 0f, _maxHealth);
        if(_health == 0f)
        {
            _spawningBombs = false;
            StartCoroutine(DeathLoop());
            
        }
    }

    IEnumerator DeathLoop()
    {
        _body.SetActive(false);
        _deadExplosion.Play();

        yield return new WaitForSeconds(2f);
        OnBossDead?.Invoke();

        gameObject.SetActive(false);
    }
}

