using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

	public GameObject TemplateTile;
    public GameObject Ground;
    public GameObject Cube;
	public float sizeX;
	public float sizeY;
    public int g;

    //store gameObject reference
    GameObject objToSpawn;
    List<GameObject> GameObjectList;


    // Use this for initialization
    void Start()
    {
        Cube.transform.position = new Vector3(10, 10, 10);
        GameObjectList = new List<GameObject>();
        //GameObjectList.Add(new GameObject());
        //float lenght = Ground.transform.lossyScale.z * (10f / sizeX);
        //float width = Ground.transform.lossyScale.x * (10f / sizeY);

        float lenght = Ground.transform.lossyScale.z * (1f / sizeX);
        float width = Ground.transform.lossyScale.x * (1f / sizeY);
        //float lenght = Ground.transform.position.z;
        //float width = Ground.transform.position.x;
        //float height = Ground.transform.position.y;

        //GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //plane.transform.localScale = new Vector3(9, 1, 9);
        //spawn object
        /*objToSpawn = new GameObject("Cool GameObject made from Code");
        //Add Components
        objToSpawn.AddComponent<Rigidbody>();
        objToSpawn.AddComponent<MeshFilter>();
        objToSpawn.AddComponent<BoxCollider>();
        objToSpawn.AddComponent<MeshRenderer>();
        objToSpawn.transform.parent = EmptyObj.gameObject.transform;    */

        //float moveHorizontal = Input.GetAxis("Horizontal");
        //float moveVertical = Input.GetAxis("Vertical");

        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //GameObjectList.Add(cube);
        //cube.transform.position = new Vector3(0, 0.5F, 0);
        float x, y;
        x = 1 / sizeX;
        y = 1 / sizeY;
        int m = 0;

        /*for (float i = -(sizeX / 2) - (width / 2); i < (sizeX / 2 + width / 2); i++)
        {
            for (float j = -(sizeY / 2) - (lenght / 2); j < (sizeY / 2 + lenght / 2); j++)
            {
         
        for (float i = 0; i < sizeX; i++)
        {
            for (float j = 0; j < sizeY; j++)
            {
                //GameObjectList.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
                //GameObjectList.Add(new GameObject(Cube));
                GameObjectList.Add(Instantiate(TemplateTile, new Vector3(i, 0, 0), Quaternion.identity));
                
                //(Ground.transform.localScale *
                //GameObjectList[m].transform.localScale = new Vector3(Ground.transform.localScale.x, Ground.transform.localScale.y, Ground.transform.localScale.z);
                GameObjectList[m].transform.localScale = new Vector3(width, 1f, lenght);
                GameObjectList[m++].transform.Translate(new Vector3(i*1, 0, j*1));
            }
        }   
         
         */

        for (float i = -(sizeX/2); i < (sizeX/2); i+=1/sizeX)
        {
            for (float j = -(sizeY/2); j < (sizeY/2); j+=1/sizeY)
            {
                //GameObjectList.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
                //GameObjectList.Add(new GameObject(Cube));
                GameObjectList.Add(Instantiate(TemplateTile, new Vector3(i * 2.0f, 0, 0), Quaternion.identity));
                
                //(Ground.transform.localScale *
                //GameObjectList[m].transform.localScale = new Vector3(Ground.transform.localScale.x, Ground.transform.localScale.y, Ground.transform.localScale.z);
                GameObjectList[m].transform.localScale = new Vector3(width, Ground.transform.localScale.y, lenght);
                GameObjectList[m++].transform.position = Ground.transform.position + (new Vector3(i*1, 0, j*1));
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

