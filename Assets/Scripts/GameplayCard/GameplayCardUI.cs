using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;


public class GameplayCardUI : MonoBehaviour
{
    [SerializeField] private Card cardData;

    [Header("States")]
    [SerializeField] private bool isSelected;

    [Header("Caches")]
    [SerializeField] private Image baseImage;
    [SerializeField] private Image cardImage;
    [SerializeField] private GameObject cardAttributes;
    [SerializeField] private TextMeshProUGUI attackPointText;
    [SerializeField] private TextMeshProUGUI defensePointText;

    private void Start()
    {
        Setup();
    }

    // TODO: called by its spawner in the further logic
    public void Setup()
    {
        if(cardData == null)
        {
            print("ERROR: cardData is null, aborting...");
            return;
        }
        SetupData();
    }

    public void Setup(Card cardData)
    {
        this.cardData = cardData;
        SetupData();
    }

    private void SetupData()
    {
        isSelected = false;

        baseImage.color = GameplayManager.Instance().GetGameplayCardUIBaseColor(cardData);
        var cardSprite = cardData.spriteBig;
        if (cardSprite == null)
        {
            print("WARNING: cardData's sprite is null, using dummy sprite instead");
            cardSprite = GameplayManager.Instance().GetDummySprite();
        }
        cardImage.sprite = cardSprite;

        if (cardData.IsMonsterCard())
        {
            cardAttributes.SetActive(true);
            var data = (MonsterCard)cardData;
            attackPointText.text = data.attackPoint.ToString();
            defensePointText.text = data.defensePoint.ToString();
        }
        else // is NonMonsterCard
        {
            cardAttributes.SetActive(false);
        }
    }

    public void SelectCard()
    {
        if (isSelected) return;

        isSelected = true;
        HandSystem.Instance().SetSelectedCard(this);
    }

    public void UnselectCard()
    {
        if (!isSelected) print("WARNING: unselecting a currently unselected card");
        isSelected = false;
    }

    public Card GetCardData() => cardData;

}
