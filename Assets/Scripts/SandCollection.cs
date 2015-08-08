using UnityEngine;
using System.Collections;

public class SandCollection : MonoBehaviour
{
    public int _sand;
    ControlTerrainDig _terrainDig;

    void Awake()
    {
        _terrainDig = GetComponentInChildren<ControlTerrainDig>();
    }

    void OnEnable()
    {
        _terrainDig.SandDig += CollectSand;
        _terrainDig.TakeSand += TakeSand;
    }

    void OnDisable()
    {
        _terrainDig.SandDig -= CollectSand;
        _terrainDig.TakeSand -= TakeSand;
    }

    void CollectSand(int amountOfSand)
    {
        _sand += amountOfSand;
    }

    int TakeSand(int amountOfSand)
    {
        int takenSand = amountOfSand > _sand ? _sand : amountOfSand;
        _sand -= takenSand;
        return takenSand;
    }

    public int Sand { get { return _sand; } }
}
