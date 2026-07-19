using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;


public static class FusionCalculator
{
    [System.Serializable]
    public struct FusionResult
    {
        public Card card;

        public FusionResultType type;
        public bool retainMonster; // specifically used for Fusion Flow's retainModification logic
        public GameplayCard.Modifier? modifier;
    }

    public static FusionResult GetFusionResult(Card m1, Card m2) // m as in
    {
        // strictly, m1 is the left-side card, m2 is the right-side card
        // where fusion flow goes from right to left

        // initialize
        Card outputCard;
        FusionResultType type = FusionResultType.Rejected;
        bool retainMonster = false; // TODO: still pretty much dummy value
        GameplayCard.Modifier? modifier = null;

        if(m1.IsMonsterCard() && m2.IsMonsterCard()) // m1 monster, m2 monster
        {
            outputCard = m2;
        } else if(!m1.IsMonsterCard() && !m2.IsMonsterCard()) // m1 nonmoster, m2 nonmonster
        {
            outputCard = m2;
        } else if(m1.IsMonsterCard() && !m2.IsMonsterCard()) // m1 monster, m2 nonmonster
        {
            outputCard = m1;
            modifier = TryEquip(m2, m1);
            if(modifier.HasValue) type = FusionResultType.Equipped;
            
            retainMonster = true;
        } else // the only remaining case is m1 nonmonster, m2 monster
        {
            outputCard = m2;
            modifier = TryEquip(m1, m2);
            if(modifier.HasValue) type = FusionResultType.Equipped;

            // NOTE: meanwhile retainMonster is initially used to tell if the m1 isnt changed,
            // the definition still fits on this case
            retainMonster = true; 
        }

        return new FusionResult
        {
            card = outputCard,
            type = type,
            retainMonster = retainMonster,
            modifier = modifier,
        };
    }

    private static GameplayCard.Modifier? TryEquip(Card equipSpellCardToBe, Card monsterCardToBe)
    {
        // validate
        if(equipSpellCardToBe is not EquipSpellCard equipSpellCard 
            || monsterCardToBe is not MonsterCard monsterCard) 
        return null;

        // TODO doing more cases
        if(equipSpellCard.cardName == "Megamorph")
        {
            // megamorph accepts every monster card
            return new GameplayCard.Modifier(1000, 1000, equipSpellCardToBe);;
        } else
        {
            Debug.Log("unhandled case");
            return null;
        }

    }
}
