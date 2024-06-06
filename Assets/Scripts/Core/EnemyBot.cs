using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBot : MonoBehaviour
{
    [SerializeField] private bool isPlaying;
    [SerializeField] private bool isAttacking;
    [SerializeField] private float actionDelay;

    private HandSystem handSystem;
    private HandFocusSystem handFocusSystem;
    private FieldSystem fieldSystem;

    public void Setup(HandSystem handSystem, HandFocusSystem handFocusSystem, FieldSystem fieldSystem)
    {
        this.handSystem = handSystem;
        this.handFocusSystem = handFocusSystem;
        this.fieldSystem = fieldSystem;
    }

    public void StartPlaying()
    {
        print("EnemyBot start playing...");
        isPlaying = true;
        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        isPlaying = true;

        yield return new WaitForSeconds(actionDelay);
        HandAction();
        yield return new WaitForSeconds(actionDelay);
        FocusAction();
        yield return new WaitForSeconds(actionDelay);
        FieldAction();
        yield return new WaitForSeconds(actionDelay);
        yield return new WaitUntil(() => isAttacking == false);
        fieldSystem.EndTurn();
    
        isPlaying = false;
    }

    #region Hand Action
    private void HandAction()
    {
        // choose monster with highest attack point if exist
        // else choose random card

        ChooseCardFromHand();
    }

    private void ChooseCardFromHand()
    {


        var containers = handSystem.GetHandCardContainers();

        int bestAttackPoint = -1;
        int bestIndex = -1;
        for(int i=0; i<containers.Count; i++)
        {
            var cardData = containers[i].GetCard().GetCardData();
            if (cardData.IsMonsterCard() == false) continue;

            var monsterData = (MonsterCard)cardData;
            if (monsterData.attackPoint <= bestAttackPoint) continue;

            bestIndex = i;
            bestAttackPoint = monsterData.attackPoint;
        }

        if(bestIndex == -1) // random card
        {
            bestIndex = Random.Range(0, 5);
        }

        var selectedContainer = containers[bestIndex];
        selectedContainer.Select();
        handSystem.FocusSelectedCard();
    }
    #endregion

    #region Focus Action

    // TODO: manage if field containers are full
    private void FocusAction()
    {
        // choose empty field container for a place to play card
        // by the leftmost empty container
        // always set to facedown
        // always select first guardian star (already by default)

        var emptyContainer = GetEmptyContainer();
        emptyContainer.Select();
        handFocusSystem.SetToFaceDown();
        handFocusSystem.PlayCard();
    }

    private FieldCardContainer GetEmptyContainer()
    {
        var containers = new List<FieldCardContainer>();
        if (handFocusSystem.IsMonster())
        {
            containers = fieldSystem.GetFrontRankContainers();
        }
        else
        {
            containers = fieldSystem.GetFrontRankContainers();
        }

        var emptyContainer = containers.Find((e) => e.IsEmpty());
        if(emptyContainer == null)
        {
            print("UNHANDLED CASE: all field containers are full when attempting to play a card");
        }

        return emptyContainer;
    }

    #endregion

    #region Field Action

    private void FieldAction()
    {
        // iterate through every monster ordered by the highest attack
        // if enemy has face up card with lower power point, attack
        // if the same power point, stay still
        // if higher attack point, go to defense position

        if (fieldSystem.HasNoMonster()) return;

        var containers = fieldSystem.GetFrontRankContainers();

        var opponentField = GameplayManager.Instance().OpponentFieldSystem();
        StartCoroutine(AttackLoop(containers, opponentField));
    }

    private IEnumerator AttackLoop(List<FieldCardContainer> containers, FieldSystem opponentField)
    {
        isAttacking = true;

        var opponentContainers = opponentField.GetFrontRankContainers();

        while (true)
        {
            // iterate monsters by the highest attack monster and hasnt been used
            var chosenMonsterContainer = ChooseHighestAttackMonster(containers);

            // null means all monster has been used
            if (chosenMonsterContainer == null)
            {
                print("all monster has been used");
                yield return new WaitForSeconds(actionDelay);
                break;
            }

            if(opponentField.IsFrontRankEmpty())
            {
                // opponent has no monster in their field, doing a direct attack

                print("direct attack");
                chosenMonsterContainer.GetCard().SetToAttackPosition();
                yield return new WaitForSeconds(actionDelay);
                chosenMonsterContainer.SetAsAttackerInBattle();
                BattleSystem.Instance().StartBattle();

                yield return new WaitUntil(() => BattleSystem.Instance().IsBattling() == false);
                yield return new WaitForSeconds(actionDelay);
            } else
            {
                // opponent has monster in their field

                var fieldCard = chosenMonsterContainer.GetCard();
                var monsterCard = (MonsterCard)fieldCard.GetCardData();
                var chosenMonsterAttackPoint = monsterCard.attackPoint;

                var chosenOpponentContainer = ChooseOpponentMonster(opponentContainers, chosenMonsterAttackPoint);

                // if null then there is no weaker monster on opponent monster than the current chosen monster 
                if (chosenOpponentContainer == null)
                {
                    print($"no weaker monster: {chosenMonsterAttackPoint}");
                    fieldCard.SetToDefensePosition();
                    yield return new WaitForSeconds(actionDelay);
                    fieldCard.SetHasBeenUsed(true);
                    yield return new WaitForSeconds(actionDelay);
                } else
                {
                    var opponentFieldCard = chosenOpponentContainer.GetCard();
                    var opponentMonsterPower = opponentFieldCard.GetPowerPoint();
                    if (opponentFieldCard.IsFaceDown() != false) opponentMonsterPower = 0; // to slips in
                    if(chosenMonsterAttackPoint == opponentMonsterPower)
                    {
                        print($"stall: {chosenMonsterAttackPoint} -> {opponentMonsterPower}");
                        fieldCard.SetHasBeenUsed(true);
                        yield return new WaitForSeconds(actionDelay);
                    } else
                    {
                        print($"battle with weaker monster: {chosenMonsterAttackPoint} -> {opponentMonsterPower}");
                        chosenMonsterContainer.SetAsAttackerInBattle();
                        chosenOpponentContainer.SetAsAttackedInBattle();
                        BattleSystem.Instance().StartBattle();

                        yield return new WaitUntil(() => BattleSystem.Instance().IsBattling() == false);
                        yield return new WaitForSeconds(actionDelay);
                    }
                }
            }

        }

        isAttacking = false;
    }

    private FieldCardContainer ChooseHighestAttackMonster(List<FieldCardContainer> containers)
    {
        var index = -1;
        var bestAttack = -1;
        for (int i = 0; i < containers.Count; i++)
        {
            if (containers[i].IsEmpty()) continue;

            var fieldCard = containers[i].GetCard();
            if (fieldCard.HasBeenUsed()) continue;

            var monsterCard = (MonsterCard)fieldCard.GetCardData();
            if (monsterCard.attackPoint <= bestAttack) continue;

            bestAttack = monsterCard.attackPoint;
            index = i;
        }

        if (index == -1) return null;
        return containers[index];
    }

    private FieldCardContainer ChooseOpponentMonster(List<FieldCardContainer> opponentContainers, int powerThreshold)
    {
        // choose order: weaker > defense face down > attack face down > strongest
        var weakerIndex = -1;
        var weakerPower = -1;

        var defenseModeFaceDownIndex = -1;
        var attackModeFaceDownIndex = -1;
        for (int i = 0; i < opponentContainers.Count; i++)
        {
            if (opponentContainers[i].IsEmpty()) continue;

            var fieldCard = opponentContainers[i].GetCard();
            if(fieldCard.IsFaceDown())
            {
                if(fieldCard.InAttackPosition())
                {
                    attackModeFaceDownIndex = i;
                } else
                {
                    defenseModeFaceDownIndex = i;
                }
                continue;
            }

            var monsterPower = fieldCard.GetPowerPoint();
            if (monsterPower >= powerThreshold) continue;

            if (monsterPower < weakerPower) continue;

            weakerPower = monsterPower;
            weakerIndex = i;
        }

        if (weakerIndex != -1)
        {
            print("found weaker monster");
            return opponentContainers[weakerIndex];
        } else if (defenseModeFaceDownIndex != -1)
        {
            print("found defense facedown monster");
            return opponentContainers[defenseModeFaceDownIndex];
        } else if (attackModeFaceDownIndex != -1)
        {
            print("found attack facedown monster");
            return opponentContainers[attackModeFaceDownIndex];
        } else
        {
            print($"No match for current power threshold {powerThreshold}");
            return null;
        }
    }

    #endregion
}
