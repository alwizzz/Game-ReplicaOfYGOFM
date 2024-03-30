using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;

public abstract class MonsterCard : Card
{
    public MonsterType type;
    public GuardianStar guardianStarOption1;
    public GuardianStar guardianStarOption2;
    public int level;
    public int attackPoint;
    public int defensePoint;

    public virtual List<GuardianStar> GetGuardianStarOptions()
    {
        return new List<GuardianStar> { guardianStarOption1, guardianStarOption2 };
    }

    // applied on gameplay instead
    // public GuardianStar ChosenGuardianStar;

    public List<string> listOfSuitableEquip;

}
