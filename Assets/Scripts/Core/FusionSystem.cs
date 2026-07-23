using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Enums;
using TMPro;
using System;

public class FusionSystem : UIModal<FusionSystem>
{
    [SerializeField] private bool isActive;
    [SerializeField] private List<Card> fusionListData;
    // [SerializeField] private List<HandCard> fusionListHandReference;
    [SerializeField] private List<HandCard> fusionListDisplay;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject returnButton;
    [SerializeField] private GameObject fuseButton;
    [SerializeField] private TextMeshProUGUI fuseResultText;
    // [SerializeField] private GameplayCard.Modifier? cachedGameplayCardModifier;
    // [SerializeField] private GuardianStar? cachedGuardianStar;

    private void Awake()
    {
        BaseAwake(this);
    }

    // TODO: currently the setup is niched to every use case. The ideal design would be more abstract
    public void SetupForFusionFlow(List<HandCard> list, Action returnButtonCallback, Action fuseButtonCallback)
    {
        if(isActive){ print("already active, setup denied"); return; }
        isActive = true;

        panel.SetActive(true);
        panel.GetComponent<Image>().color = Color.magenta;

        // new logic
        var returnButtonOnClick = returnButton.GetComponent<Button>().onClick;
        returnButtonOnClick.RemoveAllListeners();
        returnButtonOnClick.AddListener(() => returnButtonCallback?.Invoke());

        var fuseButtonOnClick = fuseButton.GetComponent<Button>().onClick;
        fuseButtonOnClick.RemoveAllListeners();
        fuseButtonOnClick.AddListener(() => {
            Fuse();
            fuseButtonCallback?.Invoke();
        });

        fuseResultText.gameObject.SetActive(false);
        returnButton.SetActive(true);
        fuseButton.SetActive(true);

        list.ForEach(e => fusionListData.Add(e.GetCardData()));
        UpdateFusionDisplay();

        Show();
    }

    public void SetupForSwitchFlow(List<Card> list, List<GameplayCard.Modifier> modifierList)
    {
        if(isActive){ print("already active, setup denied"); return; }
        isActive = true;

        panel.SetActive(true);
        panel.GetComponent<Image>().color = Color.magenta;

        fuseResultText.gameObject.SetActive(false);
        returnButton.SetActive(false);
        fuseButton.SetActive(false);

        // list.ForEach(e => fusionListData.Add(e.GetCardData()));
        list.ForEach(e => fusionListData.Add(e));
        UpdateFusionDisplay(modifierList);

        Show();

        Helpers.Instance().DelayedAction(1f, () => 
            StartCoroutine(RunFusion(modifierList, OnFinished)
        ));
    }

    public void AppendFirstIndexFusionMaterial(Card cardData, List<GameplayCard.Modifier> modifierList)
    {
        if(!isActive){ print("is inactive, aborted"); return; }

        fusionListData.Insert(0, cardData);
        UpdateFusionDisplay(modifierList);

        Helpers.Instance().DelayedAction(1f, () => 
            StartCoroutine(RunFusion(modifierList, OnFinished)
        ));
    }

    // public void Clear()
    private void Clear()
    {
        if(!isActive){ print("is inactive, Clear() denied"); return; }
        isActive = false;

        fusionListData.Clear();
        Hide();
    }

    // quick handling
    private static void OnFinished(Card cardData, bool retainModification, List<GameplayCard.Modifier> modifierList)
    {
        GameplayManager.Instance().HandFocusSystem().ExternalResolve(
            cardData, 
            false, // always face up
            retainModification, 
            modifierList
        );
    }


    private void UpdateFusionDisplay(List<GameplayCard.Modifier> modifierListOnFirstIndex = null)
    {
        for(int i=0; i<fusionListDisplay.Count; i++)
        {
            if(i >= fusionListData.Count)
            {
                fusionListDisplay[i].gameObject.SetActive(false);
            } else
            {
                if(i==0 && modifierListOnFirstIndex != null)
                {
                    fusionListDisplay[i].Setup(fusionListData[i], modifierListOnFirstIndex);
                } else
                {
                    fusionListDisplay[i].Setup(fusionListData[i]);
                }
                fusionListDisplay[i].gameObject.SetActive(true);
            }
        }
    }

