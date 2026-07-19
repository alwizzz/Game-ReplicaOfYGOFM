using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;

public abstract class SpellCard : NonMonsterCard
{
    public override sealed bool IsSpellCard() => true;
    public abstract SpellCardType GetSpellCardType();
}
