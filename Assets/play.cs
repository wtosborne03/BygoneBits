using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Audio;

// A behaviour that is attached to a playable
public class play : PlayableBehaviour
{
    public AudioMixer am;
    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
        am = Resources.Load<AudioMixer>("main");
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        am.SetFloat("musVol", -80);
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        am.SetFloat("musVol", 0);
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }
}
