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
    public override void Activate()
    {
        Debug.Log("Activate EQUIP SPELL!");
    }
}
