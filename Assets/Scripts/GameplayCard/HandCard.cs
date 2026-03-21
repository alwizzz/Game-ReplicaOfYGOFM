using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

    public HandCardContainer GetContainer() { return container; }
}
