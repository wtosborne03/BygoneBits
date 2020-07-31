using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class audioControl : MonoBehaviour
{
    public AudioMixer am;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AudioIn()
    {
        am.SetFloat("musVol", 0);
    }
    public void AudioOut()
    {
        am.SetFloat("musVol", -80);
    }
}
