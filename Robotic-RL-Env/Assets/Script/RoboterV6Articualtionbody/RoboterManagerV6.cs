using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public enum AbwurfStatus { Neutral = 0, Abwurfbereit = 1,InStartposition = 2, Wirft = 3,  Stopp= -1 };

public class RoboterManagerV6 : MonoBehaviour
{

    [SerializeField] private Rigidbody ball;
    [SerializeField] private Transform abwurfPosition;

    [SerializeField] private AchseV6 j1;
    [SerializeField] private AchseV6 j2;
    [SerializeField] private AchseV6 j3;
    [SerializeField] private AchseV6 j4;
    [SerializeField] private AchseV6 j5;

    [SerializeField] private float startRotationJ1 = 0.0f;
    [SerializeField] private float startRotationJ2 = 0.0f;
    [SerializeField] private float startRotationJ3 = 0.0f;
    [SerializeField] private float startRotationJ4 = 0.0f;
    [SerializeField] private float startRotationJ5 = 0.0f;

    [SerializeField] private float abwurfwinkel = 0.0f;
    [SerializeField] private float wurfgeschwindigkeit = 0.0f;
    [SerializeField] private float toleranzwinkel = 0.5f;
    [SerializeField] private int anzahlAchsen = 0;
    [SerializeField] private int WurfAchse = 3;

    public AbwurfStatus abwurfStatus = AbwurfStatus.Neutral;
    private Vector3 abwurfgeschwindigkeitVector3;
    
    private float abwurfwinkelJ3;
    public float AbwurfwinkelJ3 { get => abwurfwinkelJ3; set => abwurfwinkelJ3 = value; }
    private float abwurfgeschwindigkeit;
    public float Abwurfgeschwindigkeit { get => abwurfgeschwindigkeit; set => abwurfgeschwindigkeit = value; }
    
    private float abwurfwinkelBall;
    public float AbwurfwinkelBall { get => abwurfwinkelBall; set => abwurfwinkelBall = value; }

    private Vector3 letztePosition = Vector3.zero;
    private bool abwurfSignal;

    private float[] startRotation;

    public AchseV6[] achsen;


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    void Init()
    {
        startRotation = new float[anzahlAchsen];
        achsen = new AchseV6[anzahlAchsen];

        startRotation[0] = startRotationJ1;
        startRotation[1] = startRotationJ2;
        startRotation[2] = startRotationJ3;
        startRotation[3] = startRotationJ4;
        startRotation[4] = startRotationJ5;

        achsen[0] = j1;
        achsen[1] = j2;
        achsen[2] = j3;
        achsen[3] = j4;
        achsen[4] = j5;
        // Debug.Log(StartRotationJ1);
        if(WurfAchse == 0)
        {
            WurfAchse = 1;
        }
        abwurfSignal = false;
    }

    private void FixedUpdate()
    {
        switch(abwurfStatus)
        {
            case AbwurfStatus.Neutral: 
                break;
            case AbwurfStatus.InStartposition:    
                abwurfSignal = false;
                InStartpositionFahren();
                abwurfStatus = AbwurfStatus.Abwurfbereit;
                break;
            case AbwurfStatus.Abwurfbereit:
                if(abwurfSignal)
                {
                    abwurfStatus = AbwurfStatus.Wirft;
                }
                break;
            case AbwurfStatus.Wirft:   
                abwurfSignal = false;
                BerechneGeschwindigkeit();
                Abwurfvorgang(); 
                break;
        }
    }


    private void BerechneGeschwindigkeit()
    {
        // aktuelle Geschwindigkeit des Abwurfpunktes am Greifer
        abwurfgeschwindigkeitVector3 = (abwurfPosition.transform.position - letztePosition) / Time.fixedDeltaTime;
        letztePosition = abwurfPosition.transform.position;
        //Debug.Log(abwurfgeschwindigkeitVector3);
    }

    private void Abwurf()
    {
        ball.MovePosition(abwurfPosition.position);
        ball.useGravity = true;
        ball.velocity =(abwurfgeschwindigkeitVector3);
    }

    public void InStartpositionFahren()
    {
        for (int i = 0; i < anzahlAchsen; i++)
        { 
            achsen[i].RotiereAchse(startRotation[i]);
        }
            
        achsen[WurfAchse-1].rotationState = RotationsRichtung.Neutral;
        abwurfgeschwindigkeitVector3 = Vector3.zero;
        abwurfgeschwindigkeit = 0.0f;
        abwurfwinkelJ3 = 0.0f;
    }
    private void Abwurfvorgang()
    {   
        achsen[WurfAchse-1].achsengeschwindigkeit = wurfgeschwindigkeit;
        achsen[WurfAchse-1].rotationState = RotationsRichtung.Positiv;
        // ist der vorher angegebene Abwurfwinkel mit einer Toleranz erreicht wird der Abwurf des Balls gestartet
        if (achsen[WurfAchse-1].AktuelleRotationDerAchse() <= abwurfwinkel + toleranzwinkel && achsen[WurfAchse-1].AktuelleRotationDerAchse() >= abwurfwinkel - toleranzwinkel )
        {
            Abwurf();
            abwurfgeschwindigkeit= abwurfgeschwindigkeitVector3.magnitude ;
            abwurfwinkelJ3 = achsen[WurfAchse-1].AktuelleRotationDerAchse() ;
            abwurfwinkelBall =  BerechneAbwurfwinkel();
            achsen[WurfAchse-1].rotationState = RotationsRichtung.Neutral;
            abwurfStatus = AbwurfStatus.Neutral;
        }
    }

    private float BerechneAbwurfwinkel()
    {
        return  Mathf.Rad2Deg * Mathf.Acos(-abwurfgeschwindigkeitVector3.x/abwurfgeschwindigkeitVector3.magnitude);
    }

    // Rechnet die den Winkel von -50 bis -200 um
    private float UebersetzeWinkel(float winkel)
    {
        if (winkel < 0 || winkel > 1) return 0;
        return -1 * (winkel * 150 + 50);
    }

    // Rechnet die den Geschwindigkeit von 0 bis 500 um
    private float UebersetzGeschwindigkeit(float geschwindigkeit)
    {
        if (geschwindigkeit < 0 || geschwindigkeit > 1) return 0;
        return  geschwindigkeit * 500;
    }
    // Startet den Ballabwurf mit der Wurfgschwindigkeit des Arms von 0-1 und dem Abwurfwinkel von 0-1
    public void StarteAbwurfMitKI(float geschwindigkeit, float winkel)
    {
        abwurfwinkel = UebersetzeWinkel(winkel);
        wurfgeschwindigkeit = UebersetzGeschwindigkeit(geschwindigkeit);
        abwurfSignal = true;
    }
    public void StarteAbwurf()
    {
       abwurfSignal = true;
    }

    public void Startvorgang()
    {
        abwurfStatus = AbwurfStatus.InStartposition;
    }

    public void SetzeGeschwindikeitDerScene(float geschwindigkeit)
    {
        Time.timeScale = geschwindigkeit;
    }

    public void RotiereAchseJ1(float winkel)
    {
        j1.RotiereAchse(winkel);
    }

    public void RotiereAchseJ2(float winkel)
    {
        j2.RotiereAchse(winkel);
    }

    public void RotiereAchseJ3(float winkel)
    {
        j3.RotiereAchse(winkel);
    }

    public void RotiereAchseJ4(float winkel)
    {
        j4.RotiereAchse(winkel);
    }

    public void RotiereAchseJ5(float winkel)
    {
        j5.RotiereAchse(winkel);
    }
}
