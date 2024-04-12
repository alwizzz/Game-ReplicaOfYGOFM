using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayFieldManager : StaticReference<GameplayFieldManager>
{
    [Tooltip("If flipped then it is on Enemy Side")]
    [SerializeField] private bool isFlipped;
    [SerializeField] private bool isRotating;
    [SerializeField] private float rotateSpeed;

    [Header("Caches")]
    [SerializeField] private Transform gameplayField;
    [SerializeField] private GameObject playerFieldCardInformationDisplay;
    [SerializeField] private GameObject enemyFieldCardInformationDisplay;


    private void Awake()
    {
        BaseAwake(this);
        isFlipped = false;
    }

    public void FlipToPlayerSide()
    {
        if (!isFlipped) return;
        StartCoroutine(Flip(false));
    }

    public void FlipToEnemySide()
    {
        if (isFlipped) return;
        StartCoroutine(Flip(true));
    }

    private IEnumerator Flip(bool toBeFlipped)
    {
        isRotating = true;
        System.Func<bool> conditionLambda = () =>
        {
            float currentAngle = gameplayField.rotation.eulerAngles.z;
            //print(currentAngle);
            if (toBeFlipped)
            {
                return currentAngle < 180f;
            }
            else
            {
                return currentAngle >= 180f;
            }
        };

        playerFieldCardInformationDisplay.SetActive(false);
        enemyFieldCardInformationDisplay.SetActive(false);

        while (conditionLambda())
        {
            gameplayField.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
            yield return null;
        }
        gameplayField.rotation = Quaternion.Euler(
            0f,
            0f,
            toBeFlipped ? 180f : 0f
        );

        SwapInformationDisplays();
        playerFieldCardInformationDisplay.SetActive(true);
        enemyFieldCardInformationDisplay.SetActive(true);

        isFlipped = !isFlipped;
        isRotating = false;
    }

    private void SwapInformationDisplays()
    {
        var tempPosition = playerFieldCardInformationDisplay.transform.position;
        playerFieldCardInformationDisplay.transform.position = enemyFieldCardInformationDisplay.transform.position;
        enemyFieldCardInformationDisplay.transform.position = tempPosition;
    }

    public bool IsRotating() => isRotating;

    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
