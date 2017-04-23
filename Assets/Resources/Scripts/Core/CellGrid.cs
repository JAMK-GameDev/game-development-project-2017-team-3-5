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
	//Own events
	public event EventHandler ClockTickActive;
	public event EventHandler ActiveTurnStart;


	public Unit CurrentUnit { get; private set; }
    
    public List<Unit> UnitOrderCT { get; private set; }
	public int UnitNumber { get; private set; }
	public bool TurnIsActive { get; private set; }
	public bool canMove = false;
	public bool canAct = false;
    
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

    public Transform SkillsParent;

    

    public List<Player> Players { get; private set; }
    public List<Cell> Cells { get; private set; }
    public List<Unit> Units { get; private set; }
    public List<Skill> Skills { get; private set; }

    //Animator variable
    public Animator modelAnim;

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

        Skills = new List<Skill>();
        for (int i = 0; i < SkillsParent.childCount; i++)
        {
            var skill = SkillsParent.GetChild(i).GetComponent<Skill>();
            if (skill != null)
                Skills.Add(skill);
            else
                Debug.LogError("Invalid object in Skills Parent game object");
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
			//Gives units needed values, unitID us unique number and needed in sorting in ClockTick();
			for (int i = 0; i < Units.Count; i++) {
				Units[i].UnitID = i;
				if (Units [i].Speed == 0)
					Units [i].Speed = 5;
                if (Units[i].DefenceFactor == 0)
                    Units[i].DefenceFactor = 1;
                Units[i].UnitClicked += OnUnitClicked;
				Units[i].UnitDestroyed += OnUnitDestroyed;
			}
			/*
            foreach (var unit in Units)
            {
				unit.UnitID = 
                unit.UnitClicked += OnUnitClicked;
                unit.UnitDestroyed += OnUnitDestroyed;
            }*/
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
        //TODO: Käy läpi kaikki naapurit...
        /*(sender as Cell).GetNeighbours(Cells).ForEach(each => {
            //Värjätään kaikki cellit, joihin isketään AOE:lla
        });*/
        CellGridState.OnCellSelected(sender as Cell);
    } 
    private void OnCellClicked(object sender, EventArgs e)
    {
		//Can only move when is units turn and player selected move
		/*if (canMove) CellGridState.OnCellClicked(sender as Cell);
        else
        {*/
        if (CellGridState is CellGridStateUnitSelected)
        {
            //TODO: Käy läpi kaikki naapurit...
            /*(sender as Cell).GetNeighbours(Cells).ForEach(each => {
                //Värjätään kaikki cellit, joihin isketään AOE:lla
            });*/
            CellGridState.OnCellClicked(sender as Cell);

        }
    }

    private void OnUnitClicked(object sender, EventArgs e)
	{
		//Can only attack when is units turn and player selected action
		CellGridState.OnUnitClicked(sender as Unit);
    }
    private void OnUnitDestroyed(object sender, AttackEventArgs e)
    {
        Units.Remove(sender as Unit);
        var totalPlayersAlive = Units.Select(u => u.PlayerNumber).Distinct().ToList(); //Checking if the game is over
        if (totalPlayersAlive.Count == 1)
        {
            if(GameEnded != null) {
                //Trigger victory animation
                Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => {
                    u.transform.Find("hero").gameObject.GetComponent<Animator>().SetTrigger("victory");
                });

                GameEnded.Invoke(this, new EventArgs());
            }
        }
    }
    
    /// <summary>
    /// Method is called once, at the beggining of the game.
    /// </summary>
    public void StartGame()
    {
        if(GameStarted != null)
            GameStarted.Invoke(this, new EventArgs());

		UnitNumber = 0;
		ClockTick();
        //Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });
        //Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);
    }


	public void ActiveTurn(Unit a){
		print ("In Active turn");
		if (ActiveTurnStart != null) ActiveTurnStart.Invoke(this, new EventArgs());
		
		TurnIsActive = true; //NOTE: Not in use right now...

        if(CurrentUnit != null) {
            //Stop walking animation
            modelAnim.SetBool("walk", false);
        }

        CurrentUnit = a;
        
		CurrentPlayerNumber = a.PlayerNumber;
		Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart2(); });
		a.OnTurnStart();
		Players.Find(p => p.PlayerNumber.Equals(a.PlayerNumber)).Play(this);
		print ("Left Active turn");

        //Update animator
        modelAnim = CurrentUnit.transform.Find("hero").gameObject.GetComponent<Animator>();
        //Start walking animation
        modelAnim.SetBool("walk", true);

    }
	public void ClockTick() {
		if (ClockTickActive != null)
			ClockTickActive.Invoke (this, new EventArgs ());
		print ("Inside CT adds next");
		while (UnitNumber == 0) {
			TurnIsActive = false;
			Units.ForEach (c => { c.AddCT(); }); //Ct gets added to each unit
			UnitOrderCT = Units.FindAll (c => c.ChargeTime >= 100); //Makes a list of units with full CT
			UnitNumber = UnitOrderCT.Count ();
		}

		//Sorting done to determine order of characters that have their CT 100
		Unit swap;
		print ("Inside Sorter next");
		if (UnitNumber > 1) {
			print ("Inside Sorter");
			//checks if there is more then one character getting a turn, if so sorts
			for (int c = 0; c < (UnitNumber - 1); c++) //bubble sorting, overkill for the sample size, but I prefer having it for sorting the order of characters based on their CT (not much testing yet)
			{
				for (int d = 0; d < UnitNumber - c - 1; d++) //aka it sorts the characters in the order of how much ct they had
				{
					if (UnitOrderCT[d].ChargeTime > UnitOrderCT[d + 1].ChargeTime)
					{
						swap = UnitOrderCT[d];
						UnitOrderCT[d] = UnitOrderCT[d + 1];
						UnitOrderCT[d + 1] = swap;
					}
				} //end CT sort
			}
			print ("Did Sorter1");
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
				} //end ID sort
			} //end of sorting
			print ("Did Sorter2 and leaving sort");
		}
		Unit AT = UnitOrderCT[0];
		///Active Turn
		ActiveTurn(AT);
	}
    /// <summary>
    /// Method makes turn transitions. It is called by player at the end of his turn.
    /// </summary>
    public void EndTurn()
    {
		/*
        if (Units.Select(u => u.PlayerNumber).Distinct().Count() == 1)
        {
            return;
        }
        */
        CellGridState = new CellGridStateTurnChanging(this);

        Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnEnd(); });
        /*
        CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;
        while (Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).Count == 0)
        {
            CurrentPlayerNumber = (CurrentPlayerNumber + 1)%NumberOfPlayers;
        }//Skipping players that are defeated.
		*/
        if (TurnEnded != null)
        {
            modelAnim.SetBool("walk",false);
            TurnEnded.Invoke(this, new EventArgs());
        }

		CurrentUnit.ChargeTime = 0;
		if (CurrentUnit.ActionPoints != 0) { CurrentUnit.ChargeTime += 20; }
		if (CurrentUnit.MovementPoints != 0) { CurrentUnit.ChargeTime += 20; }

		UnitOrderCT = Units.FindAll(c => c.ChargeTime >= 100); //Makes a list of units with full CT
		UnitNumber = UnitOrderCT.Count();
		ClockTick();


		//Needs to be commented, otherwise sudden team attack ;D
        //Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });
        //Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);     



	}
	public void CanMove(){
		
		//Selects current unit
		if (!canAct) {
			canMove = true;
			CellGridState.OnUnitClicked (CurrentUnit);
			CellGridState.SelectMove ();
		}

	}
	public void CanAttack()
	{
		if (!canMove) {
			canAct = true;
			CellGridState.OnUnitClicked (CurrentUnit);

			CurrentUnit.CurrentSkill = Skills [0];
			CurrentUnit.CurrentSkill.SkillActivator (CurrentUnit);

            CellGridState.OnCellAttack ();
        }

    }

    public void Magic()
	{
		if (!canMove) {
			canAct = true;
			CellGridState.OnUnitClicked(CurrentUnit);

	        Debug.Log("enters magic");
	        CurrentUnit.CurrentSkill = Skills[1];
	        if (CurrentUnit.ImaginationPoints >= CurrentUnit.CurrentSkill.IPcost)
	        {
		        Debug.Log("passes if");
		        CurrentUnit.CurrentSkill.SkillActivator(CurrentUnit);
		        CellGridState.OnCellAttack();
        	}
		}

    }

    public void RealityBreak()
    {
		if (!canMove) {
			canAct = true;
			CellGridState.OnUnitClicked (CurrentUnit);

			Debug.Log ("enters magic");
			CurrentUnit.CurrentSkill = Skills [2];
			if (CurrentUnit.RealityBreak == 100) {
				Debug.Log ("passes if");

				CurrentUnit.CurrentSkill.SkillActivator (CurrentUnit);
            
				CellGridState.OnCellAttack ();
			} 
		}
    }

}
