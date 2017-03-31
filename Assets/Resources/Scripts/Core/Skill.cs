using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
namespace Formulas
{
    interface formula_base
    {
        void a();

    }
    class Formula1 : MonoBehaviour, formula_base
    {
        public void a()
        {

        }
    }
}
public abstract class Skill : MonoBehaviour {

    /*
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
    */

    

    public float rawValueDmg, rawValueChance, rawValueExtra; //Formula modifiers

    public int SkillRange;

    public bool IsWeaponRange;

    public int AOE;

    

    public  virtual void SkillActivator(Unit b)
    {
        if (IsWeaponRange == true) { SkillRange = b.AttackRange; }
        
        


    }

    public float SkillFormula(Unit b)
    {
        
        float dmg = rawValueDmg * b.AttackFactor;
        
        return dmg;

    }






}
