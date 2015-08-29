using UnityEngine;
using System.Collections;

public class PlayerActions : MonoBehaviour {

    public static SandFingerHandler OnSandFinger;
    public delegate void SandFingerHandler(Vector3 position);

    public enum PlayerAction { FINGER, SCOOP, PAT, GRAB }
    PlayerAction _selectedAction = PlayerAction.FINGER;

    void OnEnable()
    {
        InputFirstPerson.OnSandAction += PerformAction;
    }

    void OnDisable()
    {
        InputFirstPerson.OnSandAction -= PerformAction;
    }

    void PerformAction(Vector3 here)
    {
        switch (_selectedAction)
        {
            case PlayerAction.FINGER:
                OnSandFinger(here);
                break;
            default:
                break;
        }
    }
}
