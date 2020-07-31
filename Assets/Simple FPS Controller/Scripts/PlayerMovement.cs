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

// I use Physics.gravity a lot instead of Vector3.up because you can point the gravity to a different direction and i want the controller to work fine
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    private static Vector3 vecZero = Vector3.zero;
    private Rigidbody rb;
    private CapsuleCollider cc;
    public GrapplingHook grap;
    public CanvasGroup remoteSreen;
    public CanvasGroup grapScreen;
    public screener sc;
    public bool enableMovement = true;
    public Animator fader;
    AudioSource saudio;
    public GrabIt git;
    public FPSCamera camscript;
    CanvasGroup fg;
    public Transform spawnpos;

    [Header("Movement properties")]
    public float walkSpeed = 8.0f;
    public float runSpeed = 12.0f;
    public float changeInStageSpeed = 10.0f; // Lerp from walk to run and backwards speed
    public float maximumPlayerSpeed = 150.0f;
    public float groundDistance = 2.0f;
    [HideInInspector] public float vInput, hInput;
    public Transform groundChecker;
    public GameObject grapplehook;
    public AudioClip ability;
    [Header("Jump")]
    public float jumpForce = 500.0f;
    public float jumpCooldown = 1.0f;
    private bool jumpBlocked = false;
    float actionstamp;
    private Vector2 move;
    private bool sprint = false;
    private PlayerInput pin;
    
    private void Start()
    {
        fg = GameObject.Find("ForegroundUI").GetComponent<CanvasGroup>();
        saudio = GetComponent<AudioSource>();
        transform.position = spawnpos.position;
        rb = this.GetComponent<Rigidbody>();
        cc = this.GetComponent<CapsuleCollider>();
        pin = transform.parent.gameObject.GetComponent<PlayerInput>();
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
            if (savetool.loadbool(PlayerPrefs.GetInt("grapple"))) enablegrapple();
            if (savetool.loadbool(PlayerPrefs.GetInt("remote"))) git.remote = true;
        }
    }
    public void save()
    {

        PlayerPrefs.SetInt("grapple", savetool.savebool(grap.weapstate > 0));
        PlayerPrefs.SetInt("remote", savetool.savebool(git.remote));
    }

    private bool isGrounded = false;
    public bool IsGrounded { get { return isGrounded; } }

    private Vector3 inputForce;
    private void Update()
    {
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
        } else if (coll.gameObject.CompareTag("remote") && actionstamp < Time.time)
        {
            actionstamp = Time.time + 5;
            enableMovement = false;
            camscript.lookat(coll.transform, 3.0f);
            coll.gameObject.GetComponent<Animation>().Play();
            Destroy(coll.gameObject, 3.0f);
            sc.showScreen(remoteSreen, 2.0f);
            git.remote = true;
            saudio.PlayOneShot(ability);
        }
        else if (coll.gameObject.CompareTag("cTrigger"))
        {

            float ti = coll.transform.parent.GetComponent<cutscene>().play();
            if (ti != 0) {
                StartCoroutine(CGFade.FadeOut(fg, 0.2f));
                enableMovement = false;
                StartCoroutine(wait(ti));
                    }
           
        }


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
    IEnumerator delayaction(string action, float delay)
    {
        yield return new WaitForSeconds(delay);
        Invoke(action, 0);
    }
    public void recoverWater()
    {
        StartCoroutine(recover());
    }
    IEnumerator recover()
    {
        fader.SetBool("on", true);
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1.7f);
        Time.timeScale = 1;
        rb.velocity = Vector3.zero;
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
