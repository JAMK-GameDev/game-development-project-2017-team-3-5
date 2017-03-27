using UnityEngine;

public class GUIController : MonoBehaviour
{
    public CellGrid CellGrid;
	
    void Start()
    {
        Debug.Log("Press 'n' to end turn");
        Debug.Log("Press 'a' to start act");
        Debug.Log("Press 'a' in act to attack");
        Debug.Log("Press 's' in act to use magic");
        Debug.Log("Press 'd' in act to use reality break");
        Debug.Log("Press 'c' in act to cancel");
    }

	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            CellGrid.EndTurn();//User ends his turn by pressing "n" on keyboard.
            Debug.Log("Turn ended");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {

            Act();
            Debug.Log("Press 'a' to start act");
        }

    }

    void Act ()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {

        }

        if (Input.GetKeyDown(KeyCode.S))
        {

        }

        if (Input.GetKeyDown(KeyCode.D))
        {

        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            return;
        }

    }


}
