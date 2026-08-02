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

    [SerializeField] protected int baseAttackPoint;
    [SerializeField] protected int baseDefensePoint;
    [SerializeField] protected TextMeshProUGUI attackPointText;
    [SerializeField] protected TextMeshProUGUI defensePointText;




    public void Setup(Card cardData, List<Modifier> modifierList = null)
    {
        this.cardData = cardData;

        // quick handling, now modifierList default is an empty list instead of null
        if(modifierList == null) modifierList = new();

        SetModifierList(modifierList);

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

            baseAttackPoint = data.attackPoint;
            baseDefensePoint = data.defensePoint;

            RefreshAttackDefenseText();
        }
        else // is NonMonsterCard
        {
            cardAttributes.SetActive(false);

            // dummy values
            baseAttackPoint = 0;
            baseDefensePoint = 0;
        }
    }

    public void RefreshAttackDefenseText()
    {
        attackPointText.text = GetAttackPoint().ToString();
        defensePointText.text = GetDefensePoint().ToString();
    }

    public void SetSelectedGuardianStar(GuardianStar value)
    {
        // modifier.selectedGuardianStar = value;
        selectedGuardianStar = value;
    }
    // public GuardianStar GetSelectedGuardianStar() => modifier.selectedGuardianStar;
    public GuardianStar GetSelectedGuardianStar() => selectedGuardianStar;

    public Card GetCardData() => cardData;

    public void SetModifierList(List<Modifier> value){
        if(!cardData.IsMonsterCard()) return; // safeguard
        if(value == null) value = new(); // make sure no null

        modifierList = value;
        RefreshAttackDefenseText();
    }
    public List<Modifier> GetModifierList() => modifierList;

    public int GetBaseAttackPoint() => baseAttackPoint;
    public int GetAttackPoint() {
        int result = baseAttackPoint;
        modifierList?.ForEach(modifier =>
        {
            result += modifier.attackPointModifier;
        });

        return result;
    }
    
    public int GetBaseDefensePoint() => baseDefensePoint;
    public int GetDefensePoint() {
        int result = baseDefensePoint;
        modifierList?.ForEach(modifier =>
        {
            result += modifier.defensePointModifier;
        });

        return result;
    }

}
