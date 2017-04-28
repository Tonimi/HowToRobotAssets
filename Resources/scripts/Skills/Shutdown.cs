using UnityEngine;
using System.Collections;

public class Shutdown : Skill {

    // Use this for initialization
    void Start () {
        stressCost = 1;
        skType = SkillType.Individual;
        TOTAL_COOLDOWN = 10; // 10s
        cooldown = TOTAL_COOLDOWN;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void action (Student s)
    {
        print("I've shut down student " + s.row + "x" + s.column);
        
        // Animación
        
        // Audio
        if (sfxActivated)
            GetComponent<AudioSource>().Play();


        s.setDisturbing(false);
        s.setConnected(false);
        // esto sería, no? Desactivar tanto el molestar como al chaval
        this.toggleCooldown();
    }
}
