using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin : MonoBehaviour
{
    public Vector3 rot;
    void Update()
    {
        transform.rotation = transform.rotation  * (Quaternion.Euler(rot * Time.deltaTime));
    }
}
