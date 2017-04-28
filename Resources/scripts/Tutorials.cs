using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorials : MonoBehaviour {


    private GameObject instrucciones1;
    private GameObject instrucciones2;
    private GameObject instrucciones3;
    private GameObject levelSelector;
    private bool touch = false;

    // Use this for initialization
    void Start () {
        instrucciones1 = GameObject.Find("instrucciones1");
        if (instrucciones1 == null)
        {
            print("NO HAY INSTRUCCIONES 1");
        }
        instrucciones2 = GameObject.Find("instrucciones2");
        if (instrucciones2 == null)
        {
            print("NO HAY INSTRUCCIONES 2");
        }
        instrucciones3 = GameObject.Find("instrucciones3");
        if (instrucciones3 == null)
        {
            print("NO HAY INSTRUCCIONES 3");
        }
        levelSelector = GameObject.Find("selector1");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void showTutorial(int toShow)
    {

        switch (toShow)
        {
            case 1:
                instrucciones1.GetComponent<Button>().interactable = true;
                instrucciones1.GetComponent<Image>().enabled = true;
                break;
            case 2:
                instrucciones1.GetComponent<Button>().interactable = false;
                instrucciones1.GetComponent<Image>().enabled = false;
                instrucciones2.GetComponent<Button>().interactable = true;
                instrucciones2.GetComponent<Image>().enabled = true;
                break;
            case 3:
                instrucciones2.GetComponent<Button>().interactable = false;
                instrucciones2.GetComponent<Image>().enabled = false;
                instrucciones3.GetComponent<Button>().interactable = true;
                instrucciones3.GetComponent<Image>().enabled = true;
                break;
            case 4:
                //levelSelector.GetComponent<LevelSelector>().startLevel();
                break;
        }
    }
}
