using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomePortal : MonoBehaviour
{
    [SerializeField] AudioClip _saveSound;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy") && other.TryGetComponent<EnemyMinion>(out EnemyMinion minion))
        {
            if (minion.isGood)
            {
                GameManager.Instance.SavedOne();
                ObjectPool.Instance.GetSFXPlayer().Play(_saveSound, transform.position);

                GameObject effect = ObjectPool.Instance.GetSparkleEffect();
                effect.transform.position = other.transform.position;


                
            }
            else
                return;
            other.gameObject.SetActive(false);

        }


    }
}
