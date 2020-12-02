using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallManager : MonoBehaviour
{
    [SerializeField] private Transform becher;
    [SerializeField] private Text entfernungZumBecherText;

    private void OnTriggerEnter(Collider other)
    {
        // Becher
        if (other.gameObject.layer == 10)
        {
        }
        // Boden
        if (other.gameObject.layer == 11)
        {
        }
        // Decke 
        if (other.gameObject.layer == 12)
        {
        }
        // Roboter
        if (other.gameObject.layer == 13)
        {
        }
    }
}
