using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectScript : MonoBehaviour
{
    [SerializeField]
    private RatScript ratScript;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            ratScript.Startle();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            ratScript.Startle();
        }
    }
}

