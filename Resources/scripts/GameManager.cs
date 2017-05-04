using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System.Xml;


using UnityEngine.SceneManagement;

// public class haber si memuero
public class GameManager : MonoBehaviour
{

    public Canvas pauseMenu;
    bool paused;

    // GameObjects array
    GameObject[] skillGo; // Array a rellenar con skills para poder pasarlos a lista
    GameObject[] studentGo; // Lo mismo, pero con estudiantes
    public GameObject studentPrefab;


    public GameObject killedSprite, timeUpSprite, winSprite;


    // Lists
    List<Skill> skills = new List<Skill>();
    /*
        Esta lista es pública para poder añadir a mano las habilidades de este nivel
        Una vez completado el juego se debería rellenar con la información del menú
    */
    public Student[,] students;
    public ArrayList listeningStudents;    // Lista de estudiantes atendiendo
    ArrayList disturbingStudents;   // Lista de estudiantes molestando

    List<Task> tasks = new List<Task>();

    GameObject professor;
    // Iterators and selectors
    Skill activeSkill;
    int actualTask;
    public int taskBarSpeed;

    // counters
    //float totalLevelTime;
    public float stress;
    public int stressBars;
    float taskCompletion;
    float totalTaskDuration;
    float percentage;
    bool isGameOver;

    public static int rows;
    public static int columns;

    string towerNumber;

    // Multipliers
    float taskMultiplier = 1;
    float stressMultiplier = 0;

    public Text timerText;
    public Text taskText;
    public Text taskCompletionText;
    public Slider completionBar;



    //Time for the class to end, is public to change it from the editor
    public int totalTime; //we could put it in the level .txt file as well



    //Counters
    float totalStudentsDisturbing = 0;



    //For the level .xml file reading

    public string level;

    public XmlDocument readerXML = new XmlDocument();
    List<string> conflicts = new List<string>();
    int lineCounter;

    string[] elements = new string[3];
    float timesinceStart;

    char delimiter = '-';



    //conflict activated
    string actualConflict;

    //Mode of the task
    string taskType;
    int timeLeft;
    string timeText, timeLeftText;
    int minutesSinceStart, secondsSinceStart, minutesTimeLeft, secondsTimeLeft, timer1, timer2;


    // Stress Bar SpriteSheet
    Sprite[] stressBarSprites;

    public GameObject stressBar;

    Transform childRedBar, childOrangeBar, childBlueBar;
    float orangeBarProgress;
    public float stressSpeed;



    string time;
    bool ending;

    //Sounds
    public Toggle musicToggle;
    public Toggle sfxToggle;


    //GameOver parameters
    string deadby;
    bool ended = false;
    bool starting = true;

    // Blackboard parameters
    GameObject blackboard;
    float height = 0f;



    Dictionary<string, AudioClip> audioClipsPause = new Dictionary<string, AudioClip>();
						Dictionary<string, AudioClip> musicClips = new Dictionary<string, AudioClip>();
   														   
    // Slots
    public GameObject[] slots;
    public GameObject[] skillPrefabs;
    public Canvas skillUI;


    //Score
    int initialStudents,totalStudents;
    public Text score;


    public GameObject BG;


    // Use this for initialization
    void Awake()
    {
        Resources.Load("levels/");
        // En este caso, hay que llenar columns y rows a pincho, pero deberían leerse del archivo
        blackboard = GameObject.FindGameObjectWithTag("Blackboard");
        professor = GameObject.FindGameObjectWithTag("Professor");
        Time.timeScale = 1.0f;
        StartCoroutine(startLevel());
        

        listeningStudents = new ArrayList();
        disturbingStudents = new ArrayList();

        //Create the list of audios for the pause menu
        audioClipsPause.Add("menuIn_A", Resources.Load<AudioClip>("sfx/menu/menuIn_Ax2"));
        audioClipsPause.Add("menuIn_B", Resources.Load<AudioClip>("sfx/menu/menuIn_Bx2"));
        audioClipsPause.Add("menuOut_A", Resources.Load<AudioClip>("sfx/menu/menuOut_Ax2"));
        audioClipsPause.Add("menuOut_B", Resources.Load<AudioClip>("sfx/menu/menuOut_Bx2"));
        audioClipsPause.Add("click_A", Resources.Load<AudioClip>("sfx/menu/click_A"));
        audioClipsPause.Add("click_B", Resources.Load<AudioClip>("sfx/menu/click_B"));

		musicClips.Add("lesson_1010", Resources.Load<AudioClip>("music/HTR-lesson_1010"));
        musicClips.Add("dont_stop_the_class", Resources.Load<AudioClip>("music/HTR-dont_stop_the_class"));
        musicClips.Add("i_professor", Resources.Load<AudioClip>("music/HTR-i_professor"));
		musicClips.Add("ablid", Resources.Load<AudioClip>("music/HTR-ablid"));
        towerNumber = PlayerPrefs.GetString("tower");

        stress = 0;
        actualTask = 0;
        //Set sound toggles
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
        
    }

