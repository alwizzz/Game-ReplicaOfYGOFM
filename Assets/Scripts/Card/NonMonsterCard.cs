using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NonMonsterCard : Card
{
    public override sealed bool IsMonsterCard() => false;
    public abstract bool IsSpellCard();
    public abstract void Activate();
}
