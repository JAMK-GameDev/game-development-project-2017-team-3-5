using UnityEngine;

public class GUIController : MonoBehaviour
{
    public CellGrid CellGrid;
    CellGridStateUnitSelected UnitSelected;

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
            Debug.Log("Attack");
            
        }

        if (Input.GetKeyDown(KeyCode.S))
        {

            Debug.Log("Magic");

        }

        if (Input.GetKeyDown(KeyCode.D))
        {

            Debug.Log("Reality Break");

        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Act mode cancelled");
            return;

        }

    }

   




}