    void Start()
    {
        //totalLevelTime = totalTime;

        readXML();

        // Cargar habilidades y estudiantes
        skillGo = GameObject.FindGameObjectsWithTag("Skill");
        foreach (GameObject sk in skillGo)
        {
            skills.Add(sk.GetComponent<Skill>());
        }
        students = new Student[rows, columns];
        instantiateStudents();
        studentGo = GameObject.FindGameObjectsWithTag("Students");
        foreach (GameObject st in studentGo)
        {
            Student s = st.GetComponent<Student>();

            students[s.row, s.column] = s;
            listeningStudents.Add(s);
        }
        initialStudents = rows * columns; //get a track of how many students do we have at the beginning (useful for the score)
        totalStudents = initialStudents; 
        setTask(actualTask);
        updateTime();

        timer1 = (int)Time.time;
        stressBarSprites = Resources.LoadAll<Sprite>("sprites/stressBar_spriteSheet");


        childOrangeBar = stressBar.transform.FindChild("Mask/Orange");
        childRedBar = stressBar.transform.FindChild("Mask/Red");
        
        paused = false;
        isGameOver = false;
    }



    // Update is called once per frame
    void Update()
    {
        // Cuenta atrás
        if (starting)
        {
            height += 0.7f;
            blackboard.transform.localScale = new Vector3(30, height, 0);
        }
        if (!ended && !starting)

        {

            setMusic();//Check whether the music is switched on or not

            updateBars();//Update cooldowns, stress bar and task bar (to make it smoother)

            //(^^^^This code is out of the every second counter in order to make the bars smoother)

            timer2 = timer1;
            timer1 = (int)Time.time;
            if (timer1 > timer2) //We do all the stuff  each second
            {

                Debug.Log("Quedan " + totalTime + " segundos");
                timeLeft = totalTime--;



                if (timeLeft <= 0)
                {
                    Debug.Log("END OF THE CLASS");
                    deadby = "timeup";
                    StartCoroutine(gameOver(deadby));

                }
                else if (deadby == "stress")
                {
                    StartCoroutine(gameOver(deadby));
                }
                else
                {
                    updateTime();
                    totalStudentsDisturbing = disturbingStudents.Count;
                    if (totalStudentsDisturbing == 0) // Control cada segundo de que la clase vaya bien
                    {
                        stressMultiplier = 0;
                        taskMultiplier = 1;
                    }
                    Debug.Log("there are " + totalStudentsDisturbing + " students disturbing");


                    //Decrease students' cooldown
                    foreach (Student s in students)
                    {

                        if (s != null)
                        {
                            if (s.cooldown > 1)
                            {
                                s.cooldown--;
                            }
                            else if (s.cooldown == 1)
                            {
                                s.cooldown--;
                                s.setConnected(true);
                                listeningStudents.Add(s);
                            }
                        }


                    }

                    //Si la tarea se completa
                    if (percentage >= completionBar.maxValue)
                    {

                        actualTask++;
                        if (actualTask == tasks.Count)
                        {
                            Debug.Log("salgo");
                            StartCoroutine(gameOver("win"));
                        }
                        else
                        {
                            setTask(actualTask);
                        }

                    }


                    //activate conflicts
                    if ((lineCounter < conflicts.Count)) //change to foreach (string row in lines)
                    {
                        timesinceStart++;
                        secondsSinceStart = (int)timesinceStart % 60;
                        minutesSinceStart = (int)timesinceStart / 60;
                        timeText = minutesSinceStart + ":" + secondsSinceStart.ToString("00");

                        Debug.Log(timeText);
                        elements = conflicts[lineCounter].Split(delimiter);

                        if ((conflicts[lineCounter] != "") && (elements[0] == timeText))
                        {

                            //We activate the conflict with the same name as elements[1]
                            //
                            Debug.Log(elements[1] + " conflict has been activated...");
                            Debug.Log(elements[1].GetType());

                            actualConflict = GetComponent<Conflict>().getTypeConflict(elements[1]);



                            activateConflict(actualConflict, elements[1]);

                            Debug.Log("Time: " + elements[0]);
                            Debug.Log("Type: " + elements[1]);

                            lineCounter++;
                        }
                    }
                }
            }


        }
    }


