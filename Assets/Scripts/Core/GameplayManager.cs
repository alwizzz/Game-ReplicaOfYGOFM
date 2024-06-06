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
    [SerializeField] private GameplayDeck playerDeck;

    [Header("Enemy's Caches")]
    [SerializeField] private HandSystem enemyHandSystem;
    [SerializeField] private HandFocusSystem enemyHandFocusSystem;
    [SerializeField] private FieldSystem enemyFieldSystem;
    [SerializeField] private LifePointSystem enemyLifePointSystem;
    [SerializeField] private GameplayDeck enemyDeck;


    [Header("Other Caches")]
    [SerializeField] private Transform offscreenParking;
    [SerializeField] private EnemyBot enemyBot;


    private void Awake()
    {
        BaseAwake(this);
    }

    private void Start()
    {
        Setup();
        Debug();
    }

    private void Setup()
    {
        // Setup EnemyBot
        enemyBot.Setup(enemyHandSystem, enemyHandFocusSystem, enemyFieldSystem);

        // Setup lifepoint
        playerLifePointSystem.Setup(initialLifePoint);
        enemyLifePointSystem.Setup(initialLifePoint);

        phase = Phase.EndPhase;
        turn = Side.Enemy;
        ChangeTurn();
    }

    private void Debug()
    {
        // spawn monster card on enemy field
        var cardData = Resources.Load<Card>("CardLibrary/022-NormalMonster-SummonedSkull");
        var fieldCardContainer = enemyFieldSystem.GetFrontRankContainers()[0];
        var fieldCard = enemyFieldSystem.DebugSpawnFieldCard(cardData, false, fieldCardContainer);
        fieldCard.SetToDefensePosition();
        //fieldCard.SetToFaceDown();
        //enemyFieldSystem.IncrementCardCount(false);
        print("DEBUG: spawned monster card on enemy field");

        // spawn monster card on player field
        cardData = Resources.Load<Card>("CardLibrary/022-NormalMonster-SummonedSkull");
        fieldCardContainer = playerFieldSystem.GetFrontRankContainers()[0];
        fieldCard = playerFieldSystem.DebugSpawnFieldCard(cardData, false, fieldCardContainer);
        //fieldCard.SetToDefensePosition();
        //fieldCard.SetToFaceDown();
        //playerFieldSystem.IncrementCardCount(false);
        print("DEBUG: spawned monster card on player field");
    }

    public bool IsPlayerTurn() => (turn == Side.Player ? true : false);

    public void MoveToOffscreenParking(Transform obj)
    {
        obj.position = offscreenParking.position;
    }

    private void NextTurn()
    {
        if(IsPlayerTurn())
        {
            turn = Side.Enemy;
        } else
        {
            turn = Side.Player;
        }
    }

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

    public GameplayDeck Deck()
    {
        if (IsPlayerTurn())
        {
            return playerDeck;
        }
        return enemyDeck;
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

    #region Phase Management

    public void ChangeTurn()
    {
        if (phase != Phase.EndPhase)
        {
            print("WARNING: unmatching phase, aborting...");
            return;
        }

        NextTurn();
        UpdateGameplayField();
        
        
        StartCoroutine(DelayerByWaitUntil(
            predicate: () => GameplayFieldManager.Instance().IsRotating() == false,
            action: () =>
            {
                phase = Phase.DrawPhase;
                EventManager.DrawPhase();
                
            }
        ));
    }

    public void ToHandPhase()
    {
        if (phase != Phase.DrawPhase)
        {
            print("WARNING: unmatching phase, aborting...");
            return;
        }

        phase = Phase.HandPhase;
        EventManager.HandPhase();
        if (IsPlayerTurn() == false) enemyBot.StartPlaying();
    }

    public void ToFocusPhase(HandCard card)
    {
        if (phase != Phase.HandPhase)
        {
            print("WARNING: unmatching phase, aborting...");
            return;
        }

        HandFocusSystem()
            .SetupAndShow(card);

        phase = Phase.FocusPhase;
        EventManager.FocusPhase();
    }

    public void ToFieldPhase(Card card, bool isFaceDown, GuardianStar guardianStar)
    {
        if (phase != Phase.FocusPhase)
        {
            print("WARNING: unmatching phase, aborting...");
            return;
        }

        FieldSystem().SpawnFieldCard(
            card, isFaceDown, guardianStar
        );
        FieldSystem().StartFieldPhase();

        phase = Phase.FieldPhase;
        EventManager.FieldPhase();
    }

    public void ToEndPhase()
    {
        if (phase != Phase.FieldPhase)
        {
            print("WARNING: unmatching phase, aborting...");
            return;
        }


        phase = Phase.EndPhase;
        EventManager.EndPhase();

        Invoke(nameof(ChangeTurn), 1f);
    }

    #endregion

    #region Listener Methods

    private void OpenHand()
    {
        if (IsPlayerTurn())
        {
            playerHandSystem.OpenHand();
            return;
        }
        enemyHandSystem.OpenHand();
    }

    private void ResetFieldInformationDisplays()
    {
        playerFieldSystem.UpdateInformationDisplay(reset: true);
        enemyFieldSystem.UpdateInformationDisplay(reset: true);
    }

    #endregion

    private IEnumerator DelayerByWaitUntil(System.Func<bool> predicate, System.Action action)
    {
        yield return new WaitUntil(predicate);
        action();
    }

    private void UpdateGameplayField()
    {
        if (IsPlayerTurn())
        {
            GameplayFieldManager.Instance().FlipToPlayerSide();
        }
        else
        {
            GameplayFieldManager.Instance().FlipToEnemySide();
        }
    }


    #region Subcriptions

    private void OnEnable()
    {
        EventManager.OnDrawPhase += OpenHand;
        EventManager.OnEndPhase += ResetFieldInformationDisplays;
    }

    private void OnDisable()
    {
        EventManager.OnDrawPhase -= OpenHand;
        EventManager.OnEndPhase -= ResetFieldInformationDisplays;
    }

    #endregion




    private void OnDestroy()
    {
        BaseOnDestroy();
    }



}
