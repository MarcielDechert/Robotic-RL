using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum RoboterStatus { Neutral = 0, Abwurfbereit = 1, Wirft = 2, Faehrt = 3, Stopp = -1 };
public enum Befehl { Neutral = 0, Abwurf = 1, Start = 2 };

public class RoboterControllerV7 : RoboterController
{
    [SerializeField] private RobotsLearningArea area;

    private AchseV7[] achse;
    public AchseV7[] AchseV7 { get => achse; set => achse = value; }

    [SerializeField] private AchseV7 j1;
    [SerializeField] private AchseV7 j2;
    [SerializeField] private AchseV7 j3;
    [SerializeField] private AchseV7 j4;
    [SerializeField] private AchseV7 j5;
    [SerializeField] private AchseV7 j6;
    [SerializeField] private int anzahlAchsen = 6;

    private List<AchseV7> achseV7s;

    private float[] sollRotation;

    private float[] sollGeschwindigkeit;

    private float[] istRotation;
    public float[] IstRotation { get => istRotation; set => istRotation = value; }
    private bool abwurfSignal;

    private Vector3 letztePosition;



    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        sollRotation = new float[anzahlAchsen];
        istRotation = new float[anzahlAchsen];
        sollGeschwindigkeit = new float[anzahlAchsen];

        // achseV7s = new List<AchseV7>();
        // achseV7s.Add(j1);
        // achseV7s.Add(j2);
        // achseV7s.Add(j3);
        // achseV7s.Add(j4);
        // achseV7s.Add(j5);
        // achseV7s.Add(j6);

        achse = new AchseV7[anzahlAchsen];

        achse[0] = j1;
        achse[1] = j2;
        achse[2] = j3;
        achse[3] = j4;
        achse[4] = j5;
        achse[5] = j6;

        abwurfSignal = false;
    }

    public override void Step()
    {

        switch (RoboterStatus)
        {

            case RoboterStatus.Neutral:
                if (Befehl == Befehl.Start)
                {
                    RoboterStatus = RoboterStatus.Faehrt;
                    RotiereAlleAchsen();
                }
                break;

            case RoboterStatus.Faehrt:
                if (sollIstErreicht())
                {
                    if (abwurfSignal)
                    {
                        RoboterStatus = RoboterStatus.Wirft;
                        Abwurf();
                        WerteSetzen();
                    }
                    else
                    {
                        RoboterStatus = RoboterStatus.Abwurfbereit;
                        area.ResetBall();
                    }
                }
                else
                {
                    BerechneAbwurfgeschwindigkeit();
                }
                break;

            case RoboterStatus.Abwurfbereit:
                abwurfSignal = true;
                if (Befehl == Befehl.Abwurf)
                {
                    //sollIst = false;
                    RoboterStatus = RoboterStatus.Faehrt;
                    RotiereAlleAchsen();
                }
                break;

            case RoboterStatus.Wirft:
                abwurfSignal = false;
                if (Befehl == Befehl.Start)
                {
                    RoboterStatus = RoboterStatus.Faehrt;
                    RotiereAlleAchsen();
                    //sollIst = false;
                }
                break;
        }
    }

    private void WerteSetzen()
    {
        Abwurfgeschwindigkeit = AbwurfgeschwindigkeitVector3.magnitude;
        AbwurfwinkelBall = BerechneAbwurfwinkel();
    }

    private void SetzeSollrotation(float[] sollWinkel)
    {
        // foreach (var item in achseV7s)
        // {


        // }
        for (int i = 0; i < anzahlAchsen; i++)
        {
            sollRotation[i] = sollWinkel[i];
        }
    }

    private void SetzeSollRotationsGeschwindigkeit(float[] sollRotaionsGeschwindigkeit)
    {
        for (int i = 0; i < anzahlAchsen; i++)
        {
            //achse[i].achsengeschwindigkeit = sollRotaionsGeschwindigkeit[i];

            sollGeschwindigkeit[i] = sollRotaionsGeschwindigkeit[i];
        }
    }


    private void BerechneAbwurfgeschwindigkeit()
    {
        AbwurfgeschwindigkeitVector3 = (AbwurfPosition.position - letztePosition) / Time.fixedDeltaTime;
        letztePosition = AbwurfPosition.position;
        //abwurfgeschwindigkeitVector3 = achse[anzahlAchsen - 1].GetSpeed();
    }

    public void Abwurf()
    {
        Debug.Log("Abwurf");
        area.R_ball.GetComponent<Rigidbody>().MovePosition(AbwurfPosition.position);
        area.R_ball.GetComponent<Rigidbody>().useGravity = true;
        area.R_ball.GetComponent<Rigidbody>().velocity = (AbwurfgeschwindigkeitVector3);
        //abwurfPunkt = abwurfPosition.position;
    }
    private void RotiereAlleAchsen()
    {
        for (int i = 0; i < anzahlAchsen; i++)
        {
            achse[i].RotiereAchseBis(sollRotation[i], sollGeschwindigkeit[i]);
        }
    }

    private bool sollIstErreicht()
    {
        bool sollIst = false;
        bool fix = true;
        for (int i = 0; i < anzahlAchsen; i++)
        {
            if (achse[i].rotationState != RotationsRichtung.Neutral)
            {
                fix = false;
            }
        }
        if (fix)
        {
            sollIst = true;
        }
        return sollIst;
    }



    private float BerechneAbwurfwinkel()
    {
        return Mathf.Rad2Deg * Mathf.Asin(Mathf.Abs(AbwurfgeschwindigkeitVector3.y) / AbwurfgeschwindigkeitVector3.magnitude);
    }

    public override void StarteAbwurf(float[] abwurfRotation, float[] abwurfGeschwindigkeit)
    {
        SetzeSollrotation(abwurfRotation);
        SetzeSollRotationsGeschwindigkeit(abwurfGeschwindigkeit);
        Befehl = Befehl.Abwurf;
    }

    public override void InStartposition(float[] startRotation, float[] startGeschwindigkeit)
    {
        SetzeSollrotation(startRotation);
        SetzeSollRotationsGeschwindigkeit(startGeschwindigkeit);
        Befehl = Befehl.Start;
    }

    public override AchseV7[] GetAchsen()
    {
        return achse;
    }
}
