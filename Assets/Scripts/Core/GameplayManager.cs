using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;

public class GameplayManager : StaticReference<GameplayManager>
{
    [Header("Game Parameters")]
    [SerializeField] private int initialLifePoint;

    [Header("States")]
    [SerializeField] private Side turn;
    [SerializeField] private Phase phase;

    [Header("Player's Caches")]
    [SerializeField] private HandSystem playerHandSystem;
    [SerializeField] private HandFocusSystem playerHandFocusSystem;
    [SerializeField] private FieldSystem playerFieldSystem;
    [SerializeField] private LifePointSystem playerLifePointSystem;

    [Header("Enemy's Caches")]
    [SerializeField] private HandSystem enemyHandSystem;
    [SerializeField] private HandFocusSystem enemyHandFocusSystem;
    [SerializeField] private FieldSystem enemyFieldSystem;
    [SerializeField] private LifePointSystem enemyLifePointSystem;


    [Header("Other Caches")]
    [SerializeField] private Transform offscreenParking;


    private void Awake()
    {
        BaseAwake(this);
    }

    private void Start()
    {
        Setup();
        //Debug();
    }

    private void Setup()
    {
        // Setup lifepoint
        playerLifePointSystem.Setup(initialLifePoint);
        enemyLifePointSystem.Setup(initialLifePoint);

    }

    private void Debug()
    {
        // spawn monster card on enemy field
        var cardData = Resources.Load<Card>("CardLibrary/022-NormalMonster-SummonedSkull");
        var fieldCardContainer = enemyFieldSystem.GetFrontRankContainers()[0];
        var fieldCard = enemyFieldSystem.DebugSpawnFieldCard(cardData, false, fieldCardContainer);
        fieldCard.SetToDefensePosition();
        //fieldCard.SetToFaceDown();
        print("DEBUG: spawned monster card on enemy field");
    }


    public bool IsPlayerTurn() => (turn == Side.Player ? true : false);


    #region Caches Getter

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

    #endregion

    #region Life Point
    public void UpdateLifePointAfterBattle(int damageDealt)
    {
        if (damageDealt == 0) return;

        if(damageDealt > 0)
        {
            if(turn == Side.Player)
            {
                enemyLifePointSystem.DecreaseLifePoint(damageDealt);
            } else
            {
                playerLifePointSystem.DecreaseLifePoint(damageDealt);
            }
        } else if(damageDealt < 0)
        {
            if (turn == Side.Player)
            {
                playerLifePointSystem.DecreaseLifePoint(damageDealt);
            }
            else
            {
                playerLifePointSystem.DecreaseLifePoint(damageDealt);
            }
        }
    }

    #endregion


    public void MoveToOffscreenParking(Transform obj)
    {
        obj.position = offscreenParking.position;
    }

    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}
