using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "XXX-RitualSpell-Name",
    menuName = "Cards/RitualSpell",
    order = 5
)]
public class RitualSpellCard : SpellCard
{
    public override void Activate()
    {
        Debug.Log("Activate RITUAL SPELL!");
    }
}
