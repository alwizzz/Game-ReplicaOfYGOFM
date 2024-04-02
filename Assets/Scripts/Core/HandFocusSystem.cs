using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HandFocusSystem : UIModal<HandFocusSystem>
{
    [SerializeField] private HandCard focusedCard;
    [SerializeField] private bool isFaceDown;

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

        if(cardData.IsMonsterCard())
        {
            var data = (MonsterCard)cardData;
            selector.gameObject.SetActive(true);
            selector.Setup(data.guardianStarOption1, data.guardianStarOption2);
            FieldSystem.Instance().OpenFrontRankSelection();
        } else
        {
            selector.gameObject.SetActive(false);
            FieldSystem.Instance().OpenBackRankSelection();
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

    public void PlayCard()
    {

    }


    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
