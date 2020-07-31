using Lightbug.GrabIt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class collects : MonoBehaviour
{
    public GrabIt git;
    public Text num;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        num.text = git.apples.ToString("00");
    }
}
