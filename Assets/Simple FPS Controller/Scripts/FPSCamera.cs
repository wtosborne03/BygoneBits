/*
    ███████╗██╗██████╗  ██████╗████████╗  ██████╗ ███████╗██████╗  ██████╗ █████╗ ███╗  ██╗
    ██╔════╝██║██╔══██╗██╔════╝╚══██╔══╝  ██╔══██╗██╔════╝██╔══██╗██╔════╝██╔══██╗████╗ ██║
    █████╗  ██║██████╔╝╚█████╗    ██║     ██████╔╝█████╗  ██████╔╝╚█████╗ ██║  ██║██╔██╗██║
    ██╔══╝  ██║██╔══██╗ ╚═══██╗   ██║     ██╔═══╝ ██╔══╝  ██╔══██╗ ╚═══██╗██║  ██║██║╚████║
    ██║     ██║██║  ██║██████╔╝   ██║     ██║     ███████╗██║  ██║██████╔╝╚█████╔╝██║ ╚███║
    ╚═╝     ╚═╝╚═╝  ╚═╝╚═════╝    ╚═╝     ╚═╝     ╚══════╝╚═╝  ╚═╝╚═════╝  ╚════╝ ╚═╝  ╚══╝

     █████╗  █████╗ ███╗   ███╗███████╗██████╗  █████╗ 
    ██╔══██╗██╔══██╗████╗ ████║██╔════╝██╔══██╗██╔══██╗
    ██║  ╚═╝███████║██╔████╔██║█████╗  ██████╔╝███████║
    ██║  ██╗██╔══██║██║╚██╔╝██║██╔══╝  ██╔══██╗██╔══██║
    ╚█████╔╝██║  ██║██║ ╚═╝ ██║███████╗██║  ██║██║  ██║
     ╚════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝

     █████╗  █████╗ ███╗  ██╗████████╗██████╗  █████╗ ██╗     ██╗     ███████╗██████╗ 
    ██╔══██╗██╔══██╗████╗ ██║╚══██╔══╝██╔══██╗██╔══██╗██║     ██║     ██╔════╝██╔══██╗
    ██║  ╚═╝██║  ██║██╔██╗██║   ██║   ██████╔╝██║  ██║██║     ██║     █████╗  ██████╔╝
    ██║  ██╗██║  ██║██║╚████║   ██║   ██╔══██╗██║  ██║██║     ██║     ██╔══╝  ██╔══██╗
    ╚█████╔╝╚█████╔╝██║ ╚███║   ██║   ██║  ██║╚█████╔╝███████╗███████╗███████╗██║  ██║
     ╚════╝  ╚════╝ ╚═╝  ╚══╝   ╚═╝   ╚═╝  ╚═╝ ╚════╝ ╚══════╝╚══════╝╚══════╝╚═╝  ╚═╝

    █▄▄ █▄█   ▀█▀ █ █ █▀▀   █▀▄ █▀▀ █ █ █▀▀ █   █▀█ █▀█ █▀▀ █▀█
    █▄█  █     █  █▀█ ██▄   █▄▀ ██▄ ▀▄▀ ██▄ █▄▄ █▄█ █▀▀ ██▄ █▀▄
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.Users;

[RequireComponent(typeof(Camera))]
public class FPSCamera : MonoBehaviour
{
    public static FPSCamera cam;
    private Camera cam_;

    AudioSource outMus;
    public float sensitivity = 3;
    [HideInInspector]
    public float mouseX, mouseY;
    public float maxUpAngle = 80;
    Color fogColor;
    public float maxDownAngle = -80;
    public Transform player;
    public float smoothtime = 1;
    public float stcont = 0.5f;
    private float st;
    public Color waterColor;
    public Transform CameraPosition;
    bool underwater = false;
    private Vector2 _rotation;
    public PlayerInput pin;
    public buttonChange bchange;
    public Quaternion aimer;
    public bool aimassist = false;
    public float aimstrength;
    public float slowspeed = 4;

    private void Awake()
    {
        //pin.onControlsChanged += cgontrols;
        InputUser.onChange += cgontrols;
        SceneManager.activeSceneChanged += onNewLevel;
        st = smoothtime;
        cam = this;
        cam_ = this.GetComponent<Camera>();
        fogColor = RenderSettings.fogColor;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }
    
    private float rotX = 0.0f, rotY = 0.0f;
    [HideInInspector]
    public float rotZ = 0.0f;
    public bool lookenabled = true;
    private void Update()
    {
        // Mouse input
        if (Time.timeScale != 0)
        {
            if (lookenabled)
            {
                float speedy = 1;
                if (aimassist) speedy = slowspeed;
                mouseX = _rotation.x  * Time.deltaTime * speedy;
                mouseY = _rotation.y * Time.deltaTime * speedy;

                // Calculations
                rotX -= mouseY;
                rotX = Mathf.Clamp(rotX, maxDownAngle, maxUpAngle);
                rotY += mouseX;

                // Placing values
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(rotX, rotY, rotZ), Time.deltaTime * st);
               
                player.Rotate(Vector3.up * mouseX);

            }
           //_rotation = Vector2.zero;
            transform.position = CameraPosition.position;
        }
    }
    public void onNewLevel(Scene current, Scene next)
    {
        outMus = GameObject.Find("outside").GetComponent<AudioSource>();
    }
    public void quicklook(Quaternion rot)
    {
        Vector3 angles = rot.eulerAngles;
        print(transform.localRotation.eulerAngles);
        print(angles);
        rotX = angles.y;
        rotY = angles.x;
        rotZ = 0;
    }
    public void cgontrols(InputUser u, InputUserChange c, InputDevice d)
    {

        if (c == InputUserChange.ControlSchemeChanged)
        {
            print(u.controlScheme.Value.name);
            if (u.controlScheme.Value.name == "KBM")
            {
                st = smoothtime;
                slowspeed = 0.5f;
                bchange.change(0);
            }
            else
            {
                st = stcont;
                slowspeed = 0.25f;
                bchange.change(1);
            }
        }
    }
    public void OnLook(InputValue val)
    {
        Vector2 lookValue = val.Get<Vector2>();

        _rotation.x = lookValue.x * 0.022f * sensitivity;
        _rotation.y = lookValue.y * 0.022f * sensitivity;
    }
    IEnumerator concentrate(Transform target, float delay)
    {
        lookenabled = false;
        float timestamp = Time.unscaledTime + delay;
        while (Time.unscaledTime < timestamp)
        {
            transform.LookAt(target, Vector3.up);
            yield return null;
        }
        lookenabled = true;
    }
    public void lookat(Transform target, float delay = 2.0f)
    {
        StartCoroutine(concentrate(target, delay));
    }
    public void Shake(float magnitude, float duration)
    {
        StartCoroutine(IShake(magnitude, duration));
    }

    private IEnumerator IShake(float mag, float dur)
    {
        WaitForEndOfFrame wfeof = new WaitForEndOfFrame();
        for(float t = 0.0f; t <= dur; t += Time.deltaTime)
        {
            rotZ = Random.Range(-mag, mag) * (t / dur - 1.0f);
            yield return wfeof;
        }
        rotZ = 0.0f;
    }
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("water"))
        {
            underwater = true;
            updWater();
            player.gameObject.GetComponent<PlayerMovement>().recoverWater();
        }
        if (coll.gameObject.CompareTag("musVolume"))
        {
            
            StartCoroutine(AudioFadeOut.FadeOut(outMus, 1.5f));
            StartCoroutine(AudioFadeOut.FadeIn(coll.gameObject.GetComponent<AudioSource>(), 1.5f));
        }
    }
    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag("water"))
        {
            underwater = false;
            updWater();
        }
        if (coll.gameObject.CompareTag("musVolume"))
        {

            StartCoroutine(AudioFadeOut.FadeIn(outMus, 1.5f));
            StartCoroutine(AudioFadeOut.FadeOut(coll.gameObject.GetComponent<AudioSource>(), 1.5f));
        }
    }
    void updWater()
    {
        if (underwater)
        {
            RenderSettings.fogDensity = 0.03f;
            RenderSettings.fogColor = waterColor;
        } else
        {
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = 0.002f;
        }
    }
}

