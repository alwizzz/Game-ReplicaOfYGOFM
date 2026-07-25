using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using Enums;
using TMPro;
using System;

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
    // [SerializeField] private List<HandCard> fusionListDisplay;
    // [SerializeField] private GameObject panelFusionFlow;
    // [SerializeField] private GameObject returnButton;
    // [SerializeField] private GameObject fuseButton;
    // [SerializeField] private TextMeshProUGUI fuseResultText;
    // [SerializeField] private GameplayCard.Modifier? cachedGameplayCardModifier;
    // [SerializeField] private GuardianStar? cachedGuardianStar;




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
            GameplayManager.Instance().FieldSystem().OpenFrontRankSelection();
        } else
        {
            isMonster = false;
            // GameplayManager.Instance().FieldSystem().OpenBackRankSelection();
            
            // non monster now do a full selection, to enable fusion flow to monster card, possibly for equip spell card
            GameplayManager.Instance().FieldSystem().OpenFullSelection();
        }

        panelSingleFlow.SetActive(true);
        // panelFusionFlow.SetActive(false);

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

        // GameplayManager.Instance().ToFieldPhase(card, isFaceDown, guardianStar);
        handCardReferenceFromHand.GetContainer().RemoveCard(alsoDestroy: true);

        // TODO: logic to switch to Fusion flow
        FieldCardContainer selectedFieldContainer = GameplayManager.Instance().FieldSystem().GetSelectedFieldContainer();
        FieldCard selectedFieldCard = selectedFieldContainer.GetCard();
        if(selectedFieldCard != null)
        {
            // NOTE: isFaceDown becomes irrelevant as fusion flow always set every card to face up
            SwitchFromSingleFlowToFusionFlow(card, selectedFieldCard);
        } else
        {
        if(!isMonster && isFaceDown && !selectedFieldContainer.IsBackRank())
            {
                // force to face up, as nonmonster cant be facedown in front rank
                isFaceDown = false;
            }
            Resolve(card, isFaceDown);
        }

        // Hide();
        
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

        fusionListHandReference = list;

        panelSingleFlow.SetActive(false);
        // panelFusionFlow.SetActive(true);

        isOnResolve = false;
        panelResolve.SetActive(false);

        Action returnButtonCallback = () => ReturnToHand();
        Action fuseButtonCallback = () => // basically Fuse() but only the HandFocusSytem's part
        { 
            // destroy reference on hand
            fusionListHandReference.ForEach(e => e.GetContainer().RemoveCard(alsoDestroy: true));

            FieldCardContainer selectedFieldContainer = GameplayManager.Instance().FieldSystem().GetSelectedFieldContainer();
            FieldCard selectedFieldCard = selectedFieldContainer.GetCard();
            if(selectedFieldCard != null)
            {
                var carriedGuardianStar = selectedFieldCard.GetSelectedGuardianStar();
                Card cardData = selectedFieldCard.GetCardData();
                var modifierList = selectedFieldCard.GetModifierList();

                // TODO: swap the order of below two lines, arguably for better readability
                FusionSystem.Instance().AppendFirstIndexFusionMaterial(cardData, modifierList, carriedGuardianStar);
                selectedFieldCard.Destroy();


                // fusionListData.Insert(0, cardData);
                // UpdateFusionDisplay(modifierList);
                ///// selectedFieldCard.Destroy();

                // Helpers.Instance().DelayedAction(1f, () => StartCoroutine(RunFusion(modifierList)));
            } else
            {
                // StartCoroutine(RunFusion());
                FusionSystem.Instance().ExternalRunFusion();
            }
        };
        FusionSystem.Instance().SetupForFusionFlow(list, returnButtonCallback, fuseButtonCallback);
        Show();

        // if(possession == Side.Player) // just to mark it
        // {
        //     // panelFusionFlow.GetComponent<Image>().color = Color.blue;
        // }

        // fuseResultText.gameObject.SetActive(false);
        // returnButton.SetActive(true);
        // fuseButton.SetActive(true);

        // fusionListHandReference = list;
        // fusionListHandReference.ForEach(e => fusionListData.Add(e.GetCardData()));
        // UpdateFusionDisplay();

        // Show();
    }

    // private void UpdateFusionDisplay(List<GameplayCard.Modifier> modifierListOnFirstIndex = null)
    // {
    //     for(int i=0; i<fusionListDisplay.Count; i++)
    //     {
    //         if(i >= fusionListData.Count)
    //         {
    //             fusionListDisplay[i].gameObject.SetActive(false);
    //         } else
    //         {
    //             if(i==0 && modifierListOnFirstIndex != null)
    //             {
    //                 fusionListDisplay[i].Setup(fusionListData[i], modifierListOnFirstIndex);
    //             } else
    //             {
    //                 fusionListDisplay[i].Setup(fusionListData[i]);
    //             }
    //             fusionListDisplay[i].gameObject.SetActive(true);
    //         }
    //     }
    // }

    // public void Fuse() // called by button
    // {
    //     // destroy reference on hand
    //     fusionListHandReference.ForEach(e => e.GetContainer().RemoveCard(alsoDestroy: true));

    //     returnButton.SetActive(false);
    //     fuseButton.SetActive(false);

    //     FieldCardContainer selectedFieldContainer = GameplayManager.Instance().FieldSystem().GetSelectedFieldContainer();
    //     FieldCard selectedFieldCard = selectedFieldContainer.GetCard();
    //     if(selectedFieldCard != null)
    //     {
    //         // cachedGameplayCardModifier = selectedFieldCard.GetModifier();
    //         cachedGuardianStar = selectedFieldCard.GetSelectedGuardianStar();
    //         Card cardData = selectedFieldCard.GetCardData();
    //         var modifierList = selectedFieldCard.GetModifierList();

    //         fusionListData.Insert(0, cardData);
    //         // UpdateFusionDisplay();
    //         UpdateFusionDisplay(modifierList);
    //         selectedFieldCard.Destroy();

    //         Helpers.Instance().DelayedAction(1f, () => StartCoroutine(RunFusion(modifierList)));
    //     } else
    //     {
    //         StartCoroutine(RunFusion());
    //     }

    // }

    // private IEnumerator RunFusion(List<GameplayCard.Modifier> firstModifierList = null)
    // {
    //     fuseResultText.gameObject.SetActive(true);

    //     bool retainFirstMonster = true;
    //     List<GameplayCard.Modifier> modifierList = firstModifierList;

    //     while (fusionListData.Count > 1)
    //     {
    //         Card material1 = fusionListData[0];
    //         Card material2 = fusionListData[1];

    //         FusionCalculator.FusionResult result = FusionCalculator.GetFusionResult(material1, material2);

    //         if(result.type == FusionResultType.Rejected)
    //         {
    //             fuseResultText.text = "Rejected";
    //             if (result.retainMonster)
    //             {
    //                 // carried over   
    //             } else
    //             {
    //                 modifierList = null;
    //             }
    //         } else if(result.type == FusionResultType.Fused)
    //         {
    //             fuseResultText.text = "Fused";
    //             modifierList = null;
    //         } else if(result.type == FusionResultType.Equipped)
    //         {
    //             fuseResultText.text = "Equipped";
    //             if(!result.modifier.HasValue)
    //             {
    //                 print("WARN: a modifier should be added but the data is null");
    //             } else
    //             {
    //                 // carried over and also appended
    //                 if(modifierList == null) modifierList = new List<GameplayCard.Modifier>();
    //                 modifierList.Add(result.modifier.Value);
    //             }
    //         }

    //         if(retainFirstMonster == true && result.retainMonster != true)
    //         {
    //             retainFirstMonster = false;
    //         }

    //         fusionListData[0] = result.card; // change first index as fusion result
    //         fusionListData.RemoveAt(1); // exhaust index-1 as it was the material2
    //         UpdateFusionDisplay(modifierList); // update display

    //         yield return new WaitForSeconds(1f);
    //     }

    //     bool retainModification = false;
    //     // if(cachedGameplayCardModifier != null && retainFirstMonster == true)
    //     if(cachedGuardianStar != null && retainFirstMonster == true)
    //     {
    //         retainModification = true;
    //     }

    //     fuseResultText.gameObject.SetActive(false);
    //     yield return new WaitForSeconds(1f);

    //     Card resultCard = fusionListData[0];
    //     isMonster = resultCard.IsMonsterCard();
    //     Resolve(resultCard, false, retainModification:retainModification, modifierList); // fusion result is always face up
    // }

    private void SwitchFromSingleFlowToFusionFlow(Card cardFromHand, FieldCard selectedFieldCard)
    {   
        // UI handling
        panelSingleFlow.SetActive(false);
        Show();
        // panelFusionFlow.SetActive(true);
        // if(possession == Side.Player) // just to mark it
        // {
        //     // panelFusionFlow.GetComponent<Image>().color = Color.blue;
        // }
        // returnButton.SetActive(false);
        // fuseButton.SetActive(false);

        // data handling

        // cachedGameplayCardModifier = selectedFieldCard.GetModifier();
        var carriedGuardianStar = selectedFieldCard.GetSelectedGuardianStar();
        Card cardFromField = selectedFieldCard.GetCardData();
        var modifierList = selectedFieldCard.GetModifierList();

        // fusionListData.Insert(0, cardFromField);
        // fusionListData.Insert(1, cardFromHand);
        List<Card> list = new List<Card>{ cardFromField, cardFromHand };
        FusionSystem.Instance().SetupForSwitchFlow(list, modifierList, carriedGuardianStar);

        // UpdateFusionDisplay(modifierList);
        selectedFieldCard.Destroy(); // NOTE: cardFromHand's HandCard has been destroyed on Single Flow logic

        // Helpers.Instance().DelayedAction(1f, () => StartCoroutine(RunFusion(modifierList)));
    }

