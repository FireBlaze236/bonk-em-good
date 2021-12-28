using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    [SerializeField] AudioSource source;
    public void Play(AudioClip clip, Vector3 pos, bool randomPitch= false, float variation = 0.1f)
    {
        
        source = GetComponent<AudioSource>();

        if (source.isPlaying)
            source.Stop();

        gameObject.transform.position = pos;

        if(randomPitch)
        {
            source.pitch = 1f + UnityEngine.Random.Range(-variation, variation);
        }else
        {
            source.pitch = 1f;
        }

        source.clip = clip;


        source.Play();

        StartCoroutine(DisablePlayer(source.clip.length + 0.1f));
        
    }

    IEnumerator DisablePlayer(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
