using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonFX : MonoBehaviour {

    public AudioSource myFx;
    public AudioClip hoverFx;
    public AudioClip clickFx;


    public void HoverSound()
    {
        SoundManager.PlaySFX(hoverFx, false, 0.5f);
        //myFx.PlayOneShot(hoverFx);
    }
    public void ClickSound()
    {
        SoundManager.PlaySFX(clickFx, false, 0.5f);
        //myFx.PlayOneShot(clickFx);
    }

}
