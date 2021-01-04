using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum KollisionsLayer { Neutral = 0, Wand = 1, Boden = 2, Decke = 3, Becherwand = 4, Becherboden = 5, Einwurfzone = 6, Roboter = 7, Becher = 8};

public enum CollisionWith { BecherInnen = 0, Becheraußen = 1, BecherBoden = 2, Boden = 3, Roboter = 4 };

public class BallControllerV7 : MonoBehaviour
{
    private bool kollidiert;
    public bool Kollidiert { get => kollidiert; set => kollidiert = value; }

    private KollisionsLayer kollisionsStatus = KollisionsLayer.Neutral;
    public KollisionsLayer KollisionsStatus { get => kollisionsStatus; set => kollisionsStatus = value; }

    private IList<KollisionsLayer> kollisionenListe = new List<KollisionsLayer>();
    public IList<KollisionsLayer> KollisionenListe { get => kollisionenListe; set => kollisionenListe = value; }

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

            if (other.collider.gameObject.layer == 10)
            {
                if(kollisionen.Contains(KollisionsLayer.Wand) == false)
                    kollisionen.Add(KollisionsLayer.Wand);
                //Debug.Log("Kollision mit " + other.collider.gameObject.name + " erkannt");
            }
            else if (other.collider.gameObject.layer == 11)
            {
                kollisionsStatus = KollisionsLayer.Boden;
                //Debug.Log("Kollision mit " + other.collider.gameObject.name + " erkannt");
            }
            else if (other.collider.gameObject.layer == 12)
            {
                kollisionsStatus = KollisionsLayer.Decke;
                //Debug.Log("Kollision mit " + other.collider.gameObject.name + " erkannt");
            }
            else if (other.collider.gameObject.layer == 13)
            {
                kollisionsStatus = KollisionsLayer.Roboter;
                //Debug.Log("Kollision mit " + other.collider.gameObject.name + " erkannt");
            }
            else if (other.collider.gameObject.layer == 14)
            {
                kollisionsStatus = KollisionsLayer.Becherwand;
                //Debug.Log("Kollision mit " + other.collider.gameObject.name + " erkannt");
            }
            else if (other.collider.gameObject.layer == 15)
            { 
                kollisionsStatus = KollisionsLayer.Becherboden;
                //Debug.Log("Kollision mit " + other.collider.gameObject.name + " erkannt");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == 16)
        {
            kollidiert = true;
            kollisionsStatus = KollisionsLayer.Einwurfzone;

            einwurfwinkel = Mathf.Rad2Deg * Mathf.Acos(-ballgeschwindigkeit.y / ballgeschwindigkeit.magnitude);
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
