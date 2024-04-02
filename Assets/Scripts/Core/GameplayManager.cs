using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : StaticReference<GameplayManager>
{
    public enum Turn
    {
        Player,
        Opponent
    }

    public enum Phase
    {
        DrawPhase,
        HandPhase,
        FocusPhase,
        FieldPhase,
        EndPhase
    }

    [SerializeField] private Turn turn;
    [SerializeField] private Phase phase;

    [Header("Player Parameters")]
    [SerializeField] private HandSystem playerHandSystem;
    [SerializeField] private HandFocusSystem playerHandFocusSystem;
    [SerializeField] private FieldSystem playerFieldSystem;

    [Header("Opponent Parameters")]
    [SerializeField] private HandSystem opponentHandSystem;
    [SerializeField] private HandFocusSystem opponentHandFocusSystem;
    [SerializeField] private FieldSystem opponentFieldSystem;

    [Header("Caches")]
    [SerializeField] private Transform offscreenParking;


    private void Awake()
    {
        BaseAwake(this);
    }

    public bool IsPlayerTurn() => (turn == Turn.Player ? true : false);

    public HandSystem HandSystem()
    {
        if(IsPlayerTurn())
        {
            return playerHandSystem;
        }
        return opponentHandSystem;
    }
    // getting opponent's hand system is currently unnecessary

    public HandFocusSystem HandFocusSystem()
    {
        if (IsPlayerTurn())
        {
            return playerHandFocusSystem;
        }
        return opponentHandFocusSystem;
    }
    // getting opponent's hand focus system is currently unnecessary


    public FieldSystem FieldSystem()
    {
        if (IsPlayerTurn())
        {
            return playerFieldSystem;
        }
        return opponentFieldSystem;
    }
    public FieldSystem OpponentFieldSystem()
    {
        if (IsPlayerTurn())
        {
            return opponentFieldSystem;
        }
        return playerFieldSystem;
    }








    public void MoveToOffscreenParking(Transform obj)
    {
        obj.position = offscreenParking.position;
    }

    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
