using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pisadas : MonoBehaviour
{
    [SerializeField]
    private AudioClip pisada;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();        
    }

    private void Step()
    {
        audioSource.pitch = Random.Range(0.8f, 1.5f);        
        audioSource.PlayOneShot(pisada);
    }
}
