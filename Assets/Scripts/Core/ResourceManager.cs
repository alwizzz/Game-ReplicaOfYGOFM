using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;
// using System.Drawing;

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

    [Header("Card Colors")]
    [SerializeField] private Color monsterCardColor;
    [SerializeField] private Color spellCardColor;
    [SerializeField] private Color trapCardColor;
    [SerializeField] private Color ritualCardColor;


    [Header("Point Number Colors")]
    [SerializeField] private Color normalNumberColor;
    [SerializeField] private Color bonusedNumberColor;


    [Header("Field Spell")]
    [SerializeField] private Dictionary<FieldType, Color> fieldColorDict = new();
    [SerializeField] private Dictionary<FieldType, Color> fieldContainerColorDict = new();
    [SerializeField] private Dictionary<FieldType, Card> fieldCardDict = new();

    private void Awake()
    {
        BaseAwake(this);

        // runtime fill
        fieldColorDict.Add(
            FieldType.Normal, 
            new Color32(119, 100, 10, 255)
        );
        fieldContainerColorDict.Add(
            FieldType.Normal, 
            new Color32(119, 100, 10, 255)
        );
        fieldCardDict.Add(
            FieldType.Normal, 
            null
        );

        fieldColorDict.Add(
            FieldType.Forest, 
            new Color32(46, 107, 47, 255)
        );
        fieldContainerColorDict.Add(
            FieldType.Forest, 
            new Color32(77, 139, 78, 255)
        );
        fieldCardDict.Add(
            FieldType.Forest, 
            null // dummy
        );

        fieldColorDict.Add(
            FieldType.Mountain, 
            new Color32(106, 95, 85, 255)
        );
        fieldContainerColorDict.Add(
            FieldType.Mountain, 
            new Color32(136, 124, 113, 255)
        );
        fieldCardDict.Add(
            FieldType.Mountain, 
            null // dummy
        );

        fieldColorDict.Add(
            FieldType.Sogen, 
            new Color32(142, 158, 58, 255)
        );
        fieldContainerColorDict.Add(
            FieldType.Sogen, 
            new Color32(168, 184, 87, 255)
        );
        fieldCardDict.Add(
            FieldType.Sogen, 
            null // dummy
        );

        fieldColorDict.Add(
            FieldType.Umi, 
            new Color32(29, 95, 154, 255)
        );
        fieldContainerColorDict.Add(
            FieldType.Umi, 
            new Color32(63, 130, 194, 255)
        );
        fieldCardDict.Add(
            FieldType.Umi, 
            null // dummy
        );

        fieldColorDict.Add(
            FieldType.Wasteland, 
            new Color32(166, 124, 69, 255)
        );
        fieldContainerColorDict.Add(
            FieldType.Wasteland, 
            new Color32(195, 151, 94, 255)
        );
        fieldCardDict.Add(
            FieldType.Wasteland, 
            null // dummy
        );

        fieldColorDict.Add(
            FieldType.Yami, 
            new Color32(56, 34, 74, 255)
        );
        fieldContainerColorDict.Add(
            FieldType.Yami, 
            new Color32(88, 64, 107, 255)
        );
        fieldCardDict.Add(
            FieldType.Yami, 
            null // dummy
        );
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

    public Color GetNumberColor(bool isNormal)
    {
        return isNormal ? normalNumberColor : bonusedNumberColor;
    }

    public Color GetFieldColor(FieldType fieldType)
    {
        Debug.Assert(fieldColorDict.ContainsKey(fieldType));

        return fieldColorDict.GetValueOrDefault(fieldType);
    }
    public Color GetFieldContainerColor(FieldType fieldType)
    {
        Debug.Assert(fieldContainerColorDict.ContainsKey(fieldType));

        return fieldColorDict.GetValueOrDefault(fieldType);
    }

    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
