﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BallController : MonoBehaviour, IStep
{
    [SerializeField] protected RobotsLearningArea area;
    private bool kollidiert = false;

    public bool Kollidiert { get => kollidiert; set => kollidiert = value; }

    private float einwurfWinkel;
    public float EinwurfWinkel { get => einwurfWinkel; set => einwurfWinkel = value; }
    private bool luftwiderstandAktiv = true;
    public bool LuftwiderstandAktiv  { get => luftwiderstandAktiv ; set => luftwiderstandAktiv  = value; }

    private KollisionsLayer kollisionsStatus = KollisionsLayer.Neutral;
    public KollisionsLayer KollisionsStatus { get => kollisionsStatus; set => kollisionsStatus = value; }

    private IList<KollisionsLayer> kollisionsListe = new List<KollisionsLayer>();
    public IList<KollisionsLayer> KollisionsListe { get => kollisionsListe; set => kollisionsListe = value; }

    private Vector3 ballgeschwindigkeit;

    public Vector3 Ballgeschwindigkeit { get => ballgeschwindigkeit; set => ballgeschwindigkeit = value; }
    private Vector3 letztePosition = Vector3.zero;

    public abstract void Step();

    public void Abwurf(Vector3 position, Vector3 geschwindigkeit)
    {
        Debug.Log("Abwurf");
        this.GetComponent<Rigidbody>().MovePosition(position);
        this.GetComponent<Rigidbody>().useGravity = true;
        this.GetComponent<Rigidbody>().velocity = (geschwindigkeit);
        //abwurfPunkt = abwurfPosition.position;
    }
    protected void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.layer != 0)
        {
            Kollidiert = true;
            Debug.Log(KollisionsListe.Count);
            if(KollisionsListe.Count == 0)
            {
                area.BerechneAbwurfhoehe();
                area.BerechneWurfweite();
            }

            if (other.collider.gameObject.layer == 10 && KollisionsListe.Contains(KollisionsLayer.Wand) == false)
            {
                KollisionsListe.Add(KollisionsLayer.Wand);
            }
            else if (other.collider.gameObject.layer == 11 && KollisionsListe.Contains(KollisionsLayer.Boden) == false)
            {
                KollisionsListe.Add(KollisionsLayer.Boden);
            }
            else if (other.collider.gameObject.layer == 12 && KollisionsListe.Contains(KollisionsLayer.Decke) == false)
            {
                KollisionsListe.Add(KollisionsLayer.Decke);
            }
            else if (other.collider.gameObject.layer == 13 && KollisionsListe.Contains(KollisionsLayer.Roboter) == false)
            {
                KollisionsListe.Add(KollisionsLayer.Roboter);
            }
            else if (other.collider.gameObject.layer == 14 && KollisionsListe.Contains(KollisionsLayer.Becherwand) == false)
            {
                KollisionsListe.Add(KollisionsLayer.Becherwand);
            }
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 16 && KollisionsListe.Contains(KollisionsLayer.Einwurfzone) == false)
        {
            KollisionsListe.Add(KollisionsLayer.Einwurfzone);
            einwurfWinkel = BerechneEinwurfwinkel();
            if(KollisionsListe.Count == 0)
            {
                area.BerechneAbwurfhoehe();
                area.BerechneWurfweite();
            }
        }
        else if (other.gameObject.layer == 15 && KollisionsListe.Contains(KollisionsLayer.Becherboden) == false)
        {
            KollisionsListe.Add(KollisionsLayer.Becherboden);
        }
    }


    protected void BerechneBallgeschwindigkeit()
    {
        Ballgeschwindigkeit = (this.transform.position - letztePosition) / Time.fixedDeltaTime;
        letztePosition = this.transform.position;
    }

    protected float BerechneEinwurfwinkel()
    {
        return Mathf.Abs(Mathf.Rad2Deg * Mathf.Asin(Mathf.Abs(Ballgeschwindigkeit.y) / Ballgeschwindigkeit.magnitude));
    }

}
