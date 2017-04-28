using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : Skill
{
    /*
     * Destruye el estudiante seleccionado y
     * desactiva los estudiantes adyacentes (en cruz)
     */
    // Use this for initialization
    void Start()
    {
        stressCost = 3;
        skType = SkillType.Massive;
        TOTAL_COOLDOWN = 25;
        cooldown = TOTAL_COOLDOWN;
        affectsAll = false;//just few
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void action(Student s1, Student[,] sNearBy)
    {
        print("Rayasso");
        s1.setConnected(false);//destroy the student
        
        //Desactivamos los estudiantes adyacentes (en cruz)
        if (s1.row < GameManager.rows-1)
        {
            if (sNearBy[s1.row + 1, s1.column] != null)
            {
                sNearBy[s1.row + 1, s1.column].setDisturbing(false);
   
            }
        }
        if (s1.row > 0)
        {
            if (sNearBy[s1.row - 1, s1.column] != null)
            {
                sNearBy[s1.row - 1, s1.column].setDisturbing(false);
            }
        }
        if (s1.column > 0)
        {
            if (sNearBy[s1.row, s1.column - 1] != null)
            {
                sNearBy[s1.row, s1.column - 1].setDisturbing(false);
            }
        }
        if (s1.column < GameManager.columns-1)
        {
            if (sNearBy[s1.row, s1.column + 1] != null)
            {
                sNearBy[s1.row, s1.column + 1].setDisturbing(false);
            }
        }

        gameManager.SendMessage("studentDestroyed", s1);
        s1.GetComponent<Animator>().Play("student_exploding", -1, 0f);
        // Animación
        //GameObject lightning = GameObject.FindGameObjectWithTag("Lightning");
        //lightning.GetComponent<SpriteRenderer>().enabled = true;
        //lightning.GetComponent<Animator>().Play("lightning", -1, 0f);
        //gameManager.SendMessage("professorAnimation", "lightning");

        // Audio

        if (sfxActivated)
            GetComponent<AudioSource>().Play();

        this.toggleCooldown();
    }
}
