using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPickup : TurnablePickup
{
    [SerializeField] float _multiplier = 2f;
    [SerializeField] float _duration = 5f;

    [SerializeField] AudioClip _speedSound;
    [SerializeField] AudioClip _slowSound;
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (mode == PickupMode.GOOD)
            {
                pc.SpeedChange(_duration,_multiplier);

                ObjectPool.Instance.GetSFXPlayer().Play(_speedSound, transform.position, true);

            }

            else if (mode == PickupMode.BAD)
            {
                pc.SpeedChange(_duration, 1f / _multiplier);

                ObjectPool.Instance.GetSFXPlayer().Play(_slowSound, transform.position, true);
            }

            gameObject.SetActive(false);
        }
    }
}
