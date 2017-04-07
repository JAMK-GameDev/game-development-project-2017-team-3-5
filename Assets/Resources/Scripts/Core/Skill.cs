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

    public bool IsIPdmg;

    public bool IsHeal;

    public int AOE;

    public float IPcost;

    public bool RB;

    

    public  virtual void SkillActivator(Unit b)
    {
        if (IsWeaponRange == true) { SkillRange = b.AttackRange; }
        
        


    }

    public virtual float SkillFormula(Unit b) {
        float value = 0;

        //if (IsHeal == true) { value = value * -1; }
        //Still seeing what way is the best way to implement healing... Most likely this, but it might not be easy -_-
        // Yours truly, Toni~

        return value;
    }

        




}
