using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] AudioClip enemyShootingClip;
    [SerializeField] [Range(0f, 1f)] float enemyShootingVol = 1f;
    [SerializeField] AudioClip playerShootingClip;
    [SerializeField] [Range(0f,1f)] float playerShootingVol = 1f;

    [Header("Damage")]
    [SerializeField] AudioClip damageClip;
    [SerializeField] [Range(0f, 1f)] float damageVolume = 1f;

    [Header("Shield")]
    [SerializeField] AudioClip shieldOnClip;
    [SerializeField] AudioClip shieldOffClip;
    [SerializeField][Range(0f, 1f)] float shieldVolume = 1f;

    [Header("UI")]
    [SerializeField] AudioClip selectButtonClip;
    [SerializeField] AudioClip pressButtonClip;
    [SerializeField] AudioClip openPauseClip;
    [SerializeField] AudioClip closePauseClip;
    [SerializeField][Range(0f, 1f)] float uiVolume = 1f;

    [Header("Music")]
    [SerializeField] AudioClip menuMusic;
    [SerializeField] AudioClip gameMusic;
    [SerializeField] private AudioSource source;

    static AudioPlayer instance;

    private void Awake()
    {
        ManageSingleton();
    }

    void ManageSingleton()
    {
        if(instance!= null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    static public void PlayMenuMusic(int prevSceneIndex)
    {
        if (instance != null)
        {
            if (instance.source != null && prevSceneIndex != 4)
            {
                instance.source.Stop();
                instance.source.clip = instance.menuMusic;
                instance.source.Play();
            }
            else if (prevSceneIndex == 4)
            {
                return;
            }
            else
            {
                Debug.LogError("Unavailable MusicPlayer component");
            }
        }
    }

    static public void PlayGameMusic()
    {
        if (instance != null)
        {
            if (instance.source != null)
            {
                instance.source.Stop();
                instance.source.clip = instance.gameMusic;
                instance.source.Play();
            }
            else
            {
                Debug.LogError("Unavailable MusicPlayer component");
            }
        }
    }

    public void PlayEnemyShootingClip()
    {
        PlayClip(enemyShootingClip, enemyShootingVol);
    }

    public void PlayPlayerShootingClip()
    {
        PlayClip(playerShootingClip, playerShootingVol);
    }

    public void PlayShieldOnClip()
    {
        PlayClip(shieldOnClip, shieldVolume);
    }

    public void PlayShieldOffClip()
    {
        PlayClip(shieldOffClip, shieldVolume);
    }

    public void PlayDamageClip()
    {
        PlayClip(damageClip, damageVolume);
    }

    public void PlaySelectButtonClip()
    {
        PlayClip(selectButtonClip, uiVolume);
    }

    public void PlayPressButtonClip()
    {
        PlayClip(pressButtonClip, uiVolume);
    }

    public void PlayOpenPauseClip()
    {
        PlayClip(openPauseClip, uiVolume);
    }

    public void PlayClosePauseClip()
    {
        PlayClip(closePauseClip, uiVolume);
    }

    private void PlayClip(AudioClip clip, float volume)
    {
        if (clip != null)
        {
            Vector3 cameraPos = Camera.main.transform.position;
            AudioSource.PlayClipAtPoint(clip, cameraPos, volume);
        }
    }
}
