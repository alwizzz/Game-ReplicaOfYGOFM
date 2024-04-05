using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePointSystem : MonoBehaviour
{
    [SerializeField] private int lifePoint;

    public void Setup(int initialLifePoint)
    {
        lifePoint = initialLifePoint;
    }


    public void IncreaseLifePoint(int value)
    {
        value = Mathf.Abs(value);
        lifePoint += value;
        print($"{gameObject}: Increase life point by {value}");
    }

    public void DecreaseLifePoint(int value)
    {
        value = Mathf.Abs(value);
        lifePoint -= value;
        print($"{gameObject}: Decrease life point by {value}");

        if (lifePoint <= 0)
        {
            lifePoint = 0;
            // TRIGGER FINISH GAME
        }
    }
}
