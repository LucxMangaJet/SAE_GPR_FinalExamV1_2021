using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selfdestruct : MonoBehaviour
{
    [SerializeField] private float delay;
    void Start()
    {
        if (delay > 0)
            Destroy(gameObject, delay);
    }
}
