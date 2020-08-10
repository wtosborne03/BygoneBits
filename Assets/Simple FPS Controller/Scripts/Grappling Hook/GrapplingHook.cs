/*
     ██████╗ ██████╗  █████╗ ██████╗ ██████╗ ██╗     ██╗███╗  ██╗ ██████╗   ██╗  ██╗ █████╗  █████╗ ██╗  ██╗
    ██╔════╝ ██╔══██╗██╔══██╗██╔══██╗██╔══██╗██║     ██║████╗ ██║██╔════╝   ██║  ██║██╔══██╗██╔══██╗██║ ██╔╝
    ██║  ██╗ ██████╔╝███████║██████╔╝██████╔╝██║     ██║██╔██╗██║██║  ██╗   ███████║██║  ██║██║  ██║█████═╝ 
    ██║  ╚██╗██╔══██╗██╔══██║██╔═══╝ ██╔═══╝ ██║     ██║██║╚████║██║  ╚██╗  ██╔══██║██║  ██║██║  ██║██╔═██╗ 
    ╚██████╔╝██║  ██║██║  ██║██║     ██║     ███████╗██║██║ ╚███║╚██████╔╝  ██║  ██║╚█████╔╝╚█████╔╝██║ ╚██╗
     ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝     ╚═╝     ╚══════╝╚═╝╚═╝  ╚══╝ ╚═════╝   ╚═╝  ╚═╝ ╚════╝  ╚════╝ ╚═╝  ╚═╝

    █▄▄ █▄█   ▀█▀ █ █ █▀▀   █▀▄ █▀▀ █ █ █▀▀ █   █▀█ █▀█ █▀▀ █▀█
    █▄█  █     █  █▀█ ██▄   █▄▀ ██▄ ▀▄▀ ██▄ █▄▄ █▄█ █▀▀ ██▄ █▀▄
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(PlayerMovement))] // PlayerMovement also requires Rigidbody
public class GrapplingHook : MonoBehaviour
{
    private PlayerMovement pm;
    private Rigidbody rb;
    public bool showicon = false;
    public AudioClip grappleFX;
    AudioSource audio;
    private int segments;
    public int weapstate = 0;
    public int selectedweapon = 0;
    public Transform grapplePoint;
    public AudioSource grappleAudio;
    public PostProcessProfile main;
    public float speedOffset = 3.0f;
    bool flying = false;
    Coroutine pitch;
    public FPSCamera aim;
    private bool mdown = false;
    private bool omdown;
    private bool rdown = false;
    private bool ordown;
    public float aimassistbounds = 5;
    public float aimassiststrength = 2;
    public float aimdist = 40;
    public bool scube = false;
    float coyote;
    public Animator grapanim;
    public Animator swordanim;
    private float switchstamp;
    public AudioSource swordaudio;
    public AudioClip[] swings;
    private void Start()
    {
        audio = GetComponent<AudioSource>();
        segments = rope.segments;
        pm = this.GetComponent<PlayerMovement>();
        rb = this.GetComponent<Rigidbody>();
        aim.aimstrength = aimassiststrength;
        switch (selectedweapon)
        {
            case 0:
                grapanim.SetBool("vis", true);
                break;
            case 1:
                swordanim.SetBool("vis", true);
                break;
        }
    }

    private bool isGrappling = false;
    private void Update()
    {
        if (weapstate > 0 && selectedweapon == 0)
        {
            if (flying)
            {
                float speed = Mathf.Clamp((rb.velocity.magnitude / speedOffset), 0, 1);
                grappleAudio.pitch = speed;
                main.GetSetting<ChromaticAberration>().intensity.value = speed;
                //
                if (pm.IsGrounded && !isGrappling)
                {
                    flying = false;
                    speed = 0;
                    pitch = StartCoroutine(AudioFadeOut.PitchOut(grappleAudio, 0.25f));
                    main.GetSetting<ChromaticAberration>().intensity.value = speed;
                }
            }
            if (crossHairSpinningPart != null)
            {
                crossHairSpinningPart.Rotate(Vector3.forward * crossHairSpinSpeed * Time.deltaTime);
                // we need 2 raycasts bc w/ 1 you can grapple through colliders which isn't good
                if (Physics.Raycast(FPSCamera.cam.transform.position, FPSCamera.cam.transform.forward, out hitInfo, maxGrappleDistance, layerMask))
                {
                    hitName = hitInfo.collider.name;
                    if (Physics.Raycast(FPSCamera.cam.transform.position, FPSCamera.cam.transform.forward, out hitInfo, maxGrappleDistance))
                    {
                        if (hitName != hitInfo.collider.name)
                            goto _else;
                        crossHairSpinningPart.gameObject.SetActive(true);
                        goto _out;
                    }
                }
                if (Physics.SphereCast(FPSCamera.cam.transform.position, aimassistbounds, FPSCamera.cam.transform.forward, out hitInfo, maxGrappleDistance , layerMask))
                {
                    if (!scube)
                    {
                        coyote = Time.time + 0.25f;
                        //aim.quicklook(Quaternion.LookRotation(hitInfo.collider.transform.position - aim.transform.position, Vector3.up));
                    }
                    scube = true;
                    if (hitInfo.distance > aimdist )
                    {
                        aim.aimassist = true;
                    } else
                    {
                        aim.aimassist = false;
                    }
                    
                    aim.aimer = Quaternion.LookRotation(hitInfo.point - transform.position);

                } else
                {
                    scube = false;
                    aim.aimassist = false;

                }

            _else:
               
                if (showicon)
                {
                    crossHairSpinningPart.gameObject.SetActive(true);
                }
                else
                {
                    crossHairSpinningPart.gameObject.SetActive(false);
                }
            }
        _out:

            if (!isGrappling)
            {
                if (mdown && !omdown)
                {
                    Grapple();
                    StopCoroutine(pitch);
                    flying = true;
                }
                //audio.PlayOneShot(grappleFX);

                return;
            }
            else
            {
                // effectsf

                flying = true;
                //
                if (!mdown)
                {
                 
                    UnGrapple();
                }
                GrappleUpdate();

                return;
            }
        } else if (weapstate > 1 && selectedweapon == 1)
        {
            if (mdown && !omdown && !showicon)
            {
                swordaudio.PlayOneShot(swings[Random.Range(0, swings.Length)]);
                swordanim.SetTrigger("swing");
            }
            swordanim.SetBool("block", rdown);
            
        } 
        
            if (showicon)
            {
                crossHairSpinningPart.gameObject.SetActive(true);
                crossHairSpinningPart.Rotate(Vector3.forward * crossHairSpinSpeed * Time.deltaTime);
            }
            else
            {
                crossHairSpinningPart.gameObject.SetActive(false);
            }
        
        omdown = mdown;
        ordown = rdown;
    }

    public void OnVerbA(InputValue val)
    {
        mdown = val.isPressed;
    }
    public void OnVerbB(InputValue val)
    {
        rdown = val.isPressed;
    }
    [Header("Properties")]
    public float maxGrappleDistance = 100.0f;
    public Rope rope;
    public float maximumSpeed = 100.0f;
    public float deceleration = 2500.0f; // This is how much the player is going to decelerate after stopped grappling
    public float deceleratingTime = 1.4f; // This is the time the decelerating is going to act on the player after stopped grappling
    public RectTransform crossHairSpinningPart;
    public float crossHairSpinSpeed = 200.0f;
    public float distanceToStop = 2.0f;
    public LayerMask layerMask;
    public float grappleCooldown = 1.0f;
    private bool isBlocked = false;

    private Transform location; // the grappled location
    private RaycastHit hitInfo;
    private string hitName;
    public void Grapple()
    {
        if (isBlocked)
            return;

        // we need 2 raycasts bc w/ 1 you can grapple through colliders which isn't good
        if (Physics.Raycast(FPSCamera.cam.transform.position, FPSCamera.cam.transform.forward, out hitInfo, maxGrappleDistance, layerMask))
        {
            hitName = hitInfo.collider.name;
            if (Physics.Raycast(FPSCamera.cam.transform.position, FPSCamera.cam.transform.forward, out hitInfo, maxGrappleDistance))
            {
                if (hitName != hitInfo.collider.name)
                    return;
                // We create a GameObject and we parent it to the grappled object. 
                // If we don't parent it to the object and the object moves the player is stuck only on one location instead of the moving object.
                location = new GameObject().transform;//Instantiate(new GameObject(), hitInfo.point, Quaternion.identity).transform;
                location.position = hitInfo.collider.transform.position;
                location.parent = hitInfo.collider.transform;
                audio.PlayOneShot(grappleFX);
                if (decelerateTimer != 0.0f)
                    StopCoroutine(Decelerate());
                pm.DisableMovement();
                // Rope attaching
                rope.segments = (int)((hitInfo.distance / maxGrappleDistance) * segments);
                rope.Grapple(grapplePoint.position, hitInfo.point);

                rb.useGravity = false;
                isGrappling = true;
            }
        }
    }

    private Vector3 grappleForce;
    public void UnGrapple()
    {
        if (!isGrappling)
            return;
        if (location != null)
            Destroy(location.gameObject);
        if (decelerateTimer == 0.0f)
            StartCoroutine(Decelerate());
        else
            decelerateTimer = 0.0f;

        pm.EnableMovement();
        // Rope detaching
        rope.UnGrapple();

        Invoke("UnblockGrapple", grappleCooldown);
        
        rb.useGravity = true;
        isGrappling = false;
    }
    public void OnSwitchweapon(InputValue val)
    {
        if (switchstamp < Time.time)
        {
            if (val.Get<float>() > 0)
            {
                selectedweapon += 1;
                if (selectedweapon > weapstate - 1)
                {
                    selectedweapon = 0;
                }
            }
            else
            {
                selectedweapon -= 1;
                if (selectedweapon < 0)
                {
                    selectedweapon = weapstate - 1;
                }
            }
            if (true)
            {
                switch (selectedweapon)
                {
                    case 0:
                        grapanim.SetBool("vis", true);
                        swordanim.SetBool("vis", false);
                        break;
                    case 1:
                        grapanim.SetBool("vis", false);
                        swordanim.SetBool("vis", true);
                        break;
                }
            }
            switchstamp = Time.time + 0.24f;
        }
    }

    private void UnblockGrapple()
    {
        isBlocked = false;
    }

    private float decelerateTimer = 0.0f, max;
    private IEnumerator Decelerate()
    {
        WaitForEndOfFrame wfeof = new WaitForEndOfFrame();
        max = deceleratingTime * Mathf.Clamp01(targetDistance / 10.0f) * Mathf.Clamp01(rb.velocity.magnitude / 30.0f);
        for (; decelerateTimer < max; decelerateTimer += Time.deltaTime)
        {
            rb.AddForce(-rb.velocity.normalized * deceleration * (1.0f - decelerateTimer / max) * Mathf.Clamp01(rb.velocity.sqrMagnitude / 400.0f) * Time.deltaTime, ForceMode.Acceleration);
            yield return wfeof;
        }
        decelerateTimer = 0.0f;
    }

    private Vector3 dir;

    private float speed = 0.0f, targetDistance;
    private void GrappleUpdate()
    {
        if (location == null)
            return;
        
        targetDistance = Vector3.Distance(transform.position, location.position);
        rope.segments = (int)((targetDistance / maxGrappleDistance) * segments);
        dir = (location.position - transform.position).normalized;
        
        rb.velocity = Vector3.Lerp(rb.velocity, dir * maximumSpeed * Mathf.Clamp01(targetDistance / (4.0f * distanceToStop)), Time.deltaTime);

        // Rope updating
        rope.UpdateStart(grapplePoint.position);
        rope.UpdateGrapple();
    }

    private Vector3 ClampMag(Vector3 vec, float maxMag)
    {
        if (vec.sqrMagnitude > maxMag * maxMag)
            vec = vec.normalized * maxMag;
        return vec;
    }
}
