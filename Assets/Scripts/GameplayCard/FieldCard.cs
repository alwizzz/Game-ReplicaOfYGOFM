using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;

public class FieldCard : GameplayCard
{
    [Header("Field Card")]
    [SerializeField] private GuardianStar selectedGuardianStar;
    [SerializeField] private bool inAttackPosition;
    [SerializeField] private bool isFaceDown;
    [SerializeField] private FieldCardContainer container;
    [SerializeField] private GameObject faceDownImage;

    public void SetContainer(FieldCardContainer container)
    {
        this.container = container;
    }

    public void ResetContainer() { container = null; }

    public void SetToAttackPosition()
    {
        inAttackPosition = true;
        transform.rotation = Quaternion.identity;
    }
    public void SetToDefensePosition()
    {
        inAttackPosition = false;
        transform.rotation = Quaternion.Euler(0, 0, 90);
    }

    public void ChangePosition()
    {
        if(inAttackPosition)
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
