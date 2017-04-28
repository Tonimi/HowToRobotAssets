using UnityEngine;
using System.Collections;

public class Laser : Skill
{
    
    GameObject teacherHand;
    GameObject laserRay;
    public bool increase = false;
    bool activateResize = false;

    // Use this for initialization
    void Start()
    {
        stressCost = 3;
        skType = SkillType.Individual;
        TOTAL_COOLDOWN = 20; // 20s
        cooldown = TOTAL_COOLDOWN;
        teacherHand = GameObject.FindGameObjectWithTag("Teacherhand");
        laserRay = GameObject.FindGameObjectWithTag("Laserray");
    }

    // Update is called once per frame
    void Update()
    {
        if(increase && activateResize)
        {
            laserRay.transform.localScale += new Vector3(0, 0.05f, 0);
        }
        else if(!increase && activateResize)
        {
            laserRay.transform.localScale -= new Vector3(0, 0.05f, 0);
        }

        if(laserRay.transform.localScale.y <= 0)
        {
            activateResize = false;
            laserRay.transform.position = teacherHand.transform.position;
            laserRay.GetComponent<SpriteRenderer>().flipY = false;
        }
    }

    public override void action(Student s)
    {
        print("He destruido al estudiante " + s.row + "x" + s.column);



        if (sfxActivated)
            GetComponent<AudioSource>().Play();
        // Animación



        gameManager.SendMessage("professorAnimation", "laser");

        laserRay.transform.position = teacherHand.transform.position;

        laserRay.transform.localScale += new Vector3(0, 0.3f, 0);
        laserRay.transform.up = (laserRay.transform.position - s.transform.position).normalized;
        activateResize = true;
        increase = true;
        s.target = true;
        s.setConnected(false);
        this.toggleCooldown();
    }
}

