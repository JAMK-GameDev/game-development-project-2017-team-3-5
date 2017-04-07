class Formula_2 : Skill
{
    public override float SkillFormula(Unit b)
    {
        float dmg = rawValueDmg * b.MagicalPower * b.PhysicalPower * -1 ; //-1 makes it a heal formula

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
