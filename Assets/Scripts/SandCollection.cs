using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SandCollection : MonoBehaviour
{
    public int _sand;
    ControlTerrainDig _terrainDig;
    Text _text;

    void Awake()
    {
        _terrainDig = GetComponentInChildren<ControlTerrainDig>();
        _text = GameObject.Find("SandCollected").GetComponent<Text>();
    }

    void OnEnable()
    {
        _terrainDig.DigInSand += CollectSand;
        _terrainDig.AddSand += TakeSand;
    }

    void OnDisable()
    {
        _terrainDig.DigInSand -= CollectSand;
        _terrainDig.AddSand -= TakeSand;
    }

    void CollectSand(int amountOfSand)
    {
        _sand += amountOfSand;
        _text.text = "Sand: " + _sand.ToString();
    }

    int TakeSand(int amountOfSand)
    {
        int takenSand = amountOfSand > _sand ? _sand : amountOfSand;
        _sand -= takenSand;
        _text.text = "Sand: " + _sand.ToString();
        return takenSand;
    }

    public int Sand { get { return _sand; } }
}
