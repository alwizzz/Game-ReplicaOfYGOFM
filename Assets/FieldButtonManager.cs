using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// public class FieldButtonManager : StaticUIModal<FieldButtonManager>
public class FieldButtonManager : UIModal<FieldButtonManager>
{
    [SerializeField] private Button changePositionButton;
    [SerializeField] private Button useCardButton;
    [SerializeField] private Button battleButton;
    [SerializeField] private Button cancelBattleButton;
    [SerializeField] private Button endTurnButton;


    private void Awake()
    {
        BaseAwake(this);
    }


    public void UpdateButtons(FieldCardContainer container)
    {
        if(container.IsEmpty())
        {
            useCardButton.interactable = false;
            changePositionButton.interactable = false;

            return;
        }

        if (container.IsBackRank())
        {
            changePositionButton.interactable = false;
            useCardButton.interactable = true;

            return;
        }


        var fieldCard = container.GetCard();
        if(fieldCard.HasBeenUsed())
        {
            useCardButton.interactable = false;
            changePositionButton.interactable = false;

            return;
        }


        changePositionButton.interactable = true;
        if (fieldCard.InAttackPosition())
        {
            useCardButton.interactable = true;
        }
        else
        {
            useCardButton.interactable = false;
        }
    }

    public void UpdateBattleButtons(bool inBattleMode)
    {
        if(inBattleMode)
        {
            battleButton.interactable = true;
            cancelBattleButton.interactable = true;

            useCardButton.interactable = false;
            endTurnButton.interactable = false;
            changePositionButton.interactable = false;
        } else
        {
            battleButton.interactable = false;
            cancelBattleButton.interactable = false;

            useCardButton.interactable = true;
            endTurnButton.interactable = true;
            changePositionButton.interactable = true;
        }
    }

    public void ForceUpdateButtons()
    {
        // force to reselect current selected field card container to
        // reinvoke UpdateButtons and apply the changes
        print("force update buttons");
        GameplayManager.Instance().PlayerFieldSystem().GetSelectedFieldContainer().Select();
    }

    public Button GetChangePositionButton() => changePositionButton;
    public Button GetUseCardButton() => useCardButton;
    public Button GetBattleButton() => battleButton;
    public Button GetCancelBattleButton() => cancelBattleButton;
    public Button GetEndTurnButton() => endTurnButton;




    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
