using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    #region Phases

    public delegate void DrawPhaseAction();
    public static event DrawPhaseAction OnDrawPhase;

    public static void DrawPhase()
    {
        print("DrawPhase flag raised!");
        OnDrawPhase?.Invoke();
    }

    public delegate void HandPhaseAction();
    public static event HandPhaseAction OnHandPhase;

    public static void HandPhase()
    {
        print("HandPhase flag raised!");
        OnHandPhase?.Invoke();
    }

    // public delegate void FocusPhaseAction();
    // public static event FocusPhaseAction OnFocusPhase;

    // public static void FocusPhase()
    // {
    //     print("FocusPhase flag raised!");
    //     OnFocusPhase?.Invoke();
    // }

    public delegate void FieldPhaseAction();
    public static event FieldPhaseAction OnFieldPhase;

    public static void FieldPhase()
    {
        print("FieldPhase flag raised!");
        OnFieldPhase?.Invoke();
    }

    public delegate void EndPhaseAction();
    public static event EndPhaseAction OnEndPhase;

    public static void EndPhase()
    {
        print("EndPhase flag raised!");
        OnEndPhase?.Invoke();
    }


    #endregion
}
