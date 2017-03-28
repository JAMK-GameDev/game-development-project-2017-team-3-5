using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : ICellGridGenerator
{

	public GameObject Tile, Ground, TilesParent;
	public int sizeX, sizeY;
	public float gapsize = 0.2f;
	
	//private List<Cell> list;
    public override List<Cell> GenerateGrid()
    {
        //Initializing required objects
        List<Cell> list = new List<Cell>();

        //Need to take lenght and width of ground object
        //Multiply by 10, if ground object is a plane
        float lenght = Ground.transform.lossyScale.x / sizeX * 10;
        float width = Ground.transform.lossyScale.z / sizeY * 10;
        //Need to create tiles to our platform
        for (int i = 0, m = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                //Need to create new GameObject with same properties as Tile has
                var obj = Instantiate(Tile, this.transform);

                if (Tile.GetComponent<Square>() == null)
                {
                    Debug.LogError("Invalid square cell prefab provided");
                    return list;
                }
                obj.GetComponent<Cell>().MovementCost = 1;

                //Tag them as Tile
                obj.tag = "Tile";
                //Transform scale to fit the space around it
                //Disabling parent makes sure the tiles are in correct size and doesn't change later.
                obj.transform.parent = TilesParent.transform;
                //Made some gaps here, lossyScale is readonly.
                obj.transform.localScale = new Vector3(
                    lenght - gapsize,                   //X
                    width - gapsize,                    //Z
                    Tile.transform.localScale.z);       //Y
                                                        //Position them inside platform, so tiles are in a same size as it, but hovers above it.
                obj.transform.position = this.transform.position;
                //Make translate vector. Cast to float so positions to correct spot if 5:4 or similar
                Vector3 newvector = new Vector3(
                    lenght * (i + 0.5f - (float)sizeX / 2), //X
                    width * (j + 0.5f - (float)sizeY / 2),  //Z
                    0);                                     //Y
                obj.transform.Translate(newvector);
                list.Add(obj.GetComponent<Cell>());
                //list.Add(obj);
                m++;
            }
        }
        return list;
    }
    // Use this for initialization
    void Start () {
        //Need to hide original ground and tile
        Ground.SetActive(false);
        Tile.SetActive(false);
        //Initializing required objects
        /*list = new List<GameObject>();

		
		//Need to take lenght and width of ground object
		//Multiply by 10, if ground object is a plane
		float lenght = Ground.transform.lossyScale.x / sizeX * 10;
		float width = Ground.transform.lossyScale.z / sizeY * 10;
		//Need to create tiles to our platform
		for(int i = 0, m = 0; i < sizeX; i++){
			for(int j = 0; j < sizeY; j++){
				//Need to create new GameObject with same properties as Tile has
				GameObject obj = Instantiate(Tile, this.transform);
				//Tag them as Tile
				obj.tag = "Tile";
				//Transform scale to fit the space around it
				//Disabling parent makes sure the tiles are in correct size and doesn't change later.
				obj.transform.parent = TilesParent.transform;
				//Made some gaps here, lossyScale is readonly.
				obj.transform.localScale = new Vector3(
					lenght - gapsize,					//X
					Tile.transform.localScale.y,		//Y
					width - gapsize);					//Z
				//Position them inside platform, so tiles are in a same size as it, but hovers above it.
				obj.transform.position = this.transform.position;
				//Make translate vector. Cast to float so positions to correct spot if 5:4 or similar
				Vector3 newvector = new Vector3(
					lenght * (i + 0.5f - (float)sizeX / 2),	//X
					0,										//Y
					width * (j + 0.5f - (float)sizeY / 2));	//Z
				obj.transform.Translate(newvector);
				list.Add(obj);
				m++;
			}
		}
		//Need to hide original ground and tile
		Ground.SetActive(false);
		Tile.SetActive(false);
        */
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
