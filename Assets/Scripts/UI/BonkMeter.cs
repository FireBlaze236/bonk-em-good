using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonkMeter : MonoBehaviour
{
    [SerializeField] PlayerController _playerController;

    [SerializeField] Slider _bonkMeter;
    [SerializeField] Image _bonkMeterFill;
    [SerializeField] Color _minimalBonkColor;
    [SerializeField] Color _medianBonkColor;
    [SerializeField] Color _omegaBonkColor;
    [SerializeField] Color _overBonkColor;



    private void OnEnable()
    {
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _playerController.OnWeaponProgressUpdate += UpdateBonkMeter;
    }
    
    private void UpdateBonkMeter(float val)
    {
        _bonkMeter.value = val;
        

        if(val > Constants.BONK_UPPER)
        {
            _bonkMeterFill.color = _overBonkColor;
        }
        else if (val > Constants.BONK_LOWER)
        {
            _bonkMeterFill.color = _omegaBonkColor;
        }
        else if (val > 0.3f)
        {
            _bonkMeterFill.color = _medianBonkColor;
        }
        else
        {
            _bonkMeterFill.color = _minimalBonkColor;
        }
    }
}
