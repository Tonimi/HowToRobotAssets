using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour
{

    public GameObject pauseMenu;
    public Toggle sfxToggle,musicToggle;
    bool paused;
    // Use this for initialization

    Dictionary<string, AudioClip> audioClipsPause = new Dictionary<string, AudioClip>();
    void Start()
    {
        audioClipsPause.Add("menuIn_A", Resources.Load<AudioClip>("sfx/menu/menuIn_Ax2"));
        audioClipsPause.Add("menuIn_B", Resources.Load<AudioClip>("sfx/menu/menuIn_Bx2"));
        audioClipsPause.Add("menuOut_A", Resources.Load<AudioClip>("sfx/menu/menuOut_Ax2"));
        audioClipsPause.Add("menuOut_B", Resources.Load<AudioClip>("sfx/menu/menuOut_Bx2"));
        audioClipsPause.Add("click_A", Resources.Load<AudioClip>("sfx/menu/click_A"));
        audioClipsPause.Add("click_B", Resources.Load<AudioClip>("sfx/menu/click_B"));

        paused = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TogglePauseMenu()
    {
        //Que active o desactive los toggles
        if (PlayerPrefs.GetInt("music") == 1)
        {

            musicToggle.isOn = true;
        }
        else
        {

            musicToggle.isOn = false;
        }
        if (PlayerPrefs.GetInt("sfx") == 1)
        {

            sfxToggle.isOn = true;
        }
        else
        {

            sfxToggle.isOn = false;
        }


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
        
    }


    public void MusicToggle()
    {
        if (musicToggle.isOn)
        {
            PlayerPrefs.SetInt("music", 1);//enabled
            if (!this.GetComponent<AudioSource>().isPlaying)
            {
                this.GetComponent<AudioSource>().Play();
            }
        }
        else
        {
            PlayerPrefs.SetInt("music", 0);//disabled
            this.GetComponent<AudioSource>().Stop();// mute = true;

        }
    }

    public void SFXToggle()
    {
        if (sfxToggle.isOn)
            PlayerPrefs.SetInt("sfx", 1);//enabled
        else
            PlayerPrefs.SetInt("sfx", 0);//disabled

    }
}
