using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;


public abstract class GameplayCard : MonoBehaviour
{
    [SerializeField] private Card cardData;

    [Header("Caches")]
    [SerializeField] private Image baseImage;
    [SerializeField] private Image cardImage;
    [SerializeField] private GameObject cardAttributes;
    [SerializeField] private TextMeshProUGUI attackPointText;
    [SerializeField] private TextMeshProUGUI defensePointText;


    public void Setup(Card cardData)
    {
        this.cardData = cardData;
        SetupData();
    }

    private void SetupData()
    {
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

    public Card GetCardData() => cardData;

}
