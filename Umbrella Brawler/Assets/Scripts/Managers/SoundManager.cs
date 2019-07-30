using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyUtility; //DON'T FORGET THIS

public class SoundManager : MonoBehaviour
{
//------------------------------------------------------------------------CONSTANTS:

	public bool VERBOSE = false;

	// Static Manager Instance
    static SoundManager _instance;

//---------------------------------------------------------------------------FIELDS:
	// Volume Variables
    const float maxVolume_BGM = 1f;
    const float maxVolume_SFX = 1f;
    static float currVolNormalized_BGM = 1f;
    static float currVolNormalized_SFX = 1f;
    static bool isMuted = false;

	// Audio Sources
    List<AudioSource> sfxSources;
    public AudioSource bgmSource;
//---------------------------------------------------------------------MONO METHODS:

//--------------------------------------------------------------------------METHODS:
	public static SoundManager GetInstance()
    {
        if (!_instance)
        {
            GameObject soundManager = new GameObject("SoundManager");
            _instance = soundManager.AddComponent<SoundManager>();
            _instance.Initialize();
        }
        return _instance;
    }

	// For adding background music sources
    private void Initialize()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = GetBGMVolume();
        DontDestroyOnLoad(gameObject);
    }

	// Functions to Play and Stop background music.
    public static void PlayBGM(AudioClip bgmClip)
    {
        SoundManager soundMan = GetInstance();

        soundMan.bgmSource.volume = GetBGMVolume();
        soundMan.bgmSource.clip = bgmClip;
        soundMan.bgmSource.Play();
    }

    public static void StopBGM()
    {
        SoundManager soundMan = GetInstance();
        if (soundMan.bgmSource.isPlaying)
            soundMan.bgmSource.Stop();
    }

	// Sets up a sound source for every sound effect clip
    AudioSource GetSFXSource()
    {
        AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = GetSFXVolume();

        if (sfxSources == null)
            sfxSources = new List<AudioSource>();

        sfxSources.Add(sfxSource);
        return sfxSource;
    }

    // Cleans up an audio source after playing.
    IEnumerator RemoveSFXSource(AudioSource sfxSource)
    {
        if (sfxSource != null)
        {
            yield return new WaitForSeconds(sfxSource.clip.length);
            sfxSources.Remove(sfxSource);
            Destroy(sfxSource);
        }
    }

	// Stops an audio source at the specified time.
    IEnumerator RemoveSFXSourceLength(AudioSource sfxSource, float length)
    {
        yield return new WaitForSeconds(length);
        sfxSources.Remove(sfxSource);
        Destroy(sfxSource);
    }

    // Plays a sound effect clip. Pass randomize as true to randomize the sound's pitch.
    public static void PlaySFX(AudioClip sfxClip, bool randomPitch, float modifyVolume)//modifyVolume is a normalized value----   ie(.5 = 50%)
    {
        SoundManager soundMan = GetInstance();
        AudioSource source = soundMan.GetSFXSource();

        source.volume = GetSFXVolume();
        source.clip = sfxClip;
        source.volume = source.volume * modifyVolume;

        if (randomPitch)
            source.pitch = Random.Range(0.85f, 1.2f);

        source.Play();
        soundMan.StartCoroutine(soundMan.RemoveSFXSource(source));
    }

	// Muting & Unmuting Functions
    public static void DisableSoundImmediate()
    {
        SoundManager soundMan = GetInstance();
        soundMan.StopAllCoroutines();

        if (soundMan.sfxSources != null)
        {
            foreach (AudioSource source in soundMan.sfxSources)
                source.volume = 0;
        }
        soundMan.bgmSource.volume = 0f;
        isMuted = true;
    }

    public static void EnableSoundImmediate()
    {
        SoundManager soundMan = GetInstance();
        
        if (soundMan.sfxSources != null)
        {
            foreach (AudioSource source in soundMan.sfxSources)
                source.volume = GetSFXVolume();
        }
        soundMan.bgmSource.volume = GetBGMVolume();
        isMuted = false;
    }
//--------------------------------------------------------------------------HELPERS:

	// Volume Getters
    static float GetBGMVolume()
    {
        if (isMuted)
            return 0f;
        else
            return (maxVolume_BGM * currVolNormalized_BGM);
    }

    static float GetSFXVolume()
    {
        if (isMuted)
            return 0f;
        else
            return (maxVolume_SFX * currVolNormalized_SFX);
    }

	// Volume Control
	public static void SetGlobalVolume(float newVolume)
    {
        currVolNormalized_BGM = newVolume;
        currVolNormalized_SFX = newVolume;
        AdjustSoundImmediate();
    }

    public static void SetSFXVolume(float newVolume)
    {
        currVolNormalized_SFX = newVolume;
        AdjustSoundImmediate();
    }

    public static void SetBGMVolume(float newVolume)
    {
        currVolNormalized_BGM = newVolume;
        AdjustSoundImmediate();
    }

	public static void AdjustSoundImmediate()
    {
        SoundManager soundMan = GetInstance();
        if (soundMan.sfxSources != null)
        {
            foreach (AudioSource source in soundMan.sfxSources)
                source.volume = GetSFXVolume();
        }
        soundMan.bgmSource.volume = GetBGMVolume();
    }

	// Debug Log
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}