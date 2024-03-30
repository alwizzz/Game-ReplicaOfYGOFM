using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;

[CreateAssetMenu(
    fileName = "XXX-Trap-Name",
    menuName = "Cards/Trap",
    order = 7
)]
public class TrapCard : NonMonsterCard
{ 
    public TrapEffect effect;
    public int attackPointTreshold;
}