#endregion


#region Resolve flow

    private void Resolve(
        Card cardData, 
        bool isFaceDown, 
        bool retainModification = false, 
        List<GameplayCard.Modifier> modifierList = null,
        GuardianStar? carriedGuardianStar = null)
    {
        // NOTE: retainModification is only from fusion flow
        if(retainModification == true && carriedGuardianStar == null)
        {
            // this bool is initially for internal fusion flow. After the flow is separated, the "checking" is kinda reversed
            retainModification = false;
        }

        if(isOnResolve) return;
        isOnResolve = true;
        panelResolve.SetActive(true);

        if(possession == Side.Player) // just to mark it
        {
            panelResolve.GetComponent<Image>().color = Color.yellow;
        }

        panelSingleFlow.SetActive(false);
        // panelFusionFlow.SetActive(false);

        // setup
        resolvedHandCard.Setup(cardData, modifierList);
        isFaceDownResolve = isFaceDown;
        if(isFaceDown) faceDownCardImageResolve.SetActive(true);
        else faceDownCardImageResolve.SetActive(false);

        // a way to show that in this flow it is no more time for choosing the field card container
        GameplayManager.Instance().FieldSystem().CloseSelection(retainSelection:true);

        isMonster = cardData.IsMonsterCard(); // new, quick refresh
        if (isMonster)
        {
            if (retainModification)
            {
                // if(cachedGameplayCardModifier == null)
                if(carriedGuardianStar == null)
                {
                    print($"WARN: attempt to retainModification but carriedGuardianStar is null, fallbacked");

                    // fallback
                    var data = (MonsterCard)cardData;
                    selector.Setup(data.guardianStarOption1, data.guardianStarOption2);
                    proceedButton.SetActive(true);
                } else
                {
                    var data = (MonsterCard)cardData;
                    selector.Setup(data.guardianStarOption1, data.guardianStarOption2);

                    // var carriedGuardianStar = cachedGameplayCardModifier?.selectedGuardianStar;
                    if(carriedGuardianStar == data.guardianStarOption1)
                    {
                        selector.gameObject.SetActive(false);
                        proceedButton.SetActive(false);

                        selector.SelectOption1(); // ghost setup
                        Helpers.Instance().DelayedAction(1f, () => Proceed());
                    } else if(carriedGuardianStar == data.guardianStarOption2)
                    {
                        selector.gameObject.SetActive(false);
                        proceedButton.SetActive(false);

                        selector.SelectOption2(); // ghost setup
                        Helpers.Instance().DelayedAction(1f, () => Proceed());
                    } else
                    {
                        print($"WARN: attempt to retain GS but cache's GS ({carriedGuardianStar}) doesnt match with data, fallbacked");
                        // fallback
                        proceedButton.SetActive(true);
                    }
                }
            } else
            {
                var data = (MonsterCard)cardData;
                selector.Setup(data.guardianStarOption1, data.guardianStarOption2);
                proceedButton.SetActive(true);
            }
        } else
        {
            selector.gameObject.SetActive(false);
            proceedButton.SetActive(false);

            Helpers.Instance().DelayedAction(2f, () => Proceed());
        }
    }

    public void ExternalResolve(
        Card cardData, 
        bool isFaceDown, 
        bool retainModification = false, 
        List<GameplayCard.Modifier> modifierList = null,
        GuardianStar? carriedGuardianStar = null
    ){
        Resolve(cardData, isFaceDown, retainModification, modifierList, carriedGuardianStar);
    }

    public void Proceed() // either called by button or autocalled
    {
        if(!isOnResolve) return;

        var card = resolvedHandCard.GetCardData();
        var modifierList = resolvedHandCard.GetModifierList();
        var isFaceDown = isFaceDownResolve;

        GuardianStar guardianStar;
        if (card.IsMonsterCard())
        {
            guardianStar = selector.GetSelectedGuardianStar();
        } else
        {
            guardianStar = GuardianStar.NONE;
        }


        GameplayManager.Instance().ToFieldPhase(card, isFaceDown, guardianStar, modifierList);

        // cleanup
        // fusionListData.Clear();
        // FusionSystem.Instance().Clear(); // done independently
        // cachedGuardianStar = null;
        Hide();
    }

#endregion

    public bool IsMonster() => isMonster;

}
