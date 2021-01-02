using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum KollisionsLayer { Neutral = 0, Wand = 1, Boden = 2, Decke = 3, Becherwand = 4, Becherboden = 5, Einwurfzone = 6, Roboter = 7 };

public class BallControllerV7 : MonoBehaviour
{
        
    [SerializeField] private RoboterControllerV7 roboterManager;
    private bool kollidiert = false;

    public bool Kollidiert { get => kollidiert; set => kollidiert = value; }

    private KollisionsLayer kollisionsStatus = KollisionsLayer.Neutral;
    public KollisionsLayer KollisionsStatus { get => kollisionsStatus; set => kollisionsStatus = value; }


    private Vector3 ballgeschwindigkeit;
    private Vector3 letztePosition = Vector3.zero;

    private float einwurfWinkel;

    public float EinwurfWinkel { get => einwurfWinkel; set => einwurfWinkel = value; }

    private float wurfweite;

    public float Wurfweite { get => wurfweite; set => wurfweite = value; }

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        BerechneBallgeschwindigkeit();
    }

    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.layer != 0)
        {
            kollidiert = true;

            if (other.collider.gameObject.layer == 10)
            {
                kollisionsStatus = KollisionsLayer.Wand;
                wurfweite =  BerechneWurfweite();
                Debug.Log("Kollision mit " + other.collider.gameObject.name + " erkannt");
            }
            else if (other.collider.gameObject.layer == 11)
            {
                kollisionsStatus = KollisionsLayer.Boden;
                wurfweite =  BerechneWurfweite();
                Debug.Log("Kollision mit " + other.collider.gameObject.name + " erkannt");
            }
            else if (other.collider.gameObject.layer == 12)
            {
                kollisionsStatus = KollisionsLayer.Decke;
                Debug.Log("Kollision mit " + other.collider.gameObject.name + " erkannt");
            }
            else if (other.collider.gameObject.layer == 13)
            {
                kollisionsStatus = KollisionsLayer.Roboter;
                Debug.Log("Kollision mit " + other.collider.gameObject.name + " erkannt");
            }
            else if (other.collider.gameObject.layer == 14)
            {
                kollisionsStatus = KollisionsLayer.Becherwand;
                wurfweite =  BerechneWurfweite();
                Debug.Log("Kollision mit " + other.collider.gameObject.name + " erkannt");
            }
            else if (other.collider.gameObject.layer == 15)
            { 
                kollisionsStatus = KollisionsLayer.Becherboden;
                wurfweite =  BerechneWurfweite();
                Debug.Log("Kollision mit " + other.collider.gameObject.name + " erkannt");
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == 16)
        {
            einwurfWinkel = BerechneEinwurfwinkel();
            wurfweite =  BerechneWurfweite();
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

    private float BerechneEinwurfwinkel()
    {
        return Mathf.Rad2Deg * Mathf.Asin(Mathf.Abs(ballgeschwindigkeit.y) / ballgeschwindigkeit.magnitude);

    }

    private float BerechneWurfweite()
    {
        return Mathf.Abs(this.transform.position.x - roboterManager.AbwurfPunkt.x);

    }
}
