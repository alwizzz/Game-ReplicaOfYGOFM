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
        public bool isFusioned;
        public bool retainMonster; // specifically used for Fusion Flow's retainModification logic
    }

    public static FusionResult GetFusionResult(Card m1, Card m2) // m as in
    {
        // strictly, m1 is the left-side card, m2 is the right-side card
        // where fusion flow goes from right to left

        // initialize
        Card card;
        bool isFusioned = false; // TODO: still dummy value
        bool retainMonster = false; // TODO: still pretty much dummy value

        if(m1.IsMonsterCard() && m2.IsMonsterCard()) // m1 monster, m2 monster
        {
            card = m2;
        } else if(!m1.IsMonsterCard() && !m2.IsMonsterCard()) // m1 nonmoster, m2 nonmonster
        {
            card = m2;
        } else if(m1.IsMonsterCard() && !m2.IsMonsterCard()) // m1 monster, m2 nonmonster
        {
            card = m1;
            retainMonster = true;
        } else // the only remaining case is m1 nonmonster, m2 monster
        {
            card = m2;
        }

        return new FusionResult
        {
            card = card,
            isFusioned = isFusioned,
            retainMonster = retainMonster
        };
    }
}
