/*
    ███████╗██╗██████╗  ██████╗████████╗  ██████╗ ███████╗██████╗  ██████╗ █████╗ ███╗  ██╗
    ██╔════╝██║██╔══██╗██╔════╝╚══██╔══╝  ██╔══██╗██╔════╝██╔══██╗██╔════╝██╔══██╗████╗ ██║
    █████╗  ██║██████╔╝╚█████╗    ██║     ██████╔╝█████╗  ██████╔╝╚█████╗ ██║  ██║██╔██╗██║
    ██╔══╝  ██║██╔══██╗ ╚═══██╗   ██║     ██╔═══╝ ██╔══╝  ██╔══██╗ ╚═══██╗██║  ██║██║╚████║
    ██║     ██║██║  ██║██████╔╝   ██║     ██║     ███████╗██║  ██║██████╔╝╚█████╔╝██║ ╚███║
    ╚═╝     ╚═╝╚═╝  ╚═╝╚═════╝    ╚═╝     ╚═╝     ╚══════╝╚═╝  ╚═╝╚═════╝  ╚════╝ ╚═╝  ╚══╝

    ██████╗ ██╗      █████╗ ██╗   ██╗███████╗██████╗   ███╗   ███╗ █████╗ ██╗   ██╗███████╗███╗   ███╗███████╗███╗  ██╗████████╗
    ██╔══██╗██║     ██╔══██╗╚██╗ ██╔╝██╔════╝██╔══██╗  ████╗ ████║██╔══██╗██║   ██║██╔════╝████╗ ████║██╔════╝████╗ ██║╚══██╔══╝
    ██████╔╝██║     ███████║ ╚████╔╝ █████╗  ██████╔╝  ██╔████╔██║██║  ██║╚██╗ ██╔╝█████╗  ██╔████╔██║█████╗  ██╔██╗██║   ██║   
    ██╔═══╝ ██║     ██╔══██║  ╚██╔╝  ██╔══╝  ██╔══██╗  ██║╚██╔╝██║██║  ██║ ╚████╔╝ ██╔══╝  ██║╚██╔╝██║██╔══╝  ██║╚████║   ██║   
    ██║     ███████╗██║  ██║   ██║   ███████╗██║  ██║  ██║ ╚═╝ ██║╚█████╔╝  ╚██╔╝  ███████╗██║ ╚═╝ ██║███████╗██║ ╚███║   ██║   
    ╚═╝     ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚══════╝╚═╝  ╚═╝  ╚═╝     ╚═╝ ╚════╝    ╚═╝   ╚══════╝╚═╝     ╚═╝╚══════╝╚═╝  ╚══╝   ╚═╝   

    █▄▄ █▄█   ▀█▀ █ █ █▀▀   █▀▄ █▀▀ █ █ █▀▀ █   █▀█ █▀█ █▀▀ █▀█
    █▄█  █     █  █▀█ ██▄   █▄▀ ██▄ ▀▄▀ ██▄ █▄▄ █▄█ █▀▀ ██▄ █▀▄
*/
using Lightbug.GrabIt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Xml.Linq;
using UnityEngine.UI;

