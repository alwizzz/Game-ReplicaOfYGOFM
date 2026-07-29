using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

using Enums;

[CreateAssetMenu(
    fileName = "XXX-Trap-Name",
    menuName = "Cards/Trap",
    order = 7
)]
public class TrapCard : NonMonsterCard
{ 
    public struct Context
    {
        public FieldCard summonedMonsterCard;
    }

    public TrapTrigger trigger;

    public override sealed bool IsSpellCard() => false;

    // TODO: only do a single effect for now, on full trap system implementation should handle all cases
    public override bool Activate() 
    {
        Debug.Log("Activate TRAP!");
        return true;
    }
    public bool Activate(Context context) 
    {
        Debug.Log("Activate TRAP! with context");
        var result = true;

        switch(cardName){
            case ("Eatgaboon"):
                if(context.summonedMonsterCard == default) throw new ArgumentNullException(nameof(context.summonedMonsterCard));

                context.summonedMonsterCard.Destroy();

                break;
            default:
                Debug.Log($"Unhandled case {cardName}");
                result = false;
                break;
        };

        return result;
    }
}
