using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Constants
{
    public const float BONK_LOWER = 0.7f;
    public const float BONK_UPPER = 0.9f;
    public const float PICKUP_CHANCE = 0.3f;
}
[System.Serializable]

public enum WeaponState { IDLE, SMASH_BUILDUP, SMASH, SWING_BUILDUP, SWING};
public class PlayerController : MonoBehaviour
{
    [SerializeField] Camera _playerCamera;

    [SerializeField] CharacterController _characterController;
    [SerializeField] float _moveSpeed = 10f;

    [SerializeField] float _playerHealth = 100f;
    public Action<float> OnPlayerHealthUpdate;

    [SerializeField] float _smashProgress = 0f;
    [SerializeField] float _smashSpeed = 0.1f;
    [SerializeField] float _swingProgress = 0f;
    [SerializeField] float _swingSpeed = 0.1f;
    [SerializeField] int _smashDamage = 5;
    [SerializeField] int _swingDamage = 5;

    [SerializeField] Transform _homePortal;

    private WeaponState _weaponState;


    //DEATH controllers
    [SerializeField] List<Rigidbody> _ragdolls = new List<Rigidbody>();
    [SerializeField] List<Collider> _ragdollColliders = new List<Collider>();

    [SerializeField] bool _boosted = false;

    [SerializeField] TrailRenderer _speedEffect;
    public WeaponState CurrentWeaponState
    {
        get
        {
            return _weaponState;
        }

        set
        {
            _weaponState = value;

            switch(value)
            {
                case WeaponState.SMASH_BUILDUP:
                    SmashBuildupAnimation();
                    break;
                case WeaponState.SMASH:
                    SmashAnimation();
                    break;
                case WeaponState.SWING_BUILDUP:
                    SwingBuildupAnimation();
                    break;
                case WeaponState.SWING:
                    SwingAnimation();
                    break;
            }
        }
    }



    [SerializeField] bool _isGrounded;
    [SerializeField] Transform _groundCheckPoint;
    [SerializeField] float _groundCheckRadius = 0.1f;
    [SerializeField] LayerMask _groundLayers;

    Plane _viewPlane;

    [SerializeField] Vector3 _velocity;
    [SerializeField] float _gravity = 9.8f;

    [SerializeField] GameObject _playerBody;

    [SerializeField] Animator _playerAnimator;


    [SerializeField] SphereCollider _playerHitsphere;
    [SerializeField] LayerMask _hitboxTargetLayers;

    [Header("SFX")]
    [SerializeField] AudioClip _bonkHard;
    [SerializeField] AudioClip _swing;
    [SerializeField] AudioClip _tinkle;


    public Action<float> OnWeaponProgressUpdate;
    
