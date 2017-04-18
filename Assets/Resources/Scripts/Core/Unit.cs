using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

/// <summary>
/// Base class for all units in the game.
/// </summary>
public abstract class Unit : MonoBehaviour
{
    /*
    AttackBuff(Unit u, int nmr)
    {
        List skillist = new List();
        float dmg = 0;
        (skillist[nmr].DamageType == Skill.DamageTypes.MagicDamage){

        skillist[nmr].Range AOE skillcost
                u.Cell.GetNeighbours
        }
        u.DefenceFactor
    }
    */

    float Totalvalue;
    float TicksPerSecond, tickStartTime, tickTotalTime;
    
    enum DamageTypes
    {
        Damage, MagicDamage, StaticDamage, Heal, Buff, Debuff
    }
    enum CostTypes
    {
        Hitpoints, ImaginationPoints, ChargingTime, RealityBreak
    }
    DamageTypes Type;
    CostTypes Cost;
    //Type = DamageTypes.Heal;
    private List<Cell> aoeCellList;
    private List<Unit> aoeUnitList;
    private List<float> BlockList;
   

    public Skill CurrentSkill { get; set; }

    /// <summary>
    /// UnitClicked event is invoked when user clicks the unit. It requires a collider on the unit game object to work.
    /// </summary>
    public event EventHandler UnitClicked;
    /// <summary>
    /// UnitSelected event is invoked when user clicks on unit that belongs to him. It requires a collider on the unit game object to work.
    /// </summary>
    public event EventHandler UnitSelected;
    public event EventHandler UnitDeselected;
    /// <summary>
    /// UnitHighlighted event is invoked when user moves cursor over the unit. It requires a collider on the unit game object to work.
    /// </summary>
    public event EventHandler UnitHighlighted;
    public event EventHandler UnitDehighlighted;
    public event EventHandler<AttackEventArgs> UnitAttacked;
    public event EventHandler<AttackEventArgs> UnitDestroyed;
    public event EventHandler<MovementEventArgs> UnitMoved;

    public UnitState UnitState { get; set; }
    public void SetState(UnitState state)
    {
        UnitState.MakeTransition(state);
        
    }
	//Status effects
    public List<Buff> Buffs { get; private set; }

	//Total values
    public float TotalHitPoints { get; private set; }
    public float TotalImaginationPoints { get; private set; }
    protected int TotalMovementPoints;
    protected int TotalActionPoints;

    /// <summary>
    /// Cell that the unit is currently occupying.
    /// </summary>
    public Cell Cell { get; set; }

	//Values
    
    public float HitPoints;
    public float ImaginationPoints;
    public int AttackRange;
    public float PhysicalPower;
    public float MagicalPower;
    public float DefenceFactor;
	public float RealityBreak;
	public int ChargeTime;

    //Evasion stats
    public float BlockFront;
    public float BlockSide;
    public float BlockBack;

    public float MagicEv;
    
    //Attributes
    public int UnitID; //Allows us to sort units...
	public int Speed = 5;
	public int Face = 1; //Turns character between 4 different sides, North East South West.
    /// <summary>
    /// Determines how far on the grid the unit can move.
    /// </summary>
    public int MovementPoints;
    /// <summary>
    /// Determines speed of movement animation.
    /// </summary>
    public float MovementSpeed;
    /// <summary>
    /// Determines how many attacks unit can perform in one turn.
    /// </summary>
    public int ActionPoints;

    /// <summary>
    /// Indicates the player that the unit belongs to. Should correspoond with PlayerNumber variable on Player script.
    /// </summary>
    public int PlayerNumber;

    /// <summary>
    /// Indicates if movement animation is playing.
    /// </summary>
    public bool isMoving { get; set; }

    private static IPathfinding _pathfinder = new AStarPathfinding();

    /// <summary>
    /// Method called after object instantiation to initialize fields etc. 
    /// </summary>
    public virtual void Initialize()
    {
        Buffs = new List<Buff>();
        UnitState = new UnitStateNormal(this);

        TotalHitPoints = HitPoints;
        TotalImaginationPoints = ImaginationPoints;
        TotalMovementPoints = MovementPoints;
        TotalActionPoints = ActionPoints;
    }

