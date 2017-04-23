﻿using UnityEngine;

public class SampleUnit : Unit
{
    public Color LeadingColor;
    public override void Initialize()
    {
        base.Initialize();
        transform.position += new Vector3(0, 0, -1);
        GetComponent<Renderer>().material.color = LeadingColor;
    }
    public override void MarkAsAttacking(Unit other)
    {      
        //Trigger attack animation
<<<<<<< HEAD
        transform.Find("hero").gameObject.GetComponent<Animator>().SetTrigger("attack");
=======
        GetComponent<Animator>().SetTrigger("attack");
>>>>>>> master
    }

    public override void MarkAsDefending(Unit other)
    {       
    }

    public override void MarkAsDestroyed()
    {      
    }

    public override void MarkAsFinished()
    {
    }

    public override void MarkAsFriendly()
    {
        GetComponent<Renderer>().material.color = LeadingColor + new Color(0.8f, 1, 0.8f);
    }

    public override void MarkAsReachableEnemy()
    {
        GetComponent<Renderer>().material.color = LeadingColor + Color.red;
    }

    public override void MarkAsSelected()
    {
        GetComponent<Renderer>().material.color = LeadingColor + Color.green;
    }

    public override void UnMark()
    {
        GetComponent<Renderer>().material.color = LeadingColor;
    }
   
}
