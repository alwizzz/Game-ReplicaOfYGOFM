using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;
using UnityEngine.Assertions;
using System;

public class FieldSystem : MonoBehaviour
{
    [SerializeField] private Side owner;
    [SerializeField] private FieldCard fieldCardPrefab;

    [Header("States")]
    [SerializeField] private bool isOnSelection;
    [SerializeField] private bool isOnBattleMode;
    [SerializeField] private bool isOnSpellMode;
    [SerializeField] private FieldCard selectedSpellFieldCard;
    
    [SerializeField] private int frontRankCardCount;
    [SerializeField] private int backRankCardCount;
    [SerializeField] private FieldCardContainer selectedFieldCardContainer;

    [Header("Caches")]
    [SerializeField] private List<FieldCardContainer> frontRankFieldCardContainers;
    [SerializeField] private List<FieldCardContainer> backRankFieldCardContainers;
    [SerializeField] private GameObject frontRankOverlay;
    [SerializeField] private GameObject backRankOverlay;

    [SerializeField] private GameObject fieldSelector;
    //[SerializeField] private GameObject secondaryFieldSelector; //TODO on further logic like equip

    [SerializeField] private CardInformationDisplay cardInformationDisplay;
    //[SerializeField] private GameObject fieldPhaseButtons; //TODO: abstract this out of FieldSystem

    private void Awake()
    {
        Setup();
    }

    private void Setup()
    {
        frontRankFieldCardContainers.ForEach(e => e.Setup(this));
        backRankFieldCardContainers.ForEach(e => e.Setup(this));
    }

    private void Start()
    {
        CloseSelection();
    }

    public void SetSelectedCardContainer(FieldCardContainer fieldCardContainer)
    {
        if (selectedFieldCardContainer != null)
        {
            selectedFieldCardContainer.Unselect();
        }

        selectedFieldCardContainer = fieldCardContainer;
        UpdateFieldSelector();

        bool isEnemy = owner == Side.Enemy;
        bool isFaceDown = selectedFieldCardContainer.IsEmpty() ? false : selectedFieldCardContainer.GetCard().IsFaceDown();
        if(
            selectedFieldCardContainer.IsEmpty()
            ||
            (
                isEnemy && isFaceDown
            )
        ){
            UpdateInformationDisplay(reset:true);
        } else
        {
            UpdateInformationDisplay();
        }
    }

    private void UpdateFieldSelector(bool toOffscreen = false)
    {
        if(toOffscreen)
        {
            GameplayManager.Instance().MoveToOffscreenParking(fieldSelector.transform);
            return;
        }


        selectedFieldCardContainer.MovePositionOnContainer(fieldSelector.transform);
    }

    public void UpdateInformationDisplay(bool reset = false)
    {
        if(reset)
        {
            cardInformationDisplay.ResetInformation();
            return;
        }

        cardInformationDisplay.UpdateInformation(selectedFieldCardContainer.GetCard());
    }

    public void ResetSelection()
    {
        if (selectedFieldCardContainer != null)
        {
            selectedFieldCardContainer.Unselect();
        }

        selectedFieldCardContainer = null;
        UpdateFieldSelector(toOffscreen:true);
        UpdateInformationDisplay(reset:true);
    }

    #region Selection Options

    public void OpenFullSelection(bool retainSelection = false)
    {
        isOnSelection = true; 

        if(IsPlayerOwned())
        {
            frontRankOverlay.SetActive(false);
            backRankOverlay.SetActive(false);
        }

        if (retainSelection)
        {
            FieldButtonManager.Instance().UpdateButtons(selectedFieldCardContainer);
        } else
        {
            var defaultFieldCardContainer = frontRankFieldCardContainers[0];
            SetSelectedCardContainer(defaultFieldCardContainer);
            FieldButtonManager.Instance().UpdateButtons(defaultFieldCardContainer);
        }

    }

    public void OpenFrontRankSelection(bool retainSelection = false)
    {
        print($"OpenFrontRankSelection on {owner}");
        isOnSelection = true;

        frontRankOverlay.SetActive(false);
        backRankOverlay.SetActive(true);
        // if (IsPlayerOwned())
        // {
        //     frontRankOverlay.SetActive(false);
        //     backRankOverlay.SetActive(true);
        // }

        if (retainSelection) return;
        SetSelectedCardContainer(frontRankFieldCardContainers[0]);
    }