    void skillClicked(Skill skill)
    {

        if (activeSkill == skill)
        {
            skill.unHighlight();
            unHighlightStudents();
            activeSkill = null;
        }
        activeSkill = skill; // Para poder hacer las acciones individuales
        activeSkill.highlight();
        foreach (Skill sk in skills) // Desactivar habilidades inactivas
        {
            if (sk != activeSkill && !sk.onCooldown)
            {
                sk.unHighlight();
            }
        }

        // Distinguir entre habilidad de relax, masiva e individual
        switch (skill.skType)
        {
            case Skill.SkillType.Individual:
                {
                    highlightStudents();
                    break;
                }
            case Skill.SkillType.Unstress:
                {
                    activeSkill.action();
                    unHighlightStudents();
                    break;
                }
            case Skill.SkillType.Massive:
                {
                    if (!activeSkill.affectsAll) //It affects just few of the students
                    {
                        highlightStudents();
                    }
                    else//it's 
                    {
                        activeSkill.action(students);
                        unHighlightStudents();
                        foreach (Student s in students)
                        {
                            if (s != null)
                            {
                                s.setColor(Color.white);

                                //we put it again in the listening students list
                                disturbingStudents.Remove(s);
                                if (!s.activated)
                                {
                                    listeningStudents.Remove(s);
                                }
                                s.links.Clear();
                            }
                        }
                        taskMultiplier = 1;
                        stressMultiplier = 0;

                    }
                    break;
                }
        }
    }


    void studentClicked(Student s)
    {

        if ((activeSkill != null)&&(!paused))
        {

            unHighlightStudents();
            if ((activeSkill.skType == Skill.SkillType.Massive) && (!activeSkill.affectsAll))
            {
                    
                activeSkill.action(s, students);

                if (activeSkill.stressCost > 1)
                {
                    stressBars = stressBars - activeSkill.stressCost / 2;
                    setStress(stressBars);
                }
            }
            else
            {

                activeSkill.action(s);

                if (activeSkill.stressCost > 1)
                {
                    
                    stressBars = stressBars - activeSkill.stressCost / 2;
                    setStress(stressBars);
                }
            }
            activeSkill = null;

            //we put it again in the listening students list
            disturbingStudents.Remove(s);
            if (s.activated) // Controlamos que no le hayamos apagado
            {
                listeningStudents.Add(s);
            }

            clearLinks(s);
            // CONDICIONALES DE ENGANCHE



        }
    }

    public void clearLinks(Student s)
    {

        float newMultiplier = 0.1f;
        if (s.links.Count == 1) // Solo tiene un compañero (DUAL O MASIVO Y SOLO QUEDAN DOS)
        {
            Student companion = (Student)s.links[0];
            companion.setDisturbing(false);
            disturbingStudents.Remove(companion);
            listeningStudents.Add(companion);
            companion.links.Clear();

            newMultiplier = 0.3f;
        }
        else if (s.links.Count > 1 && s.instigator) // HEMOS PILLAO AL MALO
        {
            foreach (Student student in s.links)
            {
                student.setDisturbing(false);
                disturbingStudents.Remove(student);
                listeningStudents.Add(student);
                student.links.Clear();
                newMultiplier = 0.75f;
            }
            s.instigator = false;
        }
        else if (s.links.Count > 1 && !s.instigator) // LA SIGUEN LIANDO
        {
            foreach (Student student in s.links)
            {
                student.links.Remove(s);
            }
        }

        s.links.Clear();

        if (stressMultiplier >= 0.1f)
        {
            stressMultiplier -= newMultiplier;
        }
        if (taskMultiplier <= 0.9f)
        {
            taskMultiplier += newMultiplier;
        }
    }


