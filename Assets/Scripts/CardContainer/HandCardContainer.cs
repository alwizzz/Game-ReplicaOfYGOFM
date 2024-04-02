using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCardContainer : CardContainer
{
    //[SerializeField] private int index;
    [SerializeField] private HandCard containedHandCard;
    [SerializeField] private bool isSelected;

    public HandCard GetCard() => containedHandCard;
    public bool IsEmpty() => (containedHandCard == null ? true : false);

    public void SetCard(HandCard handCard)
    {
        if(containedHandCard != null)
        {
            print("ERROR: attempt to set contained card when currently one exists, aborting...");
            return;
        }

        MovePositionOnContainer(handCard.transform, setParent:true);
        containedHandCard = handCard;
        containedHandCard.SetContainer(this);
    }

    public void RemoveCard()
    {
        if(containedHandCard == null)
        {
            print("WARNING: attempt to remove contained card when it is already null");
            return;
        }

        containedHandCard.ResetContainer();
        containedHandCard = null;
    }

    public void MoveCardTo(HandCardContainer container)
    {
        // make sure the input container currently not holding any

        var tempHandCard = containedHandCard;
        RemoveCard();
        container.SetCard(tempHandCard);
    }

    public void Select()
    {
        if (isSelected) return;
        if (IsEmpty()) return; // unable to be selected if empty

        isSelected = true;
        if (IsPlayerOwned())
        {
            GameplayManager.Instance().PlayerHandSystem().SetSelectedCardContainer(this);
        }
        else
        {
            GameplayManager.Instance().EnemyHandSystem().SetSelectedCardContainer(this);
        }
    }

    public void Unselect()
    {
        if (!isSelected) return;

        isSelected = false;
    }
}
