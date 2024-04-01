using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;


public class HandSystem : UIModal<HandSystem>
{
    [Header("States")]
    [SerializeField] private GameplayCardUI currentSelectedCard;
    [SerializeField] private bool isFocusing;

    [Header("Caches")]
    [SerializeField] private List<GameplayCardUI> handCards;
    [SerializeField] private GameObject handSelector;
    [SerializeField] private CardInformationDisplay cardInformationDisplay;

    [SerializeField] private HandFocusSystem handFocusSystem;
    [SerializeField] private GameObject handOverlay;


    private void Awake()
    {
        BaseAwake(this);
        Show();
    }

    private void Start()
    {
        handCards[2].SelectCard();
    }

    public void SetSelectedCard(GameplayCardUI cardUI)
    {
        if(currentSelectedCard != null)
        {
            currentSelectedCard.UnselectCard();
        }

        currentSelectedCard = cardUI;
        UpdateHandSelector();
        cardInformationDisplay.UpdateInformation(cardUI);
    }

    private void UpdateHandSelector()
    {
        handSelector.transform.position = currentSelectedCard.transform.position;
    }

    public void FocusSelectedCard()
    {
        if (isFocusing) return;

        isFocusing = true;
        handFocusSystem.SetupAndShow(currentSelectedCard);
        handOverlay.SetActive(true);
    }

    public void UnfocusSelectedCard()
    {
        if (!isFocusing) return;

        isFocusing = false;
        handFocusSystem.Hide();
        handOverlay.SetActive(false);
    }


    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
