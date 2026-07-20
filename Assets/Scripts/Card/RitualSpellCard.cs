using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;

[CreateAssetMenu(
    fileName = "XXX-RitualSpell-Name",
    menuName = "Cards/RitualSpell",
    order = 5
)]
public class RitualSpellCard : SpellCard
{
    public override SpellCardType GetSpellCardType() => SpellCardType.Ritual;
    public override bool Activate()
    {
        Debug.Log("Activated RITUAL SPELL!");
        return true;
    }
}