    private void Start()
    {
        _viewPlane = new Plane(Vector3.up, _playerBody.transform.position);
        GameManager.Instance.StartGame();
    }
    private void Update()
    {
        if (!GameManager.Instance.gameIsRunning) return;

        MovementInput();
        LookInput();
        AttackInput();

        _isGrounded = Physics.OverlapSphere(_groundCheckPoint.position, _groundCheckRadius, _groundLayers).Length > 0;
        if(_isGrounded)
        {
            _velocity.y = 0f;
        }
        else
        {
            _velocity.y -= _gravity * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        ProcessMovement();
    }

    private void MovementInput()
    {
        _velocity.x = Input.GetAxisRaw("Horizontal");
        _velocity.z = Input.GetAxisRaw("Vertical");

        

        if(_velocity.magnitude > 0.01f)
        {
            _playerAnimator.SetBool("moving", true);
        }
        else
        {
            _playerAnimator.SetBool("moving", false);
        }

        _velocity = _velocity.normalized;
    }

    private void ProcessMovement()
    {
        _characterController.Move(_velocity * _moveSpeed * Time.deltaTime);
    }


    private void LookInput()
    {
        Ray mouseRay = _playerCamera.ScreenPointToRay(Input.mousePosition);
        _viewPlane = new Plane(Vector3.up, _playerBody.transform.position);
        _viewPlane.Raycast(mouseRay, out float enter);

        Vector3 lookPos = mouseRay.GetPoint(enter);
        _playerBody.transform.LookAt(lookPos);
    }


    private void AttackInput()
    {
        
        if(Input.GetMouseButtonDown(0))
        {
            CurrentWeaponState = WeaponState.SMASH_BUILDUP;
        }

        else if (Input.GetMouseButtonDown(1))
        {
            CurrentWeaponState = WeaponState.SWING_BUILDUP;
        }

        if(Input.GetMouseButton(0) && CurrentWeaponState == WeaponState.SMASH_BUILDUP)
        {
            _smashProgress += Time.deltaTime * _smashSpeed;
            OnWeaponProgressUpdate?.Invoke(_smashProgress);
        }
        else if(Input.GetMouseButton(1) && CurrentWeaponState == WeaponState.SWING_BUILDUP)
        {
            _swingProgress += Time.deltaTime * _swingSpeed;
            OnWeaponProgressUpdate?.Invoke(_swingProgress);
        }


        if (Input.GetMouseButtonUp(0) || _smashProgress >= 1f)
        {
            CurrentWeaponState = WeaponState.SMASH;
            
            
        }
        else if(Input.GetMouseButtonUp(1) || _swingProgress >= 1f)
        {
            CurrentWeaponState = WeaponState.SWING;
            _swingProgress = 0f;
        }

        
    }

    private void SmashHitbox()
    {
        
        Collider[] hits = Physics.OverlapSphere(_playerHitsphere.transform.position, _playerHitsphere.radius, _hitboxTargetLayers);

        List<IBonkable> bonkedAlready = new List<IBonkable>();
        if(hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].TryGetComponent<IBonkable>(out IBonkable bonkable) && (hits[i].CompareTag("Enemy") || hits[i].CompareTag("Pickup") || hits[i].CompareTag("Boss")))
                {
                    if(!bonkedAlready.Contains(bonkable))
                    {
                        bonkable.Bonk(Mathf.Clamp01(_smashProgress), _smashDamage);
                        bonkedAlready.Add(bonkable);
                    }
                    
                }

            }
        }

        
        SFXPlayer player = ObjectPool.Instance.GetSFXPlayer();

        player.Play(_bonkHard, _playerHitsphere.transform.position, true, 0.2f);
        if(_smashProgress > Constants.BONK_LOWER && _smashProgress < Constants.BONK_UPPER)
        {
            player.Play(_tinkle, _playerHitsphere.transform.position, true, 0.1f);
        }

        _smashProgress = 0f;


    }

    private void SwingHitbox()
    {
        Collider[] hits = Physics.OverlapSphere(_playerHitsphere.transform.position, _playerHitsphere.radius, _hitboxTargetLayers);

        List<IBonkable> swingedAlready = new List<IBonkable>();
        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].TryGetComponent<Rigidbody>(out Rigidbody rb) && (hits[i].CompareTag("Enemy") || hits[i].CompareTag("Pickup") || hits[i].CompareTag("Pickup")))
                {
                    if(hits[i].TryGetComponent<EnemyMinion>(out EnemyMinion minion))
                    {
                        if (!minion.isGood)
                            rb.AddForce((hits[i].transform.position - transform.position).normalized * 20f, ForceMode.Impulse);
                        else
                            rb.AddForce((_homePortal.position - transform.position).normalized * 20f, ForceMode.Impulse);




                        minion.Hit(_swingProgress, _swingDamage);
                        swingedAlready.Add(minion);
                    }

                    else if(hits[i].CompareTag("Pickup") && hits[i].TryGetComponent(out TurnablePickup pickup))
                    {
                       
                        if(pickup.mode == PickupMode.BAD)
                        {
                            rb.isKinematic = false;
                            rb.AddForce((hits[i].transform.position - transform.position).normalized * 20f, ForceMode.Impulse);
                        }
                        pickup.Hit(_swingProgress, _swingDamage);

                        swingedAlready.Add(pickup);
                    }

                    else if (hits[i].CompareTag("Boss") && hits[i].TryGetComponent(out Boss boss))
                    {
                        boss.Hit(_swingProgress, _swingDamage);
                        swingedAlready.Add(boss);
                    }
                    
                }

            }
        }

        
        SFXPlayer player = ObjectPool.Instance.GetSFXPlayer();
        player.Play(_swing, _playerHitsphere.transform.position, true, 0.2f);

        if (_swingProgress > Constants.BONK_LOWER && _swingProgress < Constants.BONK_UPPER)
        {
            ObjectPool.Instance.GetSFXPlayer().Play(_tinkle, _playerHitsphere.transform.position, true, 0.1f);
        }

        _swingProgress = 0f;

        
        
    }
    
    private void SmashBuildupAnimation()
    {
        _playerAnimator.SetBool("smashBuildup", true);
        _playerAnimator.SetBool("smashing", false);
    }
    private void SmashAnimation()
    {
        _playerAnimator.SetBool("smashBuildup", false);
        _playerAnimator.SetBool("smashing", true);
    }
    private void SwingBuildupAnimation()
    {
        _playerAnimator.SetBool("swingBuildup", true);
        _playerAnimator.SetBool("swinging", false);
    }
    private void SwingAnimation()
    {
        _playerAnimator.SetBool("swingBuildup", false);
        _playerAnimator.SetBool("swinging", true);
    }


    public void Damage(float value)
    {
        _playerHealth -= value;
        ClampHealth();
        OnPlayerHealthUpdate?.Invoke(_playerHealth);

        
    }

    public void Heal(float value)
    {
        _playerHealth += value;
        ClampHealth();

        OnPlayerHealthUpdate?.Invoke(_playerHealth);
    }

    private void ClampHealth()
    {
        _playerHealth = Mathf.Clamp(_playerHealth, 0f, 100f);

        if(_playerHealth == 0f)
        {

            StartCoroutine(DeathCoroutine());
        }
    }

    IEnumerator DeathCoroutine()
    {
        this.enabled = false;
        _playerHitsphere.gameObject.SetActive(false);
        _playerAnimator.enabled = false;
        _characterController.enabled = false;
        
        foreach(Rigidbody r in _ragdolls)
        {
            r.isKinematic = false;
            
        }
        foreach(Collider col in _ragdollColliders)
        {
            col.enabled = true;
        }
        yield return new WaitForSeconds(5f);
        GameManager.Instance.GameOver();
    }


    public void PushBack(Vector3 dir)
    {
        _characterController.Move(dir * 3f);
    }

    public void SpeedChange(float duration, float mul)
    {
        if(!_boosted)
            StartCoroutine(SpeedTimer(duration, mul));
    }

    IEnumerator SpeedTimer(float duration, float mul)
    {
        _boosted = true;
        float prev = _moveSpeed;
        _moveSpeed *= mul;
        _speedEffect.enabled = true;
        yield return new WaitForSeconds(duration);
        _speedEffect.enabled = false;
        _moveSpeed = prev;

        _boosted = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("VerticalLimit"))
        {
            GameManager.Instance.GameOver();

        }
    }
}
