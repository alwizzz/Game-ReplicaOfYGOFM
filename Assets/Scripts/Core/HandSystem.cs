using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using TMPro;
using Enums;
using Unity.VisualScripting;


public class HandSystem : UIModal<HandSystem>
{
    [SerializeField] private Side owner;
    [SerializeField] private HandCard handCardPrefab;

    [Header("States")]
    [SerializeField] private HandCardContainer selectedHandCardContainer;
    [SerializeField] private bool isFusionMode;
    [SerializeField] private List<HandCardContainer> fusionList;

    //[SerializeField] private bool isFocusing;

    [Header("Caches")]
    [SerializeField] private List<HandCardContainer> handCardContainers;
    //[SerializeField] private List<GameplayCardUI> handCards;
    [SerializeField] private GameObject handSelector;
    [SerializeField] private CardInformationDisplay cardInformationDisplay;

    //[SerializeField] private HandFocusSystem handFocusSystem;
    [SerializeField] private GameObject handOverlay;
    [SerializeField] private Button fusionButton;
    private Image fusionButtonImage;

    public override void Show()
    {
        base.Show();
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            // (RectTransform)handCardContainers[0].transform.GetComponent<RectTransform>()
            GetComponent<RectTransform>()
        );

        // GameplayManager.Instance().StartFreeSelection();
    }
    public override void Hide()
    {
        base.Hide();
        // GameplayManager.Instance().StopFreeSelection();
    }

    private void Awake()
    {
        BaseAwake(this);
        
        // // debug
        // if(IsPlayerOwned() && handCardContainers[0] != null)
        // {
        //     print($"Awake: {handCardContainers[0].transform.position}" + System.Environment.StackTrace);
        // }
    }

    private void Start()
    {
        fusionButtonImage = fusionButton.GetComponent<Image>();
        
        //UpdateHand();
        //SetSelectedCardContainer(handCardContainers[0]);
        // debug
        // if(IsPlayerOwned() && handCardContainers[0] != null)
        // {
        //     print($"Start: {handCardContainers[0].transform.position}" + System.Environment.StackTrace);
        // }

        // Canvas.ForceUpdateCanvases();
        // LayoutRebuilder.ForceRebuildLayoutImmediate(
        //     (RectTransform)handCardContainers[0].transform.GetComponent<RectTransform>()
        // );
    }

#region debugging



    // private void OnEnable()
    // {
    //     // debug
    //     if(IsPlayerOwned() && handCardContainers[0] != null)
    //     {
    //         print($"OnEnable: {handCardContainers[0].transform.position}" + System.Environment.StackTrace);
    //     }
    // }
    // private void Update()
    // {
    //     // debug
    //     if(IsPlayerOwned() && handCardContainers[0] != null)
    //     {
    //         print($"Update: {handCardContainers[0].transform.position}" + System.Environment.StackTrace);
    //     }
    // }
    // private void LateUpdate()
    // {
    //     // debug
    //     if(IsPlayerOwned() && handCardContainers[0] != null)
    //     {
    //         print($"LateUpdate: {handCardContainers[0].transform.position}" + System.Environment.StackTrace);
    //     }
    // }

#endregion

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
        HandleFusionFlow(handCardContainer);
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
        // TODO: in the future it should handle whether the focus is on single card or player is doing a fusion

        if (isFusionMode) // fusion flow
        {
            if(fusionList.Count == 0)
            {
                // basically the same as Single flow
                var card = selectedHandCardContainer.GetCard();
                GameplayManager.Instance().HandFocusSystem().SetupSingleFlow(card);
            } else if(fusionList.Count == 1)
            {
                // similar with Single flow but the card is the only element on fusionList
                var card = fusionList[0].GetCard();
                GameplayManager.Instance().HandFocusSystem().SetupSingleFlow(card);
            } else
            {
                // actual fusion flow
                List<HandCard> fusionHandCardList = new List<HandCard>();
                fusionList.ForEach(e => fusionHandCardList.Add(e.GetCard()));

                GameplayManager.Instance().HandFocusSystem().SetupFusionFlow(fusionHandCardList);
            }
        } else // single flow
        {
            var card = selectedHandCardContainer.GetCard();
            GameplayManager.Instance().HandFocusSystem().SetupSingleFlow(card);
        }

        Hide();

        // selectedHandCardContainer.RemoveCard(alsoDestroy: false);
        // selectedHandCardContainer.RemoveCard(alsoDestroy: true);
    }

    public List<HandCardContainer> GetHandCardContainers() => handCardContainers;


    #region Update and Organize Hand
    
    public void OpenHand()
    {
        print("oy from " + owner);
        UpdateHand();
        // var defaultHandCardContainer = handCardContainers[0];
        // SetSelectedCardContainer(defaultHandCardContainer);
        Show();
        handCardContainers[0].Select();
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

    #region Fusion Mode

    public void ToggleFusionMode()
    {
        if (isFusionMode)
        {
            fusionList.ForEach(e => e.ToggleFusionTag()); // TODO: might bugged if toggling isnt doing toggle off
            fusionList.Clear();

            fusionButtonImage.color = Color.white;
            isFusionMode = false;
        } else
        {
            fusionButtonImage.color = Color.gray;
            isFusionMode = true;
        }
    }

    private void HandleFusionFlow(HandCardContainer handCardContainer)
    {
        if (!isFusionMode) return;

        bool isTogglingOn = handCardContainer.ToggleFusionTag();
        if(isTogglingOn)
        {
            int fusionListLength = fusionList.Count;
            handCardContainer.UpdateFusionOrder(fusionListLength + 1);
            fusionList.Add(handCardContainer);
        } else
        {
            fusionList.Remove(handCardContainer);
            if(fusionList.Count != 0)
            {
                // adjust fusion on every fusion tag
                for(int i=0; i<fusionList.Count; i++)
                {
                    fusionList[i].UpdateFusionOrder(i+1);
                }
            }
        }
    }

    #endregion
}
