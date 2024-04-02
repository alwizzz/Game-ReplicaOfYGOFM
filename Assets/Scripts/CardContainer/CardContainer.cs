using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardContainer : MonoBehaviour
{
    public void MovePositionOnContainer(Transform obj, bool isSettingParent = false)
    {
        obj.position = transform.position;
        if (isSettingParent)
        {
            obj.SetParent(transform);
        }
    }
}
