using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lagger : MonoBehaviour
{
    public Transform cam;
    public float speed;
    public float fSpeed;
    Rigidbody rb;

    void Start()
    {
        //cam = transform.parent;
        //rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //rb.position = Vector3.Slerp(rb.position, cam.position, fSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, cam.rotation, speed * Time.deltaTime);
    }
}
