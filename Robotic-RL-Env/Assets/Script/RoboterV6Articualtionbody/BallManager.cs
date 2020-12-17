using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallManager : MonoBehaviour
{
    private bool kollidiert ;

    private RoboterManagerV6 roboter;

    public bool Kollidiert { get => kollidiert; set => kollidiert = value; }

    private Vector3 ballgeschwindigkeit;
    private Vector3 letztePosition = Vector3.zero;

    private void FixedUpdate() 
    {
        BerechneBallgeschwindigkeit();
    }

    private void OnCollisionEnter(Collision other) {
        
        
        if (other.gameObject.layer != 0)
        {
            kollidiert = true;
            Debug.Log("Kollision erkannt");
        }
    
    }

    void OnTriggerEnter(Collider other) { 
        
        if (other.gameObject.layer == 15)
        {
            //roboter.einwurfwinkelText.text = "Einwurfwinkel: "+ Mathf.Rad2Deg * Mathf.Acos(-ballgeschwindigkeit.y/ballgeschwindigkeit.magnitude) +" Grad";
            //Debug.Log(ballgeschwindigkeit.x);
            Debug.Log("Trigger erkannt");
        }
    
    }

    private void BerechneBallgeschwindigkeit()
    {
        // aktuelle Geschwindigkeit des Abwurfpunktes am Greifer
        ballgeschwindigkeit = (this.transform.position - letztePosition) / Time.fixedDeltaTime;
        letztePosition = this.transform.position;
        //Debug.Log(abwurfgeschwindigkeit);

    }
}
