using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using Enums;

public class HandFocusSystem : UIModal<HandFocusSystem>
{
    [SerializeField] private Side possession;
    [SerializeField] private bool isOnResolve;
    [SerializeField] private bool isMonster;

    [Header("Segment Single Flow")]
    [SerializeField] private bool isFaceDown; // because fusion flow will always be face up
    [SerializeField] private HandCard focusedCard;
    [SerializeField] private HandCard handCardReferenceFromHand;
    [SerializeField] private GameObject panelSingleFlow;
    [SerializeField] private GameObject faceDownCardImage; 
    [SerializeField] private GameObject faceDownButton; 
    [SerializeField] private GameObject faceUpButton; 

    [Header("Segment Fusion Flow")]
    [SerializeField] private GameObject panelFusionFlow;
    [Header("Segment Resolve")]
    [SerializeField] private bool isFaceDownResolve;
    [SerializeField] private GameObject panelResolve;
    [SerializeField] private HandCard resolvedHandCard;
    [SerializeField] private GuardianStarOptionSelector selector;
    [SerializeField] private GameObject proceedButton; 
    [SerializeField] private GameObject faceDownCardImageResolve; 


    private void Awake()
    {
        BaseAwake(this);
    }



#region Single Flow
    public void SetupSingleFlow(HandCard handCard)
    {
        var cardData = handCard.GetCardData();
        focusedCard.Setup(cardData);
        handCardReferenceFromHand = handCard;

        if(cardData.IsMonsterCard())
        {
            isMonster = true;
            var data = (MonsterCard)cardData;
            // selector.gameObject.SetActive(true);
            // selector.Setup(data.guardianStarOption1, data.guardianStarOption2);
            GameplayManager.Instance().FieldSystem().OpenFrontRankSelection();
        } else
        {
            isMonster = false;
            // selector.gameObject.SetActive(false);
            GameplayManager.Instance().FieldSystem().OpenBackRankSelection();
        }

        panelSingleFlow.SetActive(true);
        panelFusionFlow.SetActive(false);

        isOnResolve = false;
        panelResolve.SetActive(false);

        if(possession == Side.Player) // just to mark it
        {
            panelSingleFlow.GetComponent<Image>().color = Color.green;
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
        // var guardianStar = selector.GetSelectedGuardianStar();

        // GameplayManager.Instance().ToFieldPhase(card, isFaceDown, guardianStar);
        handCardReferenceFromHand.GetContainer().RemoveCard(alsoDestroy: true);

        // Hide();
        Resolve(card, isFaceDown);
    }

    public void ReturnToHand()
    {
        Hide();
        GameplayManager.Instance().HandSystem().Show();
        GameplayManager.Instance().FieldSystem().CloseSelection();
    }
#endregion

#region Fusion Flow

    public void SetupFusionFlow(List<HandCard> fusionList)
    {
        // var cardData = handCard.GetCardData();
        // focusedCard.Setup(cardData);
        // handCardReferenceFromHand = handCard;

        // if(cardData.IsMonsterCard())
        // {
        //     isMonster = true;
        //     var data = (MonsterCard)cardData;
        //     selector.gameObject.SetActive(true);
        //     selector.Setup(data.guardianStarOption1, data.guardianStarOption2);
        //     GameplayManager.Instance().FieldSystem().OpenFrontRankSelection();
        // } else
        // {
        //     isMonster = false;
        //     selector.gameObject.SetActive(false);
        //     GameplayManager.Instance().FieldSystem().OpenBackRankSelection();
        // }

        panelSingleFlow.SetActive(false);
        panelFusionFlow.SetActive(true);

        isOnResolve = false;
        panelResolve.SetActive(false);

        if(possession == Side.Player) // just to mark it
        {
            panelFusionFlow.GetComponent<Image>().color = Color.blue;
        }

        Show();
    }

#endregion


#region Resolve flow

    private void Resolve(Card cardData, bool isFaceDown)
    {
        if(isOnResolve) return;
        isOnResolve = true;
        panelResolve.SetActive(true);

        if(possession == Side.Player) // just to mark it
        {
            panelResolve.GetComponent<Image>().color = Color.yellow;
        }

        panelSingleFlow.SetActive(false);
        panelFusionFlow.SetActive(false);

        // setup
        resolvedHandCard.Setup(cardData);
        isFaceDownResolve = isFaceDown;
        if(isFaceDown) faceDownCardImageResolve.SetActive(true);
        else faceDownCardImageResolve.SetActive(false);

        // TODO: check if cardData alr got GS selected, then dont need to choose again
        if (isMonster)
        {
            if (true)
            {
                var data = (MonsterCard)cardData;
                selector.Setup(data.guardianStarOption1, data.guardianStarOption2);
                selector.gameObject.SetActive(true);
                proceedButton.SetActive(true);
            } else
            {
                // selector.gameObject.SetActive(true);
                // proceedButton.SetActive(true);
            }
        }
    }

    public void Proceed() // either called by button or autocalled
    {
        if(!isOnResolve) return;

        var card = resolvedHandCard.GetCardData();
        var isFaceDown = isFaceDownResolve;
        var guardianStar = selector.GetSelectedGuardianStar();

        GameplayManager.Instance().ToFieldPhase(card, isFaceDown, guardianStar);

        Hide();
    }

#endregion

    public bool IsMonster() => isMonster;

}
