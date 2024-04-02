using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : StaticReference<GameplayManager>
{

    private void Awake()
    {
        BaseAwake(this);
    }






    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
