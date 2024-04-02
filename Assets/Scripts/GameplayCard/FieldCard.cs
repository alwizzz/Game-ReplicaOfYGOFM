using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCard : GameplayCard
{
    [Header("Field Card")]
    [SerializeField] private bool inAttackMode;
    [SerializeField] private bool isFaceDown;
    [SerializeField] private FieldCardContainer container;
    [SerializeField] private GameObject faceDownImage;

    public void SetContainer(FieldCardContainer container)
    {
        this.container = container;
    }

    public void ResetContainer() { container = null; }

    public void SetToAttackMode()
    {
        inAttackMode = true;
        transform.rotation = Quaternion.identity;
    }
    public void SetToDefenseMode()
    {
        inAttackMode = false;
        transform.rotation = Quaternion.Euler(0, 0, 90);
    }

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
}
