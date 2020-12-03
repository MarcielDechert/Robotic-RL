using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallManager : MonoBehaviour
{
    private bool kollidiert ;

    public bool Kollidiert { get => kollidiert; set => kollidiert = value; }

    private void OnCollisionEnter(Collision other) {
        
        
        if (other.gameObject.layer != 0)
        {
            kollidiert = true;
            Debug.Log("Kollision erkannt");
        }
    
    }
}
