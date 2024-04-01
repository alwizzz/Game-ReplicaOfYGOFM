using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCard : GameplayCard
{
    [Header("Hand Card")]
    [SerializeField] private HandCardContainer container;

    public void SetContainer(HandCardContainer container)
    {
        this.container = container;
    }

    public void ResetContainer() { container = null; }
}
