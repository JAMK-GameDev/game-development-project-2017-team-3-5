class Formula_1 : Skill
{
    public override float SkillFormula(Unit b)
    {
        float dmg = rawValueDmg * b.MagicalPower;

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
