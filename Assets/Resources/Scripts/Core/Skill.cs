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

    public float x, y, z;

    public int SkillRange;

    bool IsWeaponRange;

    int AOE;

    

    public void SkillActivator(Unit b)
    {
        if (IsWeaponRange == true) { SkillRange = b.AttackRange; }

        

    }
    
    
    




}
