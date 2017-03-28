using UnityEngine;

class SampleSquare : Square
{
    public Color PathCellColor = Color.green;
    public Color ReachableCellColor = Color.yellow;
    public Color DefaultCellColor = Color.white;

    public override Vector3 GetCellDimensions()
    {
        return GetComponent<Renderer>().bounds.size;
    }

    public override void MarkAsHighlighted()
    {
        GetComponent<Renderer>().material.color = new Color(0.75f, 0.75f, 0.75f); ;
    }

    public override void MarkAsPath()
    {
        GetComponent<Renderer>().material.color = PathCellColor;
    }

    public override void MarkAsReachable()
    {
        GetComponent<Renderer>().material.color = ReachableCellColor;
    }

    public override void UnMark()
    {
        GetComponent<Renderer>().material.color = DefaultCellColor;
    }
}

