using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : StaticReference<GameplayManager>
{
    [SerializeField] private Sprite dummySpriteBig;

    [Header("GameplayCardUI's Parameters")]
    [SerializeField] private Color monsterCardColor;
    [SerializeField] private Color spellCardColor;
    [SerializeField] private Color trapCardColor;
    [SerializeField] private Color ritualCardColor;


    private void Awake()
    {
        BaseAwake(this);
    }

    public Sprite GetDummySprite() => dummySpriteBig;

    public Color GetGameplayCardUIBaseColor(Card cardData)
    {
        if(cardData is MonsterCard)
        {
            return monsterCardColor;
        } else if(cardData is SpellCard)
        {
            if(cardData is RitualSpellCard)
            {
                return ritualCardColor;
            } else
            {
                return spellCardColor;
            }
        } else if(cardData is TrapCard)
        {
            return trapCardColor;
        } else
        {
            print("WARNING: invalid card data types, returning white color as null");
            return Color.white;
        }


    }







    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
