using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class screener : MonoBehaviour
{
    public AudioMixer am;
    public PlayerMovement pm;
    CanvasGroup fg;
    bool buttonbool = false;
    public Button contbutton;
    // Start is called before the first frame update
    void Start()
    {
        fg = GameObject.Find("ForegroundUI").GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void showScreen(CanvasGroup screen, float delay = 0f)
    {
        StartCoroutine(shows(screen, delay));
    }
    IEnumerator shows(CanvasGroup screen, float delay)
    {
        StartCoroutine(CGFade.FadeOut(fg, 0.2f));
        yield return new WaitForSeconds(delay);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        am.SetFloat("musVol", -80);
        screen.gameObject.GetComponent<Animator>().SetBool("on", true);
        yield return new WaitForSecondsRealtime(3);
        contbutton.interactable = true;
        
        EventSystem.current.SetSelectedGameObject(contbutton.gameObject);
        
        buttonbool = false;
        while (!buttonbool) yield return null;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        contbutton.interactable = false;
        Time.timeScale = 1;
        am.SetFloat("musVol", 0);
        screen.gameObject.GetComponent<Animator>().SetBool("on", false);
        pm.enableMovement = true;
        StartCoroutine(CGFade.FadeIn(fg, 0.2f));
    }
    public void cont()
    {
        buttonbool = true;
    }
}
