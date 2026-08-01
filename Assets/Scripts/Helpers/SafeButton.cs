using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;


/*
    Act as extension of UnityEngine.UI.Button component, 
    extends OnPointerClick to be disrupted if game state is on InputLock
*/
public class SafeButton : Button
{
    public override void OnPointerClick(PointerEventData eventData)
    {

        if (GameplayManager.Instance().IsInputLock())
        {
            print("DEBUG: SafeButton's inputlocked!");
            return;
        }


        base.OnPointerClick(eventData);

    }
}
