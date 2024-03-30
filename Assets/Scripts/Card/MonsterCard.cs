using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;

public abstract class MonsterCard : Card
{

    public MonsterType Type;
    public int Level;
    public int AttackPoint;
    public int DefensePoint;

    public GuardianStar guardianStarOption1;
    public GuardianStar guardianStarOption2;
    public virtual List<GuardianStar> GetGuardianStarOptions()
    {
        return new List<GuardianStar> { guardianStarOption1, guardianStarOption2 };
    }
    
    // applied on gameplay instead
    // public GuardianStar ChosenGuardianStar;

}
