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
    public List<Unit> aoeUnitList;

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
    protected int TotalMovementPoints;
    protected int TotalActionPoints;

    /// <summary>
    /// Cell that the unit is currently occupying.
    /// </summary>
    public Cell Cell { get; set; }

	//Values
    public float HitPoints;
    public int AttackRange;
    public float AttackFactor;
    public int DefenceFactor;
	public int RealityBreak;
	public int ChargeTime;

	//Attributes
	public int UnitID; //Allows us to sort units...
	public int Speed = 5;
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
        Debug.Log(this);
        Debug.Log(caster);
        Debug.Log(caster.CurrentSkill);
        
        MarkAsDefending(caster);
        Unit temp = this;
        
        HitPoints -= caster.CurrentSkill.SkillFormula(caster);

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
    

    public virtual void Move(Cell destinationCell, List<Cell> path)
    {
        if (isMoving)
            return;

        var totalMovementCost = path.Sum(h => h.MovementCost);
        if (MovementPoints < totalMovementCost)
            return;

        //MovementPoints -= totalMovementCost;
		MovementPoints = 0;

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
