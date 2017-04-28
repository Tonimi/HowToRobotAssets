using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class LevelSelector : MonoBehaviour
{
    private bool touched = false;
    public bool tutorial;
    private GameObject[] loaders;
    public GameObject instrucciones1;
    public GameObject PopupMenu;
    public string level;
    public bool dragging = false;

    // Use this for initialization
    void Start () {
        loaders = GameObject.FindGameObjectsWithTag("Loading");
    }
	
	// Update is called once per frame
	void Update () {
    }

    IEnumerator Wait(float secs)
    {
        yield return new WaitForSeconds(secs);
    }

    public void click()
    {
        if (level == "exit")
        {
            print("TOCAO");
            SceneManager.LoadScene("world");
        }
        else if (!touched)
        {
            string tower = PlayerPrefs.GetString("tower");
            print("Estoy en la torre " + tower);
            if (tower == "0")
            {
                print("Abro el nivel " + level);
                PopupMenu.GetComponent<PopupMenu>().loadPopup("Level"+level);

            }
            else {

                print("Abro el nivel " + tower+level);
                PopupMenu.GetComponent<PopupMenu>().loadPopup("Level"+tower+level);
            }
            //PopupMenu.GetComponent<PopupMenu>().loadPopup(level);
        }
    }
}
