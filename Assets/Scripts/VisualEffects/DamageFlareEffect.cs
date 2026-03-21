using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class DamageFlareEffect : UIModal<DamageFlareEffect>
{
    [SerializeField] private Color damagedColor;
    [SerializeField] private Color undamagedColor;

    [Header("Caches")]
    [SerializeField] private Image flareImage;
    [SerializeField] private TextMeshProUGUI damageText;

    private void Awake()
    {
        BaseAwake(this);
    }

    public void SetupAndShow(int damage)
    {
        damage = Mathf.Abs(damage);
        if(damage == 0)
        {
            flareImage.color = undamagedColor;
            damageText.text = "";
        } else
        {
            flareImage.color = damagedColor;
            damageText.text = $"-{damage}";
        }

        Show();
    }
}
