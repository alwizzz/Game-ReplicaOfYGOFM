using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;
using System;


public class Helpers : StaticReference<Helpers>
{
    [System.Serializable]
    public struct FusionResult
    {
        public Card card;
        public bool isFusioned;
    }

    private void Awake()
    {
        BaseAwake(this);
    }


    // TODO: would be better if there's async callback to tell if the action is indeed invoked
    public void DelayedAction(float delay, Action action) 
    { 
        StartCoroutine(Delayed(delay, action));   
    }

    private IEnumerator Delayed(float delay, Action action) 
    { 
        yield return new WaitForSeconds(delay); 
        action?.Invoke(); 
    }



    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
