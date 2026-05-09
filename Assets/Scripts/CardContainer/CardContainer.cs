using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;
// using System.Numerics;

public abstract class CardContainer : MonoBehaviour
{
    [SerializeField] protected Side owner;

    public void MovePositionOnContainer(Transform obj, bool setParent = false)
    {
        obj.position = transform.position;
        if (setParent)
        {
            obj.SetParent(transform);
        }
        obj.localEulerAngles = new Vector3(0, 0, 0);

        // kind of quick fix, because there is unknown logic that change GameplayCard's scale to not 1/1/1
        obj.localScale = new Vector3(1,1,1); 
    }

    public bool IsPlayerOwned() => (owner == Side.Player ? true : false);
}
