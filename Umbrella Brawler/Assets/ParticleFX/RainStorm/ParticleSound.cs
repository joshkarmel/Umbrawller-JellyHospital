using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParticleSound : MonoBehaviour
{

    public AudioSource audioSource1, audioSource2;
    public AudioClip[] Sounds;
    private AudioClip SoundsClip;

    private int _numberOfParticles = 0;

    void Start()
    {

    }

    void Update()
    {

        int count = this.gameObject.GetComponent<ParticleSystem>().particleCount;
        if (count <= _numberOfParticles)
        { //particle has died

        }
        else if (count > _numberOfParticles)
        { //particle has been born
            int index = Random.Range(0, Sounds.Length);
            SoundsClip = Sounds[index];

            if (!audioSource1.isPlaying)
            {
                audioSource1.clip = SoundsClip;
                audioSource1.Play();
            }
            else
            {
                audioSource2.clip = SoundsClip;
                audioSource2.Play();
            }
            

        }
        _numberOfParticles = count;
    }
}
