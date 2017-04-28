using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : Skill
{
    GameObject teacherHand;
    GameObject restart;

    // Use this for initialization
    void Start()
    {
        stressCost = 4;
        skType = SkillType.Unstress;
        TOTAL_COOLDOWN = 40; // 5s
        cooldown = TOTAL_COOLDOWN;
        teacherHand = GameObject.FindGameObjectWithTag("Teacherhand");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void action()
    {

        print("reiniciar cooldowns y poner estrés a 0");
        gameManager.GetComponent<GameManager>().stressBars = 0;
        gameManager.GetComponent<GameManager>().setStress(0);
        gameManager.SendMessage("restartCooldowns");

        // Animación
        gameManager.SendMessage("professorAnimation", "reset");


        if (sfxActivated)
            GetComponent<AudioSource>().Play();
        this.toggleCooldown();
    }
}

