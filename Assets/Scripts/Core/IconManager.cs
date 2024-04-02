using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Enums;

public class IconManager : StaticReference<IconManager>
{
    [System.Serializable]
    public struct NamedSprite
    {
        public string name;
        public Sprite sprite;
    }

    [SerializeField] private Sprite dummySprite;

    [SerializeField] private List<NamedSprite> levelIcons;
    [SerializeField] private List<NamedSprite> typeIcons;
    [SerializeField] private List<NamedSprite> guardianStarIcons;

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


    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
