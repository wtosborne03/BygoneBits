using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ConfigurableJoint[] _legs;

    [Header("Animation-Curve")]
    [SerializeField] private AnimationCurve _legCurve;
    [SerializeField] private float _speed;
    private float _legRotation;
    private float _timer;
    List<Quaternion> _initialRotation;
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
    public float headspeed = 2;
    void Start()
    {
        aud = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        _initialRotation = new List<Quaternion>();
        for (int i = 0; i < _legs.Length; i++)
        {
            _initialRotation.Add(_legs[i].transform.localRotation);
        }
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").transform;
    }
    void Update()
    {
        _timer += Time.deltaTime * _speed * rb.velocity.magnitude;

        _legRotation = _legCurve.Evaluate(_timer);

        _legs[0].SetTargetRotationLocal(Quaternion.Euler(_legRotation, 0, 0), _initialRotation[0]);
        _legs[1].SetTargetRotationLocal(Quaternion.Euler(_legRotation*2, 0, 0), _initialRotation[1]);

        _legs[2].SetTargetRotationLocal(Quaternion.Euler(-_legRotation, 0, 0), _initialRotation[2]);
        _legs[3].SetTargetRotationLocal(Quaternion.Euler(-_legRotation * 2, 0, 0), _initialRotation[3]);
        if (Vector3.Distance(transform.position, player.position) < eye)
        {
            //transform.LookAt(player); (Phased out with below)
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(player.position - rb.position), Time.deltaTime * 2));
            rb.AddForce((player.position - transform.position).normalized * speed * Time.deltaTime);
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
                rb.AddForce((player.position - transform.position).normalized * knockback * -3);
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
                rb.AddForce((coll.transform.position - transform.position).normalized * knockback * -3);
                StartCoroutine(death());

        }
    }
}