    void activateConflict(string typeConflict, string conflict)
    {
        int randIndex;

        switch (typeConflict)
        {
            case "individual":


                Debug.Log("And is type individual");

                if (listeningStudents.Count > 0)
                {
                    randIndex = Random.Range(0, listeningStudents.Count);
                    Student student1 = (Student)listeningStudents[randIndex];
                    setDisturbingStudent(student1, conflict);
                    student1.GetComponent<AudioSource>().volume = student1.volume;
                    student1.GetComponent<AudioSource>().Play();
                    changeMultiplier(0.1f);
                }

                break;
            case "dual":


                Debug.Log("And is type dual");

                if (listeningStudents.Count > 1)

                {
                    // Falta conectarlos entre ellos
                    randIndex = Random.Range(0, listeningStudents.Count);
                    Student student1 = (Student)listeningStudents[randIndex];
                    setDisturbingStudent(student1, conflict);
                    randIndex = Random.Range(0, listeningStudents.Count);
                    Student student2 = (Student)listeningStudents[randIndex];
                    setDisturbingStudent(student2, conflict);






                    student1.link(student2);
                    student1.GetComponent<AudioSource>().volume = student1.volume;
                    student1.GetComponent<AudioSource>().Play();
                    changeMultiplier(0.3f);
                }




                break;
            case "massive":

                Debug.Log("And is type massive");

                if (listeningStudents.Count > 3)


                {
                    // Falta conectarlos entre ellos
                    randIndex = Random.Range(0, listeningStudents.Count);
                    Student student1 = (Student)listeningStudents[randIndex];
                    student1.instigator = true; // EL MALOSO
                    setDisturbingStudent(student1, conflict);
                    randIndex = Random.Range(0, listeningStudents.Count);
                    Student student2 = (Student)listeningStudents[randIndex];
                    setDisturbingStudent(student2, conflict);
                    randIndex = Random.Range(0, listeningStudents.Count);
                    Student student3 = (Student)listeningStudents[randIndex];
                    setDisturbingStudent(student3, conflict);
                    randIndex = Random.Range(0, listeningStudents.Count);
                    Student student4 = (Student)listeningStudents[randIndex];
                    setDisturbingStudent(student4, conflict);



                    student1.link(student2, student3, student4);
                    student1.GetComponent<AudioSource>().volume = student1.volume;
                    student1.GetComponent<AudioSource>().Play();
                    changeMultiplier(0.75f);
                }




                break;
            default:
                break;
        }
    }


    public void highlightStudents()
    {

        foreach (Student s in students)
        {
            if (s != null)
            {
                s.StartCoroutine("glow");
            }
        }
    }

    void studentDestroyed(Student s)
    {
        //si estaba molestando, deberemos quitarlo de la lista
        listeningStudents.Remove(s);
        disturbingStudents.Remove(s);

        students[s.row, s.column] = null;
        totalStudents--;
        if (totalStudents == 0)
        {
            gameOver("stress");//Sin estudiantes, se acaba el juego
        }
        // Quitar puntos


    }

    void unHighlightStudents()
    {
        foreach (Student s in students)
        {
            if (s != null)
            {
                s.unGlow();
            }
        }
    }


    void instantiateStudents()
    {
        int studentCount = 0;
        for (int i = 0; i < rows; i++)
        {
            float rowPos = rows - i - 1.5f;
            for (int j = 0; j < columns; j++)
            {
                float columnPos = ((float)(columns - 1) / 2) - j;
                GameObject student = Instantiate(studentPrefab);
                student.GetComponent<Student>().row = i;
                student.GetComponent<Student>().column = j;
                student.transform.position = new Vector3(-columnPos, rowPos, 0f);
                student.name = "Student" + studentCount;
                studentCount++;
            }
        }
    }


