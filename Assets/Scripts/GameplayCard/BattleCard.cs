using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Enums;
// using System.Drawing;
using System.Resources;

public class BattleCard : GameplayCard
{
    [Header("Battle Card")]
    [SerializeField] private bool inAttackPosition;
    [SerializeField] private GameObject faceDownImage;
    [SerializeField] private GameObject AttackPanelOverlay;
    [SerializeField] private GameObject DefensePanelOverlay;

    public void SetupBattleCard(Card cardData, bool inAttackPosition)
    {
        base.Setup(cardData);
        this.inAttackPosition = inAttackPosition;

        UpdateOverlay();
    }

    private void UpdateOverlay()
    {
        if(inAttackPosition)
        {
            AttackPanelOverlay.SetActive(false);
            DefensePanelOverlay.SetActive(true);
        } else
        {
            AttackPanelOverlay.SetActive(true);
            DefensePanelOverlay.SetActive(false);
        }
    }

    public void PlayBonusPowerTextAnimation(bool targetsAttackPoint, int bonusValue, float animationDuration)
    {
        int originalValue;
        TextMeshProUGUI textRef; // default value purpose

        if(targetsAttackPoint)
        {
            originalValue = attackPoint;  
            textRef = attackPointText; 
        } else
        {
            originalValue = defensePoint;    
            textRef = defensePointText; 
        }

        int targetValue = originalValue + bonusValue;

        StartCoroutine(
            AnimateNumber(originalValue, targetValue, animationDuration, textRef)
        );
    }

    private IEnumerator AnimateNumber(int startValue, int targetValue, float duration, TextMeshProUGUI textRef)
    {
        print("Text Animation started");

        Color startColor = ResourceManager.Instance().GetNumberColor(true); // normal color
        Color targetColor = ResourceManager.Instance().GetNumberColor(false); // bonused color

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            int currentValue = Mathf.RoundToInt(
                Mathf.Lerp(startValue, targetValue, t)
            );
            textRef.text = currentValue.ToString();

            // Color lerp
            var lerpedColor = Color.Lerp(startColor, targetColor, t);
            textRef.color = lerpedColor;

            // print($"Text Animation {lerpedColor}, {startColor}, {targetColor}");
            yield return null;
        }
        // Ensure exact final value
        textRef.text = targetValue.ToString();

        print("Text Animation ended");
    }

    public bool InAttackPosition() => inAttackPosition;

}
