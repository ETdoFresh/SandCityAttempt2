using UnityEngine;
using System.Collections;

public class PlayerActions : MonoBehaviour {

    public static SandFingerHandler OnSandFinger;
    public static ScoopHandler OnScoop;
    public static ScoopFinishHandler OnScoopFinish;
    public static SmoothHandler OnSmooth;
    public static GrabHandler OnGrab;
    public static LiftFingerHandler OnLiftFinger;
    public SandCollectHandler OnSandCollect;
    public delegate void SandFingerHandler(Vector3 position, int digSize, float digRate);
    public delegate float ScoopHandler(Vector3 position, int digSize, float digRate);
    public delegate void SmoothHandler(Vector3 position, int digSize, float digRate);
    public delegate void GrabHandler(Vector3 position);
    public delegate void LiftFingerHandler(PlayerActions playerActionsScript);
    public delegate void ScoopFinishHandler(PlayerActions playerActionsScript, Vector3 position);
    public delegate void SandCollectHandler(float sandCollected);

    public enum PlayerAction { FINGER, SCOOP, SMOOTH, GRAB }
    public PlayerAction _selectedAction = PlayerAction.FINGER;

    public int fingerDigSize = 1;
    public float fingerDigRate = 0.01f;
    public int scoopDigSize = 5;
    public float scoopDigRate = 0.01f;
    public int smoothDigSize = 7;
    public float smoothDigRate = 0.01f;

    void OnEnable()
    {
        InputFirstPerson.OnSandAction += PerformAction;
        InputFirstPerson.OnSandActionComplete += FinishAction;
    }

    void OnDisable()
    {
        InputFirstPerson.OnSandAction -= PerformAction;
        InputFirstPerson.OnSandActionComplete -= FinishAction;
    }

    void PerformAction(Vector3 position)
    {
        switch (_selectedAction)
        {
            case PlayerAction.FINGER:
                OnSandFinger(position, fingerDigSize, fingerDigRate);
                break;
            case PlayerAction.SCOOP:
                float sandScooped = OnScoop(position, scoopDigSize, scoopDigRate);
                OnSandCollect(sandScooped);
                break;
            default:
                break;
        }
    }

    void FinishAction(Vector3 position)
    {
        switch (_selectedAction)
        {
            case PlayerAction.FINGER:
                OnLiftFinger(this);
                break;
            case PlayerAction.SCOOP:
                OnScoopFinish(this, position);
                break;
            default:
                break;
        }
    }
}