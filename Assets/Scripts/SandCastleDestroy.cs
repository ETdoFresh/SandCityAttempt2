using UnityEngine;
using System.Collections;

public class SandCastleDestroy : MonoBehaviour
{
    bool _isCollapsed;

    void OnTriggerEnter(Collider other)
    {
        if (!_isCollapsed)
        {
            transform.FindChild("Mound").gameObject.SetActive(true);
            transform.FindChild("Particle System").gameObject.SetActive(true);
            transform.FindChild("SandCastle00 1").gameObject.SetActive(false);
            Invoke("DisableMound", 1);
            _isCollapsed = true;
        }
    }

    void DisableMound()
    {
        transform.FindChild("Mound").GetComponent<Collider>().enabled = false;
    }
}