    protected virtual void OnMouseDown()
    {
        if (UnitClicked != null)
            UnitClicked.Invoke(this, new EventArgs());
    }
    protected virtual void OnMouseEnter()
    {
        if (UnitHighlighted != null)
            UnitHighlighted.Invoke(this, new EventArgs());
    }
    protected virtual void OnMouseExit()
    {
        if (UnitDehighlighted != null)
            UnitDehighlighted.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Method is called at the start of each turn.
    /// </summary>
    public virtual void OnTurnStart()
    {
		MovementPoints = TotalMovementPoints;
		ActionPoints = TotalActionPoints;
		SetState(new UnitStateNormal(this));
		SetState (new UnitStateMarkedAsFriendly (this));
    }
	public virtual void OnTurnStart2()
	{
		MovementPoints = 0;
		ActionPoints = 0;

		SetState(new UnitStateMarkedAsFinished(this));
	}
	//Add ct to unit
	public virtual void AddCT() { ChargeTime += Speed; }
    /// <summary>
    /// Method is called at the end of each turn.
    /// </summary>
    public virtual void OnTurnEnd()
    {
        Buffs.FindAll(b => b.Duration == 0).ForEach(b => { b.Undo(this); });
        Buffs.RemoveAll(b => b.Duration == 0);
        Buffs.ForEach(b => { b.Duration--; });

        SetState(new UnitStateNormal(this));
    }
    /// <summary>
    /// Method is called when units HP drops below 1.
    /// </summary>
    protected virtual void OnDestroyed()
    {
        Cell.IsTaken = false;
        MarkAsDestroyed();
        Destroy(gameObject);
    }

    /// <summary>
    /// Method is called when unit is selected.
    /// </summary>
    public virtual void OnUnitSelected()
    {
        SetState(new UnitStateMarkedAsSelected(this));
        if (UnitSelected != null)
            UnitSelected.Invoke(this, new EventArgs());
    }
    /// <summary>
    /// Method is called when unit is deselected.
    /// </summary>
    public virtual void OnUnitDeselected()
    {
        SetState(new UnitStateMarkedAsFriendly(this));
        if (UnitDeselected != null)
            UnitDeselected.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Method indicates if it is possible to attack unit given as parameter, from cell given as second parameter.
    /// </summary>
    public virtual bool IsUnitAttackable(Unit other, Cell sourceCell, int Range)
    {
        if (sourceCell.GetDistance(other.Cell) <= Range)
            return true;

        return false;
    }
    public virtual bool IsCellAttackable(Cell other, Cell sourceCell, int Range)
    {
        if (sourceCell.GetDistance(other) <= Range)
            return true;

        return false;
    }
    /// <summary>
    /// Method deals damage to unit given as parameter.
    /// </summary>
    /// 


    public virtual void DoSkill(List<Unit> UnitList)
    {
        if (CurrentSkill is MySkill)
        {

        }

        if (isMoving)
            return;
        if (ActionPoints == 0)
            return;
        /*
        if (!IsCellAttackable(target, Cell, CurrentSkill.SkillRange))
            return;
            */
        //^^add back later

        /*
        aoeCellList = target.GetNeighbours(CellList);
        
        foreach (Unit currentUnit in aoeCellList.Units)
        {

            if (this.IsCellAttackable(currentUnit.Cell, this.Cell))
            {


                aoeUnitList.Add(currentUnit);
            }
        }
        */
        Debug.Log("stops");
        int o = UnitList.Count();
        Debug.Log(UnitList);
        ImaginationPoints -= CurrentSkill.IPcost;
        if (ImaginationPoints < 0) { ImaginationPoints = 0; }
        if (CurrentSkill.RB){ RealityBreak = 0; }
        for (int n = 0; n < o; ++n)
        {
            MarkAsAttacking(UnitList[n]);
            

            //I use skill to other
            UnitList[n].Cast(this);

        }

        ActionPoints--;
        
    }
    private void CastSkill(Cell c)
    {
        //Loop trhough all neighbours if skill has aoe
       // c.GetNeighbours().ForEach(each => {

       
    }
    



    /// <summary>
    /// Caster unit calls Target method on target unit. 
    /// Target unit is affected by caster units current skill.
    /// </summary>
    /// 
    protected virtual void Cast(Unit caster)
    {

        bool evade = false;
        float def = DefenceFactor;
        MarkAsDefending(caster);

        
		caster.turnUnit (this.transform.position);

        //This unit takes damage from caster and checks if its front, side or backstab attack
        if (caster.CurrentSkill.NoEvade == false) {
            System.Random random = new System.Random();
            int randomNumber = random.Next(0, 100);

            if (caster.CurrentSkill.Physical) {
                if (100 - (caster.getFaceModifier(this.transform.position, this.Face)) < randomNumber) { def = def * 0.5f; Debug.Log("blocked"); } else { }
            }

            if (caster.CurrentSkill.Magical) {
                if ((100 - MagicEv) < randomNumber) { evade = true; Debug.Log("miss"); } else { }
            }
        }

        if (evade) {/*insert dodge animation here or something */}
        else
        {

            if (caster.CurrentSkill.IsHeal) { HitPoints += caster.CurrentSkill.SkillFormula(caster); if (HitPoints > TotalHitPoints) { HitPoints = TotalHitPoints; } }
            else { RBGain(this, caster); HitPoints -= caster.CurrentSkill.SkillFormula(caster) * def; }
        }


        
        //* caster.getFaceModifier(this.transform.position, this.Face);

        //float rand = (float)(random.NextDouble() * 0.5);

        //This behaviour can be overridden in derived classes.

        Debug.Log(caster.CurrentSkill.SkillFormula(caster));
        if (UnitAttacked != null)
            UnitAttacked.Invoke(this, new AttackEventArgs(caster, this));

        if (HitPoints <= 0)
        {
            if (UnitDestroyed != null)
                UnitDestroyed.Invoke(this, new AttackEventArgs(caster, this));
            OnDestroyed();
        }
    }

        protected virtual void RBGain(Unit Gainer, Unit trigger)
        {
        RealityBreak += ((trigger.CurrentSkill.SkillFormula(trigger)) / HitPoints)*100;
        if (RealityBreak > 100) { RealityBreak = 100; }
        }



	public virtual void Move(Cell destinationCell, List<Cell> path)
    {
        if (isMoving)
            return;

        var totalMovementCost = path.Sum(h => h.MovementCost);
        if (MovementPoints < totalMovementCost)
            return;

        //MovementPoints -= totalMovementCost;
		MovementPoints = 0;
		//Face this unit toward destination
		this.turnUnit (destinationCell.transform.position);

        Cell.IsTaken = false;
        Cell = destinationCell;
        destinationCell.IsTaken = true;

        if (MovementSpeed > 0)
            StartCoroutine(MovementAnimation(path));
        else
            transform.position = Cell.transform.position;

        if (UnitMoved != null)
            UnitMoved.Invoke(this, new MovementEventArgs(Cell, destinationCell, path));    
    }
    protected virtual IEnumerator MovementAnimation(List<Cell> path)
    {
        isMoving = true;

        path.Reverse();
        foreach (var cell in path)
        {
            while (new Vector2(transform.position.x,transform.position.y) != new Vector2(cell.transform.position.x,cell.transform.position.y))
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(cell.transform.position.x,cell.transform.position.y,transform.position.z), Time.deltaTime * MovementSpeed);
                yield return 0;
            }
        }

        isMoving = false;
    }

