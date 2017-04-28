using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PopupMenu : MonoBehaviour
{

    XmlDocument readerXML = new XmlDocument();
    // Main Panel
    private string level;
    public GameObject[] mainSlots;

    public GameObject[] skillSlots;
    public Text talkCounter;
    public Text workCounter;
    public Text blackboardCounter;
    private TextAsset textAsset;

    // Skill Panel
    private GameObject[] skills;
    private List<MenuSkill> activeSkills;
    private List<MenuSkill> recommendedSkills;

    // Task Panel
    private GameObject[] tasks;
    private List<MenuTask> activeTasks;

    // Sounds
    Dictionary<string, AudioClip> audioClips;
    AudioSource audio;

    // Use this for initialization
    void Awake()
    {
        // inicialización sfx
        audioClips = new Dictionary<string, AudioClip>();
        audioClips.Add("click_A", Resources.Load<AudioClip>("sfx/menu/click_A"));
        audioClips.Add("click_B", Resources.Load<AudioClip>("sfx/menu/click_B"));
        audioClips.Add("holdInfo_A", Resources.Load<AudioClip>("sfx/menu/holdInfo_A"));
        audioClips.Add("holdInfo_B", Resources.Load<AudioClip>("sfx/menu/holdInfo_B"));
        audioClips.Add("menuIn_A", Resources.Load<AudioClip>("sfx/menu/menuIn_Ax2"));
        audioClips.Add("menuIn_B", Resources.Load<AudioClip>("sfx/menu/menuIn_Bx2"));
        audioClips.Add("menuOut_A", Resources.Load<AudioClip>("sfx/menu/menuOut_Ax2"));
        audioClips.Add("menuOut_B", Resources.Load<AudioClip>("sfx/menu/menuOut_Bx2"));
        audioClips.Add("play_A", Resources.Load<AudioClip>("sfx/menu/play_A"));
        audioClips.Add("recommended_A", Resources.Load<AudioClip>("sfx/menu/recommended_A"));
        audioClips.Add("selectSkill_A", Resources.Load<AudioClip>("sfx/menu/selectSkill_Ax2"));
        audioClips.Add("selectSkill_B", Resources.Load<AudioClip>("sfx/menu/selectSkill_Bx2"));
        audioClips.Add("selectSkill_C", Resources.Load<AudioClip>("sfx/menu/selectSkill_Cx2"));
        audioClips.Add("unselectSkill_A", Resources.Load<AudioClip>("sfx/menu/unselectSkill_Ax2"));
        audioClips.Add("unselectSkill_B", Resources.Load<AudioClip>("sfx/menu/unselectSkill_Bx2"));
        audioClips.Add("skillButton_A", Resources.Load<AudioClip>("sfx/menu/skillButton_Ax2"));
        audioClips.Add("skillButton_B", Resources.Load<AudioClip>("sfx/menu/skillButton_Bx2"));
        audioClips.Add("taskButton_A", Resources.Load<AudioClip>("sfx/menu/taskButton_A"));
        audioClips.Add("taskButton_B", Resources.Load<AudioClip>("sfx/menu/taskButton_B"));

        audio = GetComponent<AudioSource>();

        if (PlayerPrefs.GetInt("sfx") == 0)
        {
            this.GetComponent<AudioSource>().enabled = false;
        }
        else
        {

            this.GetComponent<AudioSource>().enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // GESTIÓN DEL MAIN PANEL
    public void loadPopup(string level)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        this.level = level;
        startSettings();

        // Audio
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            this.GetComponent<AudioSource>().clip = audioClips["menuIn_A"];
        }
        else
        {
            this.GetComponent<AudioSource>().clip = audioClips["menuIn_B"];
        }
        audio.Play();
    }

    void startSettings()
    {
        // Inicializamos lista para cada nivel
        activeSkills = new List<MenuSkill>();
        recommendedSkills = new List<MenuSkill>();
        activeTasks = new List<MenuTask>();

        print("LEEMOS LAS SETTINGS");
        textAsset = new TextAsset();
        textAsset = (TextAsset)Resources.Load("levels/" + level);

        readerXML.LoadXml(textAsset.text);

        // Rellenar lista de skills activas
        XmlNodeList nodesDocument = readerXML.DocumentElement.SelectNodes("/Level/Settings/Recommended/Skill");
        //Escribimos en el documento que las habilidades recomendadas sean las habilidades del nivel (hasta que no se demuestre lo contrario)
        XmlNode nodesSkills = readerXML.DocumentElement.SelectSingleNode("/Level/Skills");
        nodesSkills.RemoveAll();

        for (int i = 0; i < nodesDocument.Count; i++)
        {
            MenuSkill sk = new MenuSkill();
            sk.skName = nodesDocument[i].SelectSingleNode("Name").InnerText;
            sk.stressLevel = int.Parse(nodesDocument[i].SelectSingleNode("Stress").InnerText);

            print("GUARDO " + sk.skName);
            //
            XmlNode newNode = readerXML.CreateNode(XmlNodeType.Element, "Skill", null);
            XmlNode titleNode = readerXML.CreateNode(XmlNodeType.Element, "Name", null);
            titleNode.InnerText = sk.skName;
            XmlNode stressNode = readerXML.CreateNode(XmlNodeType.Element, "Stress", null);
            stressNode.InnerText = sk.stressLevel.ToString();

            newNode.AppendChild(titleNode);
            newNode.AppendChild(stressNode);
            nodesSkills.AppendChild(newNode);

            readerXML.Save("Assets/Resources/levels/" + level + ".xml");

            //
            activeSkills.Add(sk);
            recommendedSkills.Add((MenuSkill)sk.Clone());
        }

        // Si se veta a un número determinado de skills, se indica visualmente
        for (int i = nodesDocument.Count; i < mainSlots.Length; i++)
        {
            MenuSkill sk = new MenuSkill();
            sk.skName = "none";
            sk.stressLevel = 10;
            activeSkills.Add(sk);
            recommendedSkills.Add((MenuSkill)sk.Clone());
        }
        



        activeSkills.Sort(SortByStress);
        fillMainSlots();

        // Rellenar contadores de tareas
        int workTasks = 0;
        int talkTasks = 0;
        int blackboardTasks = 0;

        nodesDocument = readerXML.DocumentElement.SelectNodes("/Level/Tasks/Task");

        foreach (XmlNode node in nodesDocument)//reading from the XML
        {
            MenuTask tsk = new MenuTask();
            tsk.type = node.SelectSingleNode("Type").InnerText;
            switch (tsk.type)
            {
                case "Talk":
                    talkTasks++;
                    break;
                case "Work":
                    workTasks++;
                    break;
                case "Blackboard":
                    blackboardTasks++;
                    break;
            }
            tsk.title = node.SelectSingleNode("Title").InnerText;
            tsk.duration = int.Parse(node.SelectSingleNode("Duration").InnerText);
            activeTasks.Add(tsk);
        }

        talkCounter.text = talkTasks.ToString();
        workCounter.text = workTasks.ToString();
        blackboardCounter.text = blackboardTasks.ToString();
    }

    public void back()
    {
        transform.GetChild(0).gameObject.SetActive(false);

        // Audio
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            this.GetComponent<AudioSource>().clip = audioClips["menuOut_A"];
        }
        else
        {
            this.GetComponent<AudioSource>().clip = audioClips["menuOut_B"];
        }
        audio.Play();
    }

    void fillMainSlots()
    {
        for (int i = 0; i < activeSkills.Count; i++)
        {
            if (activeSkills[i].skName == "none")
            {
                mainSlots[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprites/nope");

                mainSlots[i].GetComponent<Image>().color = Color.white; // Reajustar el alfa
            }
            else
            {
                mainSlots[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprites/skills/button_" + activeSkills[i].skName);
                print(activeSkills[i].skName);

                mainSlots[i].GetComponent<Image>().color = Color.white; // Reajustar el alfa
                if (activeSkills[i].skName == "null") 
                {
                    mainSlots[i].GetComponent<Image>().color = new Color32(0, 0, 0, 128);
                }
            }





        }
    }

    public void startLevel()
    {
        PlayerPrefs.SetString("level", level);
        SceneManager.LoadScene("level");

        // Audio

        this.GetComponent<AudioSource>().clip = audioClips["play_A"];
        audio.Play();
    }

    // GESTIÓN DEL SKILL PANEL
    public void loadSkillPanel()
    {
        transform.GetChild(1).gameObject.SetActive(true);
        skills = GameObject.FindGameObjectsWithTag("Skill");
        fillSkillSlots();
        foreach (GameObject skill in skills)
        {
            skill.GetComponent<MenuSkill>().selected = false;
            for (int i = 0; i < activeSkills.Count; i++)
            {
                if (skill.name == activeSkills[i].skName)
                {
                    skill.GetComponent<Image>().color = new Color32(160, 160, 160, 255);
                    skill.GetComponent<MenuSkill>().selected = true;
                    activeSkills[i].linkedSkill = skill;
                    recommendedSkills[i].linkedSkill = skill;
                }
            }
        }
        // Audio
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            this.GetComponent<AudioSource>().clip = audioClips["skillButton_A"];
        }
        else
        {
            this.GetComponent<AudioSource>().clip = audioClips["skillButton_B"];
        }
        audio.Play();
        this.gameObject.SetActive(true);
    }

    void fillSkillSlots()
    {
        for (int i = 0; i < activeSkills.Count; i++)
        {
            if (activeSkills[i].skName == "none")
            {
                skillSlots[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprites/nope");
                skillSlots[i].GetComponent<Image>().color = Color.white; // Reajustar el alfa
                skillSlots[i].GetComponent<MenuSkill>().forbidden = true;
            }
            else if (activeSkills[i].skName == "null")
            {
                skillSlots[i].GetComponent<Image>().sprite = null;

                skillSlots[i].GetComponent<Image>().color = new Color32(0, 0, 0, 128);
            }
            else
            {
                skillSlots[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprites/skills/button_" + activeSkills[i].skName);
                skillSlots[i].GetComponent<Image>().color = Color.white; // Reajustar el alfa
            }
        }
    }

    public void done()
    {
        transform.GetChild(1).gameObject.SetActive(false);
        XmlNode nodesDocument = readerXML.DocumentElement.SelectSingleNode("/Level/Skills");
        nodesDocument.RemoveAll();
        foreach (MenuSkill skill in activeSkills)
        {
            XmlNode newNode = readerXML.CreateNode(XmlNodeType.Element, "Skill", null);
            XmlNode titleNode = readerXML.CreateNode(XmlNodeType.Element, "Name", null);
            titleNode.InnerText = skill.skName;
            XmlNode stressNode = readerXML.CreateNode(XmlNodeType.Element, "Stress", null);
            stressNode.InnerText = skill.stressLevel.ToString();

            newNode.AppendChild(titleNode);
            newNode.AppendChild(stressNode);
            nodesDocument.AppendChild(newNode);
        }
        readerXML.Save("Assets/Resources/levels/" + level + ".xml");
        
        fillMainSlots();
        // Audio
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            this.GetComponent<AudioSource>().clip = audioClips["menuOut_A"];
        }
        else
        {
            this.GetComponent<AudioSource>().clip = audioClips["menuOut_B"];
        }
        audio.Play();
    }

    public void recommended()
    {
        activeSkills.Clear();
        foreach (GameObject skill in skills) // Desmarcamos todas
        {
            skill.GetComponent<Image>().color = Color.white;
            skill.GetComponent<MenuSkill>().selected = false;
        }
        for (int i = 0; i < recommendedSkills.Count; i++) // Ponemos como activas las recomendadas
        {
            activeSkills.Add((MenuSkill)recommendedSkills[i].Clone());
            if (activeSkills[i].linkedSkill != null)
            {
                activeSkills[i].linkedSkill.GetComponent<Image>().color = new Color32(160, 160, 160, 255);
                activeSkills[i].linkedSkill.GetComponent<MenuSkill>().selected = true;
            }

        }
        fillSkillSlots();
        // Audio
        this.GetComponent<AudioSource>().clip = audioClips["recommended_A"];
        audio.Play();
    }

    void skillClicked(GameObject skill)
    {
        bool encontrado = false;
        int i = 0;
        while (!encontrado && i < skillSlots.Length)
        {
            if (activeSkills[i].skName == "null")
            {
                activeSkills[i].skName = skill.GetComponent<MenuSkill>().skName;
                activeSkills[i].stressLevel = skill.GetComponent<MenuSkill>().stressLevel;
                activeSkills[i].linkedSkill = skill;
                encontrado = true;
            }
            else
            {
                i++;
            }
        }
        if (!encontrado)
        {
            print("No sé qué coño has hecho tío");
        }
        else
        {
            skill.GetComponent<Image>().color = new Color32(127, 127, 127, 255);
            skill.GetComponent<MenuSkill>().selected = true;
            activeSkills.Sort(SortByStress);
            fillSkillSlots();
            // Audio
            int rand = Random.Range(0, 3);
            if (rand == 0)
            {
                this.GetComponent<AudioSource>().clip = audioClips["selectSkill_A"];
            }
            else if (rand == 1)
            {
                this.GetComponent<AudioSource>().clip = audioClips["selectSkill_B"];
            }
            else
            {
                this.GetComponent<AudioSource>().clip = audioClips["selectSkill_C"];
            }
            audio.Play();
        }


    }

    void deleteActiveSkill(GameObject skill)
    {
        bool encontrado = false;
        int i = 0;
        while (!encontrado && i < skillSlots.Length)
        {
            if (skillSlots[i].Equals(skill))
            {
                activeSkills[i].skName = "null";
                activeSkills[i].stressLevel = 9;
                encontrado = true;
            }
            else
            {
                i++;
            }
        }
        if (!encontrado)
        {
            print("No sé qué coño has hecho tío");
        }
        else
        {
            activeSkills[i].linkedSkill.GetComponent<Image>().color = Color.white;
            activeSkills[i].linkedSkill.GetComponent<MenuSkill>().selected = false;
            activeSkills.Sort(SortByStress);
            fillSkillSlots();
            // Audio
            int rand = Random.Range(0, 2);
            if (rand == 0)
            {
                this.GetComponent<AudioSource>().clip = audioClips["unselectSkill_A"];
            }
            else
            {
                this.GetComponent<AudioSource>().clip = audioClips["unselectSkill_B"];
            }
            audio.Play();
        }
    }

    private static int SortByStress(MenuSkill o1, MenuSkill o2)
    {
        return o1.stressLevel.CompareTo(o2.stressLevel);
    }

    // GESTIÓN DEL TASK PANEL
    public void loadTaskPanel()
    {
        transform.GetChild(2).gameObject.SetActive(true);
        tasks = GameObject.FindGameObjectsWithTag("Task");
        for (int i = 0; i < activeTasks.Count; i++)
        {
            tasks[i].transform.localScale = new Vector3(1f, 1f, 1f);
            tasks[i].transform.GetChild(1).GetComponent<Text>().text = activeTasks[i].title;
            tasks[i].transform.GetChild(3).GetComponent<Text>().text = activeTasks[i].type;
            if (activeTasks[i].duration <= 15)
            {
                tasks[i].transform.GetChild(5).GetComponent<Text>().text = "short";
            }
            else if (activeTasks[i].duration > 15 && activeTasks[i].duration <= 30)
            {
                tasks[i].transform.GetChild(5).GetComponent<Text>().text = "medium";
            }
            else if (activeTasks[i].duration > 30)
            {
                tasks[i].transform.GetChild(5).GetComponent<Text>().text = "long";
            }
        }
        for (int i = activeTasks.Count; i < tasks.Length; i++)
        {
            tasks[i].transform.localScale = new Vector3(1f, 0f, 1f);
        }

        // Audio
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            this.GetComponent<AudioSource>().clip = audioClips["taskButton_A"];
        }
        else
        {
            this.GetComponent<AudioSource>().clip = audioClips["taskButton_B"];
        }
        audio.Play();
        this.gameObject.SetActive(true);
    }

    public void exitTasks()
    {
        // Audio
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            this.GetComponent<AudioSource>().clip = audioClips["menuOut_A"];
        }
        else
        {
            this.GetComponent<AudioSource>().clip = audioClips["menuOut_B"];
        }
        audio.Play();
        transform.GetChild(2).gameObject.SetActive(false);
    }
}

