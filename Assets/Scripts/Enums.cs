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
        FocusPhase,
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
        Pluto // Thunder
    }

    public enum TrapEffect
    {
        None,
        DestroyMonsterOnAttack,
        DestroyMonsterOnSummon
    }

    public enum PhaseTrigger
    {
        Null,
        OnStartOfPlayerPhase,
        OnStartOfOpponentPhase,

        OnPlayerAttack,
        OnOpponentAttack,

        OnPlayerSummon,
        OnOpponentSummon,

    }


}


