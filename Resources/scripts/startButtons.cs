using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class startButtons : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
   public  void LoadScene()
    {
        
        SceneManager.LoadScene("world");
    }
   public void ExitGame()
    {
        Application.Quit();
    }
}
