using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "XXX-NormalSpell-Name",
    menuName = "Cards/NormalSpell",
    order = 3
)]
public class NormalSpellCard : SpellCard
{
    public override void Activate()
    {
        Debug.Log("Activate NORMAL SPELL!");
    }
}
