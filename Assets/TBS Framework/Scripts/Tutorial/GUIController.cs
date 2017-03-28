using UnityEngine;
using UnityEngine.UI;
using System;

public class GUIController : MonoBehaviour
{
	public Button NextTurnButton;

    public CellGrid CellGrid;

	private void OnTurnEnded(object sender, EventArgs e)
	{
		NextTurnButton.interactable = ((sender as CellGrid).CurrentPlayer is HumanPlayer);
	}

    void Start()
    {
        Debug.Log("Press 'n' to end turn");
    }

	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            CellGrid.EndTurn();//User ends his turn by pressing "n" on keyboard.
		}

		if (CellGrid.Units.FindAll (u => (u.AttackFactor.Equals(1) && u.PlayerNumber.Equals(CellGrid.CurrentPlayerNumber))).Count == 0) {
			CellGrid.EndTurn();
		}
	}
}
