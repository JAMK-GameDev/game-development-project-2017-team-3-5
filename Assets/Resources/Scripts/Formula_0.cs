﻿class Formula_0 : Skill
{
    public override float SkillFormula(Unit b)
    {
        float dmg = rawValueDmg * b.PhysicalPower;

        return dmg;

    }
}
/*
class BasicAttack : Skill
{
    public override void SkillActivator(Unit user, Unit target)
    {

    }
}*/
