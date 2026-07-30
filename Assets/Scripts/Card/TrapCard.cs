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
        public Context(FieldCard _summonedMonsterCard)
        {
            summonedMonsterCard = _summonedMonsterCard;
        }
    }

    public TrapTrigger trigger;

    public override sealed bool IsSpellCard() => false;

    public override bool Activate() // an "empty" function, mainly used for trap activation out of its trigger condition
    {
        Debug.Log("Activate TRAP!");
        return true;
    }

    // TODO: only do a single effect for now, on full trap system implementation should handle all cases
    public bool Activate(Context context) 
    {
        Debug.Log("Activate TRAP! with context");
        bool succeed;

        // implicitly also do a Check()
        bool canTrigger = Check(context);

        if (canTrigger)
        {
            switch(cardName){
                case "Eatgaboon":
                    context.summonedMonsterCard.Destroy();
                    succeed = true; // NOTE: make sure the Destroy() succeed would be nice
                    break;
                default:
                    Debug.Log($"Unhandled case {cardName}");
                    succeed = false;
                    break;
            };
        } else
        {
            succeed = false;
        }

        return succeed;
    }

    public bool Check(Context context)
    {
        bool canTrigger;

        switch(cardName){
            case "Eatgaboon":
                if(context.summonedMonsterCard == default) throw new ArgumentNullException(nameof(context.summonedMonsterCard));

                // TODO: apply actual Eatgaboon trigger condition
                canTrigger = true;

                break;
            default:
                Debug.Log($"Unhandled case {cardName}");
                canTrigger = false;
                break;
        };

        return canTrigger;
    }
}
