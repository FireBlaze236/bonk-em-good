using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerParticles : MonoBehaviour
{
    [SerializeField] ParticleSystem _bonkEffect;
    [SerializeField] PlayerController _playerController;

    private void OnEnable()
    {
        _playerController.OnWeaponProgressUpdate += CheckParticleEffect;
    }

    private void CheckParticleEffect(float val)
    {
        if(val > 0.7 && val < 0.9f && !_bonkEffect.isPlaying)
        {
            _bonkEffect.Play();
        }
        else if (_bonkEffect.isPlaying)
        {
            _bonkEffect.Stop();
        }
    }
}
