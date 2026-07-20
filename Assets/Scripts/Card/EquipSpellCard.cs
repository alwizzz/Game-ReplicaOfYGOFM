using System.Collections;
using System.Collections.Generic;
using Enums;

// using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(
    fileName = "XXX-EquipSpell-Name",
    menuName = "Cards/EquipSpell",
    order = 4
)]
public class EquipSpellCard : SpellCard
{
    public int attackPointIncrease;
    public int defensePointIncrease;

    public override SpellCardType GetSpellCardType() => SpellCardType.Equip;
    public override bool Activate()
    {
        // NOTE: As Activate() strictly means activating effect as an independent card,
        // where EquipSpellCard only has effect if it went to fusion with another card.
        // So, EquipSpellCard Activate() technically does nothing
        Debug.Log("Activated EQUIP SPELL which does nothing...");
        return true;
    }
}
