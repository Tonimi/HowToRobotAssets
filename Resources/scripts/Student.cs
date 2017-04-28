using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Student : MonoBehaviour {

    public int row;
    public int column;
    bool individualSkillClicked = false;
    public bool disturbActivated;


    public bool activated = true;
    public bool target = false;
    public int cooldown=0;

    public ArrayList links;
    public bool instigator = false;

    public bool sfxActivated;

     GameObject gameManager;
    string idleAnimation;

    public float volume = 0f;					 
    Dictionary<string, AudioClip> audioClips;

    // Use this for initialization
    void Start () {
        links = new ArrayList();
        gameManager = GameObject.FindGameObjectWithTag("Manager");
		
        							   
        // inicialización sfx
        audioClips = new Dictionary<string, AudioClip>();
        audioClips.Add("chat_A", Resources.Load<AudioClip>("sfx/students/chat_A"));
        audioClips.Add("chat_B", Resources.Load<AudioClip>("sfx/students/chat_B"));
        audioClips.Add("dance_disco", Resources.Load<AudioClip>("sfx/students/dance_disco"));
        audioClips.Add("dance_salsa", Resources.Load<AudioClip>("sfx/students/dance_salsa"));
        audioClips.Add("draw", Resources.Load<AudioClip>("sfx/students/draw"));
        audioClips.Add("emails", Resources.Load<AudioClip>("sfx/students/emails"));
        audioClips.Add("funnyNoises_A", Resources.Load<AudioClip>("sfx/students/funnyNoises_A"));
        audioClips.Add("funnyNoises_B", Resources.Load<AudioClip>("sfx/students/funnyNoises_B"));
        audioClips.Add("FunnyNoises_C", Resources.Load<AudioClip>("sfx/students/FunnyNoises_C"));
        audioClips.Add("internet_long", Resources.Load<AudioClip>("sfx/students/internet_long"));
        audioClips.Add("internet_short", Resources.Load<AudioClip>("sfx/students/internet_short"));
        audioClips.Add("jokes_A", Resources.Load<AudioClip>("sfx/students/jokes_A"));
        audioClips.Add("jokes_B", Resources.Load<AudioClip>("sfx/students/jokes_B"));
        audioClips.Add("music_A", Resources.Load<AudioClip>("sfx/students/music_A"));
        audioClips.Add("music_B", Resources.Load<AudioClip>("sfx/students/music_B"));
        audioClips.Add("play_long", Resources.Load<AudioClip>("sfx/students/play_long"));
        audioClips.Add("play_short", Resources.Load<AudioClip>("sfx/students/play_short"));
        audioClips.Add("sleep", Resources.Load<AudioClip>("sfx/students/sleep"));
        audioClips.Add("windowArgue", Resources.Load<AudioClip>("sfx/students/windowArgue"));
       audioClips.Add("laser_killed", Resources.Load<AudioClip>("sfx/students/laser_killed"));
    }
	
	// Update is called once per frame
	void Update () {
	

	}

    public void disturb(string conflict)
    {
        // parseo
        if(disturbActivated)
        {
            switch(conflict)
            {
                case "gaming":

                    this.GetComponent<Animator>().Play("student_play", -1, 0f);
                    if (sfxActivated)
                    {
                        volume = 0.4f;
                        this.GetComponent<AudioSource>().clip = audioClips["play_long"];
                    }
                    break;
                case "screensaver":
                    this.GetComponent<Animator>().Play("student_sleeping", -1, 0f);
                    if (sfxActivated)
                    {
                        volume = 0.3f;
                        this.GetComponent<AudioSource>().clip = audioClips["sleep"];

                    }

                    break;
                case "internet":
                    this.GetComponent<Animator>().Play("student_netsurfing", -1, 0f);
                    if (sfxActivated) {

                        volume = 0.27f;
                        this.GetComponent<AudioSource>().clip = audioClips["internet_long"];
                    }
                    break;
                case "mailing":
                    this.GetComponent<Animator>().Play("student_emailing", -1, 0f);
                    if (sfxActivated)
                    {
                        volume = 0.65f;
                        this.GetComponent<AudioSource>().clip = audioClips["emails"];
                    }
                    break;
                case "annoyingNoises":
                    this.GetComponent<Animator>().Play("student_noises", -1, 0f);
                    if (sfxActivated)
                    {
                        volume = 0.45f;
                        this.GetComponent<AudioSource>().clip = audioClips["FunnyNoises_C"];
                    }
                        
                    break;
                case "dancing":
                    this.GetComponent<Animator>().Play("student_dancing", -1, 0f);
                    if (sfxActivated)
                    {

                        //"dancing"
                        volume = 1.0f;
                        this.GetComponent<AudioSource>().clip = audioClips["dance_salsa"];
                    }
                    break;
                case "joking":
                    this.GetComponent<Animator>().Play("student_emailing", -1, 0f); // NECESITAMOS ANIMAÇAO DE LOS LOLES
                    if(sfxActivated)
                    {
                        volume = 1.0f;
                        int rand = Random.Range(0, 2);
                        if(rand == 0)
                        {
                            this.GetComponent<AudioSource>().clip = audioClips["jokes_A"];
                        } else
                        {
                            this.GetComponent<AudioSource>().clip = audioClips["jokes_B"];
                        }
                    }
                    break;		  
            }
        }
    }

public IEnumerator glow()
    {
        individualSkillClicked = true;
        GameObject halo = transform.GetChild(0).gameObject;
        // Rutina de Resaltar estudiante
        while(true)
        {
            for (float f = 0f; f <= 1f; f += 0.02f)
            {
                Color c = halo.GetComponent<SpriteRenderer>().color;
                c.a = f;
                halo.GetComponent<SpriteRenderer>().color = c;
                yield return null;
            }
            for (float f = 1f; f >= 0f; f -= 0.02f)
            {
                Color c = halo.GetComponent<SpriteRenderer>().color;
                c.a = f;
                halo.GetComponent<SpriteRenderer>().color = c;
                yield return null;
            }
            yield return null;
        }
    }

    public void unGlow()
    {
        StopCoroutine("glow");
        GameObject halo = transform.GetChild(0).gameObject;
        Color tmp = halo.GetComponent<SpriteRenderer>().color;
        tmp.a = 0f;
        halo.GetComponent<SpriteRenderer>().color = tmp;
    }
        

    void OnMouseDown()
    {
            GameObject manager = GameObject.FindGameObjectWithTag("Manager");
            manager.SendMessage("studentClicked", this);
    }

    public void setColor(Color color)
    {
        var _renderer = GetComponent<SpriteRenderer>();
        if (_renderer != null)
        {
            _renderer.color = color;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Laserchalk" && this.target == true)
        {
            GameObject chalk = col.gameObject;
            if(sfxActivated)
            {
                chalk.GetComponent<AudioSource>().Play();
            }		 
            chalk.GetComponent<SpriteRenderer>().enabled = false;
            chalk.GetComponent<Collider2D>().enabled = false;
            
            setDisturbing(false);
			GetComponent<Animator>().Play("student_bonk", -1, 0f);									  
            target = false;
        } else if(col.gameObject.tag == "Laserray" && this.target == true)
        {
            GameObject laser = GameObject.Find("laser(Clone)"); // GITANADA EXTREME
            laser.GetComponent<Laser>().increase = false;
            col.GetComponent<SpriteRenderer>().flipY = true;
            col.transform.position = this.transform.position;
           if (sfxActivated)
            {
                this.GetComponent<AudioSource>().clip = audioClips["laser_killed"];
                this.GetComponent<AudioSource>().Play();
            }
            this.GetComponent<Animator>().Play("student_exploding", -1, 0f);
            target = false;
            gameManager.SendMessage("studentDestroyed", this);
            
        }
    }

    public void setDisturbing(bool disturbing)
    {
        if(disturbing)
        {
            disturbActivated = true;
        } else
        {
            this.GetComponent<Animator>().Play(idleAnimation, -1, 0f);
            this.GetComponent<AudioSource>().Stop();
            disturbActivated = false;
        }
    }

    public void setConnected(bool conected)
    {
        if(!conected)
        {
            activated = false;
            print("LLEGO");
            cooldown = 5;
            this.GetComponent<Animator>().Play("student_disconnected", -1, 0f);
            this.GetComponent<AudioSource>().Stop();
        } else
        {
            activated = true;
            this.GetComponent<Animator>().Play(idleAnimation, -1, 0f);
            this.GetComponent<AudioSource>().Stop();
        }
    }

    public void link(Student s)
    {
        this.links.Add(s);
        s.links.Add(this);
    }
    public void link(Student a, Student b, Student c)
    {
        // links a
        this.links.Add(a);
        this.links.Add(b);
        this.links.Add(c);

        // links b
        a.links.Add(this);
        a.links.Add(b);
        a.links.Add(c);

        // links c
        b.links.Add(this);
        b.links.Add(a);
        b.links.Add(c);

        // links d
        c.links.Add(this);
        c.links.Add(a);
        c.links.Add(b);
    }
    public void setIdleAnimation(string animation,bool work)
    {
        idleAnimation = animation;
		        this.GetComponent<Animator>().SetBool("Working", work);
													   
        if(!disturbActivated)
        {
            this.GetComponent<Animator>().Play(idleAnimation, -1, 0f);
        }
    }						
    
    public void setAnimation(string animation)
    {
        print("Qué tal si bailamos?");
        this.GetComponent<Animator>().Play(animation, -1, 0f);
        print("Pues no");
    }			  
}
