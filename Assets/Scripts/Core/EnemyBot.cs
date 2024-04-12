using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBot : MonoBehaviour
{
    [SerializeField] private bool isPlaying;
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
        yield return new WaitForSeconds(actionDelay);

        HandAction();

        yield return new WaitForSeconds(actionDelay);

        FocusAction();

        yield return new WaitForSeconds(actionDelay);

        FieldAction();
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
        if(opponentField.HasNoMonster())
        {
            StartCoroutine(DirectAttacks(containers));
        } else
        {
            StartCoroutine(NormalAttacks(containers, opponentField.GetFrontRankContainers()));
        }

    }

    private IEnumerator DirectAttacks(List<FieldCardContainer> containers)
    {
        foreach(var container in containers)
        {
            if (container.IsEmpty()) continue;
            if (container.GetCard().HasBeenUsed()) continue;

            var fieldCard = container.GetCard();
            if (fieldCard.InAttackPosition() == false)
            {
                fieldCard.SetToAttackPosition();
            }

            container.SetAsAttackerInBattle();
            BattleSystem.Instance().StartBattle();

            yield return new WaitUntil(() => BattleSystem.Instance().IsBattling() == false);
            yield return new WaitForSeconds(actionDelay);
        }
    }

    private IEnumerator NormalAttacks(List<FieldCardContainer> containers, List<FieldCardContainer> opponentContainers)
    {
        // iterate monsters by the highest attack monster and hasnt been used
        var chosenContainer = ChooseHighestAttackMonster(containers);

        // TODO: continue from here, 
        yield return null;


    }

    private FieldCardContainer ChooseHighestAttackMonster(List<FieldCardContainer> containers)
    {
        var index = -1;
        var bestAttack = -1;
        for (int i = 0; i < containers.Count; i++)
        {
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

    #endregion
}
