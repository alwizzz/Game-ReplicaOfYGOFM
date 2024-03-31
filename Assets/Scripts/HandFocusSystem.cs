using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandFocusSystem : UIModal<HandFocusSystem>
{
    [SerializeField] private GameplayCardUI focusedCard;
    [SerializeField] private bool isFacedown;

    [Header("Cache")]
    [SerializeField] private GuardianStarOptionSelector selector;

    private void Awake()
    {
        BaseAwake(this);
    }

    public void SetupAndShow(GameplayCardUI cardUI)
    {
        var cardData = cardUI.GetCardData();
        focusedCard.Setup(cardData);

        if(cardData.IsMonsterCard())
        {
            var data = (MonsterCard)cardData;
            selector.gameObject.SetActive(true);
            selector.Setup(data.guardianStarOption1, data.guardianStarOption2);
        } else
        {
            selector.gameObject.SetActive(false);
        }

        Show();
    }



    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