    ///<summary>
    /// Method indicates if unit is capable of moving to cell given as parameter.
    /// </summary>
    public virtual bool IsCellMovableTo(Cell cell)
    {
        return !cell.IsTaken;
    }
    /// <summary>
    /// Method indicates if unit is capable of moving through cell given as parameter.
    /// </summary>
    public virtual bool IsCellTraversable(Cell cell)
	{
		//TODO optional, units can move through allies
		//return true; //Need to check who is in that cell...
        return !cell.IsTaken;
    }
    /// <summary>
    /// Method returns all cells that the unit is capable of moving to.
    /// </summary>
    public List<Cell> GetAvailableDestinations(List<Cell> cells)
    {
        var ret = new List<Cell>();
        var cellsInMovementRange = cells.FindAll(c => IsCellMovableTo(c) && c.GetDistance(Cell) <= MovementPoints);

        var traversableCells = cells.FindAll(c => IsCellTraversable(c) && c.GetDistance(Cell) <= MovementPoints);
        traversableCells.Add(Cell);

        foreach (var cellInRange in cellsInMovementRange)
        {
            if (cellInRange.Equals(Cell)) continue;

            var path = FindPath(traversableCells, cellInRange);
            var pathCost = path.Sum(c => c.MovementCost);
            if (pathCost > 0 && pathCost <= MovementPoints)
                ret.AddRange(path);
        }
        return ret.FindAll(IsCellMovableTo).Distinct().ToList();
    }