    public void OpenBackRankSelection(bool retainSelection = false)
    {
        isOnSelection = true;

        if (IsPlayerOwned())
        {
            frontRankOverlay.SetActive(true);
            backRankOverlay.SetActive(false);
        }

        if (retainSelection) return;
        SetSelectedCardContainer(backRankFieldCardContainers[0]);
    }

    public void CloseSelection(bool retainSelection = false)
    {
        isOnSelection = false;
        frontRankOverlay.SetActive(true);
        backRankOverlay.SetActive(true);

        if (retainSelection) return;
        ResetSelection();
    }

    #endregion

    public void SpawnFieldCard(
        Card cardData, 
        bool isFacedown, 
        GuardianStar selectedGuardianStar, 
        List<GameplayCard.Modifier> modifierList,
        bool retainedMonster = false
    ){
        // currently commented cuz now there's scenario that the selection is closed but the selectedFieldCardContainer still used
        // so now, more determining variable is the selectedFieldCardContainer.IsEmpty()
        // if (!isOnSelection) return;

        if(selectedFieldCardContainer.IsEmpty() == false)
        {
            print("spawning field card on occupied field card container");
            // currently unable to spawn on occupied container
            // TODO: implement fusion/equip in this manner
            return;
        }

        var spawnedFieldCard = Instantiate(fieldCardPrefab);
        spawnedFieldCard.Setup(cardData);
        spawnedFieldCard.SetToAttackPosition(); // default when spawning
        if(isFacedown)
        {
            spawnedFieldCard.SetToFaceDown();
        } else
        {
            spawnedFieldCard.SetToFaceUp();
        }
        spawnedFieldCard.SetSelectedGuardianStar(selectedGuardianStar);
        spawnedFieldCard.SetModifierList(modifierList);
        spawnedFieldCard.SetHasBeenUsed(false);
        selectedFieldCardContainer.SetCard(spawnedFieldCard);
        UpdateInformationDisplay();

        IncrementCardCount(selectedFieldCardContainer.IsBackRank());

        if(cardData.IsMonsterCard() && !retainedMonster)
        {
            EventManager.MonsterSummoned(owner);
        }
    }

    private void IncrementCardCount(bool isBackRank)
    {
        if (isBackRank)
        {
            if (IsBackRankFull())
            {
                print("WARNING: attempt to increment back rank count when it is already full, aborting...");
                return;
            }
            backRankCardCount++;
        }
        else
        {
            if (IsFrontRankFull())
            {
                print("WARNING: attempt to increment front rank count when it is already full, aborting...");
                return;
            }
            frontRankCardCount++;
        }
    }

    public void DecrementCardCount(bool isBackRank)
    {
        if (isBackRank)
        {
            if(IsBackRankEmpty())
            {
                print("WARNING: attempt to decrement back rank count when it is already empty, aborting...");
                return;
            }
            backRankCardCount--;
        }
        else
        {
            if (IsFrontRankEmpty())
            {
                print("WARNING: attempt to decrement front rank count when it is already empty, aborting...");
                return;
            }
            frontRankCardCount--;
        }
    }

    public bool IsBackRankFull() => (backRankCardCount >= 5 ? true : false);
    public bool IsBackRankEmpty() => (backRankCardCount <= 0 ? true : false);
    public bool IsFrontRankFull() => (frontRankCardCount >= 5 ? true : false);
    public bool IsFrontRankEmpty() => (frontRankCardCount <= 0 ? true : false);


    private bool IsPlayerOwned() => (owner == Side.Player ? true : false);


    #region Field Phase

    public void StartFieldPhase()
    {
        OpenFullSelection(true);

        if (IsPlayerOwned() == false) return;
        //fieldPhaseButtons.SetActive(true);
        FieldButtonManager.Instance().Show();
        FieldButtonManager.Instance().UpdateBattleButtons(false);
        FieldButtonManager.Instance().UpdateSpellButtons(false);
    }

    public void EndTurn()
    {
        CloseSelection();
        GameplayManager.Instance().ToEndPhase();

        if (IsPlayerOwned() == false) return;
        //fieldPhaseButtons.SetActive(false);
        FieldButtonManager.Instance().Hide();
    }

