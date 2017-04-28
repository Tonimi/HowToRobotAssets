using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System.Xml;

public class Conflict : MonoBehaviour {

    //public float stressInc;
    //public float taskRed;

    public XmlDocument conflictsReaderXML;

    public Conflict(string type, string name)
    {

    }

	// Use this for initialization
	void Start () {

        conflictsReaderXML = new XmlDocument();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

     public string getTypeConflict(string nameConflict)
    {


        TextAsset textAsset = new TextAsset();
        textAsset = (TextAsset)Resources.Load("conflicts/conflictsListXML");

        conflictsReaderXML.LoadXml(textAsset.text);
        //conflictsReaderXML.Load(@"Assets/Resources/conflicts/conflictsListXML.xml");
        
        XmlNodeList nodesDocument = conflictsReaderXML.DocumentElement.SelectNodes("/ConflictsList/Conflict");

        foreach (XmlNode node in nodesDocument)//reading from the XML
        {

            if (node.SelectSingleNode("Title").InnerText == nameConflict){
                
                return node.SelectSingleNode("Type").InnerText;
                
            }

      

        }
        return "ERROR"; //If it has not found any matching conflict, it means that there is an error
    }
}
