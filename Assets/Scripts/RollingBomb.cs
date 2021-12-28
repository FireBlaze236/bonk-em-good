using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBomb : MonoBehaviour
{
    [SerializeField] bool _following = true;
    [SerializeField] bool _swinged = false;
    
    [SerializeField] PlayerController _player;
    [SerializeField] float _moveForce = 10f;

    private Rigidbody _rb;


    
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        _following = true;
        _swinged = false;

    }

    private void Update()
    {
        if(_following)
        {
            Vector3 dir = _player.transform.position - transform.position;
            _rb.AddForce(dir * _moveForce * Time.deltaTime);
        }
            
    }
}