    public void ChangeCardPosition()
    {
        if (selectedFieldCardContainer.IsBackRank()) return; // only front rank able to change position
        if (selectedFieldCardContainer.IsEmpty()) return;

        selectedFieldCardContainer.GetCard().ChangePosition();
        
    }

    public void UseFieldCard()
    {
        if(GameplayManager.Instance().IsInputLock()) { print("[input locked]"); return; }

        if (selectedFieldCardContainer.IsEmpty()) return;
        if (selectedFieldCardContainer.IsBackRank())
        {
            OpenSpellMode();

            // var fieldCard = selectedFieldCardContainer.GetCard();
            // var cardData = fieldCard.GetCardData();
            // if (cardData.IsMonsterCard()) return; // backrank card should be a NonMonsterCard

            // var nonMonsterCard = (NonMonsterCard)cardData;
            // if (!nonMonsterCard.IsSpellCard()) return; // do nothing if a trap card

            // var spellCard = (SpellCard)nonMonsterCard;
            // StartCoroutine(AnimateSpellActivation(fieldCard, spellCard));
        } else
        {
            // battle mode
            var fieldCard = selectedFieldCardContainer.GetCard();
            if (fieldCard.InAttackPosition() == false) return;

            OpenBattleMode();
        }
    }

    public void UseSpellCard()
    {
        var fieldCard = selectedSpellFieldCard;
        var cardData = fieldCard.GetCardData();
        if (cardData.IsMonsterCard()) return; // backrank card should be a NonMonsterCard

        var nonMonsterCard = (NonMonsterCard)cardData;
        if (!nonMonsterCard.IsSpellCard()) return; // do nothing if a trap card



        var spellCard = (SpellCard)nonMonsterCard;
        if(spellCard.GetSpellCardType() == SpellCardType.Equip)
        {
            FieldCard frontRankFieldCard = selectedFieldCardContainer.GetCard();
            StartCoroutine(AnimateEquipActivation(fieldCard, spellCard, frontRankFieldCard));
        } else
        {
            StartCoroutine(AnimateSpellActivation(fieldCard, spellCard));
        }


    }

    private IEnumerator AnimateSpellActivation(FieldCard fieldCard, SpellCard spellCard)
    {
        GameplayManager.Instance().SetInputLock(true);

        fieldCard.SetToFaceUp();
        bool succeed = spellCard.Activate();

        yield return new WaitForSeconds(1);
        fieldCard.Destroy();

        // yield return new WaitForSeconds(1);
        // fieldCard.Destroy();

        CancelSpellMode();
        GameplayManager.Instance().SetInputLock(false);
    }

    private IEnumerator AnimateEquipActivation(FieldCard fieldCard, SpellCard spellCard, FieldCard frontRankFieldCard)
    {
        if(frontRankFieldCard == null) throw new ArgumentNullException(nameof(frontRankFieldCard));

        GameplayManager.Instance().SetInputLock(true);

        fieldCard.SetToFaceUp();
        bool succeed = spellCard.Activate();

        yield return new WaitForSeconds(1);
        var modifierList = frontRankFieldCard.GetModifierList();
        var carriedGuardianStar = frontRankFieldCard.GetSelectedGuardianStar();
        // var container = frontRankFieldCard.GetContainer();

        fieldCard.Destroy();
        frontRankFieldCard.Destroy();
        print(modifierList);

        var list = new List<Card>
        {
            frontRankFieldCard.GetCardData(),
            (Card)spellCard
        };
        
        GameplayManager.Instance().FieldSystem().CloseSelection(retainSelection:true);
        FusionSystem.Instance().SetupForEquipOnFieldFlow(list, modifierList, carriedGuardianStar
            // () => {
            //     //////////////// continue here
            //     CancelSpellMode();
            //     GameplayManager.Instance().SetInputLock(false);
            // }   
        );
    }

