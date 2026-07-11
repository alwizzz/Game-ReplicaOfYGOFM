using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using Enums;
using TMPro;

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
    [SerializeField] private List<Card> fusionListData;
    [SerializeField] private List<HandCard> fusionListHandReference;
    [SerializeField] private List<HandCard> fusionListDisplay;
    [SerializeField] private GameObject panelFusionFlow;
    [SerializeField] private GameObject returnButton;
    [SerializeField] private GameObject fuseButton;
    [SerializeField] private TextMeshProUGUI fuseResultText;




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

    public void SetupFusionFlow(List<HandCard> list)
    {
        // fuse flow always assume the end result is monster card
        GameplayManager.Instance().FieldSystem().OpenFrontRankSelection();

        panelSingleFlow.SetActive(false);
        panelFusionFlow.SetActive(true);

        isOnResolve = false;
        panelResolve.SetActive(false);

        if(possession == Side.Player) // just to mark it
        {
            panelFusionFlow.GetComponent<Image>().color = Color.blue;
        }

        fuseResultText.gameObject.SetActive(false);
        returnButton.SetActive(true);
        fuseButton.SetActive(true);

        fusionListHandReference = list;
        fusionListHandReference.ForEach(e => fusionListData.Add(e.GetCardData()));
        SetupFusionDisplay();

        Show();
    }

    private void SetupFusionDisplay()
    {
        for(int i=0; i<fusionListDisplay.Count; i++)
        {
            if(i >= fusionListData.Count)
            {
                fusionListDisplay[i].gameObject.SetActive(false);
            } else
            {
                fusionListDisplay[i].Setup(fusionListData[i]);
                fusionListDisplay[i].gameObject.SetActive(true);
            }
        }
    }

    public void Fuse() // called by button
    {
        // TODO: later on if the selected field card container has card, it means 
        // we are adding a card on fusionList's first idx

        // destroy reference on hand
        fusionListHandReference.ForEach(e => e.GetContainer().RemoveCard(alsoDestroy: true));

        returnButton.SetActive(false);
        fuseButton.SetActive(false);

        StartCoroutine(AnimateFusion());
    }

    private IEnumerator AnimateFusion()
    {
        yield return null;
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

        // a way to show that in this flow it is no more time for choosing the field card container
        GameplayManager.Instance().FieldSystem().CloseSelection(maintainSelection:true);

        // TODO: check if cardData alr got GS selected, then dont need to choose again
        if (isMonster)
        {
            var data = (MonsterCard)cardData;
            selector.Setup(data.guardianStarOption1, data.guardianStarOption2);
            if (true)
            {
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