    void changeMultiplier(float multiplier)
    {
        if (taskMultiplier - multiplier <= 0)
        {
            taskMultiplier = 0;
        }
        else
        {
            taskMultiplier -= multiplier;
        }
        if (stressMultiplier + multiplier >= 1)
        {
            stressMultiplier = 1;
        }
        else
        {
            stressMultiplier += multiplier;
        }
    }

    void setTask(int iterator)
    {
        taskCompletion = tasks[iterator].duration;
        totalTaskDuration = taskCompletion;
        taskText.text = tasks[iterator].title;

        taskType = tasks[iterator].tkType.ToString();//define the kind of task we are currently doing

        switch (taskType)
        {
            case "Talk":
                professor.GetComponent<Animator>().SetInteger("Task", 0);//For the animations to know to which of the tasks to come back
                foreach (Student s in students)
                {
                    if (s != null)
                    {
                        s.setIdleAnimation("student_idle", false);
                    }
                }
                professor.GetComponent<Animator>().Play("professor_talk", -1, 0f);
                break;
            case "Work":
                professor.GetComponent<Animator>().SetInteger("Task", 1);//For the animations to know to which of the tasks to come back
                foreach (Student s in students)
                {
                    if (s != null)
                    {
                        s.setIdleAnimation("student_work", true);
                    }
                }
                professor.GetComponent<Animator>().Play("professor_work", -1, 0f);
                break;
            case "Blackboard":
                professor.GetComponent<Animator>().SetInteger("Task", 2);//For the animations to know to which of the tasks to come back
                foreach (Student s in students)
                {
                    if (s != null)
                    {
                        s.setIdleAnimation("student_idle", false);
                    }
                }
                professor.GetComponent<Animator>().Play("professor_blackboard", -1, 0f);
                break;
        }
    }

    void restartCooldowns()
    {
        foreach (GameObject sk in skillGo)
        {
            if (sk.name != "restart" && sk.GetComponent<Skill>().onCooldown)
            {
                sk.GetComponent<Skill>().butComp.fillAmount = 0f;
                sk.GetComponent<Skill>().decreaseCooldownBar();
            }
        }
    }


