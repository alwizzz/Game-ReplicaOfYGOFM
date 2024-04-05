using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class BattleSystem : StaticUIModal<BattleSystem>
{
    [Header("Parameters")]
    [SerializeField] private FieldCard attackerReference;
    [SerializeField] private FieldCard attackedReference;
    [SerializeField] private float preDamageCalculationDelay;
    [SerializeField] private float postDamageCalculationDelay;

    [Header("States")]
    [Tooltip("Positive for damage received by attacked, " +
        "Negative for damage received by attacker, " +
        "and 0 for a tie")]
    [SerializeField] private int damageDealt;
    [SerializeField] private bool attackerDestroyed;
    [SerializeField] private bool attackedDestroyed;


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

        attackerDestroyed = false;
        attackedDestroyed = false;

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

        attackerReference.SetToFaceUp();
        attackedReference.SetToFaceUp();
    }

    private IEnumerator Battle()
    {
        Show();

        yield return new WaitForSeconds(preDamageCalculationDelay);

        DamageCalculation();
        BattleResolution();

        yield return new WaitForSeconds(postDamageCalculationDelay);

        attackerFlareEffect.Hide();
        attackedFlareEffect.Hide();
        Hide();
        GameplayManager.Instance().FieldSystem().StartFieldPhase();
    }

    private void DamageCalculation()
    {
        int attackerPower = GetPowerPoint(attackerBattleCard);
        int attackedPower = GetPowerPoint(attackedBattleCard);

        // attacker must have been in attack position
        //bool attackerInAttackPosition = attackerBattleCard.InAttackPosition();
        bool attackedInAttackPosition = attackedBattleCard.InAttackPosition();

        damageDealt = attackerPower - attackedPower;
        if (damageDealt == 0)
        { // TIE
            if(attackedInAttackPosition)
            {
                attackerDestroyed = true;
                attackedDestroyed = true;
            }
            attackerFlareEffect.SetupAndShow(0);
            attackedFlareEffect.SetupAndShow(0);
        } else if(damageDealt > 0)
        { // ATTACKER WINS
            if(!attackedInAttackPosition)
            {
                damageDealt = 0;
            } 
            attackedFlareEffect.SetupAndShow(damageDealt);
            attackedDestroyed = true;
        } else
        { // ATTACKED WINS
            attackerFlareEffect.SetupAndShow(damageDealt);
            if (attackedInAttackPosition)
            {
                attackerDestroyed = true;
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

    private void BattleResolution()
    {
        attackerReference.SetHasAttacked(true);
        DestroyCards();
        UpdateLifePoint();

        Reset();
    }

    private void DestroyCards()
    {
        if (attackerDestroyed)
        {
            attackerReference.Destroy();
        }

        if (attackedDestroyed)
        {
            attackedReference.Destroy();
        }
    }

    private void UpdateLifePoint()
    {
        GameplayManager.Instance().UpdateLifePointAfterBattle(damageDealt);
    }


    public void SetAttackerReference(FieldCard reference)
    {
        attackerReference = reference;
    }

    public void SetAttackedReference(FieldCard reference)
    {
        attackedReference = reference;
    }


    public void SetOpponentCardAsAttackedInBattle()
    {
        var opponentFieldSystem = GameplayManager.Instance().OpponentFieldSystem();
        var opponentSelectedFieldContainer = opponentFieldSystem.GetSelectedFieldContainer();
        if (opponentSelectedFieldContainer == null)
        {
            print("ERROR: currently no selected field container on opponent");
            return;
        }

        opponentSelectedFieldContainer.SetAsAttackedInBattle();
    }


    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
