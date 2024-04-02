using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSystem : StaticReference<FieldSystem>
{
    [SerializeField] private bool isOnSelection;

    [Header("States")]
    [SerializeField] private FieldCard fieldCardPrefab;
    [SerializeField] private FieldCardContainer selectedFieldCardContainer;

    [Header("Caches")]
    [SerializeField] private GameObject frontRankOverlay;
    [SerializeField] private GameObject backRankOverlay;

    [SerializeField] private Transform offscreenParking;
    [SerializeField] private List<FieldCardContainer> frontRankFieldCardContainers;
    [SerializeField] private List<FieldCardContainer> backRankFieldCardContainers;

    //[SerializeField] private List<GameplayCardUI> fieldCards;
    [SerializeField] private GameObject fieldSelector;
    //[SerializeField] private GameObject secondaryFieldSelector; //TODO on further logic like equip

    [SerializeField] private CardInformationDisplay cardInformationDisplay;



    private void Awake()
    {
        BaseAwake(this);
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
            fieldSelector.transform.position = offscreenParking.position;
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

    public void OpenFullSelection()
    {
        isOnSelection = true;

        frontRankOverlay.SetActive(false);
        backRankOverlay.SetActive(false);

        SetSelectedCardContainer(frontRankFieldCardContainers[0]);
    }

    public void OpenFrontRankSelection()
    {
        isOnSelection = true;

        frontRankOverlay.SetActive(false);
        backRankOverlay.SetActive(true);

        SetSelectedCardContainer(frontRankFieldCardContainers[0]);
    }

    public void OpenBackRankSelection()
    {
        isOnSelection = true;

        frontRankOverlay.SetActive(true);
        backRankOverlay.SetActive(false);

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
        spawnedFieldCard.SetToAttackMode(); // default when spawning
        if(isFacedown)
        {
            spawnedFieldCard.SetToFaceDown();
        } else
        {
            spawnedFieldCard.SetToFaceUp();
        }
        selectedFieldCardContainer.SetCard(spawnedFieldCard);
    }

    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
