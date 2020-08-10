using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class bullet : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 1;
    float creation;
    // Start is called before the first frame update
    void Start()
    {
        creation = Time.time + 0.1f;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(transform.forward * speed * Time.deltaTime, ForceMode.VelocityChange);
    }
    void OnCollisionEnter(Collision coll)
    {
        if (creation < Time.time)
        Destroy(this.gameObject);
    }
}