    private void Fuse() // basically Fuse() but only the FusionSystem's part
    {
        // // destroy reference on hand
        // fusionListHandReference.ForEach(e => e.GetContainer().RemoveCard(alsoDestroy: true));

        returnButton.SetActive(false);
        fuseButton.SetActive(false);

        // FieldCardContainer selectedFieldContainer = GameplayManager.Instance().FieldSystem().GetSelectedFieldContainer();
        // FieldCard selectedFieldCard = selectedFieldContainer.GetCard();
        // if(selectedFieldCard != null)
        // {
        //     // cachedGameplayCardModifier = selectedFieldCard.GetModifier();
        //     cachedGuardianStar = selectedFieldCard.GetSelectedGuardianStar();
        //     Card cardData = selectedFieldCard.GetCardData();
        //     var modifierList = selectedFieldCard.GetModifierList();

        //     fusionListData.Insert(0, cardData);
        //     // UpdateFusionDisplay();
        //     UpdateFusionDisplay(modifierList);
        //     selectedFieldCard.Destroy();

        //     Helpers.Instance().DelayedAction(1f, () => StartCoroutine(RunFusion(modifierList)));
        // } else
        // {
        //     StartCoroutine(RunFusion());
        // }

    }

    // private IEnumerator RunFusion(List<GameplayCard.Modifier> firstModifierList = null)
    private IEnumerator RunFusion(
        List<GameplayCard.Modifier> firstModifierList = null, 
        Action<Card, bool, List<GameplayCard.Modifier>> OnFinishedCallback = null
    ){
        fuseResultText.gameObject.SetActive(true);

        bool retainFirstMonster = true;
        List<GameplayCard.Modifier> modifierList = firstModifierList;

        while (fusionListData.Count > 1)
        {
            Card material1 = fusionListData[0];
            Card material2 = fusionListData[1];

            FusionCalculator.FusionResult result = FusionCalculator.GetFusionResult(material1, material2);

            if(result.type == FusionResultType.Rejected)
            {
                fuseResultText.text = "Rejected";
                if (result.retainMonster)
                {
                    // carried over   
                } else
                {
                    modifierList = null;
                }
            } else if(result.type == FusionResultType.Fused)
            {
                fuseResultText.text = "Fused";
                modifierList = null;
            } else if(result.type == FusionResultType.Equipped)
            {
                fuseResultText.text = "Equipped";
                if(!result.modifier.HasValue)
                {
                    print("WARN: a modifier should be added but the data is null");
                } else
                {
                    // carried over and also appended
                    if(modifierList == null) modifierList = new List<GameplayCard.Modifier>();
                    modifierList.Add(result.modifier.Value);
                }
            }

            if(retainFirstMonster == true && result.retainMonster != true)
            {
                retainFirstMonster = false;
            }

            fusionListData[0] = result.card; // change first index as fusion result
            fusionListData.RemoveAt(1); // exhaust index-1 as it was the material2
            UpdateFusionDisplay(modifierList); // update display

            yield return new WaitForSeconds(1f);
        }

        bool retainModification = retainFirstMonster;

        // bool retainModification = false;
        // if(cachedGuardianStar != null && retainFirstMonster == true)
        // {
        //     retainModification = true;
        // }

        fuseResultText.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        Card resultCard = fusionListData[0];
        var isMonster = resultCard.IsMonsterCard();

        // quick handling to directly call HandFocusSytem's Resolve()
        // this.OnFinished(resultCard, retainModification, modifierList);

        OnFinishedCallback?.Invoke(resultCard, retainModification, modifierList);
        // Resolve(resultCard, false, retainModification:retainModification, modifierList); // fusion result is always face up
    
        Clear();
    }

    public void ExternalRunFusion(List<GameplayCard.Modifier> modifierList = null)
    {
        StartCoroutine(RunFusion(modifierList, OnFinished));
    }


    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
