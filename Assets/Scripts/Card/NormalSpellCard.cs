using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;

[CreateAssetMenu(
    fileName = "XXX-NormalSpell-Name",
    menuName = "Cards/NormalSpell",
    order = 3
)]
public class NormalSpellCard : SpellCard
{
    public override SpellCardType GetSpellCardType() => SpellCardType.Normal;

    /*
        Spell activation will nearly always succeed (also applicable to Equip, Field, and Ritual)
        The only thing that can disrupt is by TrapCard (which will be applied later)
        Such, for now the return value is not "succeed" (like commonly used in other script)
        but effectActivated, as a normal spell card is able to activate but the effect itself is not applied anywhere 
        because the condition does not met (e.g., Destroy all dragon monster where there's no dragon monster)
    */
    public override bool Activate() 
    {
        Debug.Log("Activated NORMAL SPELL!");
        bool effectActivated;

        switch(cardName){ // TODO: classify effect
            case "RedMedicine":
                GameplayManager.Instance().IncreaseLifePointFromSpell(500);
                effectActivated = true; 
                break;
            default:
                Debug.Log($"Unhandled case {cardName}");
                effectActivated = false;
                break;
        };

        return effectActivated;
    }
}
