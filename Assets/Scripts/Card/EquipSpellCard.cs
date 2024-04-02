using System.Collections;
using System.Collections.Generic;
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

    public override void Activate()
    {
        Debug.Log("Activate EQUIP SPELL!");
    }
}
