using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using TMPro;

using Enums;

// public class BattleSystem : StaticUIModal<BattleSystem>
public class BattleSystem : UIModal<BattleSystem>
{
    [SerializeField] private bool isBattling;
    [Header("Parameters")]
    [SerializeField] private FieldCard attackerFieldCardReference;
    [SerializeField] private FieldCard attackedFieldCardReference;
    [SerializeField] private float preDamageCalculationDelay;
    [SerializeField] private float postDamageCalculationDelay;

    [Header("States")]
    [SerializeField] private bool isDirectAttack;
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
        attackerFieldCardReference = null;
        attackedFieldCardReference = null;
        isDirectAttack = false;
    }

    public void StartBattle()
    {
        if (attackerFieldCardReference == null) return;
        if (attackerFieldCardReference.InAttackPosition() == false) return;
        if(attackedFieldCardReference == null)
        {
            
            isDirectAttack = CheckEmptyOpponentField();
            //if (isDirectAttack) StartDirectBattle();
            //return;
        }

        Setup(isDirectAttack);

        attackerDestroyed = false;
        attackedDestroyed = false;

        StartCoroutine(Battle());
    }


    private bool CheckEmptyOpponentField()
    {

        return GameplayManager.Instance().OpponentFieldSystem().IsFrontRankEmpty();
    }

    private void Setup(bool isDirectAttack = false)
    {
        attackerBattleCard.SetupBattleCard(
            cardData: attackerFieldCardReference.GetCardData(),
            inAttackPosition: attackerFieldCardReference.InAttackPosition()
        );
        attackerFieldCardReference.SetToFaceUp();

        if(isDirectAttack)
        {
            attackedBattleCard.gameObject.SetActive(false);
            //return;
        } else
        {
            // error will be raised if going to do direct attack when opponent front rank is not empty

            attackedBattleCard.gameObject.SetActive(true);
            attackedBattleCard.SetupBattleCard(
                cardData: attackedFieldCardReference.GetCardData(),
                inAttackPosition: attackedFieldCardReference.InAttackPosition()
            );
            attackedFieldCardReference.SetToFaceUp();
        }

    }

    private IEnumerator Battle()
    {
        isBattling = true;
        Show();

        yield return new WaitForSeconds(preDamageCalculationDelay);

        DamageCalculation(isDirectAttack);
        BattleResolution();

        yield return new WaitForSeconds(postDamageCalculationDelay);

        attackerFlareEffect.Hide();
        attackedFlareEffect.Hide();
        Hide();
        GameplayManager.Instance().FieldSystem().StartFieldPhase();
        GameplayManager.Instance().OpponentFieldSystem().CloseSelection(false);

        if(GameplayManager.Instance().IsPlayerTurn())
        {
            FieldButtonManager.Instance().ForceUpdateButtons();
        }

        isBattling = false;
    }

    private void DamageCalculation(bool isDirectAttack)
    {
        if(isDirectAttack)
        {
            damageDealt = GetPowerPoint(attackerBattleCard);
            attackedFlareEffect.SetupAndShow(damageDealt);
            return;
        }

        int attackerPower = GetPowerPoint(attackerBattleCard);
        int attackedPower = GetPowerPoint(attackedBattleCard);

        // handle guardian star interaction
        GuardianStar attackerGs = attackerFieldCardReference.GetSelectedGuardianStar();
        GuardianStar attackedGs = attackedFieldCardReference.GetSelectedGuardianStar();
        GuardianStarCalculator.ApplyBonusPower(ref attackerPower, attackerGs, ref attackedPower, attackedGs);


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
        attackerFieldCardReference.SetHasBeenUsed(true);
        DestroyCards();
        UpdateLifePoint();

        Reset();
    }

    private void DestroyCards()
    {
        if (attackerDestroyed)
        {
            attackerFieldCardReference.Destroy();
        }

        if (attackedDestroyed)
        {
            attackedFieldCardReference.Destroy();
        }
    }

    private void UpdateLifePoint()
    {
        GameplayManager.Instance().UpdateLifePointAfterBattle(damageDealt);
    }


    public void SetAttackerReference(FieldCard reference)
    {
        attackerFieldCardReference = reference;
    }

    public void SetAttackedReference(FieldCard reference)
    {
        attackedFieldCardReference = reference;
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

    public bool IsBattling() => isBattling;

    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
