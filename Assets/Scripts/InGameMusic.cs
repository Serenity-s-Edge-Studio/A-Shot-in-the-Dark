using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMusic : MonoBehaviour
{
    public AudioSource source;
    public AudioClip intro;
    public AudioClip loop;
    private void Start()
    {
        source.clip = intro;
        source.Play();
        Invoke("playLoop", intro.length);
    }
    private void playLoop()
    {
        source.clip = loop;
        source.loop = true;
        source.Play();
    }
}
