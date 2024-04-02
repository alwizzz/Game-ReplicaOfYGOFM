using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCard : GameplayCard
{
    [Header("Hand Card")]
    [SerializeField] private FieldCardContainer container;

    public void SetContainer(FieldCardContainer container)
    {
        this.container = container;
    }

    public void ResetContainer() { container = null; }
}
