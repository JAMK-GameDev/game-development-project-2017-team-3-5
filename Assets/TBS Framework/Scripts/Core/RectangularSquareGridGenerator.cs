using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates rectangular shaped grid of squares.
/// </summary>
[ExecuteInEditMode()]
public class RectangularSquareGridGenerator : ICellGridGenerator
{
    public GameObject SquarePrefab;

    public int Width;
    public int Height;

    public override List<Cell> GenerateGrid()
    {
        var ret = new List<Cell>();

        if (SquarePrefab.GetComponent<Square>() == null)
        {
            Debug.LogError("Invalid square cell prefab provided");
            return ret;
        }

        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
				var square = Instantiate(SquarePrefab);
				square.transform.parent = CellsParent;
                var squareSize = square.GetComponent<Cell>().GetCellDimensions();

				square.transform.localPosition = new Vector3(i*squareSize.x,j*squareSize.y,0);
				square.transform.localScale = SquarePrefab.transform.localScale;
				square.transform.Translate( new Vector3(CellsParent.transform.localPosition.x, CellsParent.transform.localPosition.y, 0));
                square.GetComponent<Cell>().OffsetCoord = new Vector2(i,j);
                square.GetComponent<Cell>().MovementCost = 1;
                ret.Add(square.GetComponent<Cell>());

            }
        }
        return ret;
    }
}
