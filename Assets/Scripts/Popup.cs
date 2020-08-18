using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    public GameObject zapper;

    public float attackTime = 3f;
    private bool zapping = false;

    public float time = 6f;
    public float timer = 0;
    public bool popped = false;
    public bool poppedDown = true;

    void Awake()
    {
        timer = time;
    }

    void Update()
    {
        if (zapping)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0 && !poppedDown)
        {
            GetComponent<Animation>().Play("popdown");
            poppedDown = true;
            timer = time;
        }

        else if (timer <= attackTime / 2)
        {
            zapping = false;
            timer -= Time.deltaTime;
            zapper.gameObject.SetActive(false);
        }
    }

    public void Popped()
    {
        zapper.gameObject.SetActive(true);
        poppedDown = false;
        zapping = true;
        popped = true;
    }

    public void UnPopped()
    {
        popped = false;

    }

    void OnTriggerEnter(Collider col)
    {
        if (!popped)
        {
            if (col.gameObject.layer == 10)
            {
                GetComponent<Animation>().Play("popup");
            }
        }
    }
}