    public List<Cell> FindPath(List<Cell> cells, Cell destination)
    {
        return _pathfinder.FindPath(GetGraphEdges(cells), Cell, destination);
    }
    /// <summary>
    /// Method returns graph representation of cell grid for pathfinding.
    /// </summary>
    protected virtual Dictionary<Cell, Dictionary<Cell, int>> GetGraphEdges(List<Cell> cells)
    {
        Dictionary<Cell, Dictionary<Cell, int>> ret = new Dictionary<Cell, Dictionary<Cell, int>>();
        foreach (var cell in cells)
        {
            if (IsCellTraversable(cell) || cell.Equals(Cell))
            {
                ret[cell] = new Dictionary<Cell, int>();
                foreach (var neighbour in cell.GetNeighbours(cells).FindAll(IsCellTraversable))
                {
                    ret[cell][neighbour] = neighbour.MovementCost;
                }
            }
        }
        return ret;
    }

    /// <summary>
    /// Gives visual indication that the unit is under attack.
    /// </summary>
    /// <param name="other"></param>
    public abstract void MarkAsDefending(Unit other);
    /// <summary>
    /// Gives visual indication that the unit is attacking.
    /// </summary>
    /// <param name="other"></param>
    public abstract void MarkAsAttacking(Unit other);
    /// <summary>
    /// Gives visual indication that the unit is destroyed. It gets called right before the unit game object is
    /// destroyed, so either instantiate some new object to indicate destruction or redesign Defend method. 
    /// </summary>
    public abstract void MarkAsDestroyed();

    /// <summary>
    /// Method marks unit as current players unit.
    /// </summary>
    public abstract void MarkAsFriendly();
    /// <summary>
    /// Method mark units to indicate user that the unit is in range and can be attacked.
    /// </summary>
    public abstract void MarkAsReachableEnemy();
    /// <summary>
    /// Method marks unit as currently selected, to distinguish it from other units.
    /// </summary>
    public abstract void MarkAsSelected();
    /// <summary>
    /// Method marks unit to indicate user that he can't do anything more with it this turn.
    /// </summary>
    public abstract void MarkAsFinished();
    /// <summary>
    /// Method returns the unit to its base appearance
    /// </summary>
    public abstract void UnMark();

    public static List<float> BlockSort(List<float> ChanceList)
    {
        int count = ChanceList.Count();
        float swap;
        for (int c = 0; c < (count - 1); c++) //bubble sorting
        {
            for (int d = 0; d < count - c - 1; d++) 
            {
                if (ChanceList[d] > ChanceList[d + 1])
                {
                    swap = ChanceList[d];
                    ChanceList[d] = ChanceList[d + 1];
                    ChanceList[d + 1] = swap;
                }
            } 
        } //end of sorting 

        return ChanceList;
    }

    //Käännä unit kohteen suuntaan
    public void turnUnit(Vector3 target){
		float x1 = this.transform.position.x;
		float y1 = this.transform.position.y;
		float x2 = target.x;
		float y2 = target.y;
		int suunta1 = this.Face;

		if (y2 > y1)
		{
			if (x2 > x1 && (x2 - x1 < y2 - y1)) suunta1 = 1;
			if (x2 < x1 && (x1 - x2 < y2 - y1)) suunta1 = 1;
			if (x2 == x1) suunta1 = 1; //up
		}
		if (x2 > x1)
		{
			if (y2 > y1 && (y2 - y1 < x2 - x1)) suunta1 = 2;
			if (y2 < y1 && (y1 - y2 < x2 - x1)) suunta1 = 2;
			if (y2 == y1) suunta1 = 2; //right
		}
		if (y2 < y1)
		{
			if (x2 > x1 && (x2 - x1 < y1 - y2)) suunta1 = 3;
			if (x2 < x1 && (x1 - x2 < y1 - y2)) suunta1 = 3;
			if (x2 == x1) suunta1 = 3; //down
		}
		if (x2 < x1)
		{
			if (y2 > y1 && (y2 - y1 < x2 - x1)) suunta1 = 4;
			if (y2 < y1 && (y1 - y2 < x2 - x1)) suunta1 = 4;
			if (y2 == y1) suunta1 = 4; //left
		}

		//Jos hyökkääjän ja kohteen vertaus
		if (x2 > x1 && y2 > y1 && (x2 - x1 == y2 - y1)) suunta1 = 1;
		if (x2 > x1 && y1 > y2 && (x2 - x1 == y1 - y2)) suunta1 = 2;
		if (x1 > x2 && y1 > y2 && (x1 - x2 == y1 - y2)) suunta1 = 3;
		if (x1 > x2 && y2 > y1 && (x1 - x2 == y2 - y1)) suunta1 = 4;

		//Turns character now
		this.Face = suunta1;
	}

    

