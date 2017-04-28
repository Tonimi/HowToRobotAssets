using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSkill : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEquatable<MenuSkill>, IComparable<MenuSkill>, ICloneable {
    public bool activeSkill;
    public bool selected;
    public GameObject popupmenu;
    public string skName;
    public int stressLevel;
    public GameObject linkedSkill;
    public bool forbidden = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!forbidden)
        {
            if (activeSkill)
            {
                popupmenu.SendMessage("deleteActiveSkill", this.gameObject);
            }
            else if (!selected)
            {
                popupmenu.SendMessage("skillClicked", this.gameObject);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Do action
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        MenuSkill objAsMS = obj as MenuSkill;
        if (objAsMS == null) return false;
        else return Equals(objAsMS);
    }

    // Default comparer for MenuSkill type.
    public int CompareTo(MenuSkill compareMS)
    {
        // A null value means that this object is greater.
        if (compareMS == null)
            return 1;

        else
            return stressLevel.CompareTo(compareMS.stressLevel);
    }
    public override int GetHashCode()
    {
        return stressLevel;
    }
    public bool Equals(MenuSkill other)
    {
        if (other == null) return false;
        return (this.stressLevel.Equals(other.stressLevel));
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
