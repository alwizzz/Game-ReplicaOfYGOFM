using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;

public class GameplayManager : StaticReference<GameplayManager>
{
    [SerializeField] private Side turn;
    [SerializeField] private Phase phase;

    [Header("Player Parameters")]
    [SerializeField] private HandSystem playerHandSystem;
    [SerializeField] private HandFocusSystem playerHandFocusSystem;
    [SerializeField] private FieldSystem playerFieldSystem;

    [Header("Enemy Parameters")]
    [SerializeField] private HandSystem enemyHandSystem;
    [SerializeField] private HandFocusSystem enemyHandFocusSystem;
    [SerializeField] private FieldSystem enemyFieldSystem;

    [Header("Caches")]
    [SerializeField] private Transform offscreenParking;


    private void Awake()
    {
        BaseAwake(this);
    }

    private void Start()
    {
        Debug();
    }

    private void Debug()
    {
        // spawn monster card on enemy field
        var cardData = Resources.Load<Card>("CardLibrary/022-NormalMonster-SummonedSkull");
        var fieldCardContainer = enemyFieldSystem.GetFrontRankContainers()[0];
        enemyFieldSystem.DebugSpawnFieldCard(cardData, false, fieldCardContainer);
        print("DEBUG: spawned monster card on enemy field");
    }


    public bool IsPlayerTurn() => (turn == Side.Player ? true : false);


    // considering turn,
    // on player turn, opponent is the enemy
    // on enemy turn, opponent is the player

    public HandSystem HandSystem()
    {
        if(IsPlayerTurn())
        {
            return playerHandSystem;
        }
        return enemyHandSystem;
    }
    // getting enemy's hand system is currently unnecessary

    public HandFocusSystem HandFocusSystem()
    {
        if (IsPlayerTurn())
        {
            return playerHandFocusSystem;
        }
        return enemyHandFocusSystem;
    }
    // getting enemy's hand focus system is currently unnecessary


    public FieldSystem FieldSystem()
    {
        if (IsPlayerTurn())
        {
            return playerFieldSystem;
        }
        return enemyFieldSystem;
    }
    public FieldSystem OpponentFieldSystem()
    {
        if (IsPlayerTurn())
        {
            return enemyFieldSystem;
        }
        return playerFieldSystem;
    }


    // direct getter without considering which turn
    public HandSystem PlayerHandSystem() => playerHandSystem;
    public HandSystem EnemyHandSystem() => enemyHandSystem;
    public HandFocusSystem PlayerHandFocusSystem() => playerHandFocusSystem;
    public HandFocusSystem EnemyHandFocusSystem() => enemyHandFocusSystem;
    public FieldSystem PlayerFieldSystem() => playerFieldSystem;
    public FieldSystem EnemyFieldSystem() => enemyFieldSystem;


    public void SetAttackedOnOpponentInBattle()
    {
        var opponentSelectedFieldContainer = OpponentFieldSystem().GetSelectedFieldContainer();
        if (opponentSelectedFieldContainer == null)
        {
            print("ERROR: currently no selected field container on opponent");
            return;
        }

        opponentSelectedFieldContainer.SetAsAttackedInBattle();
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