    //Task reader
    void readXML()
    {
        TextAsset textAsset = new TextAsset();

        //If we want to debug
        //textAsset = (TextAsset)Resources.Load("levels/" + level);

        //To choose the level from the selector
        textAsset = (TextAsset)Resources.Load("levels/" + PlayerPrefs.GetString("level"));
        Debug.Log("Abrimos nivel " + PlayerPrefs.GetString("level"));

        Debug.Log(textAsset.text);
        readerXML.LoadXml(textAsset.text);
        string music = readerXML.DocumentElement.SelectSingleNode("/Level/General/Music").InnerText;
        this.GetComponent<AudioSource>().clip = musicClips[music];


        Debug.Log("Se ha pintado la clase classrooms/class_" + towerNumber);
        BG.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprites/classrooms/class_" + towerNumber);

        string timeParser = readerXML.DocumentElement.SelectSingleNode("/Level/General/Duration").InnerText;
        string[] tok = timeParser.Split(':');
        int minToSec = int.Parse(tok[0]) * 60;
        int sec = int.Parse(tok[1]);
        totalTime = minToSec + sec;

        XmlNodeList nodesDocument = readerXML.DocumentElement.SelectNodes("/Level/General/Students");

        foreach (XmlNode node in nodesDocument)//reading from the XML
        {
            rows = int.Parse(node.SelectSingleNode("rows").InnerText);
            columns = int.Parse(node.SelectSingleNode("columns").InnerText);
        }

        // Leemos las skills
        //
        string skills = PlayerPrefs.GetString("PlayerSkills");
        string[] PlayerSkills = PlayerPrefs.GetString("PlayerSkills").Split('/');
        int i = 0;
        foreach (string sk in PlayerSkills)
        {
            
            bool found = false;
            int j = 0;

            //Esto de aquí hace falta???vvvvv
            while (!found && j < skillPrefabs.Length)
            {
                string name = sk;
                if (name == skillPrefabs[j].name)
                {
                    found = true;
                    Debug.Log("ENCONTRAO " + name);
                }
                else
                {
                    j++;
                }
            }
            //^^^^^^^^

            if (found)//if (name==skillPrefabs.name)
            {
                GameObject skill = Instantiate(skillPrefabs[j], skillUI.transform);

                skill.GetComponent<RectTransform>().anchorMax = slots[i].GetComponent<RectTransform>().anchorMax;
                skill.GetComponent<RectTransform>().anchorMin = slots[i].GetComponent<RectTransform>().anchorMin;
                skill.GetComponent<RectTransform>().sizeDelta = slots[i].GetComponent<RectTransform>().sizeDelta;
                skill.GetComponent<RectTransform>().transform.localScale = slots[i].GetComponent<RectTransform>().localScale;
                skill.GetComponent<RectTransform>().anchoredPosition = slots[i].GetComponent<RectTransform>().anchoredPosition;
                skill.GetComponent<Skill>().setColor(Color.gray);
                Destroy(slots[i]);
                i++;
            }
        }

        nodesDocument = readerXML.DocumentElement.SelectNodes("/Level/Tasks/Task");

        foreach (XmlNode node in nodesDocument)//reading from the XML
        {

            elements[0] = node.SelectSingleNode("Type").InnerText;
            elements[1] = node.SelectSingleNode("Title").InnerText;
            elements[2] = node.SelectSingleNode("Duration").InnerText;
            Debug.Log("Type: " + elements[0]);
            Debug.Log("Title: " + elements[1]);
            Debug.Log("Duration: " + elements[2]);
            tasks.Add(new Task(elements[0], elements[1], float.Parse(elements[2])));

        }

        nodesDocument = readerXML.DocumentElement.SelectNodes("/Level/Walkthrough/Event");
        
        Debug.Log(nodesDocument.Count);
        foreach (XmlNode node in nodesDocument)//reading from the XML
        {
            Debug.Log("Time: " + node.SelectSingleNode("Time").InnerText);
            Debug.Log("Title: " + node.SelectSingleNode("Type").InnerText);
            conflicts.Add(node.SelectSingleNode("Time").InnerText + "-" + node.SelectSingleNode("Type").InnerText);

        }

        Debug.Log("Hay " + conflicts.Count + " conflictos");



        Debug.Log("--STARTING WALKTHROUGH--");
    }

    public void setStress(int stressLevel)
    {
        Debug.Log("Lo pongo a " + stressLevel);
        //revisar
        childRedBar.GetComponent<Image>().fillAmount = stressLevel * 0.2f;
        float actualAmountOrange = childOrangeBar.GetComponent<Image>().fillAmount%0.2f;
        childOrangeBar.GetComponent<Image>().fillAmount = (stressLevel * 0.2f) + actualAmountOrange;

        

    }

    private void setDisturbingStudent(Student s, string conflict)
    {
        s.setDisturbing(true);
        s.disturb(conflict);

        disturbingStudents.Add(s);
        listeningStudents.Remove(s);
    }


