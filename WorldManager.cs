using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour {

    public GameObject pauseMenu;
    public Toggle sfxToggle;
    bool paused;
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
}