    public void ResolveEquipActivation(Card cardData, List<GameplayCard.Modifier> modifierList, GuardianStar carriedGuardianStar)
    {
        // TODO: state checks
        // TODO: make a fake resolve panel like in HandFocusSytem's flow, for visual purpose only

        bool retainedMonster = true; // always true for this case
        SpawnFieldCard(cardData, false, carriedGuardianStar, modifierList, retainedMonster); // TODO: check if the fieldcardcontainer is correct
        // SpawnFieldCard(cardData, false, GuardianStar.NONE, modifierList); // debug GS with NONE

        CancelSpellMode();
        GameplayManager.Instance().SetInputLock(false);
    }

    private void OpenBattleMode()
    {
        print("BATTLE MODE");
        isOnBattleMode = true;
        selectedFieldCardContainer.SetAsAttackerInBattle();
        CloseSelection(retainSelection:true);

        GameplayManager.Instance().OpponentFieldSystem().OpenFrontRankSelection();
        FieldButtonManager.Instance().UpdateBattleButtons(true);
    }

    public void CancelBattleMode()
    {
        print("CANCEL BATTLE MODE");

        GameplayManager.Instance().OpponentFieldSystem().CloseSelection(false);
        OpenFullSelection(true);
        FieldButtonManager.Instance().UpdateBattleButtons(false);

        isOnBattleMode = false;
    }

    private void OpenSpellMode()
    {
        print("Spell MODE");
        isOnSpellMode = true;
        selectedSpellFieldCard = selectedFieldCardContainer.GetCard();
        // selectedFieldCardContainer.SetAsAttackerInBattle();
        // CloseSelection(retainSelection:true);

        GameplayManager.Instance().PlayerFieldSystem().OpenFrontRankSelection();
        FieldButtonManager.Instance().UpdateSpellButtons(true);
    }

    public void CancelSpellMode()
    {
        print("CANCEL Spell MODE");

        OpenFullSelection(true);
        FieldButtonManager.Instance().UpdateSpellButtons(false);

        selectedSpellFieldCard = null;
        isOnSpellMode = false;
    }

    #endregion

    public List<FieldCardContainer> GetFrontRankContainers() => frontRankFieldCardContainers;
    public List<FieldCardContainer> GetBackRankContainers() => backRankFieldCardContainers;
    public List<FieldCardContainer> GetAllContainers()
    {
        var result = new List<FieldCardContainer>(frontRankFieldCardContainers);
        result.AddRange(backRankFieldCardContainers);
        return result;
    }
    public FieldCardContainer GetSelectedFieldContainer() => selectedFieldCardContainer;
    public bool IsOnSpellMode() => isOnSpellMode;

    public bool HasNoMonster()
    {
       return frontRankFieldCardContainers.TrueForAll((e) => e.IsEmpty());
    }

    public void RefreshStatus()
    {
        print("refresh status on " + owner);
        // Refresh has been used status on field cards
        frontRankFieldCardContainers.ForEach(e =>
        {
            if(e.IsEmpty() == false)
            {
                e.GetCard().SetHasBeenUsed(false);
            }
        });
    }

    #region DEBUG

    public FieldCard DebugSpawnFieldCard(
        Card cardData, 
        bool isFacedown, 
        FieldCardContainer fieldCardContainer,
        List<GameplayCard.Modifier> modifierList = null
    ){
        //if (selectedFieldCardContainer.IsEmpty() == false)
        //{
        //    // currently unable to spawn on occupied container
        //    // TODO: implement fusion/equip in this manner
        //    return;
        //}

        var spawnedFieldCard = Instantiate(fieldCardPrefab);
        spawnedFieldCard.Setup(cardData, modifierList);

        if (cardData.IsMonsterCard())
        {
            spawnedFieldCard.SetToAttackPosition(); // default when spawning
            spawnedFieldCard.SetSelectedGuardianStar(((MonsterCard)cardData).guardianStarOption1);
            // spawnedFieldCard.SetSelectedGuardianStar(((MonsterCard)cardData).guardianStarOption2);
        } else
        {
            // 
        }
        if (isFacedown)
        {
            spawnedFieldCard.SetToFaceDown();
        }
        else
        {
            spawnedFieldCard.SetToFaceUp();
        }
        fieldCardContainer.SetCard(spawnedFieldCard);
        spawnedFieldCard.SetHasBeenUsed(false);

        IncrementCardCount(fieldCardContainer.IsBackRank());

        return spawnedFieldCard;

    }

    #endregion

}
