using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using Enums;
using System.Diagnostics;

public class CardInformationDisplay : MonoBehaviour
{
    [SerializeField] private bool onField;

    [Header("Caches")]
    [SerializeField] private GameObject nameOverlay;
    [SerializeField] private GameObject attributesOverlay;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI attackPointText;
    [SerializeField] private TextMeshProUGUI defensePointText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image typeImage;
    [SerializeField] private Image guardianStar1Image;
    [SerializeField] private Image guardianStar2Image;


    private void Awake()
    {
        if (!onField) return; // only the selected guardian star is used in field
        ResetInformation();
        guardianStar1Image.transform.parent.gameObject.SetActive(false);
    }
    public void UpdateInformation(GameplayCard gameplayCard)
    {
        if(gameplayCard == null)
        {
            print("ERROR: gameplayCard is null");
            return;
        }

        var data = gameplayCard.GetCardData();
        nameText.text = data.cardName;
        if (data.IsMonsterCard())
        {
            var monsterData = (MonsterCard)data;
            nameOverlay.SetActive(false);
            attributesOverlay.SetActive(false);

            // attackPointText.text = monsterData.attackPoint.ToString();
            // defensePointText.text = monsterData.defensePoint.ToString();
            attackPointText.text = gameplayCard.GetAttackPoint().ToString();
            defensePointText.text = gameplayCard.GetDefensePoint().ToString();
            levelText.text = monsterData.level.ToString();

            typeImage.sprite = ResourceManager
                .Instance().GetTypeIcon(monsterData.type);

            if(onField)
            {
                var fieldCard = (FieldCard)gameplayCard;
                var selectedGuardianStar = fieldCard.GetSelectedGuardianStar();
                // var selectedGuardianStar = fieldCard.GetModifier().selectedGuardianStar;
                

                guardianStar2Image.sprite = ResourceManager
                    .Instance().GetGuardianStarIcon(selectedGuardianStar);
            } else
            {
                guardianStar1Image.sprite = ResourceManager
                    .Instance().GetGuardianStarIcon(monsterData.guardianStarOption1);
                guardianStar2Image.sprite = ResourceManager
                    .Instance().GetGuardianStarIcon(monsterData.guardianStarOption2);
            }

        }
        else
        {
            nameOverlay.SetActive(false);
            attributesOverlay.SetActive(true);
        }
    }

    public void ResetInformation()
    {
        // not necessarily resetting the info, just hiding them by overlaying
        nameOverlay.SetActive(true);
        attributesOverlay.SetActive(true);
    }
}
