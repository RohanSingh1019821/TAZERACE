using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bapper : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 10)
        {
            GameObject.FindWithTag("Player").GetComponent<CarControl>().GetBlazed();
        }
    }
}
