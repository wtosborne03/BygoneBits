using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosive : MonoBehaviour
{
    public Transform explosion;
    private AudioSource beep;
    Animator anim;
    bool done = false;
    // Start is called before the first frame update
    void Start()
    {
        beep = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }
    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("hb_attack") || coll.gameObject.CompareTag("explosion"))
        {
            StartCoroutine(explode());
        }
    }
    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("harm"))
        {
            StartCoroutine(explode());
        }
    }
    IEnumerator explode()
    {
        if (done) yield break;
        done = true;
        beep.Play();
        anim.SetTrigger("fuse");
        yield return new WaitForSeconds(1);
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}
