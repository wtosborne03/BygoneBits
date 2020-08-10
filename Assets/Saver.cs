using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saver : MonoBehaviour
{
    PlayerMovement pm;
    FPSCamera cam;
    Animator anim;
    CanvasGroup fg;
    public List<GameObject> saveables;
    // Start is called before the first frame update
    void Start()
    {
        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
        cam = GameObject.Find("fpcam").GetComponent<FPSCamera>();
        Animator anim = GameObject.Find("saveanim").GetComponent<Animator>();
        fg = GameObject.Find("ForegroundUI").GetComponent<CanvasGroup>();
        pm.gameObject.transform.parent.gameObject.BroadcastMessage("load");
        foreach (GameObject g in saveables)
        {
            g.BroadcastMessage("load");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void save()
    {
        
        StartCoroutine(saver());
        pm.gameObject.transform.parent.gameObject.BroadcastMessage("save");
        foreach (GameObject g in saveables)
        {
            g.BroadcastMessage("save");
        }
        if (PlayerPrefs.GetInt("New") == 1)
        {
            PlayerPrefs.SetInt("New", 0);
        }
    }
    IEnumerator saver()
    {
        StartCoroutine(CGFade.FadeOut(fg, 0.2f));
        pm.enableMovement = false;
        cam.lookenabled = false;
        anim.SetTrigger("save");
        yield return new WaitForSeconds(4);
        pm.enableMovement = true;
        cam.lookenabled = true;
        StartCoroutine(CGFade.FadeIn(fg, 0.2f));
    }

}
