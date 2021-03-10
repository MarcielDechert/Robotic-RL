using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BallController : MonoBehaviour, IStep
{
    [SerializeField] protected RobotsLearningArea area;
<<<<<<< Updated upstream
    private bool kollidiert = false;
=======

    private bool isKollidiert = false;
>>>>>>> Stashed changes

    public bool IsKollidiert { get => isKollidiert; set => isKollidiert = value; }

    private float einwurfWinkel;
    public float EinwurfWinkel { get => einwurfWinkel; set => einwurfWinkel = value; }

    private bool isLuftwiderstandAktiv = true;
    public bool IsLuftwiderstandAktiv  { get => isLuftwiderstandAktiv ; set => isLuftwiderstandAktiv  = value; }

    private KollisionsLayer kollisionsStatus = KollisionsLayer.Neutral;
    public KollisionsLayer KollisionsStatus { get => kollisionsStatus; set => kollisionsStatus = value; }

    private IList<KollisionsLayer> kollisionsListe = new List<KollisionsLayer>();
    public IList<KollisionsLayer> KollisionsListe { get => kollisionsListe; set => kollisionsListe = value; }

    private Vector3 ballGeschwindigkeit;

<<<<<<< Updated upstream
    public Vector3 Ballgeschwindigkeit { get => ballgeschwindigkeit; set => ballgeschwindigkeit = value; }
    private Vector3 letztePosition = Vector3.zero;
=======
    public Vector3 BallGeschwindigkeit { get => ballGeschwindigkeit; set => ballGeschwindigkeit = value; }
>>>>>>> Stashed changes

    public abstract void Step();

    public void Abwurf(Vector3 position, Vector3 geschwindigkeit)
    {
        this.GetComponent<Rigidbody>().MovePosition(position);
        this.GetComponent<Rigidbody>().useGravity = true;
        this.GetComponent<Rigidbody>().velocity = (geschwindigkeit);
    }
    protected void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.layer != 0)
        {
<<<<<<< Updated upstream
            Kollidiert = true;
            Debug.Log(KollisionsListe.Count);
=======
            IsKollidiert = true;

            //wenn die KollisonsListe leer ist
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        Ballgeschwindigkeit = (this.transform.position - letztePosition) / Time.fixedDeltaTime;
        letztePosition = this.transform.position;
=======
        BallGeschwindigkeit = area.R_ball.GetComponent<Rigidbody>().velocity;
>>>>>>> Stashed changes
    }

    protected float BerechneEinwurfwinkel()
    {
        return Mathf.Abs(Mathf.Rad2Deg * Mathf.Asin(Mathf.Abs(BallGeschwindigkeit.y) / BallGeschwindigkeit.magnitude));
    }

}
