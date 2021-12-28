using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { IDLE, FOLLOWING, GOOD}
public class EnemyMinion : MonoBehaviour, IBonkable
{
    [SerializeField] NavMeshAgent _agent;
    [SerializeField] GameObject _player;

    [SerializeField] int _health = 3;
    [SerializeField] int _maxHealth = 30;
    [SerializeField] float _knockDownDuration = 2f;
    public bool knockedDown = false;

    [SerializeField] CharacterController _characterController;
    [SerializeField] Animator _enemyAnimator;
    [SerializeField] Rigidbody _rb;
    [SerializeField] SphereCollider _collider;

    [SerializeField] Color _badColor;
    [SerializeField] Color _goodColor;

    [SerializeField] AudioClip _damaged;


    public bool isGood = false;

    private void Start()
    {
        
    }
    private void OnEnable()
    {
        _characterController.enabled = true;
        _enemyAnimator.enabled = true;
        _agent.enabled = true;
        _rb.isKinematic = true;
        _collider.enabled = false;
        _health = _maxHealth;
        transform.rotation = Quaternion.identity;

        Renderer[] rens = GetComponentsInChildren<Renderer>();

        for (int i = 0; i < rens.Length; i++)
        {
            rens[i].material.color = _badColor;
        }

        isGood = false;

        NavMeshHit myNavHit;
        if (NavMesh.SamplePosition(transform.position, out myNavHit, 100, -1))
        {
            _agent.Warp(myNavHit.position);
        }

        _player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Bonk(float value, int damage)
    {
        if (isGood) return;

        if (value > Constants.BONK_LOWER && value < Constants.BONK_UPPER)
        {
            TurnGood();

        }
        else if (value > Constants.BONK_UPPER)
        {
            _health -= damage * 2;
            KnockDown();

        }
        else
        {
            _health -= damage;
        }

        ClampHealth();

        SFXPlayer player = ObjectPool.Instance.GetSFXPlayer();
        player.Play(_damaged, transform.position);

        GameObject hitEffect = ObjectPool.Instance.GetHitEffect();
        hitEffect.transform.position = transform.position;

    }

    private void ClampHealth()
    {
        _health = Mathf.Clamp(_health, 0, _maxHealth);
        if (_health <= 0)
        {
            GameObject effect = ObjectPool.Instance.GetBloodEffect();
            if (UnityEngine.Random.Range(0, 1f) < Constants.PICKUP_CHANCE)
            {
                GameObject pickup = ObjectPool.Instance.GetRandomPickup();
                pickup.transform.position = transform.position;
            }

            effect.transform.position = transform.position;
            _collider.enabled = true;

            GameManager.Instance.KilledOne(this);

            this.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!GameManager.Instance.gameIsRunning) return;

        if (!isGood && _agent.enabled)
        {
            _agent.SetDestination(_player.transform.position);

            _enemyAnimator.SetBool("moving", true);

        }
        else
        {
            _enemyAnimator.SetBool("moving", false);
        }

    }

    private void TurnGood()
    {
        isGood = true;

        Renderer[] rens = GetComponentsInChildren<Renderer>();

        for(int i = 0; i < rens.Length; i++)
        {
            rens[i].material.color = _goodColor;
        }

        _characterController.enabled = false;
        _enemyAnimator.enabled = false;
        _agent.enabled = false;
        _rb.isKinematic = false;
        _collider.enabled = true;

        GameManager.Instance.TurnGoodOne(this);

        
    }

    private void KnockDown()
    {
        StartCoroutine(KnockedDownLoop());
    }

    IEnumerator KnockedDownLoop()
    {
        _characterController.enabled = false;
        _enemyAnimator.enabled = false;
        _agent.enabled = false;
        _rb.isKinematic = false;
        _collider.enabled = true;
        knockedDown = true;
        _rb.AddForce(-transform.forward * 1.2f, ForceMode.Impulse);
        yield return new WaitForSeconds(_knockDownDuration);
        knockedDown = false;
        while (_rb.velocity.magnitude > 2f)
        {
            yield return null;
        }
        _characterController.enabled = true;
        _enemyAnimator.enabled = true;
        _agent.enabled = true;
        _rb.isKinematic = true;
        _collider.enabled = false;

        transform.rotation = Quaternion.identity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(isGood)
            {
                return;
            }
            PlayerController controller = other.GetComponent<PlayerController>();
            controller.Damage(10f);
            controller.PushBack((transform.forward).normalized * 1.2f);
        }

        if (other.CompareTag("VerticalLimit"))
        {
            GameManager.Instance.KilledOne(this);

            this.gameObject.SetActive(false);
        }
        
    }

    public void Hit(float value, int damage)
    {

        GameObject hitEffect = ObjectPool.Instance.GetHitEffect();
        hitEffect.transform.position = transform.position;

        SFXPlayer player = ObjectPool.Instance.GetSFXPlayer();
        player.Play(_damaged, transform.position);

        if (isGood)
        {
            _rb.isKinematic = false;
            return;
        }

        if(value > Constants.BONK_UPPER)
        {
            _health -= 2 * _health;
        }
        else if (value > Constants.BONK_LOWER && value < Constants.BONK_UPPER)
        {
            _health -= damage;
        }
        else
        {
            _health -= damage / 2;
        }

        ClampHealth();

        
    }
}
