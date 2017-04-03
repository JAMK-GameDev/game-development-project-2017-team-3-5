using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class CellGridStateUnitSelected : CellGridState
{
    private Unit _unit;
    private List<Cell> _pathsInRange;
    private List<Cell> _cellsInRange;
    private List<Unit> _unitsInRange;
    private List<Cell> aoeCellList;
    private List<Unit> aoeUnitList;

    private Cell _unitCell;

    public CellGridStateUnitSelected(CellGrid cellGrid, Unit unit) : base(cellGrid)
    {
        _unit = unit;
        _pathsInRange = new List<Cell>();
        _unitsInRange = new List<Unit>();
        _cellsInRange  = new List<Cell>();
    }

    public override void OnCellClicked(Cell cell)
    {
        if (_unit.isMoving)
            return;
        if(cell.IsTaken && _cellGrid.canMove && _unit.MovementPoints > 0)
        {
            _cellGrid.CellGridState = new CellGridStateWaitingForInput(_cellGrid);
            _cellGrid.canMove = false;
            return;
        }

        if (_cellsInRange.Contains(cell) && _unit.ActionPoints > 0)
        {
            //Check neighbour cells if aoe > 0
            //Add these cells to list

            //Check through this celllist
            //if cell is taken, find that unit in cell if it is unit, might be obstacle so doesn't find unit
            //if unit found, add it to aoe unit list

            //Go through unit list and cast skill to all of them to make dmg
            //check damaga in unit to calculate dmg based on attributes
            aoeCellList.Add(cell);

            for(int i = 0; i < _unit.CurrentSkill.AOE; i++)
            {
                int b = aoeCellList.Count();
                for (int a = 0; a < b; a++)
                {
                    aoeCellList.AddRange(aoeCellList[a].GetNeighbours(_cellGrid.Cells));
                    //Dont worry, Distinct is god!
                }
            }
            //If it doesn't add this cell swell...
            
            aoeCellList = new List<Cell>(aoeCellList.Distinct());
            //Go through cells
            foreach(Cell currentCell in aoeCellList)
            {
                //Check if cell is taken
                if (currentCell.IsTaken)
                {
                    //Find cell taker
                    _cellGrid.Units.ForEach(u => {
                        //If unit cell contains this cell
                        if (u.Cell.Equals(currentCell))
                        {
                            //Add unit to list
                            aoeUnitList.Add(u);
                        }
                    });
                }
            }
            
            //Go through units
            
            aoeUnitList = new List<Unit>(aoeUnitList.Distinct());
            
            _unit.DoSkill(aoeUnitList);
            _cellGrid.CellGridState = new CellGridStateUnitSelected(_cellGrid, _unit);
        }

        if (!_pathsInRange.Contains(cell))
        {
            _cellGrid.CellGridState = new CellGridStateWaitingForInput(_cellGrid);
        }
        else
        {
            var path = _unit.FindPath(_cellGrid.Cells, cell);
            _unit.Move(cell,path);
            _cellGrid.CellGridState = new CellGridStateUnitSelected(_cellGrid, _unit);
        }
    }
    public override void OnUnitClicked(Unit unit)
    {
        if (unit.Equals(_unit) || unit.isMoving)
            return;

        if (_unitsInRange.Contains(unit) && _unit.ActionPoints > 0)
        {
            //_unit.DoSkill(unit);
            _cellGrid.CellGridState = new CellGridStateUnitSelected(_cellGrid, _unit);
        }

        if (unit.PlayerNumber.Equals(_unit.PlayerNumber))
        {
            _cellGrid.CellGridState = new CellGridStateUnitSelected(_cellGrid, unit);
        }
            
    }

    public override void AttackSelector(Unit i)
    {
        foreach (var currentUnit in _cellGrid.Units)
        {
            if (currentUnit.PlayerNumber.Equals(_unit.PlayerNumber))
                continue;
            
            if (_unit.IsCellAttackable(currentUnit.Cell, _unit.Cell, i.CurrentSkill.SkillRange))
            {
                currentUnit.SetState(new UnitStateMarkedAsReachableEnemy(currentUnit));
                
                _unitsInRange.Add(currentUnit);
            }
        }
    }


    public override void OnCellAttack()
    {
        foreach (var currentCell in _cellGrid.Cells)
        {
            if (_unit.IsCellAttackable(currentCell, _unit.Cell, _unit.CurrentSkill.SkillRange))
            {
                currentCell.MarkAsTarget();
                _cellsInRange.Add(currentCell);
                if (currentCell.IsTaken) {
                    Unit u = _cellGrid.Units.Find(unit => unit.Cell == currentCell);
                    u.SetState(new UnitStateMarkedAsReachableEnemy(u));
                }
            }
        }

        //We have AOE
        if (_unit.CurrentSkill.AOE > 0)
        {

        }
        else
        {

        }




        
        

        _pathsInRange = _unit.GetAvailableDestinations(_cellGrid.Cells);
        var cellsNotInRange = _cellGrid.Cells.Except(_pathsInRange);

        foreach (var cell in cellsNotInRange)
        {
            cell.UnMark();
        }
        foreach (var cell in _pathsInRange)
        {
            cell.MarkAsReachable();
        }

        if (_unit.ActionPoints <= 0) return;

        /*
        foreach (var currentUnit in _cellGrid.Units)
        {
            if (currentUnit.PlayerNumber.Equals(_unit.PlayerNumber))
                continue;
        
            if (_unit.IsUnitAttackable(currentUnit,_unit.Cell))
            {
                currentUnit.SetState(new UnitStateMarkedAsReachableEnemy(currentUnit));
                _unitsInRange.Add(currentUnit);
            }
        }*/

        if (_unitCell.GetNeighbours(_cellGrid.Cells).FindAll(c => c.MovementCost <= _unit.MovementPoints).Count == 0
            && _unitsInRange.Count == 0)
            _unit.SetState(new UnitStateMarkedAsFinished(_unit));


    }


    public override void OnCellDeselected(Cell cell)
    {
        base.OnCellDeselected(cell);

        foreach (var _cell in _pathsInRange)
        {
            _cell.MarkAsReachable();
        }
        foreach (var _cell in _cellGrid.Cells.Except(_pathsInRange))
        {
            _cell.UnMark();
        }
    }
    public override void OnCellSelected(Cell cell)
    {
        base.OnCellSelected(cell);
        if (!_pathsInRange.Contains(cell)) return;
        var path = _unit.FindPath(_cellGrid.Cells, cell);
        foreach (var _cell in path)
        {
            _cell.MarkAsPath();
        }
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        _unit.OnUnitSelected();
        _unitCell = _unit.Cell;

        _pathsInRange = _unit.GetAvailableDestinations(_cellGrid.Cells);
        var cellsNotInRange = _cellGrid.Cells.Except(_pathsInRange);

        foreach (var cell in cellsNotInRange)
        {
            cell.UnMark();
        }
        foreach (var cell in _pathsInRange)
        {
            cell.MarkAsReachable();
        }

        if (_unit.ActionPoints <= 0) return;

        /*
        foreach (var currentUnit in _cellGrid.Units)
        {
            if (currentUnit.PlayerNumber.Equals(_unit.PlayerNumber))
                continue;
        
            if (_unit.IsUnitAttackable(currentUnit,_unit.Cell))
            {
                currentUnit.SetState(new UnitStateMarkedAsReachableEnemy(currentUnit));
                _unitsInRange.Add(currentUnit);
            }
        }*/
        
        if (_unitCell.GetNeighbours(_cellGrid.Cells).FindAll(c => c.MovementCost <= _unit.MovementPoints).Count == 0 
            && _unitsInRange.Count == 0)
            _unit.SetState(new UnitStateMarkedAsFinished(_unit));
    }
    public override void OnStateExit()
    {
        _unit.OnUnitDeselected();
        foreach (var unit in _unitsInRange)
        {
            if (unit == null) continue;
            unit.SetState(new UnitStateNormal(unit));
        }
        foreach (var cell in _cellGrid.Cells)
        {
            cell.UnMark();
        }   
    }
}

