using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandCardContainer : CardContainer
{
    //[SerializeField] private int index;
    [SerializeField] private HandCard containedHandCard;
    [SerializeField] private bool isSelected;

    [SerializeField] private GameObject fusionTag;
    private TextMeshProUGUI fusionOrderText;

    void Start()
    {
        fusionOrderText = fusionTag.GetComponentInChildren<TextMeshProUGUI>();
    }

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

        // safeguard to make sure fusionTag always ordered last
        fusionTag.transform.SetAsLastSibling();
    }

    public void RemoveCard(bool alsoDestroy = false)
    {
        if(containedHandCard == null)
        {
            print("WARNING: attempt to remove contained card when it is already null");
            return;
        }

        containedHandCard.ResetContainer();
        
        if(alsoDestroy)
        {
            Destroy(containedHandCard.gameObject);
        } 

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
        // multiclick floodgate is risen up to enable fusion flow handling
        // if (isSelected) return; 

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

    public bool ToggleFusionTag()
    {
        if(fusionTag.activeSelf == true)
        {
            // toggling off
            fusionTag.SetActive(false);
        } else
        {
            // toggling on
            fusionTag.SetActive(true);
        }

        return fusionTag.activeSelf;
    }
    
    public void UpdateFusionOrder(int fusionOrder)
    {
        fusionOrderText.text = fusionOrder.ToString();
    }
}
