using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum KollisionsLayer { Neutral = 0, Wand = 1, Boden = 2, Decke = 3, Becherwand = 4, Becherboden = 5, Einwurfzone = 6, Roboter = 7 };

public enum CollisionWith { BecherInnen = 0, Becheraußen = 1, BecherBoden = 2, Boden = 3, Roboter = 4 };

public class BallControllerV7 : MonoBehaviour
{
    private bool kollidiert;

    public bool Kollidiert { get => kollidiert; set => kollidiert = value; }

    private KollisionsLayer kollisionsStatus = KollisionsLayer.Neutral;


    private Vector3 ballgeschwindigkeit;
    private Vector3 letztePosition = Vector3.zero;

    private float einwurfwinkel;

    public float Einwurfwinkel { get => einwurfwinkel; set => einwurfwinkel = value; }

    private void FixedUpdate()
    {
        BerechneBallgeschwindigkeit();
    }

    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.layer != 0)
        {
            kollidiert = true;
            Debug.Log("Kollision erkannt");
        }

        if (other.gameObject.layer == 10)
        {
            kollisionsStatus = KollisionsLayer.Wand;
            Debug.Log("Kollision mit " + other.gameObject.name + " erkannt");
        }

        if (other.gameObject.layer == 11)
        {
            kollisionsStatus = KollisionsLayer.Boden;
            Debug.Log("Kollision mit " + other.gameObject.name + " erkannt");
        }

        if (other.gameObject.layer == 12)
        {
            kollisionsStatus = KollisionsLayer.Decke;
            Debug.Log("Kollision mit " + other.gameObject.name + " erkannt");
        }

        if (other.gameObject.layer == 13)
        {
            kollisionsStatus = KollisionsLayer.Roboter;
            Debug.Log("Kollision mit " + other.gameObject.name + " erkannt");
        }

        if (other.gameObject.layer == 17)
        {
            kollisionsStatus = KollisionsLayer.Becherwand;
            Debug.Log("Kollision mit " + other.gameObject.name + " erkannt");
        }

        if (other.gameObject.layer == 15)
        {
            kollisionsStatus = KollisionsLayer.Becherboden;
            Debug.Log("Kollision mit " + other.gameObject.name + " erkannt");
        }

    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == 16)
        {
            einwurfwinkel = Mathf.Rad2Deg * Mathf.Acos(-ballgeschwindigkeit.y / ballgeschwindigkeit.magnitude);
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
