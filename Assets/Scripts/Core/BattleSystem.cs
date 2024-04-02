using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : StaticUIModal<BattleSystem>
{
    [SerializeField] private BattleCard attackerBattleCard;
    [SerializeField] private BattleCard attackedBattleCard;

    [SerializeField] private FieldCard attackerReference;
    [SerializeField] private FieldCard attackedReference;


    private void Awake()
    {
        BaseAwake(this);
        Reset();
    }

    private void Reset()
    {
        attackerReference = null;
        attackedReference = null;
    }

    public void StartBattle()
    {
        if (attackerReference == null || attackedReference == null) return;
        if (attackerReference.InAttackPosition() == false) return;

        Setup();
        Show();
    }

    private void Setup()
    {
        attackerBattleCard.SetupBattleCard(
            cardData: attackerReference.GetCardData(),
            inAttackPosition: attackerReference.InAttackPosition()
        );

        attackedBattleCard.SetupBattleCard(
            cardData: attackedReference.GetCardData(),
            inAttackPosition: attackedReference.InAttackPosition()
        );
    }

    public void SetAttackerReference(FieldCard reference)
    {
        attackerReference = reference;
    }

    public void SetAttackedReference(FieldCard reference)
    {
        attackedReference = reference;
    }


    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
