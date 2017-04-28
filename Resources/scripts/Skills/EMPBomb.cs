using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPBomb : Skill {

	// Use this for initialization
	void Start () {
        stressCost = 4;
        skType = SkillType.Massive;
        TOTAL_COOLDOWN = 30; // 30s
        cooldown = TOTAL_COOLDOWN;
        affectsAll = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void action(Student[,] s)
    {
        print("bombaso");
        foreach(Student student in s)
        {
            if(student != null)
            {
                print("estudiante " + student.row + "x" + student.column + " estuneao");
                student.setDisturbing(false);
                student.setConnected(false);
            } 
        }
        // Animación
        GameObject bomb = GameObject.FindGameObjectWithTag("Bomb");
        bomb.GetComponent<SpriteRenderer>().enabled = true;
        bomb.GetComponent<Animator>().Play("empBomb", -1, 0f);
        gameManager.SendMessage("professorAnimation", "emp");

        // Audio

        if (sfxActivated)
            GetComponent<AudioSource>().Play();

        this.toggleCooldown();
    }
}
