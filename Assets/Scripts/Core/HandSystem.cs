using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using TMPro;
using Enums;


public class HandSystem : UIModal<HandSystem>
{
    [SerializeField] private Side owner;
    [SerializeField] private HandCard handCardPrefab;

    [Header("States")]
    [SerializeField] private HandCardContainer selectedHandCardContainer;
    //[SerializeField] private bool isFocusing;

    [Header("Caches")]
    [SerializeField] private List<HandCardContainer> handCardContainers;
    //[SerializeField] private List<GameplayCardUI> handCards;
    [SerializeField] private GameObject handSelector;
    [SerializeField] private CardInformationDisplay cardInformationDisplay;

    //[SerializeField] private HandFocusSystem handFocusSystem;
    [SerializeField] private GameObject handOverlay;


    private void Awake()
    {
        BaseAwake(this);
    }

    private void Start()
    {
        //UpdateHand();
        //SetSelectedCardContainer(handCardContainers[0]);
    }

    private bool IsPlayerOwned() => (owner == Side.Player ? true : false);

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
        //if (!IsPlayerPossession()) return;
        selectedHandCardContainer.MovePositionOnContainer(handSelector.transform);
    }

    private void UpdateInformationDisplay()
    {
        cardInformationDisplay.UpdateInformation(selectedHandCardContainer.GetCard());
    }

    public void FocusSelectedCard()
    {
        var card = selectedHandCardContainer.GetCard();
        GameplayManager.Instance().ToFocusPhase(card);
        Hide();

        selectedHandCardContainer.RemoveCard(alsoDestroy: true);
    }

    public List<HandCardContainer> GetHandCardContainers() => handCardContainers;


    #region Update and Organize Hand
    
    public void OpenHand()
    {
        print("oy from " + owner);
        UpdateHand();
        handCardContainers[0].Select();
        Show();
    }

    // TODO: differentiate DrawPhase and HandPhase on UpdateHand
    private void UpdateHand()
    {
        int length = handCardContainers.Count;
        int i = 0;
        // Reorganizing mode
        ReorganizeHand(ref i, length);

        // Draw mode
        DrawUntilFull(ref i, length);

        GameplayManager.Instance().ToHandPhase();
    }

    private void ReorganizeHand(ref int i, int length)
    {
        for (; i < length; i++)
        {
            var container = handCardContainers[i];
            if (container.IsEmpty())
            {
                bool foundCardToFill = false;
                int j = i + 1;
                for (; j < length; j++)
                {
                    if (handCardContainers[j].IsEmpty() == false)
                    {
                        foundCardToFill = true;
                        break;
                    }
                }

                if (foundCardToFill)
                {
                    handCardContainers[j].MoveCardTo(container);
                }
                else
                {
                    break; // continue to draw mode
                }

            }
        }
    }

    private void DrawUntilFull(ref int i, int length)
    {
        var drawnCardList = new List<string>();
        for (; i < length; i++)
        {
            var cardData = GameplayManager.Instance().Deck().Draw();
            if (cardData == null) return;

            drawnCardList.Add(cardData.cardName);
            var spawnedHandCard = Instantiate(
                handCardPrefab
            );
            spawnedHandCard.Setup(cardData);
            handCardContainers[i].SetCard(spawnedHandCard);
        }

        print($"Drawn cards: {string.Join(", ", drawnCardList)}");
    }

    #endregion


}
