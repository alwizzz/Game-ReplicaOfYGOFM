using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;

public class FieldCard : GameplayCard
{
    [Header("Field Card States")]
    [SerializeField] private bool inAttackPosition;
    [SerializeField] private bool isFaceDown;
    [SerializeField] private bool hasBeenUsed;
    [SerializeField] private GuardianStar selectedGuardianStar;
    [SerializeField] private FieldCardContainer container;

    [Header("Field Card Caches")]
    [SerializeField] private GameObject faceDownImage;
    [SerializeField] private GameObject hasBeenUsedOverlay;

    public void SetContainer(FieldCardContainer container)
    {
        this.container = container;
    }

    public void ResetContainer() { container = null; }

    #region Update Card States
    public void SetToAttackPosition()
    {
        inAttackPosition = true;
        transform.localRotation = Quaternion.identity;
    }
    public void SetToDefensePosition()
    {
        inAttackPosition = false;
        transform.localRotation = Quaternion.Euler(0, 0, 90);
    }

    public void ChangePosition()
    {
        if (inAttackPosition)
        {
            SetToDefensePosition();
        } else
        {
            SetToAttackPosition();
        }
    }
    public bool InAttackPosition() => inAttackPosition;

    public void SetToFaceUp()
    {
        isFaceDown = false;
        faceDownImage.SetActive(false);
    }
    public void SetToFaceDown()
    {
        isFaceDown = true;
        faceDownImage.SetActive(true);
    }

    public void SetHasBeenUsed(bool value)
    { 
        hasBeenUsed = value;
        hasBeenUsedOverlay.SetActive(hasBeenUsed);
    }

    #endregion
    public bool HasBeenUsed() => hasBeenUsed;
    public bool IsFaceDown() => isFaceDown;
    public int GetPowerPoint()
    {
        var monsterCard = (MonsterCard)GetCardData();
        if (InAttackPosition())
        {
            return monsterCard.attackPoint;
        }
        else
        {
            return monsterCard.defensePoint;
        }
    }

    public void SetSelectedGuardianStar(GuardianStar value)
    {
        selectedGuardianStar = value;
    }
    public GuardianStar GetSelectedGuardianStar() => selectedGuardianStar;


    public void Destroy()
    {
        container.RemoveCard();
        print("Destroy Field Card " + GetCardData().cardName);
        Destroy(gameObject);
    }
}