    IEnumerator gameOver(string deadby)
    {
        foreach (Skill sk in skills) // Desactivar habilidades
        {
            sk.highlight();

        }
        Time.timeScale = 0f;
        isGameOver = true;
        Debug.Log("Es Gameover? " + isGameOver);
        switch (deadby)
        {
            case "timeup":

                if (totalStudents >= (initialStudents - 1)){ //se ha destruido como mucho un alumno
                    PlayerPrefs.SetString(PlayerPrefs.GetString("level")+"score", "B");

                    score.text = "You got a B";
                }
                else
                {
                    PlayerPrefs.SetString(PlayerPrefs.GetString("level") + "score", "C");
                    score.text = "You got a C";
                }

                timeUpSprite.SetActive(true);
                while (!Input.GetMouseButtonDown(0))
                    yield return StartCoroutine(WaitForKeyDown(Input.GetMouseButtonDown(0)));

                restartGame();

                break;
            case "stress":
                PlayerPrefs.SetString(PlayerPrefs.GetString("level")+"score", "FAIL");
                Time.timeScale = 1f;
                professor.GetComponent<Animator>().Play("professor_explode", -1, 0f);

                if (PlayerPrefs.GetInt("sfx") == 1)
                {
                    professor.GetComponent<AudioSource>().Play();
                }

                yield return new WaitForSeconds(2f);
                Time.timeScale = 0f;
                killedSprite.SetActive(true);

                while (!Input.GetMouseButtonDown(0))
                {

                    yield return StartCoroutine(WaitForKeyDown(Input.GetMouseButtonDown(0)));
                }
                restartGame();
                break;
            case "win":
                //Record score
                if (totalStudents ==initialStudents)
                { //se ha destruido como mucho un alumno
                    PlayerPrefs.SetString(PlayerPrefs.GetString("level")+"score", "A");
                }
                else if(totalStudents==(initialStudents-1))
                {
                    PlayerPrefs.SetString(PlayerPrefs.GetString("level")+"score", "B");
                }
                else //si se han destruido + de 1 alumno
                {
                    PlayerPrefs.SetString(PlayerPrefs.GetString("level")+"score", "C");
                }


                winSprite.SetActive(true);

                while (!Input.GetMouseButton(0))
                {
                    yield return StartCoroutine(WaitForKeyDown(Input.GetMouseButtonDown(0)));
                }

                restartGame();
                break;
            default:
                Debug.Log("No idea why I'm dead");
                break;
        }
    }

    IEnumerator startLevel()
    {

        if (PlayerPrefs.GetInt("sfx") == 1)
            blackboard.GetComponent<AudioSource>().Play();
        // aquí podríamos poner algo como los alumnos entrando a clase, o algo así. De momento, el timbre y la pizarra subiendo
        while (height < 30f)//Repeat until blackboard is complete
        {
            yield return null;
        }
        starting = false;
    }

    public void restartGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator WaitForKeyDown(bool mousePressed)
    {

        yield return null;

    }


    //Show time
    void updateTime()
    {
        int seconds = (int)timeLeft % 60;
        int minutes = (int)timeLeft / 60;
        time = minutes.ToString("00") + ":" + seconds.ToString("00");

        timerText.text = time;
        if (seconds == 10 && !ending && minutes == 0)
        {
            ending = true;
            levelEnding();
        }
    }

    void levelEnding()
    {
        StartCoroutine("endingTimer");

        GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("music/How To Robot - 10 Seconds");
        GetComponent<AudioSource>().loop = false;
        GetComponent<AudioSource>().Play();
    }

    public IEnumerator endingTimer()
    {
        while (true)
        {
            int i = 14;
            int adding = 0;
            for (float f = 1f; f >= 0; f -= 0.017f)
            {
                timerText.color = new Color(f, 0, 0);
                adding = (int)(f * 10);
                Debug.Log(adding);
                timerText.fontSize = i + adding;
                yield return null;
            }
        }
    }

    public void exitGame()
    {
        Debug.Log("SALIR");
        SceneManager.LoadScene("tower");
    }

    public void TogglePauseMenu()
    {
        AudioSource[] allAudioSources =  FindObjectsOfType<AudioSource>();
        for(int i=0; i < allAudioSources.Length; i++)
        {
            allAudioSources[i].Pause();
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
            Time.timeScale = 1.0f;
            for (int i = 0; i < allAudioSources.Length; i++)
            {
                allAudioSources[i].UnPause();
            }
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
            Time.timeScale = 0f;
        }
        
    }

    public void setMusic()
    {

        if (PlayerPrefs.GetInt("music") == 1)
        {

            this.GetComponent<AudioSource>().mute = false;
            if (!this.GetComponent<AudioSource>().isPlaying)
            {
                this.GetComponent<AudioSource>().Play();
            }
        }
        else
        {

            this.GetComponent<AudioSource>().mute = true;
        }
        if (sfxToggle.isOn)
        {
            foreach (Skill sk in skills) // Desactivar habilidades inactivas
            {

                sk.sfxActivated = true;
            }
        }
        else
        {
            foreach (Skill sk in skills) // Desactivar habilidades inactivas
            {

                sk.sfxActivated = false;
            }
        }

        if (sfxToggle.isOn)
        {
            foreach (Student st in students) // Desactivar habilidades inactivas
            {
                if (st != null)
                {

                    st.sfxActivated = true;
                }
            }
        }
        else
        {
            foreach (Student st in students) // Desactivar habilidades inactivas
            {
                if (st != null)
                {

                    st.sfxActivated = false;

                }
            }
        }
    }


