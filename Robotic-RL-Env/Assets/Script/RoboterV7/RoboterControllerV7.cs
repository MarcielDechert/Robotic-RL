using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboterControllerV7 : MonoBehaviour
{

    [SerializeField] private Rigidbody ball;
    [SerializeField] private Transform abwurfPosition;

    [SerializeField] private AchseV7 j1;
    [SerializeField] private AchseV7 j2;
    [SerializeField] private AchseV7 j3;
    [SerializeField] private AchseV7 j4;
    [SerializeField] private AchseV7 j5;
    [SerializeField] private AchseV7 j6;

    [SerializeField] private float toleranzwinkel = 4.0f;
    [SerializeField] private int anzahlAchsen = 6;

    private RoboterStatus roboterStatus = RoboterStatus.Neutral;

    private Befehl befehl = Befehl.Neutral;
    private Vector3 abwurfgeschwindigkeitVector3;
    private float abwurfgeschwindigkeit;
    public float Abwurfgeschwindigkeit { get => abwurfgeschwindigkeit; set => abwurfgeschwindigkeit = value; }
    
    private float abwurfwinkelBall;
    public float AbwurfwinkelBall { get => abwurfwinkelBall; set => abwurfwinkelBall = value; }
    
    private float zeit;
    public float Zeit { get => zeit; set => zeit = value; }


    private Vector3 letztePosition = Vector3.zero;

    private AchseV7[] achse;

    private float[] sollRotation;

    private float[] istRotation;
    public float[] IstRotation { get => istRotation; set => istRotation= value; }

    private bool sollIst;
    private bool abwurfSignal;



    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    void Init()
    {
        sollRotation = new float[anzahlAchsen];
        istRotation = new float[anzahlAchsen];

        achse = new AchseV7[anzahlAchsen];

        achse[0] = j1;
        achse[1] = j2;
        achse[2] = j3;
        achse[3] = j4;
        achse[4] = j5;
        achse[5] = j6;
        
        abwurfSignal = false;
        sollIst = false;
        
    }

    private void FixedUpdate()
    {
        AktuelleRotationenAuslesen();

        switch(roboterStatus)
        {

            case RoboterStatus.Neutral:
                                        if(befehl == Befehl.Start)
                                        {
                                            roboterStatus = RoboterStatus.Faehrt;

                                        }
                                        break;

            case RoboterStatus.Faehrt:
                                        if(sollIst){
                                            if(abwurfSignal)
                                            {
                                                roboterStatus = RoboterStatus.Wirft;
                                                //zeit = Time.deltaTime -zeit;
                                                Debug.Log(zeit);
                                                Abwurf();
                                                WerteSetzen();
                                            }
                                            else
                                            {
                                                roboterStatus = RoboterStatus.Abwurfbereit;
                                                WerteZurueckSetzen();
                                            }
                                        }
                                        else
                                        {
                                            BerechneAbwurfgeschwindigkeit();
                                            RotiereAlleAchsen();
                                           // zeit = Time.deltaTime;
                                        }
                                        break;

            case RoboterStatus.Abwurfbereit:
                                        abwurfSignal = true;
                                        if (befehl == Befehl.Abwurf)
                                        {
                                            sollIst = false;
                                            roboterStatus = RoboterStatus.Faehrt;
                                            zeit += Time.fixedDeltaTime;
                                        }
                                        break;

            case RoboterStatus.Wirft:
                                        abwurfSignal = false;
                                        if (befehl == Befehl.Start)
                                        {
                                            roboterStatus = RoboterStatus.Faehrt;
                                            sollIst = false;
                                        }
                                        break;
        }
    }

    private void WerteZurueckSetzen()
    {
        abwurfgeschwindigkeitVector3 = Vector3.zero;
        abwurfgeschwindigkeit = 0.0f;
        abwurfwinkelBall = 0.0f;
        ball.velocity = Vector3.zero;
        ball.useGravity = false;

    }

    private void WerteSetzen()
    {
        abwurfgeschwindigkeit= abwurfgeschwindigkeitVector3.magnitude ;
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
            achse[i].achsengeschwindigkeit = sollRotaionsGeschwindigkeit[i];
        }
    }

    private void AktuelleRotationenAuslesen()
    {
        for(int i = 0; i < anzahlAchsen; i++)
        {
            istRotation[i] = achse[i].AktuelleRotationDerAchse();
        }
    }


    private void BerechneAbwurfgeschwindigkeit()
    {
        //aktuelle Geschwindigkeit des Abwurfpunktes am Greifer
       // abwurfgeschwindigkeitVector3 = (abwurfPosition.transform.position - letztePosition) / Time.fixedDeltaTime;
       // letztePosition = abwurfPosition.transform.position;

        abwurfgeschwindigkeitVector3 = achse[anzahlAchsen-1].GetSpeed();

    }

    private void Abwurf()
    {
        Debug.Log("Abwurf");
        ball.MovePosition(abwurfPosition.position);
        ball.useGravity = true;
        ball.velocity =(abwurfgeschwindigkeitVector3);
    }
    

    private void RotiereAlleAchsen()
    {
        sollIst = false;
        for(int i = 0; i < anzahlAchsen; i++)
        {
            if(sollRotation[i] < 0)
            {
                if(istRotation[i] <= sollRotation[i] + toleranzwinkel && istRotation[i] >= sollRotation[i] - toleranzwinkel)
                {
                    achse[i].rotationState = RotationsRichtung.Neutral;
                }
                else
                {
                    if(istRotation[i] - sollRotation[i] < 0)
                    {
                    achse[i].rotationState = RotationsRichtung.Positiv;
                    }
                    else
                    {
                    achse[i].rotationState = RotationsRichtung.Negativ;
                    }
                }

            }
            else
            {
                if(istRotation[i] >= sollRotation[i] - toleranzwinkel && istRotation[i]<= sollRotation[i] + toleranzwinkel)
                {
                    achse[i].rotationState = RotationsRichtung.Neutral;
                }
                else
                {
                    if(istRotation[i] - sollRotation[i] < 0)
                    {
                    achse[i].rotationState = RotationsRichtung.Positiv;
                    }
                    else
                    {
                    achse[i].rotationState = RotationsRichtung.Negativ;
                    }
                }

            }
        }
        bool fix = true;
        for(int i = 0; i < anzahlAchsen; i++)
        {
            if(achse[i].rotationState != RotationsRichtung.Neutral)
            {
                fix = false;
            }

        }
        if(fix)
        {
            sollIst = true; 
        }
    }

    private float BerechneAbwurfwinkel(){
        return  Mathf.Rad2Deg * Mathf.Acos(-abwurfgeschwindigkeitVector3.x/abwurfgeschwindigkeitVector3.magnitude);
    }

    public void StarteAbwurf(float[] abwurfRotation,float[] abwurfGeschwindigkeit)
    {
        // Debug.Log("Los");
        SetzeSollrotation(abwurfRotation);
        SetzeSollRotationsGeschwindigkeit(abwurfGeschwindigkeit);
        befehl = Befehl.Abwurf;

    }

    public void InStartposition(float[] startRotation,float[] startGeschwindigkeit )
    {
        SetzeSollrotation(startRotation);
        SetzeSollRotationsGeschwindigkeit(startGeschwindigkeit);
        befehl = Befehl.Start;
    }

    public void SetzeGeschwindikeitDerScene(float geschwindigkeit)
    {
        Time.timeScale = geschwindigkeit;
    }
}
