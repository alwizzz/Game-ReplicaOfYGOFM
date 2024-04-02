using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;


public class HandSystem : UIModal<HandSystem>
{
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
        BaseAwake(this);
        Show();
    }

    private void Start()
    {
        UpdateHand();
        //SetSelectedCardContainer(handCardContainers[0]);
        handCardContainers[0].Select();
    }

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

        HandFocusSystem.Instance().SetupAndShow(selectedHandCardContainer.GetCard()); ;
        Hide();
    }

    //public void UnfocusSelectedCard()
    //{
    //    if (!isFocusing) return;

    //    isFocusing = false;
    //    handFocusSystem.Hide();
    //    handOverlay.SetActive(false);
    //}


    #region Update and Organize Hand

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
            var cardData = GameplayDeck.Instance().Draw();
            if (cardData == null) return;
            var spawnedHandCard = Instantiate(
                handCardPrefab
            );
            spawnedHandCard.Setup(cardData);
            handCardContainers[i].SetCard(spawnedHandCard);
        }
    }

    #endregion


    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
