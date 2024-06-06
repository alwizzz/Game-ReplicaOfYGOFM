using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LifePointSystem : MonoBehaviour
{
    [SerializeField] private int lifePoint;
    [SerializeField] private TextMeshProUGUI lpText;

    public void Setup(int initialLifePoint)
    {
        lifePoint = initialLifePoint;
        UpdateLPText();
    }

    public void IncreaseLifePoint(int value)
    {
        value = Mathf.Abs(value);
        lifePoint += value;
        print($"{gameObject}: Increase life point by {value}");
        UpdateLPText();
    }

    public void DecreaseLifePoint(int value)
    {
        value = Mathf.Abs(value);
        lifePoint -= value;
        print($"{gameObject}: Decrease life point by {value}");
        UpdateLPText();

        if (lifePoint <= 0)
        {
            lifePoint = 0;
            // TRIGGER FINISH GAME
        }
    }

    private void UpdateLPText()
    {
        lpText.text = "LP " + lifePoint.ToString();
    }
}
