using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioMixer bgmMixer;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioMixer sfxMixer;
    [SerializeField] private AudioClip[] audioClips;

    public float bgmVolume = 0.8f; // public : SetAudio 호출
    public float sfxVolume = 0.15f; // public : SetAudio 호출

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

    public void SetBgmVolume(float value) // public : SetAudio 호출
    {
        bgmVolume = value;
        bgmMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
    }

    public void SetSfxVolume(float value) // public : SetAudio 호출
    {
        sfxVolume = value;
        sfxMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
    }

    public void PlaySound(string type) // public : Player 관련 스크립트 호출
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
