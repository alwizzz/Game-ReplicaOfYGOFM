using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;

public class BattleCard : GameplayCard
{
    [Header("Battle Card")]
    [SerializeField] private bool inAttackPosition;
    [SerializeField] private GameObject faceDownImage;
    [SerializeField] private GameObject AttackPanelOverlay;
    [SerializeField] private GameObject DefensePanelOverlay;

    public void SetupBattleCard(Card cardData, bool inAttackPosition)
    {
        base.Setup(cardData);
        this.inAttackPosition = inAttackPosition;

        UpdateOverlay();
    }

    private void UpdateOverlay()
    {
        if(inAttackPosition)
        {
            AttackPanelOverlay.SetActive(false);
            DefensePanelOverlay.SetActive(true);
        } else
        {
            AttackPanelOverlay.SetActive(true);
            DefensePanelOverlay.SetActive(false);
        }
    }

    public bool InAttackPosition() => inAttackPosition;

}
