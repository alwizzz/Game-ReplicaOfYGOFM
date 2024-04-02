using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;

// TODO: name isnt that suitable because it also stores color informations and dummy sprites
public class ResourceManager : StaticReference<ResourceManager>
{
    [System.Serializable]
    public struct NamedSprite
    {
        public string name;
        public Sprite sprite;
    }

    [SerializeField] private Sprite dummySprite;

    //[SerializeField] private List<NamedSprite> levelIcons;
    [SerializeField] private List<NamedSprite> typeIcons;
    [SerializeField] private List<NamedSprite> guardianStarIcons;

    [Header("Color Parameters")]
    [SerializeField] private Color monsterCardColor;
    [SerializeField] private Color spellCardColor;
    [SerializeField] private Color trapCardColor;
    [SerializeField] private Color ritualCardColor;

    private void Awake()
    {
        BaseAwake(this);
    }

    public Sprite GetTypeIcon(MonsterType type)
    {
        var typeName = type.ToString();
        return GetIcon(typeName, typeIcons);
    }

    public Sprite GetGuardianStarIcon(GuardianStar guardianStar)
    {
        var codeName = guardianStar.ToString();
        return GetIcon(codeName, guardianStarIcons);
    }

    private Sprite GetIcon(string codeName, List<NamedSprite> list)
    {
        var spriteResult = list.Find((e) => e.name == codeName).sprite;

        // default() of a struct is an instance with all of its fields in default()
        if (spriteResult == null)
        {
            print("WARNING: no type matched, returning dummy sprite instead");
            spriteResult = dummySprite;
        }
        return spriteResult;
    }

    public Sprite GetDummySprite() => dummySprite;

    public Color GetGameplayCardBaseColor(Card cardData)
    {
        if (cardData is MonsterCard)
        {
            return monsterCardColor;
        }
        else if (cardData is SpellCard)
        {
            if (cardData is RitualSpellCard)
            {
                return ritualCardColor;
            }
            else
            {
                return spellCardColor;
            }
        }
        else if (cardData is TrapCard)
        {
            return trapCardColor;
        }
        else
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
