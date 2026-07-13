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
    }

    public static FusionResult GetFusionResult(Card m1, Card m2) // m as in
    {
        // strictly, m1 is the left-side card, m2 is the right-side card
        // where fusion flow goes from right to left

        // initialize
        Card card;
        bool isFusioned = false;

        if(m1.IsMonsterCard() && m2.IsMonsterCard())
        {
            card = m2;
        } else if(!m1.IsMonsterCard() && !m2.IsMonsterCard())
        {
            card = m2;
        } else if(m1.IsMonsterCard() && !m2.IsMonsterCard())
        {
            card = m1;
        } else // the only remaining case is !m1.IsMonsterCard() && m2.IsMonsterCard()
        {
            card = m2;
        }

        return new FusionResult
        {
            card = card,
            isFusioned = isFusioned
        };
    }
}
