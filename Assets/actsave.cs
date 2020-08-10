using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class actsave : MonoBehaviour
{
    public int asid = 0;
    public bool completed = false;
    public UnityEvent act;
    // Start is called before the first frame update
    void Start()
    {


    }
    public void load()
    {
        if (PlayerPrefs.GetInt("New") == 1)
        {
            PlayerPrefs.SetInt("as" + asid.ToString(), savetool.savebool(false));
        } else
        {
            if (savetool.loadbool(PlayerPrefs.GetInt("as" + asid.ToString()))) {
                completed = true;
                act.Invoke();
            }
        }

    }
    public void save()
    {

        PlayerPrefs.SetInt("as" + asid.ToString(), savetool.savebool(completed));
      
    }

    // Update is called once per frame
   public void trigger()
    {
        completed = true;
    }
}
