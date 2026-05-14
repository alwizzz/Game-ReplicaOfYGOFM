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
    [SerializeField] private float firstDelay;
    [SerializeField] private float preDamageCalculationDelay;
    [SerializeField] private float postDamageCalculationDelay;

    [Header("States")]
    [SerializeField] private bool isDirectAttack;
    [SerializeField] private bool? gsInteractionResult;


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
        gsInteractionResult = null;
    }

    public void StartBattle()
    {
        if (attackerFieldCardReference == null) return;
        if (attackerFieldCardReference.InAttackPosition() == false) return;
        if(attackedFieldCardReference == null)
        {
            
            isDirectAttack = CheckEmptyOpponentField();
            // TODO: if in this case isDirectAttack is false, it would raise an error
        }

        Setup(isDirectAttack);

        // default value
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

        yield return new WaitForSeconds(firstDelay);

        gsInteractionResult = GuardianStarInteraction(isDirectAttack);

        yield return new WaitForSeconds(preDamageCalculationDelay);

        DamageCalculation(isDirectAttack, gsInteractionResult);
        BattleResolution();

        yield return new WaitForSeconds(postDamageCalculationDelay);

        // cleanups
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

    private bool? GuardianStarInteraction(bool isDirectAttack)
    {
        if(isDirectAttack){ return null; }

        GuardianStar attackerGs = attackerFieldCardReference.GetSelectedGuardianStar();
        GuardianStar attackedGs = attackedFieldCardReference.GetSelectedGuardianStar();
        bool? interactionResult = GuardianStarCalculator.GetInteraction(
            attackerGs, 
            attackedGs
        );

        if(interactionResult != null)
        {
            int bonusPower = GameplayManager.Instance().GuardianStarBonusPower();
            BattleCard affectedBattleCard;
            bool targetsAttackPoint;
            if(interactionResult == true)
            {
                affectedBattleCard = attackerBattleCard;
                // attacker always on attack position
                targetsAttackPoint = true;
            } else // automatically "false"
            // } else if(interactionResult == false)
            {
                affectedBattleCard = attackedBattleCard;
                bool attackedInAttackPosition = attackedBattleCard.InAttackPosition();
                if (attackedInAttackPosition)
                {
                    targetsAttackPoint = true;
                } else
                {
                    targetsAttackPoint = false;
                }
            }

            affectedBattleCard.PlayBonusPowerTextAnimation(
                targetsAttackPoint, bonusPower, preDamageCalculationDelay
            );
        }

        return interactionResult;
    }

    private void DamageCalculation(bool isDirectAttack, bool? gsInteractionResult)
    {
        if(isDirectAttack)
        {
            damageDealt = GetPowerPoint(attackerBattleCard);
            attackedFlareEffect.SetupAndShow(damageDealt);
            return;
        }

        int attackerPower = GetPowerPoint(attackerBattleCard);
        int attackedPower = GetPowerPoint(attackedBattleCard);

        if(gsInteractionResult != null)
        {
            int bonusPower = GameplayManager.Instance().GuardianStarBonusPower();
            if(gsInteractionResult == true)
            {
                attackerPower += bonusPower;
            } else if(gsInteractionResult == false)
            {
                attackedPower += bonusPower;
            }
        }


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
