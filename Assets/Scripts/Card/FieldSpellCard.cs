using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "XXX-FieldSpell-Name",
    menuName = "Cards/FieldSpell",
    order = 5
)]
public class FieldSpellCard : SpellCard
{
    public override void Activate()
    {
        Debug.Log("Activate FIELD SPELL!");
    }
}
