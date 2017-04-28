using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walkthroughReader : MonoBehaviour
{

    string[] lines;
    int lineCounter;
    public string level;

    string[] elements;
    float timesinceStart;
    List<Task> tasksLevel = new List<Task>();
   

    char delimiter = '-';

    GameObject gameManager;

    // Use this for initialization
    void Start()
    {
        lines = System.IO.File.ReadAllLines(@"Assets/levels/" + level + ".txt");
        readTasks();
    }

    void readTasks()
    {
        
        while (lines[lineCounter] != "--")
        {
            elements=lines[lineCounter].Split(delimiter);

            
            
            //Creates a task, the time it keeps to complete it (elements[2]) we define it in each level .txt file or we keep all of them in a DB?
            //tasksLevel[lineCounter] = new Task(elements[0], elements[1], float.Parse(elements[2])); 
            //en vez de print, enviar estos valores a la clase Tarea
            print("Type: " + elements[0]);
            print("Title: " + elements[1]);


            lineCounter++;
        }

        gameManager.SendMessage("listOfTasks",tasksLevel); //enviamos al gameManager la lista de tareas del nivel
        lineCounter++;
        
        print("--STARTING WALKTHROUGH--");
    }

    // Update is called once per frame
    //Here we follow the time since the start of the level to activate the conflicts according to the script we made in the level .txt file.
    void Update()
    {
        if ((lineCounter < lines.Length))
        {
            timesinceStart = Time.realtimeSinceStartup;
            int seconds = (int) timesinceStart % 60;
            int minutes =(int) timesinceStart / 60;
            string time = minutes + ":" + seconds.ToString("00");

            print(time);
            elements = lines[lineCounter].Split(delimiter);

            if ((lines[lineCounter] != "")&&(elements[0]==time))
            {
                //Sends to gameManager the conflict active at the moment

                gameManager.SendMessage("activateConflict",elements[1]);

                print("Time: " + elements[0]);
                print("Type: " + elements[1]);

                lineCounter++;
            }
        }
    }
}
