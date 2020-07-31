using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("New")) PlayerPrefs.SetInt("New", 1);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void StartGame() { SceneManager.LoadScene(1); }
    public void clear() { 
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("New", 1);
    }
    public void quit() { Application.Quit(); }
}
