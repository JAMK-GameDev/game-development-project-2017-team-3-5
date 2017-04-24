class MySkill : Skill
{
    public override float SkillFormula(Unit b)
    {
        float value = rawValueDmg * b.PhysicalPower;

        return value;

    }
}
/*
class BasicAttack : Skill
{
    public override void SkillActivator(Unit user, Unit target)
    {

    }
}*/
