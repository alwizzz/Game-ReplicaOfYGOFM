using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


using Enums;

public class GuardianStarOptionSelector : MonoBehaviour
{
    [SerializeField] private GuardianStar selectedGuardianStar;

    [Header("Cache")]
    [SerializeField] private GuardianStar guardianStarOption1;
    [SerializeField] private Button button1;
    [SerializeField] private Image imageIcon1;
    [SerializeField] private TextMeshProUGUI text1;

    [SerializeField] private GuardianStar guardianStarOption2;
    [SerializeField] private Button button2;
    [SerializeField] private Image imageIcon2;
    [SerializeField] private TextMeshProUGUI text2;

    public void Setup(GuardianStar guardianStarOption1, GuardianStar guardianStarOption2)
    {
        this.guardianStarOption1 = guardianStarOption1;
        text1.text = guardianStarOption1.ToString();
        imageIcon1.sprite = IconManager.Instance().GetGuardianStarIcon(guardianStarOption1);

        this.guardianStarOption2 = guardianStarOption2;
        text2.text = guardianStarOption2.ToString();
        imageIcon2.sprite = IconManager.Instance().GetGuardianStarIcon(guardianStarOption2);

        SelectOption1(); // default value
    }

    public void SelectOption1()
    {
        if (selectedGuardianStar == guardianStarOption1) return;

        selectedGuardianStar = guardianStarOption1;
        button1.interactable = false;
        button2.interactable = true;
    }

    public void SelectOption2()
    {
        if (selectedGuardianStar == guardianStarOption2) return;

        selectedGuardianStar = guardianStarOption2;
        button1.interactable = true;
        button2.interactable = false;
    }

    public GuardianStar GetSelectedGuardianStar() => selectedGuardianStar;
}
