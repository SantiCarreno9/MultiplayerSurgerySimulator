using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonidosCirugia : MonoBehaviour
{
    [SerializeField]
    private AudioClip Corte, extraccionLipoma;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySonido(byte sonido)
    {
        if (sonido==0)
        {
            if(!audioSource.isPlaying) audioSource.PlayOneShot(Corte);
        }
        else
        {
            if (!audioSource.isPlaying) audioSource.PlayOneShot(extraccionLipoma);
        }
    }

}