    //This units dmg modifier against target
    public float getFaceModifier(Vector3 target, int targetFace){
		float damage = 9999f;
        BlockList = new List<float>();
        
        float back = this.BlockBack; //max
        float side = this.BlockSide;
        float front = this.BlockFront;

        float x1 = this.transform.position.x;
		float x2 = target.x;
		float y1 = this.transform.position.y;
		float y2 = target.y;
		int suunta2 = targetFace;
        

        //Kohteen suunta hyökätessä ja sen seuraukset
        switch (suunta2)
		{
		case 1:
			if (y1 <= y2)
			{
				if (x1 > x2 && (x1 - x2 >= y2 - y1))
				{
					damage = side;
				}
				else if (x2 > x1 && (x2 - x1 >= y2 - y1))
				{
					damage = side;
				}
				else damage = back;
			}
			else
			{
				if (x1 > x2)
				{
					if (x1 - x2 > y1 - y2) damage = side;
					else damage = front;
				}
				else if (x1 < x2)
				{
					if (x2 - x1 > y1 - y2) damage = side;
					else damage = front;
				}
				if (x1 == x2) damage = front;
			}
			break;
		case 2:
			if (x1 <= x2)
			{
				if (y1 > y2 && (y1 - y2 >= x2 - x1))
				{
					damage = side;
				}
				else if (y2 > y1 && (y2 - y1 >= x2 - x1))
				{
					damage = side;
				}
				else damage = back;
			}
			else
			{
				if (y1 > y2)
				{
					if (y1 - y2 > x1 - x2) damage = side;
					else damage = front;
				}
				else if (y1 < y2)
				{
					if (y2 - y1 > x1 - x2) damage = side;
					else damage = front;
				}
				if (y1 == y2) damage = front;
			}
			break;
		case 3:
			if (y2 <= y1)
			{
				if (x1 > x2 && (x1 - x2 >= y1 - y2))
				{
					damage = side;
				}
				else if (x2 > x1 && (x2 - x1 >= y1 - y2))
				{
					damage = side;
				}
				else damage = back;
			}
			else
			{
				if (x1 > x2)
				{
					if (x1 - x2 > y2 - y1) damage = side;
					else damage = front;
				}
				else if (x1 < x2)
				{
					if (x2 - x1 > y2 - y1) damage = side;
					else damage = front;
				}
				if (x1 == x2) damage = front;
			}
			break;
		case 4:
			if (x2 <= x1)
			{
				if (y1 > y2 && (y1 - y2 >= x1 - x2))
				{
					damage = side;
				}
				else if (y2 > y1 && (y2 - y1 >= x1 - x2))
				{
					damage = side;
				}
				else damage = back;
			}
			else
			{
				if (y1 > y2)
				{
					if (y1 - y2 > x2 - x1) damage = side;
					else damage = front;
				}
				else if (y1 < y2)
				{
					if (y2 - y1 > x2 - x1) damage = side;
					else damage = front;
				}
				if (y1 == y2) damage = front;
			}
			break;
		default:
			damage = side;
			break;
		}

        //Here we do chance add ups and such calculations
        float RealDamage;
        if (damage == side) {
        BlockList.Add(this.BlockBack); BlockList.Add(this.BlockSide);
            BlockList = BlockSort(BlockList);
            if (BlockList[1] == 0) { BlockList[1] = 1; }
            RealDamage = BlockList[0] / BlockList[1];
            RealDamage = (RealDamage - 1) * 100;
        }
        else if (damage == front) {
        BlockList.Add(this.BlockBack); BlockList.Add(this.BlockSide); BlockList.Add(this.BlockFront);
            BlockList = BlockSort(BlockList);
            RealDamage = BlockList[0] * ((BlockList[1]/100)+1) * ((BlockList[2] / 100) + 1);
            
        }
        else RealDamage = back;
        

		return RealDamage;
	}

   

    }

public class MovementEventArgs : EventArgs
{
    public Cell OriginCell;
    public Cell DestinationCell;
    public List<Cell> Path;

    public MovementEventArgs(Cell sourceCell, Cell destinationCell, List<Cell> path)
    {
        OriginCell = sourceCell;
        DestinationCell = destinationCell;
        Path = path;
    }
}
public class AttackEventArgs : EventArgs
{
    public Unit Attacker;
    public Unit Defender;

    public int Damage;

    public AttackEventArgs(Unit attacker, Unit defender/*, int damage*/)
    {
        Attacker = attacker;
        Defender = defender;

       // Damage = damage;
    }
}
