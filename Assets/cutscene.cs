using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class cutscene : MonoBehaviour
{
    public int id = 0;
    public float time;
    bool played = false;
    PlayableDirector pd;
    // Start is called before the first frame update
    void Start()
    {
        pd = GetComponent<PlayableDirector>();
    }
    public void load()
    {
        if (PlayerPrefs.GetInt("New") == 0)
        {
            if (savetool.loadbool(PlayerPrefs.GetInt("cut" + id.ToString()))) played = true;

        }
    }
    public void save()
    {

        PlayerPrefs.SetInt("cut" + id.ToString(), savetool.savebool(played));
    }
    public float play()
    {
        if (!played)
        {
            played = true;
            pd.Play();
            return time;
        }
        else
        {
            return 0;
        }
    }
}
