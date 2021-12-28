using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupMode { GOOD, BAD }
public class TurnablePickup : MonoBehaviour, IBonkable
{
    public PickupMode mode = PickupMode.BAD;
    [SerializeField] float _rotationSpeed = 30f;
    [SerializeField] bool _rotating = false;

    [SerializeField] GameObject _goodObj;
    [SerializeField] GameObject _badObj;


    [SerializeField] AudioClip _swingSound;

    private void OnEnable()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        transform.rotation = Quaternion.identity;
        _rotating = true;

        TurnBad();
    }
    private void Update()
    {
        if(_rotating)
           transform.Rotate(new Vector3(0, Time.deltaTime * _rotationSpeed, 0));
    }

    public void Bonk(float value, int damage)
    {
        if(value > Constants.BONK_LOWER && value < Constants.BONK_UPPER)
        {
            TurnGood();
        }
        
    }

    public void Hit(float value, int damage)
    {
        _rotating = false;
        ObjectPool.Instance.GetSFXPlayer().Play(_swingSound, transform.position, true);

    }


    private void TurnGood()
    {
        mode = PickupMode.GOOD;
        _goodObj.SetActive(true);
        _badObj.SetActive(false);
        GetComponent<Rigidbody>().isKinematic = true;
    }

    private void TurnBad()
    {
        mode = PickupMode.BAD;
        _goodObj.SetActive(false);
        _badObj.SetActive(true);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("VerticalLimit"))
        {
            gameObject.SetActive(false);
        }
    }
}
