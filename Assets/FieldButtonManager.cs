using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FieldButtonManager : StaticUIModal<FieldButtonManager>
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
        }
        else
        {
            if (container.IsBackRank())
            {
                changePositionButton.interactable = false;
                useCardButton.interactable = true;
            }
            else
            {
                // BUG: use card interactable not updated when clicking ChangePosition button 
                // because the implementation currently only exist on FieldCardContaiener.Select()
                changePositionButton.interactable = true;
                if(container.GetCard().InAttackPosition())
                {
                    useCardButton.interactable = true;
                } else
                {
                    useCardButton.interactable = false;
                }
            }
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

    public void OnChangePositionButtonUpdate()
    {
        // force to reselect current selected field card container to
        // apply the changes
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
