using UnityEngine;
using System.Collections;

public class Shortcircuit : Skill
{

    // Use this for initialization
    void Start()
    {
        stressCost = 1;
        skType = SkillType.Individual;
        TOTAL_COOLDOWN = 25; 
        cooldown = TOTAL_COOLDOWN;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void action(Student s)
    {
        print("I've shortcircuited the student " + s.row + "x" + s.column);

        // Animación
        GameObject shortcircuit = GameObject.FindGameObjectWithTag("Shortcircuit");
        shortcircuit.transform.position = s.transform.position;
        shortcircuit.GetComponent<SpriteRenderer>().enabled = true;
        shortcircuit.GetComponent<Animator>().Play("shortCircuit", -1, 0f);
        
        // Audio
        if (sfxActivated)
            GetComponent<AudioSource>().Play();

        //different from shutdown since it doesn't disconnect the student
        s.setDisturbing(false);
        s.setAnimation("student_shocked");
        this.toggleCooldown();
    }
}
