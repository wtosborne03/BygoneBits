using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class clearcondition : MonoBehaviour
{
    public List<GameObject> defeatables;
    bool done = true;
    public UnityEvent onwin;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("check", 0.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }public void check()
    {
        done = true;
        foreach(GameObject g in defeatables)
        {
            if (g != null)
            {
                done = false;
                break;
            }
        }
        //win --v
        if (done)
        {
            onwin.Invoke();
        }
    }
}
