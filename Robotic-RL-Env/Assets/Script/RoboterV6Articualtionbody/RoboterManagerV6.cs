using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public enum RoboterStatus { Neutral = 0, FahrInStartPosition = 1, InStartposition = 2, Abwurfbereit = 3, Wirft = 4, Faehrt = 5, Stopp= -1 };
public enum Befehl { Neutral = 0, Abwurf = 1, Start = 2 };

public class RoboterManagerV6 : MonoBehaviour
{

    [SerializeField] private Rigidbody ball;
    [SerializeField] private Transform abwurfPosition;

    [SerializeField] private AchseV6 j1;
    [SerializeField] private AchseV6 j2;
    [SerializeField] private AchseV6 j3;
    [SerializeField] private AchseV6 j4;
    [SerializeField] private AchseV6 j5;
    [SerializeField] private AchseV6 j6;


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

    private RoboterStatus abwurfStatus = RoboterStatus.Neutral;

    private Befehl befehl = Befehl.Neutral;
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
    private float[] abwurfRotation;

    private AchseV6[] achsen;

    private float[] sollRotation;

    private float[] istRotation;
    public float[] IstRotation { get => istRotation; set => istRotation= value; }

    private float[] sollRotationsGeschwindigkeit;

    private bool sollIst;




    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    void Init()
    {
        startRotation = new float[anzahlAchsen];
        sollRotation = new float[anzahlAchsen];
        istRotation = new float[anzahlAchsen];
        abwurfRotation= new float[anzahlAchsen];
        sollRotationsGeschwindigkeit = new float[anzahlAchsen];

        abwurfRotation[0] = 180.0f;
        abwurfRotation[1] = 90.0f;
        abwurfRotation[2] = abwurfwinkel;
        abwurfRotation[3] = 0.0f;
        abwurfRotation[4] = -40.0f;

        sollRotationsGeschwindigkeit[0] = 500.0f;
        sollRotationsGeschwindigkeit[1] = 500.0f;
        sollRotationsGeschwindigkeit[2] = wurfgeschwindigkeit;
        sollRotationsGeschwindigkeit[3] = 500.0f;
        sollRotationsGeschwindigkeit[4] = 500.0f;

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
        sollIst = false;
        
    }

    private void FixedUpdate()
    {
        AktuelleRotationenAuslesen();
        BerechneGeschwindigkeit();

        switch(abwurfStatus)
        {

            case RoboterStatus.Neutral:
                                        if(befehl == Befehl.Start)
                                        {
                                            abwurfStatus = RoboterStatus.Faehrt;
                                            //SetzeSollrotation(startRotation);
                                           //SetzeSollRotationsGeschwindigkeit(sollRotationsGeschwindigkeit);

                                        }
                                        break;

            case RoboterStatus.Faehrt:
                                        if(sollIst){
                                            if(abwurfSignal)
                                            {
                                                abwurfStatus = RoboterStatus.Wirft;
                                                Abwurf();
                                                WerteSetzen();
                                            }
                                            else
                                            {
                                                abwurfStatus = RoboterStatus.Abwurfbereit;
                                                WerteZurueckSetzen();
                                            }
                                        }
                                        else
                                        {
                                            RotiereAlleAchsen();
                                        }
                                        // if(abwurfSignal && AbwurfwinkelErreicht())
                                        // {
                                        //      Abwurf();   
                                        // }
                                        break;

            case RoboterStatus.Abwurfbereit:
                                                abwurfSignal = true;
                                                if (befehl == Befehl.Abwurf)
                                                {
                                                   // SetzeSollrotation(abwurfRotation);
                                                    sollIst = false;
                                                    abwurfStatus = RoboterStatus.Faehrt;
                                                }
                                                break;

            case RoboterStatus.Wirft:
                                        abwurfSignal = false;
                                        if (befehl == Befehl.Start)
                                        {
                                            abwurfStatus = RoboterStatus.Faehrt;
                                            sollIst = false;
                                            //SetzeSollrotation(startRotation);
                                        }
                                        break;
        }

            /*

        switch(abwurfStatus)
        {


            case RoboterStatus.Neutral: 
                                                break;

            case RoboterStatus.FahrInStartPosition:    
                                                abwurfSignal = false;
                                                InStartpositionFahren();
                                                abwurfStatus = RoboterStatus.InStartposition; 
                                                break;

            case RoboterStatus.InStartposition:
                                                abwurfSignal = false;
                                                if (true)
                                                {
                                                    abwurfStatus = RoboterStatus.Abwurfbereit;                                                            
                                                }
                                                break;

            case RoboterStatus.Abwurfbereit:
                                                if(abwurfSignal)
                                                {
                                                    abwurfStatus = RoboterStatus.Wirft;
                                                }
                                                break;

            case RoboterStatus.Wirft:   
                                                abwurfSignal = false;
                                                BerechneGeschwindigkeit();
                                                Abwurfvorgang(); 
                                                break;

          

        }
  */
    }

