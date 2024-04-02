using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardContainer : MonoBehaviour
{
    public void MovePositionOnContainer(Transform obj, bool setParent = false)
    {
        obj.position = transform.position;
        if (setParent)
        {
            obj.SetParent(transform);
        }
        obj.localEulerAngles = new Vector3(0, 0, 0);
    }
}
