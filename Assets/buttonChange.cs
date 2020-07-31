using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonChange : MonoBehaviour
{
    public Sprite butt_a;
    public Sprite LMB;
    public Image hold1;
    public Image hold2;
    // Start is called before the first frame update
    private int dev = 0;

    public void change(int device)
    {
        switch (device)
        {
            case 0:
                hold1.sprite = LMB;
                break;
            case 1:
                hold1.sprite = butt_a;
                break;
        }
        dev = device;
    }
}
