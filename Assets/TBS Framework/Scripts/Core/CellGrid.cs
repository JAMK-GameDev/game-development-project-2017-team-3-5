using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// CellGrid class keeps track of the game, stores cells, units and players objects. It starts the game and makes turn transitions. 
/// It reacts to user interacting with units or cells, and raises events related to game progress. 
/// </summary>
public class CellGrid : MonoBehaviour
{
    public event EventHandler GameStarted;
    public event EventHandler GameEnded;
    public event EventHandler TurnEnded;
	public event EventHandler ClockTickActive;
    public event EventHandler ActiveTurnStart;
	
    
    private CellGridState _cellGridState;//The grid delegates some of its behaviours to cellGridState object.
    public CellGridState CellGridState
    {
        private get
        {
            return _cellGridState;
        }
        set
        {
            if(_cellGridState != null)
                _cellGridState.OnStateExit();
            _cellGridState = value;
            _cellGridState.OnStateEnter();
        }
    }

    public int NumberOfPlayers { get; private set; }

    public Player CurrentPlayer
    {
        get { return Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)); }
    }
    public int CurrentPlayerNumber { get; private set; }

    public Transform PlayersParent;

    public List<Player> Players { get; private set; }
    public List<Cell> Cells { get; private set; }
    public List<Unit> Units { get; private set; }

    void Start()
    {
        Players = new List<Player>();
        for (int i = 0; i < PlayersParent.childCount; i++)
        {
            var player = PlayersParent.GetChild(i).GetComponent<Player>();
            if (player != null)
                Players.Add(player);
            else
                Debug.LogError("Invalid object in Players Parent game object");
        }
        NumberOfPlayers = Players.Count;
        CurrentPlayerNumber = Players.Min(p => p.PlayerNumber);

        Cells = new List<Cell>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var cell = transform.GetChild(i).gameObject.GetComponent<Cell>();
            if (cell != null)
                Cells.Add(cell);
            else
                Debug.LogError("Invalid object in cells paretn game object");
        }
      
        foreach (var cell in Cells)
        {
            cell.CellClicked += OnCellClicked;
            cell.CellHighlighted += OnCellHighlighted;
            cell.CellDehighlighted += OnCellDehighlighted;
        }
             
        var unitGenerator = GetComponent<IUnitGenerator>();
        if (unitGenerator != null)
        {
            Units = unitGenerator.SpawnUnits(Cells);
            foreach (var unit in Units)
            {
                unit.UnitClicked += OnUnitClicked;
                unit.UnitDestroyed += OnUnitDestroyed;
            }
        }
        else
            Debug.LogError("No IUnitGenerator script attached to cell grid");
        
        StartGame();
    }

    private void OnCellDehighlighted(object sender, EventArgs e)
    {
        CellGridState.OnCellDeselected(sender as Cell);
    }
    private void OnCellHighlighted(object sender, EventArgs e)
    {
        CellGridState.OnCellSelected(sender as Cell);
    } 
    private void OnCellClicked(object sender, EventArgs e)
    {
        CellGridState.OnCellClicked(sender as Cell);
    }

    private void OnUnitClicked(object sender, EventArgs e)
    {
        CellGridState.OnUnitClicked(sender as Unit);
    }
    private void OnUnitDestroyed(object sender, AttackEventArgs e)
    {
        Units.Remove(sender as Unit);
        var totalPlayersAlive = Units.Select(u => u.PlayerNumber).Distinct().ToList(); //Checking if the game is over
        if (totalPlayersAlive.Count == 1)
        {
            if(GameEnded != null)
                GameEnded.Invoke(this, new EventArgs());
        }
    }
    
    /// <summary>
    /// Method is called once, at the beggining of the game.
    /// </summary>
    public void StartGame()
    {
        
        if (GameStarted != null)
            GameStarted.Invoke(this, new EventArgs());

        int o = 2;
        o ++;
        
        ClockTick();
        
        //Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });
        //Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);
    }
	
	public void ActiveTurn(Unit a)
    {
        
        if (ActiveTurnStart != null)
            ActiveTurnStart.Invoke(this, new EventArgs());
        
		a.OnTurnStart();
        Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);


        //Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });
        //Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);
    }
	
    /// <summary>
    /// Method makes turn transitions. It is called by player at the end of his turn.
    /// </summary>
    public void EndTurn()
    {
        if (Units.Select(u => u.PlayerNumber).Distinct().Count() == 1)
        {
            return;
        }
        CellGridState = new CellGridStateTurnChanging(this);

        Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnEnd(); });

        CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;
        while (Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).Count == 0)
        {
            CurrentPlayerNumber = (CurrentPlayerNumber + 1)%NumberOfPlayers;
        }//Skipping players that are defeated.

        if (TurnEnded != null)
            TurnEnded.Invoke(this, new EventArgs());
		
		//.Invoke(this, new EventArgs()

        Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });
        Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);     
    }
	
	public void ClockTick() {
		
		if (ClockTickActive != null)
            ClockTickActive.Invoke(this, new EventArgs());
        int UnitNumber = Units.FindAll(c => c.UnitID > 100).Count();
        var UnitOrderCT = Units.FindAll(c => c.ChargeTime >= 0); //Makes a list of units with full CT
        

        do {
            Units.ForEach(c => { c.AddCT(); }); //Ct gets added to each unit
            UnitOrderCT =  Units.FindAll(c => c.ChargeTime >= 100); //Makes a list of units with full CT
            UnitNumber = UnitOrderCT.Count();

        } while (UnitNumber == 0);

        //bool FullCT = Units.Any(c => c.ChargeTime <= 100); //checks if any unit has more or equal to 100 CT


   

         // Loop runs while there is unit with FullCT and stops when there is none
			
			    // vv sorting values
			    // var UnitOrderCT = Units.FindAll(c => c.ChargeTime <= 100); //Makes a list of units with full CT
                // var UnitNumber = UnitOrderCT.Count(); 	 
			
				
				Unit swap; //for sorting

           

				//int single; //When there is only one player's turn
	
			
			
			//Sorting done to determine order of characters that have their CT 100
				if (UnitNumber > 1) { //checks if there is more then one character getting a turn, if so sorts

					for (int c = 0; c < (UnitNumber - 1); c++) //bubble sorting, overkill for the sample size, but I prefer having it for sorting the order of characters based on their CT (not much testing yet)
					{
						for (int d = 0; d < UnitNumber - c - 1; d++) //aka it sorts the characters in the order of how much ct they had
						{
							
							
							if (UnitOrderCT[d].ChargeTime > UnitOrderCT[d+1].ChargeTime) 
							{
								swap = UnitOrderCT[d];
                                UnitOrderCT[d] = UnitOrderCT[d + 1];
                                UnitOrderCT[d + 1] = swap;
							}

						
						}

						
					} //end CT sort

                
                //this sorts units with equal CT in order of their UnitID
                for (int c = 0; c < (UnitNumber - 1); c++) //bubble sorting, overkill for the sample size, but I prefer having it for sorting the order of characters based on their CT (not much testing yet)
                {
                    for (int d = 0; d < UnitNumber - c - 1; d++) //aka it sorts the characters in the order of how much ct they had
                    {


                        if (UnitOrderCT[d].ChargeTime == UnitOrderCT[d + 1].ChargeTime && UnitOrderCT[d].UnitID > UnitOrderCT[d + 1].UnitID)
                        {
                            swap = UnitOrderCT[d];
                            UnitOrderCT[d] = UnitOrderCT[d + 1];
                            UnitOrderCT[d + 1] = swap;
                        }


                    }


                } //end ID sort




            } //end of sorting

            Unit AT = UnitOrderCT[0];
            ///Active Turn
			ActiveTurn(AT);
            //SetState(new CellGridStateUnitSelected(this));


            //Active turn ends (CT of the current unit goes to 0)

            


        //end?
		


	}
	
	
	
}
