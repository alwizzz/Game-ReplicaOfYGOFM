using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardInformationDisplay : MonoBehaviour
{
    [SerializeField] private GameObject attributesOverlay;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI attackPointText;
    [SerializeField] private TextMeshProUGUI defensePointText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image typeImage;
    [SerializeField] private Image guardianStar1Image;
    [SerializeField] private Image guardianStar2Image;

    public void UpdateInformation(HandCard handCard)
    {
        var data = handCard.GetCardData();
        nameText.text = data.cardName;
        if (data.IsMonsterCard())
        {
            var monsterData = (MonsterCard)data;
            attributesOverlay.SetActive(false);

            attackPointText.text = monsterData.attackPoint.ToString();
            defensePointText.text = monsterData.defensePoint.ToString();
            levelText.text = monsterData.level.ToString();

            typeImage.sprite = IconManager
                .Instance().GetTypeIcon(monsterData.type);
            guardianStar1Image.sprite = IconManager
                .Instance().GetGuardianStarIcon(monsterData.guardianStarOption1);
            guardianStar2Image.sprite = IconManager
                .Instance().GetGuardianStarIcon(monsterData.guardianStarOption2);
        }
        else
        {
            attributesOverlay.SetActive(true);
        }
    }
}
