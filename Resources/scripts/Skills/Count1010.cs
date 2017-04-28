using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Count1010 : Skill {
    GameObject teacherHand;
    GameObject countto1010;					   
    
	// Use this for initialization
	void Start () {
        stressCost = 2;
        skType = SkillType.Unstress;
        TOTAL_COOLDOWN = 5; // 5s
        cooldown = TOTAL_COOLDOWN;
	    teacherHand = GameObject.FindGameObjectWithTag("Teacherhand");
        countto1010 = GameObject.FindGameObjectWithTag("1010");																  
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void action()
    {
        print("reducir 2 de estrés");
        if (gameManager.GetComponent<GameManager>().stressBars - 2 < 0)
        {
            gameManager.GetComponent<GameManager>().stressBars = 0;
            gameManager.GetComponent<GameManager>().setStress(0);
        } else
        {
            gameManager.GetComponent<GameManager>().stressBars -= 2;
            gameManager.GetComponent<GameManager>().setStress(gameManager.GetComponent<GameManager>().stressBars);
        }
		        // Animación
        countto1010.transform.position = teacherHand.transform.position;
        countto1010.GetComponent<Animator>().Play("1010", -1, 0f);

        gameManager.SendMessage("professorAnimation", "1010");

        if (sfxActivated)
            GetComponent<AudioSource>().Play();
        // Comenzar corrutina de relajarse: gameManager.startCoroutine("Relax", CuantoRelajaEsto);
        this.toggleCooldown();
    }
}
