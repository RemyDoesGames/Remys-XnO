using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    private AudioSource audioSource;


    private void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
    }
    private void Start()
    {
        Play("bm");
        audioSource = GameObject.Find("AudioManager").GetComponent<AudioSource>();
    }

    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    public float FadeVolume(float vol)
    {
        if (GameManager.Instance.musicMuted)
        {
            return 0;
        }
        else
        {
            return Mathf.Lerp(audioSource.volume, vol, Time.deltaTime);
        }
    }
}
