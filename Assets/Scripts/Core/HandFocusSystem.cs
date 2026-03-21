using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Enums;

public class HandFocusSystem : UIModal<HandFocusSystem>
{
    [SerializeField] private Side possession;
    [SerializeField] private HandCard focusedCard;
    [SerializeField] private HandCard handCardReferenceFromHand;
    [SerializeField] private bool isFaceDown;
    [SerializeField] private bool isMonster;

    [Header("Cache")]
    [SerializeField] private GuardianStarOptionSelector selector;
    [SerializeField] private GameObject faceDownCardImage;
    [SerializeField] private GameObject faceDownButton;
    [SerializeField] private GameObject faceUpButton;


    private void Awake()
    {
        BaseAwake(this);
    }

    public void SetupAndShow(HandCard handCard)
    {
        var cardData = handCard.GetCardData();
        focusedCard.Setup(cardData);
        handCardReferenceFromHand = handCard;

        if(cardData.IsMonsterCard())
        {
            isMonster = true;
            var data = (MonsterCard)cardData;
            selector.gameObject.SetActive(true);
            selector.Setup(data.guardianStarOption1, data.guardianStarOption2);
            GameplayManager.Instance().FieldSystem().OpenFrontRankSelection();
        } else
        {
            isMonster = false;
            selector.gameObject.SetActive(false);
            GameplayManager.Instance().FieldSystem().OpenBackRankSelection();
        }

        Show();
        SetToFaceUp();
    }

    public void SetToFaceUp()
    {
        if (isFaceDown == false) return;

        isFaceDown = false;
        faceDownCardImage.SetActive(false);
        faceDownButton.SetActive(true);
        faceUpButton.SetActive(false);
    }

    public void SetToFaceDown()
    {
        if (isFaceDown == true) return;

        isFaceDown = true;
        faceDownCardImage.SetActive(true);
        faceDownButton.SetActive(false);
        faceUpButton.SetActive(true);
    }

    // called by button
    public void PlayCard()
    {
        var card = focusedCard.GetCardData();
        var isFaceDown = this.isFaceDown;
        var guardianStar = selector.GetSelectedGuardianStar();

        GameplayManager.Instance().ToFieldPhase(card, isFaceDown, guardianStar);
        handCardReferenceFromHand.GetContainer().RemoveCard(alsoDestroy: true);

        Hide();

    }

    public bool IsMonster() => isMonster;

}
