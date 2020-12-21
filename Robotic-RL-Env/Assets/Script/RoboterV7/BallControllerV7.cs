using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControllerV7 : MonoBehaviour
{
    private bool kollidiert ;

    public bool Kollidiert { get => kollidiert; set => kollidiert = value; }


    private Vector3 ballgeschwindigkeit;
    private Vector3 letztePosition = Vector3.zero;

    private float einwurfwinkel;

    public float Einwurfwinkel { get => einwurfwinkel; set => einwurfwinkel = value; }

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
            einwurfwinkel =  Mathf.Rad2Deg * Mathf.Acos(-ballgeschwindigkeit.y/ballgeschwindigkeit.magnitude);
            Debug.Log("Trigger erkannt");
        }
    }

    private void BerechneBallgeschwindigkeit()
    {
        // aktuelle Geschwindigkeit des Balls
        ballgeschwindigkeit = (this.transform.position - letztePosition) / Time.fixedDeltaTime;
        letztePosition = this.transform.position;
        //Debug.Log(abwurfgeschwindigkeit);

    }
}