    void updateBars()
    {
        if (!isGameOver)
        {
            if ((!paused))
            {
                float orangeAmount = childOrangeBar.GetComponent<Image>().fillAmount;
                childOrangeBar.GetComponent<Image>().fillAmount = childOrangeBar.GetComponent<Image>().fillAmount + (stressSpeed * stressMultiplier);
                
                if (orangeAmount >= 0.2f)
                {
                    childRedBar.GetComponent<Image>().fillAmount = 0.2f;
                    stressBars = 1;
                    
                }
                if (orangeAmount >= 0.4f)
                {
                    childRedBar.GetComponent<Image>().fillAmount = 0.4f;
                    stressBars = 2;
                }
                 if (orangeAmount >= 0.6f)
                {
                    childRedBar.GetComponent<Image>().fillAmount = 0.6f;
                    stressBars = 3;
                }
                 if (orangeAmount >= 0.8f)
                {
                    childRedBar.GetComponent<Image>().fillAmount = 0.8f;
                    stressBars = 4;
                }
                 if (orangeAmount >= 1f)
                {
                    childRedBar.GetComponent<Image>().fillAmount = 1f;
                    StartCoroutine(gameOver("stress"));
                    if (isGameOver)
                        return;
                }

              
                switch (taskType)
                {
                    case "Talk":
                        //Increasing task each seconds (keeping in mind the multiplier), we have to alsokeep in mind the ones destroyed
                        taskCompletion = taskCompletion - (1 * taskMultiplier);//every second we 
                        break;
                    case "Work":
                        if (totalStudentsDisturbing == 0)//If there is someone disturnbing, keep in mind that they could be destroyed will not affect
                        {
                            taskMultiplier = 1;
                            taskCompletion = taskCompletion - (1 * taskMultiplier);//every second we 
                        }
                        else
                        {
                            //Increasing task each seconds (keeping in mind the multiplier)
                            taskMultiplier = 0;
                            taskCompletion = taskCompletion - (1 * taskMultiplier);

                        }
                        break;
                    case "Blackboard":
                        //Increasing task each seconds (keeping in mind the multiplier), we have to alsokeep in mind the ones destroyed


                        taskCompletion = taskCompletion - (1 * taskMultiplier);//for now we do it as in Talk tasks, later we have to adapt it
                        break;
                    default:
                        Debug.Log("ERROR");
                        break;
                }
            }
            percentage = (taskBarSpeed * (totalTaskDuration - taskCompletion)) / totalTaskDuration;

            taskCompletionText.text = ((int)percentage).ToString() + "%";

            completionBar.value = percentage;

            //Decrease cooldown skills
            foreach (Skill sk in skills)
            {

                if (stressBars < sk.stressCost && !sk.onCooldown)
                    sk.setColor(Color.gray);
                else if (stressBars>= sk.stressCost && !sk.onCooldown)
                    sk.setColor(Color.white);
                if (sk.cooldown > 1 && sk.onCooldown)
                {
                    //Reduce fill amount over 30 seconds
                    sk.decreaseCooldownBar();
                }
                else if (sk.cooldown == 1 && sk.onCooldown)
                {
                    //sk.cooldown--;
                    //sk.decreaseCooldownBar();
                    //sk.toggleCooldown();
                }
            }
        }
    }


    public void MusicToggle()
    {
        if (musicToggle.isOn)
            PlayerPrefs.SetInt("music", 1);//enabled
        else
            PlayerPrefs.SetInt("music", 0);//disabled

    }

    public void SFXToggle()
    {
        if (sfxToggle.isOn)
            PlayerPrefs.SetInt("sfx", 1);//enabled
        else
            PlayerPrefs.SetInt("sfx", 0);//disabled

    }

    void professorAnimation(string anim)
    {
        professor.GetComponent<Animator>().Play("professor_" + anim, -1, 0f);
    }

}
