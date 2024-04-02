using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellCard : NonMonsterCard
{
    public override sealed bool IsSpellCard() => true;
}
