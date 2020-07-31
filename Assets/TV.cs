using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TV : MonoBehaviour
{
    VideoPlayer vp;
    void Start()
    {
        vp = GetComponent<VideoPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }public void click() { if (!vp.isPlaying) { vp.Play(); } else { vp.Pause(); } }
}
