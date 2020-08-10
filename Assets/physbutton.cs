using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class physbutton : MonoBehaviour
{
    public bool time = false;
    public UnityEvent act;
    public Material on;
    public Material off;
    public MeshRenderer buttSurf;
    AudioSource ding;
    float timestamp;
    public void press()
    {
        if (timestamp < Time.time || !time)
        {
            ding = GetComponent<AudioSource>();
            if (buttSurf.material == on)
            {
                buttSurf.material = off;
            } else
            {
                buttSurf.material = on;
            }
            ding.Play();
            act.Invoke();
            timestamp = Time.time + 0.5f;
        }
    }
}
