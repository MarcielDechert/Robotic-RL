using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum RoboterStatus { Neutral = 0, Abwurfbereit = 1, Wirft = 2, Faehrt = 3, Stopp = -1 };
public enum Befehl { Neutral = 0, Abwurf = 1, Start = 2 };

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
    [SerializeField] private int anzahlAchsen = 6;

    private RoboterStatus roboterStatus = RoboterStatus.Neutral;
    public RoboterStatus RoboterStatus { get => roboterStatus; }

    private Befehl befehl = Befehl.Neutral;
    private Vector3 abwurfgeschwindigkeitVector3;
    private float abwurfgeschwindigkeit;
    public float Abwurfgeschwindigkeit { get => abwurfgeschwindigkeit; set => abwurfgeschwindigkeit = value; }

    private float abwurfwinkelBall;
    public float AbwurfwinkelBall { get => abwurfwinkelBall; set => abwurfwinkelBall = value; }

    private AchseV7[] achse;

    private List<AchseV7> achseV7s;

    private float[] sollRotation;

    private float[] sollGeschwindigkeit;

    private float[] istRotation;
    public float[] IstRotation { get => istRotation; set => istRotation = value; }

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
        sollGeschwindigkeit = new float[anzahlAchsen];

        achse = new AchseV7[anzahlAchsen];

        achse[0] = j1;
        achse[1] = j2;
        achse[2] = j3;
        achse[3] = j4;
        achse[4] = j5;
        achse[5] = j6;

        abwurfSignal = false;
    }

    private void FixedUpdate()
    {

        switch (RoboterStatus)
        {

            case RoboterStatus.Neutral:
                if (befehl == Befehl.Start)
                {
                    roboterStatus = RoboterStatus.Faehrt;
                    RotiereAlleAchsen();
                }
                break;

            case RoboterStatus.Faehrt:
                if (sollIstErreicht())
                {
                    if (abwurfSignal)
                    {
                        roboterStatus = RoboterStatus.Wirft;
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
                }
                break;

            case RoboterStatus.Abwurfbereit:
                abwurfSignal = true;
                if (befehl == Befehl.Abwurf)
                {
                    //sollIst = false;
                    roboterStatus = RoboterStatus.Faehrt;
                    RotiereAlleAchsen();
                }
                break;

            case RoboterStatus.Wirft:
                abwurfSignal = false;
                if (befehl == Befehl.Start)
                {
                    roboterStatus = RoboterStatus.Faehrt;
                    RotiereAlleAchsen();
                    //sollIst = false;
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
        ball.angularVelocity = Vector3.zero;
        ball.useGravity = false;
    }

    private void WerteSetzen()
    {
        abwurfgeschwindigkeit = abwurfgeschwindigkeitVector3.magnitude;
        abwurfwinkelBall = BerechneAbwurfwinkel();
    }

    private void SetzeSollrotation(float[] sollWinkel)
    {
        for (int i = 0; i < anzahlAchsen; i++)
        {
            sollRotation[i] = sollWinkel[i];
        }
    }

    private void SetzeSollRotationsGeschwindigkeit(float[] sollRotaionsGeschwindigkeit)
    {
        for (int i = 0; i < anzahlAchsen; i++)
        {
            sollGeschwindigkeit[i] = sollRotaionsGeschwindigkeit[i];
        }
    }


    private void BerechneAbwurfgeschwindigkeit()
    {

        abwurfgeschwindigkeitVector3 = achse[anzahlAchsen - 1].GetSpeed();

    }

    private void Abwurf()
    {
        Debug.Log("Abwurf");
        ball.MovePosition(abwurfPosition.position);
        ball.useGravity = true;
        ball.velocity = (abwurfgeschwindigkeitVector3);
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
        return Mathf.Rad2Deg * Mathf.Acos(-abwurfgeschwindigkeitVector3.x / abwurfgeschwindigkeitVector3.magnitude);
    }

    public void StarteAbwurf(float[] abwurfRotation, float[] abwurfGeschwindigkeit)
    {
        SetzeSollrotation(abwurfRotation);
        SetzeSollRotationsGeschwindigkeit(abwurfGeschwindigkeit);
        befehl = Befehl.Abwurf;
    }

    public void InStartposition(float[] startRotation, float[] startGeschwindigkeit)
    {
        SetzeSollrotation(startRotation);
        SetzeSollRotationsGeschwindigkeit(startGeschwindigkeit);
        befehl = Befehl.Start;
    }
}
