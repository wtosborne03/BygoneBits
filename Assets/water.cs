using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class water : MonoBehaviour
{
    public Transform effect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider coll)
    {
        if (!coll.gameObject.isStatic)
        {
            Instantiate(effect, coll.transform.position, Quaternion.Euler(90, 0, 0));
        }
    }
}
