using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class towerManager : MonoBehaviour {

    public Image towerSprite;
    public GameObject pauseMenu;
    public Toggle sfxToggle;
    bool paused;

    string tower;

    public string numberTower;
    // Use this for initialization


    Dictionary<string, AudioClip> audioClipsPause = new Dictionary<string, AudioClip>();
    void Start () {
        audioClipsPause.Add("menuIn_A", Resources.Load<AudioClip>("sfx/menu/menuIn_Ax2"));
        audioClipsPause.Add("menuIn_B", Resources.Load<AudioClip>("sfx/menu/menuIn_Bx2"));
        audioClipsPause.Add("menuOut_A", Resources.Load<AudioClip>("sfx/menu/menuOut_Ax2"));
        audioClipsPause.Add("menuOut_B", Resources.Load<AudioClip>("sfx/menu/menuOut_Bx2"));
        audioClipsPause.Add("click_A", Resources.Load<AudioClip>("sfx/menu/click_A"));
        audioClipsPause.Add("click_B", Resources.Load<AudioClip>("sfx/menu/click_B"));

        paused = false;

        //Ponemos los sprites de puntuación
        foreach(Transform child in GameObject.Find("TowerImage").transform)
        {
            print(child.name);
            if (child.transform.childCount>0)//Cogemos la imagen de cada uno
            {
                switch(PlayerPrefs.GetString("Level"+numberTower+child.name + "score"))
                {
                    case "A":
                        child.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("sprites/ScoreA");
                        break;
                    case "B":
                        child.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("sprites/ScoreB");
                        break;
                    case  "C":
                        child.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("sprites/ScoreC");
                        break;
                    case "FAIL":
                        child.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("sprites/ScoreFAIL");
                        break;
                    default:
                        child.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("sprites/ScoreFAIL");
                        break;
                }
                
                print("YOH");
            }
        }
        loadTower();

        //SceneManager.sceneLoaded += this.OnLoadCallback;

    }
	
	// Update is called once per frame
	void Update () {
		
	}




    public void TogglePauseMenu()
    {

        // not the optimal way but for the sake of readability
        if (pauseMenu.GetComponentInChildren<Canvas>().enabled)
        {
            if (sfxToggle.isOn)
            {
                // Audio
                int rand = Random.Range(0, 2);
                if (rand == 0)
                {
                    pauseMenu.GetComponent<AudioSource>().clip = audioClipsPause["menuOut_A"];
                    pauseMenu.GetComponent<AudioSource>().Play();
                }
                else
                {
                    pauseMenu.GetComponent<AudioSource>().clip = audioClipsPause["menuOut_B"];
                    pauseMenu.GetComponent<AudioSource>().Play();
                }
            }

            paused = false;
            pauseMenu.GetComponentInChildren<Canvas>().enabled = false;
            print("despauso");
        }
        else
        {
            if (sfxToggle.isOn)
            {
                // Audio
                int rand = Random.Range(0, 2);
                if (rand == 0)
                {
                    pauseMenu.GetComponent<AudioSource>().clip = audioClipsPause["menuIn_A"];
                    pauseMenu.GetComponent<AudioSource>().Play();
                }
                else
                {
                    pauseMenu.GetComponent<AudioSource>().clip = audioClipsPause["menuIn_B"];
                    pauseMenu.GetComponent<AudioSource>().Play();
                }
            }

            paused = true;
            pauseMenu.GetComponentInChildren<Canvas>().enabled = true;
            print("pauso");
        }

        Debug.Log("GAMEMANAGER:: TimeScale: " + Time.timeScale);
    }

    public void loadTower()
    {
        //We get the name of the tower we clicked on the world scene
        tower = PlayerPrefs.GetString("tower");
        print("Se ha cargado la torre " + tower);
        
        towerSprite.GetComponent<Image>().sprite =Resources.Load<Sprite>("sprites/tower"+tower+"_sprite");
        

    }
}