    private void WerteZurueckSetzen()
    {
        abwurfgeschwindigkeitVector3 = Vector3.zero;
        abwurfgeschwindigkeit = 0.0f;
        abwurfwinkelJ3 = 0.0f;
        abwurfwinkelBall = 0.0f;
        ball.velocity = Vector3.zero;
        ball.useGravity = false;

    }

    private void WerteSetzen()
    {
        abwurfgeschwindigkeit= abwurfgeschwindigkeitVector3.magnitude ;
        abwurfwinkelJ3 = achsen[WurfAchse-1].AktuelleRotationDerAchse() ;
        abwurfwinkelBall =  BerechneAbwurfwinkel();
    }

    private void SetzeSollrotation(float[] sollWinkel)
    {
        for(int i = 0; i < anzahlAchsen; i++)
        {
            sollRotation[i] = sollWinkel[i];
        }
    }

    private void SetzeSollRotationsGeschwindigkeit(float[] sollRotaionsGeschwindigkeit)
    {
        for(int i = 0; i < anzahlAchsen; i++)
        {
            achsen[i].achsengeschwindigkeit = sollRotaionsGeschwindigkeit[i];
        }
    }

    private void AktuelleRotationenAuslesen()
    {
        for(int i = 0; i < anzahlAchsen; i++)
        {
            istRotation[i] = achsen[i].AktuelleRotationDerAchse();
        }
    }


    private void BerechneGeschwindigkeit()
    {
        //aktuelle Geschwindigkeit des Abwurfpunktes am Greifer
        abwurfgeschwindigkeitVector3 = (abwurfPosition.transform.position - letztePosition) / Time.fixedDeltaTime;
        letztePosition = abwurfPosition.transform.position;

        //abwurfgeschwindigkeitVector3 = j6.GetSpeed();
        Debug.Log(abwurfgeschwindigkeitVector3);

        //Debug.Log(abwurfgeschwindigkeitVector3);

    }

    private void Abwurf()
    {
        Debug.Log("Abwurf");
        ball.MovePosition(abwurfPosition.position);
        ball.useGravity = true;
        ball.velocity =(abwurfgeschwindigkeitVector3);
    }

    /*

    private void InStartpositionFahren()
    {
            for (int i = 0; i < anzahlAchsen; i++)
            {
                achsen[i].RotiereAchse(startRotation[i]);
            }
            
        achsen[WurfAchse-1].rotationState = RotationsRichtung.Neutral;
        abwurfgeschwindigkeitVector3 = Vector3.zero;
        abwurfgeschwindigkeit = 0.0f;
        abwurfwinkelJ3 = 0.0f;
        abwurfwinkel = 0.0f;
        ball.velocity = Vector3.zero;
        ball.useGravity = false;
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
            abwurfStatus = RoboterStatus.Neutral;
        }

    }
    */

