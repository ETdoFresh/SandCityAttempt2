using UnityEngine;
using System.Collections;

public class CharacterFootStep : MonoBehaviour
{

    public static FootStepHandler OnFootStep;
    public delegate void FootStepHandler(Vector2 position, Vector2 size, float weight);

    enum Foot { LEFT, RIGHT }

    public Vector2 offset = new Vector2(0.25f, 0);
    public float timeBetweenSteps = 1;
    public Vector2 size = new Vector2(0.25f, 0.5f);
    public float digRate = 0.001f;
    Foot _foot = Foot.LEFT;
    float _footTimer;

    // Update is called once per frame
    void Update()
    {
        _footTimer += Time.deltaTime;

        if (_footTimer > timeBetweenSteps)
        {
            _footTimer = 0;
            _foot = _foot == Foot.LEFT ? Foot.RIGHT : Foot.LEFT;
            Vector2 myPosition = new Vector2(transform.position.x, transform.position.z);
            OnFootStep(_foot == Foot.LEFT ? myPosition + offset : myPosition - offset, size, digRate);
        }
    }
}
