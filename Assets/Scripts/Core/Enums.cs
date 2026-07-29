using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enums
{
    public enum Side
    { 
        Player,
        Enemy
    }
    public enum Phase
    {
        DrawPhase,
        HandPhase,
        // FocusPhase,
        FieldPhase,
        EndPhase
    }



    public enum MonsterType
    {
        Aqua,
        Beast,
        BeastWarrior,
        Dinosaur,
        Dragon,
        Fairy,
        Fiend,
        Fish,
        Insect,
        Machine,
        Plant,
        Pyro,
        Reptile,
        Rock,
        SeaSerpent,
        Spellcaster,
        Thunder,
        Warrior,
        WingedBeast,
        Zombie
    }

    public enum GuardianStar
    { 
        Sun, // White Magic
        Mercury, // Black Magic
        Venus, // Illusion Magic
        Moon, // Demon Magic
        Mars, // Fire
        Jupiter, // Forest
        Saturn, // Wind
        Uranus, // Earth
        Neptune, // Water
        Pluto, // Thunder

        NONE, // used for non monster
    }

    public enum SpellCardType
    {
        Normal,
        Equip,
        Field,
        Ritual,

    }

    public enum TrapTrigger
    {
        OnAttack,

        OnMonsterSummoned,

        OnSpellActivated,

        OnLifePointChanged,
    }

    public enum EventTrigger
    {
        // use case: sword of revealing light
        OnStartOfPlayerPhase,
        OnStartOfOpponentPhase,

        // use case: trap cards
        OnPlayerAttack,
        OnOpponentAttack,

        // use case: field spell
        OnPlayerSummon,
        OnOpponentSummon,
    }

    public enum FusionResultType
    {
        Rejected,
        Fused,
        Equipped // technically can be said as subtype of fused
    }

    public enum FieldType
    {
        Normal, 
        Forest,
        Mountain,
        Sogen,
        Umi,
        Wasteland,
        Yami,
    }
}


