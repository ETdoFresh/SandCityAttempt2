using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SandCollection : MonoBehaviour
{
    public float _sand;
    PlayerActions _playerActions;
    Text _text;

    void Awake()
    {
        _playerActions = GetComponent<PlayerActions>();
        _text = GameObject.Find("SandCollected").GetComponent<Text>();
    }

    void OnEnable()
    {
        _playerActions.OnSandCollect += SandCollectCallback;
    }

    void OnDisable()
    {
        _playerActions.OnSandCollect -= SandCollectCallback;
    }

    void SandCollectCallback(float amountOfSand)
    {
        _sand += amountOfSand;
        _text.text = "Sand: " + _sand.ToString();
    }
}
