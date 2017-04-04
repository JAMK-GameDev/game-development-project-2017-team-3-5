class MySkill2 : Skill
{
    public override float SkillFormula(Unit b)
    {
        float dmg = rawValueDmg * b.AttackFactor + b.AttackRange;

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
