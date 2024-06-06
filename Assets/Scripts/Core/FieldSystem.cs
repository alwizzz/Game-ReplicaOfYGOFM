using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;

public class FieldSystem : MonoBehaviour
{
    [SerializeField] private Side owner;
    [SerializeField] private FieldCard fieldCardPrefab;

    [Header("States")]
    [SerializeField] private bool isOnSelection;
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

        // TODO: manage hiding info on opponent's facedown card
        if(selectedFieldCardContainer.IsEmpty())
        {
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

        cardInformationDisplay.UpdateInformation(selectedFieldCardContainer.GetCard()); ;
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

    public void OpenFullSelection(bool maintainSelection = false)
    {
        isOnSelection = true; 

        if(IsPlayerOwned())
        {
            frontRankOverlay.SetActive(false);
            backRankOverlay.SetActive(false);
        }

        if (maintainSelection)
        {
            FieldButtonManager.Instance().UpdateButtons(selectedFieldCardContainer);
        } else
        {
            var defaultFieldCardContainer = frontRankFieldCardContainers[0];
            SetSelectedCardContainer(defaultFieldCardContainer);
            FieldButtonManager.Instance().UpdateButtons(defaultFieldCardContainer);
        }

    }

    public void OpenFrontRankSelection(bool maintainSelection = false)
    {
        print($"OpenFrontRankSelection on {owner}");
        isOnSelection = true;

        if (IsPlayerOwned())
        {
            frontRankOverlay.SetActive(false);
            backRankOverlay.SetActive(true);
        }

        if (maintainSelection) return;
        SetSelectedCardContainer(frontRankFieldCardContainers[0]);
    }

    public void OpenBackRankSelection(bool maintainSelection = false)
    {
        isOnSelection = true;

        if (IsPlayerOwned())
        {
            frontRankOverlay.SetActive(true);
            backRankOverlay.SetActive(false);
        }

        if (maintainSelection) return;
        SetSelectedCardContainer(backRankFieldCardContainers[0]);
    }

    public void CloseSelection(bool maintainSelection = false)
    {
        isOnSelection = false;
        frontRankOverlay.SetActive(true);
        backRankOverlay.SetActive(true);

        if (maintainSelection) return;
        ResetSelection();
    }

    #endregion

    public void SpawnFieldCard(Card cardData, bool isFacedown, GuardianStar selectedGuardianStar)
    {
        if (!isOnSelection) return;

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
        spawnedFieldCard.SetHasBeenUsed(false);
        selectedFieldCardContainer.SetCard(spawnedFieldCard);
        UpdateInformationDisplay();

        IncrementCardCount(selectedFieldCardContainer.IsBackRank());
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
        if (selectedFieldCardContainer.IsEmpty()) return;
        if (selectedFieldCardContainer.IsBackRank())
        {
            var cardData = selectedFieldCardContainer.GetCard().GetCardData();
            if (cardData.IsMonsterCard()) return; // backrank card should have be a NonMonsterCard

            var nonMonsterCard = (NonMonsterCard)cardData;
            if (!nonMonsterCard.IsSpellCard()) return; // do nothing if a trap card

            var spellCard = (SpellCard)nonMonsterCard;
            spellCard.Activate();
        } else
        {
            // battle mode
            var fieldCard = selectedFieldCardContainer.GetCard();
            if (fieldCard.InAttackPosition() == false) return;

            OpenBattleMode();
        }
    }

    private void OpenBattleMode()
    {
        print("BATTLE MODE");
        selectedFieldCardContainer.SetAsAttackerInBattle();
        CloseSelection(maintainSelection:true);

        GameplayManager.Instance().OpponentFieldSystem().OpenFrontRankSelection();
        FieldButtonManager.Instance().UpdateBattleButtons(true);
    }

    public void CancelBattleMode()
    {
        print("CANCEL BATTLE MODE");

        GameplayManager.Instance().OpponentFieldSystem().CloseSelection(false);
        OpenFullSelection(true);
        FieldButtonManager.Instance().UpdateBattleButtons(false);
    }

    #endregion

    public List<FieldCardContainer> GetFrontRankContainers() => frontRankFieldCardContainers;
    public List<FieldCardContainer> GetBackRankContainers() => backRankFieldCardContainers;
    public FieldCardContainer GetSelectedFieldContainer() => selectedFieldCardContainer;

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

    public FieldCard DebugSpawnFieldCard(Card cardData, bool isFacedown, FieldCardContainer fieldCardContainer)
    {
        //if (selectedFieldCardContainer.IsEmpty() == false)
        //{
        //    // currently unable to spawn on occupied container
        //    // TODO: implement fusion/equip in this manner
        //    return;
        //}

        var spawnedFieldCard = Instantiate(fieldCardPrefab);
        spawnedFieldCard.Setup(cardData);
        spawnedFieldCard.SetToAttackPosition(); // default when spawning
        if (isFacedown)
        {
            spawnedFieldCard.SetToFaceDown();
        }
        else
        {
            spawnedFieldCard.SetToFaceUp();
        }
        spawnedFieldCard.SetSelectedGuardianStar(((MonsterCard)cardData).guardianStarOption1);
        fieldCardContainer.SetCard(spawnedFieldCard);
        spawnedFieldCard.SetHasBeenUsed(false);

        IncrementCardCount(fieldCardContainer.IsBackRank());

        return spawnedFieldCard;

    }

    #endregion

}
