using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class BattleSystem : StaticUIModal<BattleSystem>
{
    [SerializeField] private FieldCard attackerReference;
    [SerializeField] private FieldCard attackedReference;
    [SerializeField] private float preDamageCalculationDelay;
    [SerializeField] private float postDamageCalculationDelay;


    [Header("Caches")]
    [SerializeField] private BattleCard attackerBattleCard;
    [SerializeField] private BattleCard attackedBattleCard;
    [SerializeField] private DamageFlareEffect attackerFlareEffect;
    [SerializeField] private DamageFlareEffect attackedFlareEffect;







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
        StartCoroutine(Battle());
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

    private IEnumerator Battle()
    {
        Show();

        yield return new WaitForSeconds(preDamageCalculationDelay);

        DamageCalculation();

        yield return new WaitForSeconds(postDamageCalculationDelay);

        attackerFlareEffect.Hide();
        attackedFlareEffect.Hide();

        Hide();
    }

    // TODO: separate BattleResolution from DamageCalculation
    // where DamageCalculation solely calculating damage and
    // BattleResolution resolves battle result
    private void DamageCalculation()
    {
        int attackerPower = GetPowerPoint(attackerBattleCard);
        int attackedPower = GetPowerPoint(attackedBattleCard);

        // attacker must have been in attack position
        //bool attackerInAttackPosition = attackerBattleCard.InAttackPosition();
        bool attackedInAttackPosition = attackedBattleCard.InAttackPosition();


        if (attackerPower == attackedPower)
        { // TIE
            attackerFlareEffect.SetupAndShow(0);
            attackedFlareEffect.SetupAndShow(0);
        } else if(attackerPower > attackedPower)
        { // ATTACKER WINS
            if(attackedInAttackPosition)
            {
                int damage = attackedPower - attackerPower;
                attackedFlareEffect.SetupAndShow(damage);
            } else
            {
                attackedFlareEffect.SetupAndShow(0);
            } 
            print("TODO: destroy attacked battle card");
        } else
        { // ATTACKED WINS
            int damage = attackedPower - attackerPower;
            attackerFlareEffect.SetupAndShow(damage);
            if (attackedInAttackPosition)
            {
                print("TODO: destroy attacker battle card");
            }
        }

    }

    private int GetPowerPoint(BattleCard battleCard)
    {
        var monsterCard = (MonsterCard)battleCard.GetCardData();
        if(battleCard.InAttackPosition())
        {
            return monsterCard.attackPoint;
        } else
        {
            return monsterCard.defensePoint;
        }
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
