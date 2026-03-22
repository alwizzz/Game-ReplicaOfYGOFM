using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;


public static class GuardianStarCalculator
{
    private const int BONUS_POWER_VALUE = 500;

    private static bool? GetInteraction(GuardianStar refA, GuardianStar refB)
    {
        bool? result = null;

        switch ((refA, refB))
        {
            // loop 1
            case (GuardianStar.Sun, GuardianStar.Moon): result = true; break;
            case (GuardianStar.Sun, GuardianStar.Mercury): result = false; break;

            case (GuardianStar.Mercury, GuardianStar.Sun): result = true; break;
            case (GuardianStar.Mercury, GuardianStar.Venus): result = false; break;

            case (GuardianStar.Venus, GuardianStar.Mercury): result = true; break;
            case (GuardianStar.Venus, GuardianStar.Moon): result = false; break;

            case (GuardianStar.Moon, GuardianStar.Venus): result = true; break;
            case (GuardianStar.Moon, GuardianStar.Sun): result = false; break;
 
            // loop 2
            case (GuardianStar.Mars, GuardianStar.Jupiter): result = true; break;
            case (GuardianStar.Mars, GuardianStar.Neptune): result = false; break;

            case (GuardianStar.Jupiter, GuardianStar.Saturn): result = true; break;
            case (GuardianStar.Jupiter, GuardianStar.Mars): result = false; break;

            case (GuardianStar.Saturn, GuardianStar.Uranus): result = true; break;
            case (GuardianStar.Saturn, GuardianStar.Jupiter): result = false; break;

            case (GuardianStar.Uranus, GuardianStar.Pluto): result = true; break;
            case (GuardianStar.Uranus, GuardianStar.Saturn): result = false; break;

            case (GuardianStar.Pluto, GuardianStar.Neptune): result = true; break;
            case (GuardianStar.Pluto, GuardianStar.Uranus): result = false; break;

            case (GuardianStar.Neptune, GuardianStar.Mars): result = true; break;
            case (GuardianStar.Neptune, GuardianStar.Pluto): result = false; break;

            default:
                {
                    Debug.Log("no interaction");
                    result = null;
                    break;
                }
        }

        return result;
    }

    public static void ApplyBonusPower(ref int refAPower, GuardianStar refAGuardianStar, 
        ref int refBPower, GuardianStar refBGuardianStar)
    {
        

        bool? result = GetInteraction(refAGuardianStar, refBGuardianStar);
        if(result == true)
        {
            refAPower += BONUS_POWER_VALUE;
            Debug.Log($"refA buffed to {refAPower} by guardian star effect ({refAGuardianStar} wins to {refBGuardianStar})");
        } else if(result == false)
        {
            refBPower += BONUS_POWER_VALUE;
            Debug.Log($"refB buffed to {refBPower} by guardian star effect ({refAGuardianStar} loses to {refBGuardianStar})");
        } else
        {
            Debug.Log($"guardian star has no interaction ({refAGuardianStar} and {refBGuardianStar})");
        }
    }
}
