using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;

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
    }

    public bool IsPlayerOwned() => (owner == Side.Player ? true : false);
}
