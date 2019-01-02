using UnityEngine;
using System.Collections;

public class SandCastleDestroy : MonoBehaviour
{
    bool _isCollapsed;

    void OnTriggerEnter(Collider other)
    {
        if (!_isCollapsed)
        {
            transform.Find("Mound").gameObject.SetActive(true);
            transform.Find("Particle System").gameObject.SetActive(true);
            transform.Find("SandCastle00 1").gameObject.SetActive(false);
            Invoke("DisableMound", 1);
            _isCollapsed = true;
        }
    }

    void DisableMound()
    {
        transform.Find("Mound").GetComponent<Collider>().enabled = false;
    }
}
