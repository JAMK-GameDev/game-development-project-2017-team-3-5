using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

	public GameObject Tile, Ground;
	public int sizeX, sizeY;
	
	private List<GameObject> list;
	
	// Use this for initialization
	void Start () {
		//Initializing required objects
		list = new List<GameObject>();
		
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
				obj.transform.parent = null;
				//Made some gaps here, lossyScale is readonly.
				obj.transform.localScale = new Vector3(
					lenght - 0.2f,						//X
					Tile.transform.localScale.y,		//Y
					width - 0.2f);						//Z
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
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
