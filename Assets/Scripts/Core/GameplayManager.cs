using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Enums;
using Unity.VisualScripting;

public class GameplayManager : StaticReference<GameplayManager>
{
    [Header("Game Parameters")]
    [SerializeField] private int initialLifePoint;
    [SerializeField] private int guardianStarBonusPower;

    [Header("States")]
    [SerializeField] private Side turn;
    [SerializeField] private Phase phase;
    [SerializeField] private bool inputLock;
    [SerializeField] private FieldType fieldType = FieldType.Normal; // default

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
    [SerializeField] private Image fieldBaseImage;




    private void Awake()
    {
        BaseAwake(this);
    }

    IEnumerator Start()
    {
        yield return null;

        Setup();
        Debug();
        GameplayFieldManager.Instance().RefreshFieldCardOrientation();

        SetFieldType(fieldType, true);
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
        // --------- ENEMY

        // spawn monster card on enemy field
        var cardData = Resources.Load<Card>("CardLibrary/022-NormalMonster-SummonedSkull");
        var fieldCardContainer = enemyFieldSystem.GetFrontRankContainers()[0];
        var fieldCard = enemyFieldSystem.DebugSpawnFieldCard(cardData, false, fieldCardContainer);

        // // spawn trap card on enemy field
        // cardData = Resources.Load<Card>("CardLibrary/682-Trap-Eatgaboon");
        // fieldCardContainer = enemyFieldSystem.GetBackRankContainers()[0];
        // fieldCard = enemyFieldSystem.DebugSpawnFieldCard(cardData, true, fieldCardContainer);

        // --------- PLAYER

        // spawn monster card on player field
        cardData = Resources.Load<Card>("CardLibrary/022-NormalMonster-SummonedSkull");
        fieldCardContainer = playerFieldSystem.GetFrontRankContainers()[0];
        fieldCard = playerFieldSystem.DebugSpawnFieldCard(
            cardData, 
            false, 
            fieldCardContainer,
            new List<GameplayCard.Modifier>{
                new GameplayCard.Modifier(1000, 1000, Resources.Load<Card>("CardLibrary/657-EquipSpell-Megamorph"))
            }
        );

        // spawn monster card on player field [[ TO TEST OUT RITUAL ]]
        cardData = Resources.Load<Card>("CardLibrary/001-NormalMonster-BlueEyesWhiteDragon");
        fieldCardContainer = playerFieldSystem.GetFrontRankContainers()[1];
        fieldCard = playerFieldSystem.DebugSpawnFieldCard(
            cardData, 
            false, 
            fieldCardContainer,
            new List<GameplayCard.Modifier>{
                new GameplayCard.Modifier(1000, 1000, Resources.Load<Card>("CardLibrary/657-EquipSpell-Megamorph"))
            }
        );
        // spawn monster card on player field
        cardData = Resources.Load<Card>("CardLibrary/001-NormalMonster-BlueEyesWhiteDragon");
        fieldCardContainer = playerFieldSystem.GetFrontRankContainers()[2];
        fieldCard = playerFieldSystem.DebugSpawnFieldCard(
            cardData, 
            false, 
            fieldCardContainer,
            new List<GameplayCard.Modifier>{
                new GameplayCard.Modifier(1000, 1000, Resources.Load<Card>("CardLibrary/657-EquipSpell-Megamorph"))
            }
        );
        // spawn monster card on player field
        cardData = Resources.Load<Card>("CardLibrary/001-NormalMonster-BlueEyesWhiteDragon");
        fieldCardContainer = playerFieldSystem.GetFrontRankContainers()[3];
        fieldCard = playerFieldSystem.DebugSpawnFieldCard(
            cardData, 
            false, 
            fieldCardContainer,
            new List<GameplayCard.Modifier>{
                new GameplayCard.Modifier(1000, 1000, Resources.Load<Card>("CardLibrary/657-EquipSpell-Megamorph"))
            }
        );

        // spawn magic card on player field
        cardData = Resources.Load<Card>("CardLibrary/657-EquipSpell-Megamorph");
        fieldCardContainer = playerFieldSystem.GetBackRankContainers()[0];
        fieldCard = playerFieldSystem.DebugSpawnFieldCard(cardData, true, fieldCardContainer);

        // spawn magic card on player field
        cardData = Resources.Load<Card>("CardLibrary/335-FieldSpell-Yami");
        fieldCardContainer = playerFieldSystem.GetBackRankContainers()[1];
        fieldCard = playerFieldSystem.DebugSpawnFieldCard(cardData, true, fieldCardContainer);

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
    public int GuardianStarBonusPower() => guardianStarBonusPower;

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

    public void IncreaseLifePointFromSpell(int value, bool forEnemy = false)
    {   
        // normally the effect is for self
        if (forEnemy)
        {
            enemyLifePointSystem.IncreaseLifePoint(value);
        } else
        {
            playerLifePointSystem.IncreaseLifePoint(value);
        }
    }
    public void DecreaseLifePointFromSpell(int value, bool forSelf = false)
    {   
        // normally the effect is for enemy
        if (forSelf)
        {
            playerLifePointSystem.DecreaseLifePoint(value);
        } else
        {
            enemyLifePointSystem.DecreaseLifePoint(value);
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

    // public void ToFocusPhase(HandCard card)
    // {
    //     if (phase != Phase.HandPhase)
    //     {
    //         print("WARNING: unmatching phase, aborting...");
    //         return;
    //     }

    //     HandFocusSystem()
    //         .SetupAndShow(card);

    //     phase = Phase.FocusPhase;
    //     EventManager.FocusPhase();
    // }

    public void ToFieldPhase(
        Card card, 
        bool isFaceDown, 
        GuardianStar guardianStar, 
        List<GameplayCard.Modifier> modifierList,
        bool retainedMonster = false
    ){
        // if (phase != Phase.FocusPhase)
        if (phase != Phase.HandPhase)
        {
            print("WARNING: unmatching phase, aborting...");
            return;
        }

        if(!card.IsMonsterCard() && !isFaceDown)
        {
            // TODO: activate the non monster card
            ((NonMonsterCard)card).Activate();
        } else
        {
            FieldSystem().SpawnFieldCard(card, isFaceDown, guardianStar, modifierList, retainedMonster);
        }
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

    #region Full Field Control

    // public void StartFreeSelection()
    // {
    //     enemyFieldSystem.OpenFullSelection(false); 
    //     // player on last order so selected field card continer is on player's
    //     playerFieldSystem.OpenFullSelection(false); 
    // }
    // public void StopFreeSelection()
    // {
    //     enemyFieldSystem.CloseSelection(); 
    //     playerFieldSystem.CloseSelection(); 
    // }

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

    private void RefreshFieldStatus()
    {
        var currentField = FieldSystem();
        currentField.RefreshStatus();
    }

    #region Input Lock

    public void SetInputLock(bool value) { inputLock = value; }
    public bool IsInputLock() => inputLock;

    #endregion

    #region Field Spell

    public void SetFieldType(FieldType value, bool init = false)
    {
        if(!init && fieldType == value)
        {
            print($"attempt to set field type of same value ({value}), nothing happened");
            return;
        }

        var fieldColor = ResourceManager.Instance().GetFieldColor(value);
        var fieldContainerColor = ResourceManager.Instance().GetFieldContainerColor(value);

        void _HandleModifier(FieldCardContainer e)
        {
            e.gameObject.GetComponent<Image>().color = fieldContainerColor;
            if(e.IsEmpty()) return;
            if(!e.GetCard().GetCardData().IsMonsterCard()) return;

            SetFieldModifier(e.GetCard(), value);
        }
        PlayerFieldSystem().GetAllContainers().ForEach(e =>
        {
            _HandleModifier(e);
        });
        EnemyFieldSystem().GetAllContainers().ForEach(e =>
        {
            _HandleModifier(e);
        });

        fieldBaseImage.color = fieldColor;
        fieldType = value;
        print($"succeed setting field type to {value}");
    }

    private void SetFieldModifier(GameplayCard gCard, FieldType fieldType)
    {
        var modifierListRef = gCard.GetModifierList();
        if(modifierListRef == null){ return; }
        var modifierList = new List<GameplayCard.Modifier>(modifierListRef); // creating a copy

        var fieldCardData = ResourceManager.Instance().GetFieldCard(fieldType);
        
        // first, remove existing field modifier if any
        var toBeRemoved = new List<GameplayCard.Modifier>();
        for(int i=0; i<modifierList.Count; i++)
        {
            var modifier = modifierList[i];
            var card = modifier.source;
            if(card.IsMonsterCard()) continue;
            
            var spellCard = (SpellCard)card;
            if(spellCard.GetSpellCardType() != SpellCardType.Field) continue;

            toBeRemoved.Add(modifier);
        }
        // NOTE: validate flow by looking on toBeRemoved.Count
        print($"DEBUG toBeRemoved.Count: {toBeRemoved.Count} {gCard}");
        toBeRemoved.ForEach(e => modifierList.Remove(e));

        // second, update field modifier
        int value = int.Parse(fieldCardData.id); // dummy
        modifierList.Add(new GameplayCard.Modifier( 
            value, value, fieldCardData
        ));

        // reassignment
        gCard.SetModifierList(modifierList);
    }


    #endregion


    #region Trap Activation

    private void CheckTrapOnMonsterSummoned(FieldCard monsterFieldCard, Side side)
    {
        // get opposite side's containers
        List<FieldCardContainer> containers;
        if(side == Side.Player) 
        {
            containers = EnemyFieldSystem().GetAllContainers();
        } else
        {
            containers = PlayerFieldSystem().GetAllContainers();
        }

        // list all traps that canTrigger
        List<FieldCard> canTriggerTrapList = new();
        containers.ForEach(e =>
        {
            if(e.IsEmpty()) return;

            var fieldCard = e.GetCard();
            var card = fieldCard.GetCardData();

            if(card is not NonMonsterCard nonMonsterCard) return;
            if(nonMonsterCard is not TrapCard trapCard) return;
            if(trapCard.trigger != TrapTrigger.OnMonsterSummoned) return;

            var canTrigger = trapCard.Check(new TrapCard.Context(monsterFieldCard));
            if(!canTrigger) return;

            canTriggerTrapList.Add(fieldCard);
        });

        if(canTriggerTrapList.Count == 0)
        {
            print("DEBUG: no OnMonsterSummoned trap can be triggered");
            return;   
        }

        // handle if there are multiple trap cards that can be triggered
        FieldCard toBeTriggeredTrapFieldCard;
        if(canTriggerTrapList.Count == 1)
        {
            toBeTriggeredTrapFieldCard = canTriggerTrapList[0];
        } else
        {
            // TODO: actually handle the choosing of which trap to be triggered
            toBeTriggeredTrapFieldCard = canTriggerTrapList[0];
        }

        StartCoroutine(ActivateTrapOnMonsterSummoned(toBeTriggeredTrapFieldCard, monsterFieldCard));
    }

    private IEnumerator ActivateTrapOnMonsterSummoned(FieldCard trapFieldCard, FieldCard monsterFieldCard)
    {
        SetInputLock(true);

        yield return new WaitForSeconds(1f);

        // set the trap face up
        trapFieldCard.SetToFaceUp();

        yield return new WaitForSeconds(1f);

        // activate trap
        TrapCard trapCard = (TrapCard)trapFieldCard.GetCardData();
        bool succeed = trapCard.Activate(new TrapCard.Context(monsterFieldCard));

        yield return new WaitForSeconds(1f);

        // destroy trap
        trapFieldCard.Destroy();

        yield return new WaitForSeconds(1f);

        print($"XXX {trapFieldCard.GetCardData().cardName} [Trap] is activated with target {monsterFieldCard.GetCardData().cardName} [Monster] (succeed:{succeed})");
        SetInputLock(false);
    }

    #endregion


    #region Subcriptions

    // functions put here are more of a wrapper
    private void SetFieldModifierOnMonsterSummoned(FieldCard monsterFieldCard, Side _)
    {
        print("XXX");
        SetFieldModifier(monsterFieldCard, fieldType);
    }



    private void OnEnable()
    {
        EventManager.OnDrawPhase += OpenHand;
        EventManager.OnDrawPhase += RefreshFieldStatus;

        EventManager.OnEndPhase += ResetFieldInformationDisplays;

        EventManager.OnMonsterSummoned += SetFieldModifierOnMonsterSummoned;
        EventManager.OnMonsterSummoned += CheckTrapOnMonsterSummoned;
    }

    private void OnDisable()
    {
        EventManager.OnDrawPhase -= OpenHand;
        EventManager.OnDrawPhase -= RefreshFieldStatus;

        EventManager.OnEndPhase -= ResetFieldInformationDisplays;

        EventManager.OnMonsterSummoned -= SetFieldModifierOnMonsterSummoned;
        EventManager.OnMonsterSummoned -= CheckTrapOnMonsterSummoned;
    }

    #endregion




    private void OnDestroy()
    {
        BaseOnDestroy();
    }



}