// I use Physics.gravity a lot instead of Vector3.up because you can point the gravity to a different direction and i want the controller to work fine
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    private static Vector3 vecZero = Vector3.zero;
    private Rigidbody rb;
    private CapsuleCollider cc;
    public GrapplingHook grap;
    public CanvasGroup katanscreen;
    public CanvasGroup remoteSreen;
    public CanvasGroup grapScreen;
    public screener sc;
    public bool enableMovement = true;
    public Animator fader;
    AudioSource saudio;
    public GrabIt git;
    public FPSCamera camscript;
    CanvasGroup fg;
    Transform spawnpos;

    [Header("Movement properties")]
    public float walkSpeed = 8.0f;
    public float runSpeed = 12.0f;
    public float changeInStageSpeed = 10.0f; // Lerp from walk to run and backwards speed
    public float maximumPlayerSpeed = 150.0f;
    public float groundDistance = 2.0f;
    [HideInInspector] public float vInput, hInput;
    public Transform groundChecker;
    public GameObject grapplehook;
    public GameObject katana;
    public AudioClip ability;
    [Header("Jump")]
    public float jumpForce = 500.0f;
    public float jumpCooldown = 1.0f;
    private bool jumpBlocked = false;
    float actionstamp;
    private Vector2 move;
    private bool sprint = false;
    private PlayerInput pin;
    public int spawn;
    public Animator hurt;
    public AudioClip hurtsound;
    public float health = 1.0f;
    public Image healthim;
    public float fillspeed;
    private float hurtstamp;
    public AudioSource footsteps;
    public float explospeed = 100;
    public AudioClip[] steps;
    float steptime;
    AsyncOperation levelload;
    private int stepcount = 0;
    private void Start()
    {
        fg = GameObject.Find("ForegroundUI").GetComponent<CanvasGroup>();
        saudio = GetComponent<AudioSource>();
        
        rb = this.GetComponent<Rigidbody>();
        cc = this.GetComponent<CapsuleCollider>();
        pin = transform.parent.gameObject.GetComponent<PlayerInput>();
        if (PlayerPrefs.GetInt("New") == 0)
        {
            levelload =
            SceneManager.LoadSceneAsync(PlayerPrefs.GetInt("level"), LoadSceneMode.Single);
            

           
        } else
        {
            //change level to the overworld one
            levelload =
            SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
        }
        levelload.completed += onLevelLoad;
    }
    void onLevelLoad(AsyncOperation a)
    {
        fader.SetBool("on", false);
        load();
    }
    public void OnSprint(InputValue val)
    {

        if (val.isPressed)
        {
            sprint = true;
        } else 
        {
            sprint = false;
        }
    }
    public void load()
    {
        if (PlayerPrefs.GetInt("New") == 0)
        {
            if (PlayerPrefs.GetInt("weapstate") > 0) enablegrapple();
            if (PlayerPrefs.GetInt("weapstate") > 1) enablesword();
            if (savetool.loadbool(PlayerPrefs.GetInt("remote"))) git.remote = true;
            spawn = PlayerPrefs.GetInt("spawn");
            if (PlayerPrefs.GetInt("buildpoint") == 0)
            {
                    spawnpos = GameObject.Find("SpawnPos" + spawn.ToString()).transform;
                    transform.position = spawnpos.position;
            } else
            {
                spawnpos = GameObject.Find("SpawnPos" + PlayerPrefs.GetInt("buildpoint").ToString()).transform;
                transform.position = spawnpos.position;
            }
        }
        else 
        {
            spawn = 0;
            spawnpos = GameObject.Find("SpawnPos" + spawn.ToString()).transform;
            transform.position = spawnpos.position;
        }
        
        
    }
    public void save()
    {

        PlayerPrefs.SetInt("weapstate", grap.weapstate);
        PlayerPrefs.SetInt("remote", savetool.savebool(git.remote));
        PlayerPrefs.SetInt("spawn", spawn);
        PlayerPrefs.SetInt("level", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.SetInt("buildpoint", 0);
    }

    private bool isGrounded = false;
    public bool IsGrounded { get { return isGrounded; } }

    private Vector3 inputForce;
    private void Update()
    {
        
        healthim.fillAmount = Mathf.Lerp(healthim.fillAmount, health, Time.deltaTime * fillspeed);
        // Input
        vInput = move.y;
        hInput = move.x;

        // Clamping speed
        rb.velocity = ClampMag(rb.velocity, maximumPlayerSpeed);

        if (!enableMovement)
            return;
        inputForce = (transform.forward * vInput + transform.right * hInput).normalized * (sprint ? runSpeed : walkSpeed);
        if (Physics.Raycast(groundChecker.position, Vector3.down, groundDistance))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        if (isGrounded)
        {
            // Ground controller
            rb.velocity = Vector3.Lerp(rb.velocity, inputForce, changeInStageSpeed * Time.deltaTime);
            if (Mathf.Abs(vInput) + Mathf.Abs(hInput) > 0.2f)
            {
                if (steptime < Time.time)
                {

                    footsteps.PlayOneShot(steps[stepcount]);
                    if (sprint)
                    {
                        steptime = Time.time + 0.2f;
                    }
                    else
                    {
                        steptime = Time.time + 0.4f;
                    }
                    stepcount++;
                    if (stepcount > steps.Length - 1)
                    {
                        stepcount = 0;
                    }
                }
            }
        }
        else
            // Air control
            rb.velocity = ClampSqrMag(rb.velocity + inputForce * Time.deltaTime, rb.velocity.sqrMagnitude);

    }
    public void OnMove(InputValue val)
    {
        move = val.Get<Vector2>();
    }
    public void OnJump(InputValue val)
    {
        if (isGrounded)
        {
            // Jump
            if (val.isPressed && !jumpBlocked)
            {
                rb.AddForce(-jumpForce * rb.mass * Vector3.down);
                jumpBlocked = true;
                Invoke("UnblockJump", jumpCooldown);
            }
        }
    }


    private static Vector3 ClampSqrMag(Vector3 vec, float sqrMag)
    {
        if (vec.sqrMagnitude > sqrMag)
            vec = vec.normalized * Mathf.Sqrt(sqrMag);
        return vec;
    }

    private static Vector3 ClampMag(Vector3 vec, float maxMag)
    {
        if (vec.sqrMagnitude > maxMag * maxMag)
            vec = vec.normalized * maxMag;
        return vec;
    }

    private void OnTriggerEnter(Collider coll) 
    {
        if (coll.gameObject.CompareTag("grapplegun") && actionstamp < Time.time)
        {
            actionstamp = Time.time + 5;
            enableMovement = false;
            camscript.lookat(coll.transform, 3.0f);
            coll.gameObject.GetComponent<Animation>().Play();
            Destroy(coll.gameObject, 3.0f);
            sc.showScreen(grapScreen, 2.0f);
            saudio.PlayOneShot(ability);
            StartCoroutine(delayaction("enablegrapple", 3));
        }
        else if (coll.gameObject.CompareTag("katana") && actionstamp < Time.time)
        {
            actionstamp = Time.time + 5;
            enableMovement = false;
            camscript.lookat(coll.transform, 3.0f);
            coll.gameObject.GetComponent<Animation>().Play();
            coll.gameObject.GetComponent<actsave>().trigger();
            Destroy(coll.gameObject, 3.0f);
            sc.showScreen(katanscreen, 2.0f);
            saudio.PlayOneShot(ability);
            StartCoroutine(delayaction("enablesword", 3));
        }
        else if (coll.gameObject.CompareTag("cTrigger"))
        {

            float ti = coll.transform.parent.GetComponent<cutscene>().play();
            if (ti != 0) {
                StartCoroutine(CGFade.FadeOut(fg, 0.2f));
                enableMovement = false;
                rb.velocity = Vector3.zero;
                StartCoroutine(wait(ti));
                    }
           
        }
        else if (coll.gameObject.CompareTag("door"))
        {

            StartCoroutine(travel(coll.gameObject.GetComponent<door>().build, coll.gameObject.GetComponent<door>().spawn));

        }
        if (coll.gameObject.CompareTag("explosion") && hurtstamp + 1f < Time.time)
        {
            rb.AddForce((coll.transform.position - transform.position) * explospeed);
            StartCoroutine(hurteffect(.15f));
        }


    }
    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("harm") && hurtstamp + 1f < Time.time )
        {
            StartCoroutine(hurteffect(.1f));
        }


    }
    public void startCutscene(GameObject cut)
    {
        float ti = cut.GetComponent<cutscene>().play();
        if (ti != 0)
        {
            StartCoroutine(CGFade.FadeOut(fg, 0.2f));
            enableMovement = false;
            rb.velocity = Vector3.zero;
            StartCoroutine(wait(ti));
        }
    }
    IEnumerator travel(int build, int level)
    {
        fader.SetBool("on", true);
        PlayerPrefs.SetInt("buildpoint", level);
        yield return new WaitForSecondsRealtime(0.5f);
        
        levelload =
            SceneManager.LoadSceneAsync(build, LoadSceneMode.Single);
        while (!levelload.isDone) yield return null;
        spawnpos = GameObject.Find("TPos" + level.ToString()).transform;
    }
    IEnumerator wait(float sec)
    {
        yield return new WaitForSeconds(sec);
        enableMovement = true;
        StartCoroutine(CGFade.FadeIn(fg, 0.2f));
    }
    public void enablegrapple()
    {
        grapplehook.SetActive(true);
        grap.weapstate = 1;
    }
    public void enablesword()
    {
        katana.SetActive(true);
        grap.weapstate = 2;
    }
    IEnumerator delayaction(string action, float delay)
    {
        yield return new WaitForSeconds(delay);
        Invoke(action, 0);
    }
    public void recoverWater()
    {
        StartCoroutine(recover());
    }
    IEnumerator hurteffect(float healthp)
    {
        hurtstamp = Time.time;
        health -= healthp;
        yield return new WaitForSeconds(0.01f);
        hurt.SetTrigger("hurt");
        saudio.PlayOneShot(hurtsound);
    }
    IEnumerator recover()
    {
        fader.SetBool("on", true);
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1.7f);
        Time.timeScale = 1;
        rb.velocity = Vector3.zero;
        spawnpos = GameObject.Find("SpawnPos" + spawn.ToString()).transform;
        transform.position = spawnpos.position;
        fader.SetBool("on", false);
        yield return new WaitForSecondsRealtime(0.5f);
        
    }
    private void OnCollisionExit(Collision collision)
    {
        
    }

    private void UnblockJump()
    {
        jumpBlocked = false;
    }
    
    
    // Enables jumping and player movement
    public void EnableMovement()
    {
        enableMovement = true;
    }

    // Disables jumping and player movement
    public void DisableMovement()
    {
        enableMovement = false;
    }
}
    
