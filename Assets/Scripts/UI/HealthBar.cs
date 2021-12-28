using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] PlayerController _playerController;

    [SerializeField] Slider _healthBar;
    private void OnEnable()
    {
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _playerController.OnPlayerHealthUpdate += UpdateHealthBar;
    }

    private void UpdateHealthBar(float value)
    {
        _healthBar.value = value;
    }
}
