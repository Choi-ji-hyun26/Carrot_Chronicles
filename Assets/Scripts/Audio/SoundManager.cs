using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource bgmSource;
    public AudioMixer bgmMixer;

    public AudioSource sfxSource;
    public AudioMixer sfxMixer;
    public AudioClip[] audioClips;

    public float bgmVolume = 1f;
    public float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SetBgmVolume(float value)
    {
        bgmVolume = value;
        bgmMixer.SetFloat("Volume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = value;
        sfxMixer.SetFloat("Volume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
    }

    public void PlaySound(string type)
    {
        int index = type switch
        {
            "JUMP" => 0,
            "ATTACK" => 1,
            "DAMAGED" => 2,
            "DIE" => 3,
            "ITEM" => 4,
            "FINISH" => 5,
            _ => -1
        };

        if (index >= 0 && index < audioClips.Length)
        {
            sfxSource.PlayOneShot(audioClips[index]);
        }
    }
}
