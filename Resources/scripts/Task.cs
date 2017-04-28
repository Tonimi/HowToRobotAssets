using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour {

    public enum  TaskType { Talk,Work,Blackboard};
    public TaskType tkType;
    public string title;
    public float duration;

    GameObject gameManager;

    public Task(string taskType, string title, float duration)
    {
        switch(taskType)
        {
            case "Talk":
                tkType = TaskType.Talk;
                break;
            case "Work":
                tkType = TaskType.Work;
                break;
            case "Blackboard":
                tkType = TaskType.Blackboard;
                break;
        }
        this.title = title;
        this.duration = duration;
    }
   

	// Use this for initialization
	void Start () {
        gameManager = GameObject.FindGameObjectWithTag("Manager");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void taskStarted()
    {
        switch(tkType)
        {
            case TaskType.Talk:
                gameManager.SendMessage("startTalkTask");
                break;
            case TaskType.Work:
                gameManager.SendMessage("startWorkTask");
                break;
            case TaskType.Blackboard:
                gameManager.SendMessage("startBlackBoardTask");
                break;
        }
    }
}
