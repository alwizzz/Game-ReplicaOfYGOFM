using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;

[CreateAssetMenu(
    fileName = "XXX-FieldSpell-Name",
    menuName = "Cards/FieldSpell",
    order = 5
)]
public class FieldSpellCard : SpellCard
{
    public override SpellCardType GetSpellCardType() => SpellCardType.Field;
    public override bool Activate()
    {
        Debug.Log("Activated FIELD SPELL!");
        return true;
    }
}
