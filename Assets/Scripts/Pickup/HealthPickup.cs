using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : TurnablePickup
{
    [SerializeField] float _healAmount = 25f;
    [SerializeField] float _damageAmount = 10f;

    [SerializeField] AudioClip _damageSound;
    [SerializeField] AudioClip _healSound;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if(other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if(mode == PickupMode.GOOD)
            {
                pc.Heal(_healAmount);
                ObjectPool.Instance.GetSFXPlayer().Play(_healSound, transform.position, true);
            }

            else if(mode == PickupMode.BAD)
            {
                pc.Damage(_damageAmount);
                ObjectPool.Instance.GetSFXPlayer().Play(_damageSound, transform.position, true);
            }

            gameObject.SetActive(false);
        }
    }
}