    private void RotiereAlleAchsen()
    {
        sollIst = false;
        for(int i = 0; i < anzahlAchsen; i++)
        {
            if(sollRotation[i] < 0)
            {
                if(istRotation[i] <= sollRotation[i] + toleranzwinkel && istRotation[i] >= sollRotation[i] - toleranzwinkel)
                {
                    achsen[i].rotationState = RotationsRichtung.Neutral;
                }
                else
                {
                    if(istRotation[i] - sollRotation[i] < 0)
                    {
                    achsen[i].rotationState = RotationsRichtung.Positiv;
                    }
                    else
                    {
                    achsen[i].rotationState = RotationsRichtung.Negativ;
                    }
                }

            }
            else
            {
                if(istRotation[i] >= sollRotation[i] - toleranzwinkel && istRotation[i]<= sollRotation[i] + toleranzwinkel)
                {
                    achsen[i].rotationState = RotationsRichtung.Neutral;
                }
                else
                {
                    if(istRotation[i] - sollRotation[i] < 0)
                    {
                    achsen[i].rotationState = RotationsRichtung.Positiv;
                    }
                    else
                    {
                    achsen[i].rotationState = RotationsRichtung.Negativ;
                    }
                }

            }
        }
        bool fix = true;
        for(int i = 0; i < anzahlAchsen; i++)
        {
            if(achsen[i].rotationState != RotationsRichtung.Neutral)
            {
                fix = false;
            }

        }
        if(fix)
        {
            sollIst = true;
            
        }
        

    }

    // private bool AbwurfwinkelErreicht()
    // {

    //     for(int i = 0; i < anzahlAchsen; i++)
    //     {
    //         if(sollRotation[i] < 0)
    //         {
    //             if(istRotation[i] <= sollRotation[i] + toleranzwinkel && istRotation[i] >= sollRotation[i] - toleranzwinkel)
    //             {
    //                 return true;
    //             }

    //         }
    //         else
    //         {
    //             if(istRotation[i] >= sollRotation[i] - toleranzwinkel && istRotation[i]<= sollRotation[i] + toleranzwinkel)
    //             {
    //                 return true;
    //             }
    //         }
    //     }
    //     return false;

    // }

    private float BerechneAbwurfwinkel(){
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

    /*
    public void StarteAbwurfMitKI(float geschwindigkeit, float winkel)
    {
        abwurfwinkel = UebersetzeWinkel(winkel);
        wurfgeschwindigkeit = UebersetzGeschwindigkeit(geschwindigkeit);
        abwurfSignal = true;
    }
    */
    public void StarteAbwurf(float[] abwurfRotation,float[] abwurfGeschwindigkeit)
    {
        Debug.Log("Los");
        SetzeSollrotation(abwurfRotation);
        SetzeSollRotationsGeschwindigkeit(abwurfGeschwindigkeit);
        befehl = Befehl.Abwurf;

    }

    public void InStartposition(float[] startRotation,float[] startGeschwindigkeit )
    {
        //abwurfStatus = RoboterStatus.InStartposition;
        SetzeSollrotation(startRotation);
        SetzeSollRotationsGeschwindigkeit(startGeschwindigkeit);
        befehl = Befehl.Start;
    }
    // public void StarteAbwurf()
    // {
    //     befehl = Befehl.Abwurf;

    // }

    // public void InStartposition()
    // {
    //     befehl = Befehl.Start;
    // }

    public void SetzeGeschwindikeitDerScene(float geschwindigkeit)
    {
        Time.timeScale = geschwindigkeit;
    }

    // public void RotiereAchseJ1(float winkel)
    // {
    //     j1.RotiereAchse(winkel);
    // }

    // public void RotiereAchseJ2(float winkel)
    // {
    //     j2.RotiereAchse(winkel);
    // }

    // public void RotiereAchseJ3(float winkel)
    // {
    //     j3.RotiereAchse(winkel);
    // }

    // public void RotiereAchseJ4(float winkel)
    // {
    //     j4.RotiereAchse(winkel);
    // }

    // public void RotiereAchseJ5(float winkel)
    // {
    //     j5.RotiereAchse(winkel);
    // }

}
