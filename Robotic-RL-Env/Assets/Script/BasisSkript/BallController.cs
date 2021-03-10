using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Abstrakte Klasse die die allgemeinen Attribute und Methoden für das Verhalten des Balls bereitstellt 
/// </summary>
public abstract class BallController : MonoBehaviour, IStep
{
    [SerializeField] protected RobotsLearningArea area;

    private bool isKollidiert = false;

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

    public Vector3 BallGeschwindigkeit { get => ballGeschwindigkeit; set => ballGeschwindigkeit = value; }

    public abstract void Step();

    /// <summary>
    /// Positioniert den Ball an vorgegebner Stelle und gibt ihn eine vorgegeben Anfangsgeschwindigkeit für den Abwurf
    /// </summary>
    /// <param name="_position"> Position des Abwurfs als Vektor3</param>
    /// <param name="_geschwindigkeit"> Startgeschwindigkeit als Vektor3</param>
    public void Abwurf(Vector3 _position, Vector3 _geschwindigkeit)
    {
        this.GetComponent<Rigidbody>().MovePosition(_position);
        this.GetComponent<Rigidbody>().useGravity = true;
        this.GetComponent<Rigidbody>().velocity = (_geschwindigkeit);
    }

    /// <summary>
    /// Verarbeitet Kollisionen mit andern Objekten
    /// </summary>
    /// <param name="other"> Andere GameObjects</param>
    protected void OnCollisionEnter(Collision other)
    {   
        //wenn der Layer des GameObjects ungleich 0 isz
        if (other.gameObject.layer != 0)
        {
            IsKollidiert = true;

            //wenn die KollisonsListe leer ist
            if(KollisionsListe.Count == 0)
            {
                area.BerechneAbwurfhoehe();
                area.BerechneWurfweite();
            }

            //wenn das Objekt mit Layer x getroffen wird er zur KollisionsListe hinzugefügt

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
            else if (other.collider.gameObject.layer == 13 && KollisionsListe.Contains(KollisionsLayer.Roboter) == false )
            {
                KollisionsListe.Add(KollisionsLayer.Roboter);;
            }
            else if (other.collider.gameObject.layer == 14 && KollisionsListe.Contains(KollisionsLayer.Becherwand) == false)
            {
                KollisionsListe.Add(KollisionsLayer.Becherwand);
            }
            else if (other.collider.gameObject.layer == 16 && KollisionsListe.Contains(KollisionsLayer.TCP) == false )
            {
                KollisionsListe.Add(KollisionsLayer.TCP);
            }
        }
    }

    /// <summary>
    /// Verarbeitet Kollisionen mit Objekten die die Eigenschaft "Is Trigger" hat
    /// </summary>
    /// <param name="other"></param>
    protected void OnTriggerEnter(Collider other)
    {
        // wenn der getroffene Layer Einwurfzone ist und noch nicht in der Kollisionsliste ist
        if (other.gameObject.layer == 16 && KollisionsListe.Contains(KollisionsLayer.Einwurfzone) == false)
        {
            KollisionsListe.Add(KollisionsLayer.Einwurfzone);
            einwurfWinkel = BerechneEinwurfwinkel();

            //wenn die KollisonsListe leer ist
            if (KollisionsListe.Count == 0)
            {
                area.BerechneAbwurfhoehe();
                area.BerechneWurfweite();
            }
        }
        // wenn der getroffene Layer Becherboden ist und noch nicht in der Kollisionsliste ist
        else if (other.gameObject.layer == 15 && KollisionsListe.Contains(KollisionsLayer.Becherboden) == false)
        {
            KollisionsListe.Add(KollisionsLayer.Becherboden);
        }
    }

    /// <summary>
    /// Berechnet die aktuelle Ballgeschwindigkeit
    /// </summary>
    protected void BerechneBallgeschwindigkeit()
    {
        BallGeschwindigkeit = area.R_ball.GetComponent<Rigidbody>().velocity;
    }

    /// <summary>
    /// Berechnet den Einwurfwinkel anhand des Geschwindigkeitsvektors
    /// </summary>
    /// <returns> Winkel in Grad</returns>
    protected float BerechneEinwurfwinkel()
    {
        return Mathf.Abs(Mathf.Rad2Deg * Mathf.Asin(Mathf.Abs(BallGeschwindigkeit.y) / BallGeschwindigkeit.magnitude));
    }

}
