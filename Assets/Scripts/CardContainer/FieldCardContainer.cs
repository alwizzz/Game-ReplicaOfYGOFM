using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCardContainer : CardContainer
{
    public enum Rank
    { 
        FrontRank,
        BackRank
    }
    [SerializeField] private FieldSystem fieldSystem;
    [SerializeField] private Rank rank;

    //[SerializeField] private int index;
    [SerializeField] private FieldCard containedFieldCard;
    [SerializeField] private bool isEmpty;
    [SerializeField] private bool isSelected;


    public void Setup(FieldSystem fieldSystem)
    {
        this.fieldSystem = fieldSystem;
        isEmpty = true;
    }

    public FieldCard GetCard() => containedFieldCard;
    public bool IsEmpty() => (containedFieldCard == null ? true : false);

    public void SetCard(FieldCard fieldCard)
    {
        if (isEmpty == false)
        {
            print("ERROR: attempt to set contained card when currently one exists, aborting...");
            return;
        }
        isEmpty = false;

        MovePositionOnContainer(fieldCard.transform, setParent: true);
        containedFieldCard = fieldCard;
        containedFieldCard.SetContainer(this);
    }

    public void RemoveCard()
    {
        if (isEmpty == true)
        {
            print("WARNING: attempt to remove contained card when it is already null");
            return;
        }
        isEmpty = true;

        containedFieldCard.ResetContainer();
        containedFieldCard = null;

        fieldSystem.UpdateInformationDisplay(reset: true);
        fieldSystem.DecrementCardCount(IsBackRank());
    }


    public void Select()
    {
        //if (isSelected) return;

        isSelected = true;

        if (IsPlayerOwned())
        {
            FieldButtonManager.Instance().UpdateButtons(this);
            GameplayManager.Instance().PlayerFieldSystem().SetSelectedCardContainer(this);
        } else
        {
            GameplayManager.Instance().EnemyFieldSystem().SetSelectedCardContainer(this);
        }
    }

    public void Unselect()
    {
        if (!isSelected) return;

        isSelected = false;
    }

    public bool IsBackRank() => (rank == Rank.BackRank ? true : false);

    public void SetAsAttackerInBattle()
    {
        if (GetCard().HasBeenUsed()) return; 
        BattleSystem.Instance().SetAttackerReference(containedFieldCard);
    }
    public void SetAsAttackedInBattle()
    {
        BattleSystem.Instance().SetAttackedReference(containedFieldCard);
    }
}
