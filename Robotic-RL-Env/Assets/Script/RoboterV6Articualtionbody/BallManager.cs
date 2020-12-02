using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallManager : MonoBehaviour
{
    private bool kollidiert;

    public bool Kollidiert { get => kollidiert; set => kollidiert = value; }

    private void OnTriggerEnter(Collider other)
    {
        kollidiert = true;
        Debug.Log("HHHHHH");
    }
}
