using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;


public class GameplayCardUI : MonoBehaviour
{
    [SerializeField] private Card cardData;

    [Header("States")]
    [SerializeField] private bool isSelected;

    [Header("Caches")]
    [SerializeField] private Image baseImage;
    [SerializeField] private Image cardImage;
    [SerializeField] private GameObject cardAttributes;
    [SerializeField] private TextMeshProUGUI attackPointText;
    [SerializeField] private TextMeshProUGUI defensePointText;

}
