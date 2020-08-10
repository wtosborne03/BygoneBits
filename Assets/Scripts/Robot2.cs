using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Robot2 : MonoBehaviour
{
    Rigidbody rb;
    public float speed;
    Transform player;
    public float eye;
    public float knockback = 40;
    private float defendstamp;
    Animator anim;
    public float health = 3;
    private AudioSource aud;
    public AudioClip hurtsound;
    public Transform bullet;
    public Transform bulletpoint;
    public float headspeed = 2;
    float shootstamp;
    public AudioClip shootsound;
    void Start()
    {
        aud = GetComponent<AudioSource>();
        anim = transform.GetChild(0).gameObject.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").transform;
    }
    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist < eye)
        {
            if (shootstamp < Time.time)
            {
                Instantiate(bullet, bulletpoint.position, bulletpoint.rotation);
                aud.PlayOneShot(shootsound);
                anim.SetTrigger("shoot");
                shootstamp = Time.time + 2f;
            }
            //transform.LookAt(player); (Phased out with below)
            //transform.rotation = (Quaternion.Slerp(transform.rotation, LookAt(transform.position, player.position), Time.deltaTime * headspeed));
            rb.AddTorque(Vector3.Cross(transform.forward, player.position - transform.position) * 5f, ForceMode.Force);
            if (dist > 16 || true)
            {
                rb.AddForce((player.position - transform.position).normalized * speed * Time.deltaTime);
            }
        }
    }
    IEnumerator death()
    {
        GetComponent<SphereCollider>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        Instantiate(Resources.Load<Transform>("expo"), transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("hb_attack") && defendstamp < Time.time)
        {
            aud.PlayOneShot(hurtsound);
            health -= 1;
            defendstamp = Time.time + 0.5f;
            anim.SetTrigger("hurt");
            
            if (health < 1)
            {
                rb.AddForce((player.position - transform.position).normalized * knockback * -2);
                StartCoroutine(death());
            } else
            {

                rb.AddForce((player.position - transform.position).normalized * knockback * -1);
            }
        }
        if (coll.CompareTag("explosion"))
        {
            aud.PlayOneShot(hurtsound);
            anim.SetTrigger("hurt");
                rb.AddForce((coll.transform.position - transform.position).normalized * knockback * -2);
                StartCoroutine(death());

        }
    }
    public static Quaternion LookAt(Vector3 sourcePoint, Vector3 destPoint)
    {
        Vector3 forwardVector = Vector3.Normalize(destPoint - sourcePoint);

        float dot = Vector3.Dot(Vector3.forward, forwardVector);

        if (Mathf.Abs(dot - (-1.0f)) < 0.000001f)
        {
            return new Quaternion(Vector3.up.x, Vector3.up.y, Vector3.up.z, 3.1415926535897932f);
        }
        if (Mathf.Abs(dot - (1.0f)) < 0.000001f)
        {
            return Quaternion.identity;
        }

        float rotAngle = (float)Mathf.Acos(dot);
        Vector3 rotAxis = Vector3.Cross(Vector3.forward, forwardVector);
        rotAxis = Vector3.Normalize(rotAxis);
        return CreateFromAxisAngle(rotAxis, rotAngle);
    }

    // just in case you need that function also
    public static Quaternion CreateFromAxisAngle(Vector3 axis, float angle)
    {
        float halfAngle = angle * .5f;
        float s = (float)Mathf.Sin(halfAngle);
        Quaternion q;
        q.x = axis.x * s;
        q.y = axis.y * s;
        q.z = axis.z * s;
        q.w = (float)Mathf.Cos(halfAngle);
        return q;
    }
}
