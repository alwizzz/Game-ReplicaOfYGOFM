using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSystem : MonoBehaviour
{
    [SerializeField] private bool isOnSelection;

    [Header("States")]
    [SerializeField] private FieldCard fieldCardPrefab;
    [SerializeField] private FieldCardContainer selectedFieldCardContainer;

    [Header("Caches")]
    [SerializeField] private List<FieldCardContainer> frontRankFieldCardContainers;
    [SerializeField] private List<FieldCardContainer> backRankFieldCardContainers;
    [SerializeField] private GameObject frontRankOverlay;
    [SerializeField] private GameObject backRankOverlay;

    [SerializeField] private GameObject fieldSelector;
    //[SerializeField] private GameObject secondaryFieldSelector; //TODO on further logic like equip

    [SerializeField] private CardInformationDisplay cardInformationDisplay;
    [SerializeField] private GameObject fieldPhaseButtons;



    private void Awake()
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

    private void UpdateInformationDisplay(bool reset = false)
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

    public void OpenFullSelection(bool maintainSelection = false)
    {
        isOnSelection = true;

        frontRankOverlay.SetActive(false);
        backRankOverlay.SetActive(false);

        if (maintainSelection) return;
        SetSelectedCardContainer(frontRankFieldCardContainers[0]);
    }

    public void OpenFrontRankSelection(bool maintainSelection = false)
    {
        isOnSelection = true;

        frontRankOverlay.SetActive(false);
        backRankOverlay.SetActive(true);

        if (maintainSelection) return;
        SetSelectedCardContainer(frontRankFieldCardContainers[0]);
    }

    public void OpenBackRankSelection(bool maintainSelection = false)
    {
        isOnSelection = true;

        frontRankOverlay.SetActive(true);
        backRankOverlay.SetActive(false);

        if (maintainSelection) return;
        SetSelectedCardContainer(backRankFieldCardContainers[0]);
    }

    public void CloseSelection()
    {
        isOnSelection = false;
        frontRankOverlay.SetActive(true);
        backRankOverlay.SetActive(true);

        ResetSelection();
    }

    public void SpawnFieldCard(Card cardData, bool isFacedown)
    {
        if (!isOnSelection) return;

        if(selectedFieldCardContainer.IsEmpty() == false)
        {
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
        selectedFieldCardContainer.SetCard(spawnedFieldCard);
    }

    #region Field Phase

    public void StartFieldPhase()
    {
        OpenFullSelection(true);
        fieldPhaseButtons.SetActive(true);
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
            print("BATTLE MODE");
        }
    }

    #endregion

}
