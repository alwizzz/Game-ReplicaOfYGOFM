using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Enums;


public class HandSystem : UIModal
{
    [SerializeField] private Side possession;
    [SerializeField] private HandCard handCardPrefab;

    [Header("States")]
    [SerializeField] private HandCardContainer selectedHandCardContainer;
    [SerializeField] private bool isFocusing;

    [Header("Caches")]
    [SerializeField] private List<HandCardContainer> handCardContainers;
    //[SerializeField] private List<GameplayCardUI> handCards;
    [SerializeField] private GameObject handSelector;
    [SerializeField] private CardInformationDisplay cardInformationDisplay;

    //[SerializeField] private HandFocusSystem handFocusSystem;
    [SerializeField] private GameObject handOverlay;


    private void Awake()
    {
        BaseAwake();
    }

    private void Start()
    {
        //UpdateHand();
        //SetSelectedCardContainer(handCardContainers[0]);
        handCardContainers[0].Select();
    }

    private bool IsPlayerPossession() => (possession == Side.Player ? true : false);

    public void SetSelectedCardContainer(HandCardContainer handCardContainer)
    {
        if(selectedHandCardContainer != null)
        {
            selectedHandCardContainer.Unselect();
        }

        selectedHandCardContainer = handCardContainer;
        UpdateHandSelector();
        UpdateInformationDisplay();
    }

    private void UpdateHandSelector()
    {
        if (!IsPlayerPossession()) return;
        selectedHandCardContainer.MovePositionOnContainer(handSelector.transform);
    }

    private void UpdateInformationDisplay()
    {
        cardInformationDisplay.UpdateInformation(selectedHandCardContainer.GetCard()); ;
    }

    public void FocusSelectedCard()
    {
        if (isFocusing) return;

        isFocusing = true;
        //handOverlay.SetActive(true); // unused as it will then be hidden
        var card = selectedHandCardContainer.GetCard();
        GameplayManager.Instance().ToFocusPhase(card);
        Hide();
    }

    #region Update and Organize Hand

    
    public void OpenHand()
    {
        UpdateHand();
        Show();
    }

    // TODO: differentiate DrawPhase and HandPhase on UpdateHand
    // TODO: refactor this method
    private void UpdateHand()
    {
        int length = handCardContainers.Count;
        int i = 0;
        // Reorganizing mode
        for(; i<length; i++)
        {
            var container = handCardContainers[i];
            if(container.IsEmpty())
            {
                bool foundCardToFill = false;
                int j = i + 1;
                for (; j<length; j++)
                {
                    if(handCardContainers[j].IsEmpty() == false)
                    {
                        foundCardToFill = true;
                        break;
                    }
                }

                if(foundCardToFill)
                {
                    handCardContainers[j].MoveCardTo(container);
                } else
                {
                    break; // continue to draw mode
                }

            }
        }

        // Draw mode
        for(; i<length;i++)
        {
            var cardData = GameplayManager.Instance().Deck().Draw();
            if (cardData == null) return;
            var spawnedHandCard = Instantiate(
                handCardPrefab
            );
            spawnedHandCard.Setup(cardData);
            handCardContainers[i].SetCard(spawnedHandCard);
        }

        GameplayManager.Instance().ToHandPhase();
    }

    #endregion


}
