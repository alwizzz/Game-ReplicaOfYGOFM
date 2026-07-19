using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System;

using Enums;


public abstract class GameplayCard : MonoBehaviour
{
    [Serializable]
    public struct Modifier
    {
        public int attackPointModifier;
        public int defensePointModifier;     
        public Card source;

        public Modifier(int _attackPointModifier = 0, int _defensePointModifier = 0, Card _source = null)
        {
            attackPointModifier = _attackPointModifier;
            defensePointModifier = _defensePointModifier;
            source = _source;
        }
    }


    [SerializeField] private Card cardData;
    [SerializeField] private GuardianStar selectedGuardianStar;
    [SerializeField] private List<Modifier> modifierList;


    // [Header("Modifier")]
    // [SerializeField] protected int attackPointModifier = 0;
    // [SerializeField] protected int defensePointModifier = 0;

    [Header("Caches")]
    [SerializeField] private Image baseImage;
    [SerializeField] private Image cardImage;
    [SerializeField] private GameObject cardAttributes;

    [SerializeField] protected int attackPoint;
    [SerializeField] protected int defensePoint;
    [SerializeField] protected TextMeshProUGUI attackPointText;
    [SerializeField] protected TextMeshProUGUI defensePointText;




    public void Setup(Card cardData)
    {
        this.cardData = cardData;

        baseImage.color = ResourceManager.Instance().GetGameplayCardBaseColor(cardData);
        var cardSprite = cardData.spriteBig;
        if (cardSprite == null)
        {
            print("WARNING: cardData's sprite is null, using dummy sprite instead");
            cardSprite = ResourceManager.Instance().GetDummySprite();
        }
        cardImage.sprite = cardSprite;

        if (cardData.IsMonsterCard())
        {
            cardAttributes.SetActive(true);
            var data = (MonsterCard)cardData;

            attackPoint = data.attackPoint;
            defensePoint = data.defensePoint;

            attackPointText.text = data.attackPoint.ToString();
            defensePointText.text = data.defensePoint.ToString();
        }
        else // is NonMonsterCard
        {
            cardAttributes.SetActive(false);

            // dummy values
            attackPoint = 0;
            defensePoint = 0;
        }
    }

    public void SetSelectedGuardianStar(GuardianStar value)
    {
        // modifier.selectedGuardianStar = value;
        selectedGuardianStar = value;
    }
    // public GuardianStar GetSelectedGuardianStar() => modifier.selectedGuardianStar;
    public GuardianStar GetSelectedGuardianStar() => selectedGuardianStar;

    public Card GetCardData() => cardData;
    public void SetModifierList(List<Modifier> value) => modifierList = value;
    public List<Modifier> GetModifierList() => modifierList;

}
