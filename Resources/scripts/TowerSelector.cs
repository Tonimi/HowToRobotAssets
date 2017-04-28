using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TowerSelector : MonoBehaviour {

    public string tower;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void loadTower()
    {
        PlayerPrefs.SetString("tower", tower);
        print("Cargando la torre " + tower);
        SceneManager.LoadScene("tower");
    }
}
