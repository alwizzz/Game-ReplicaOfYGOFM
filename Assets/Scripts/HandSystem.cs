using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;


public class HandSystem : UIModal<HandSystem>
{
    [Header("States")]
    [SerializeField] private GameplayCardUI currentSelectedCard;

    [Header("Caches")]
    [SerializeField] private List<GameplayCardUI> handCards;
    [SerializeField] private GameObject handSelector;
    [SerializeField] private GameObject cardAttributeInformationsHolder;
    [SerializeField] private TextMeshProUGUI nameInformationText;
    [SerializeField] private TextMeshProUGUI attackPointInformationText;
    [SerializeField] private TextMeshProUGUI defensePointInformationText;
    [SerializeField] private TextMeshProUGUI levelInformationText;
    [SerializeField] private Image typeInformationImage;
    [SerializeField] private Image guardianStar1InformationImage;
    [SerializeField] private Image guardianStar2InformationImage;



    private void Awake()
    {
        BaseAwake(this);
        Show();
    }

    private void Start()
    {
        handCards[2].SelectCard();
    }

    public void SetSelectedCard(GameplayCardUI cardUI)
    {
        if(currentSelectedCard != null)
        {
            currentSelectedCard.UnselectCard();
        }

        currentSelectedCard = cardUI;
        UpdateHandSelector();
        UpdateInformations();
    }

    private void UpdateHandSelector()
    {
        handSelector.transform.position = currentSelectedCard.transform.position;
    }

    private void UpdateInformations()
    {
        var data = currentSelectedCard.GetCardData();
        nameInformationText.text = data.cardName;
        if(data.IsMonsterCard())
        {
            var monsterData = (MonsterCard)data;
            cardAttributeInformationsHolder.SetActive(true);

            attackPointInformationText.text = monsterData.attackPoint.ToString();
            defensePointInformationText.text = monsterData.defensePoint.ToString();
            levelInformationText.text = monsterData.level.ToString();

            typeInformationImage.sprite = IconManager
                .Instance().GetTypeIcon(monsterData.type);
            guardianStar1InformationImage.sprite = IconManager
                .Instance().GetGuardianStarIcon(monsterData.guardianStarOption1);
            guardianStar2InformationImage.sprite = IconManager
                .Instance().GetGuardianStarIcon(monsterData.guardianStarOption2);
        }
        else
        {
            cardAttributeInformationsHolder.SetActive(false);
        }
    }


    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
