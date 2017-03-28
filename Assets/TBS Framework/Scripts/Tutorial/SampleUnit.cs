using UnityEngine;

public class SampleUnit : Unit
{
    public Color LeadingColor;
    public Color FriendlyColor = Color.blue;
    public Color EnemyColor = Color.red;
    public Color SelectedColor = Color.green;
    public override void Initialize()
    {
        base.Initialize();
        transform.position += new Vector3(0, 0, -1);
        GetComponent<Renderer>().material.color = LeadingColor;
    }
    public override void MarkAsAttacking(Unit other)
    {      
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
        GetComponent<Renderer>().material.color = LeadingColor + FriendlyColor;
    }

    public override void MarkAsReachableEnemy()
    {
        GetComponent<Renderer>().material.color = LeadingColor + EnemyColor;
    }

    public override void MarkAsSelected()
    {
        GetComponent<Renderer>().material.color = LeadingColor + SelectedColor;
    }

    public override void UnMark()
    {
        GetComponent<Renderer>().material.color = LeadingColor;
    }
}
