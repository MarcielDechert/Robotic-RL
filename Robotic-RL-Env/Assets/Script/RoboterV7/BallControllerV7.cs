using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum KollisionsLayer { Neutral = 0, Wand = 1, Boden = 2, Decke = 3, Becherwand = 4, Becherboden = 5, Einwurfzone = 6, Roboter = 7 };

public class BallControllerV7 : MonoBehaviour
{
    [SerializeField] private RobotsLearningArea area;
    private bool kollidiert = false;

    public bool Kollidiert { get => kollidiert; set => kollidiert = value; }

    private KollisionsLayer kollisionsStatus = KollisionsLayer.Neutral;
    public KollisionsLayer KollisionsStatus { get => kollisionsStatus; set => kollisionsStatus = value; }

    private IList<KollisionsLayer> kollisionsListe = new List<KollisionsLayer>();
    public IList<KollisionsLayer> KollisionsListe { get => kollisionsListe; set => kollisionsListe = value; }

    private Vector3 ballgeschwindigkeit;
    private Vector3 letztePosition = Vector3.zero;

    private float einwurfWinkel;
    public float EinwurfWinkel { get => einwurfWinkel; set => einwurfWinkel = value; }
    private void Start()
    {
        
    }
    public void Step()
    {
        BerechneBallgeschwindigkeit();
    }

    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.layer != 0)
        {
            kollidiert = true;
            area.BerechneAbwurfhoehe();
            area.BerechneWurfweite();

            if (other.collider.gameObject.layer == 10 && kollisionsListe.Contains(KollisionsLayer.Wand) == false)
            {
                kollisionsListe.Add(KollisionsLayer.Wand);
            }
            else if (other.collider.gameObject.layer == 11 && kollisionsListe.Contains(KollisionsLayer.Boden) == false)
            {
                kollisionsListe.Add(KollisionsLayer.Boden);
            }
            else if (other.collider.gameObject.layer == 12 && kollisionsListe.Contains(KollisionsLayer.Decke) == false)
            {
                kollisionsListe.Add(KollisionsLayer.Decke);
            }
            else if (other.collider.gameObject.layer == 13 && kollisionsListe.Contains(KollisionsLayer.Roboter) == false)
            {
                kollisionsListe.Add(KollisionsLayer.Roboter);
            }
            else if (other.collider.gameObject.layer == 14 && kollisionsListe.Contains(KollisionsLayer.Becherwand) == false)
            {
                kollisionsListe.Add(KollisionsLayer.Becherwand);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 16 && kollisionsListe.Contains(KollisionsLayer.Einwurfzone) == false)
        {
            kollisionsListe.Add(KollisionsLayer.Einwurfzone);
            einwurfWinkel = BerechneEinwurfwinkel();
            area.BerechneWurfweite();
            area.BerechneAbwurfhoehe();
        }
        else if (other.gameObject.layer == 15 && kollisionsListe.Contains(KollisionsLayer.Becherboden) == false)
        {
            kollisionsListe.Add(KollisionsLayer.Becherboden);
        }
    }

    private void BerechneBallgeschwindigkeit()
    {
        ballgeschwindigkeit = (this.transform.position - letztePosition) / Time.fixedDeltaTime;
        letztePosition = this.transform.position;
    }

    private float BerechneEinwurfwinkel()
    {
        return Mathf.Abs(Mathf.Rad2Deg * Mathf.Asin(Mathf.Abs(ballgeschwindigkeit.y) / ballgeschwindigkeit.magnitude));
    }
}
