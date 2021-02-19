using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum RoboterStatus { Neutral = 0, Abwurfbereit = 1, Wirft = 2, Faehrt = 3, Stopp = -1 };
public enum Befehl { Neutral = 0, Abwurf = 1, Start = 2 };

public class Achsen6RoboterController : RoboterController
{
    [SerializeField] private RobotsLearningArea area;

    private RotationsAchse[] achse;
    public RotationsAchse[] RotationsAchse { get => achse; set => achse = value; }

    [SerializeField] private RotationsAchse j1;
    [SerializeField] private RotationsAchse j2;
    [SerializeField] private RotationsAchse j3;
    [SerializeField] private RotationsAchse j4;
    [SerializeField] private RotationsAchse j5;
    [SerializeField] private RotationsAchse j6;
    [SerializeField] private int anzahlAchsen = 6;

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

        // achseV7s = new List<RotationsAchse>();
        // achseV7s.Add(j1);
        // achseV7s.Add(j2);
        // achseV7s.Add(j3);
        // achseV7s.Add(j4);
        // achseV7s.Add(j5);
        // achseV7s.Add(j6);

        achse = new RotationsAchse[anzahlAchsen];

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
                if (RoboterBefehl == Befehl.Start)
                {
                    RoboterStatus = RoboterStatus.Faehrt;
                    RotiereAlleAchsen();
                }
                break;

            case RoboterStatus.Faehrt:
                if (SollIstErreicht())
                {
                    if (abwurfSignal)
                    {
                        area.R_ball.Abwurf(AbwurfPosition.position,AbwurfgeschwindigkeitVector3);
                        WerteSetzen();
                        RoboterStatus = RoboterStatus.Wirft;
                    }
                    else
                    {
                        RoboterStatus = RoboterStatus.Abwurfbereit;
                        area.AreaReset();
                    }
                }
                else
                {
                    if (abwurfSignal)
                    {
                    area.R_ball.GetComponent<Rigidbody>().MovePosition(AbwurfPosition.position); 
                    }
                    BerechneAbwurfgeschwindigkeit();
                }
                break;

            case RoboterStatus.Abwurfbereit:
                abwurfSignal = true;
                if (RoboterBefehl == Befehl.Abwurf)
                {
                    //sollIst = false;
                    j3.Wirft = true;
                    RoboterStatus = RoboterStatus.Faehrt;
                    RotiereAlleAchsen();
                }
                break;

            case RoboterStatus.Wirft:
                abwurfSignal = false;
                j3.Wirft = false;
                if (RoboterBefehl == Befehl.Start)
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
    private void RotiereAlleAchsen()
    {
        for (int i = 0; i < anzahlAchsen; i++)
        {
            achse[i].RotiereAchseBis(sollRotation[i], sollGeschwindigkeit[i]);
        }
    }

    private bool SollIstErreicht()
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

    public override void StarteAbwurf(float[] abwurfRotation, float[] abwurfGeschwindigkeit)
    {
        SetzeSollrotation(abwurfRotation);
        SetzeSollRotationsGeschwindigkeit(abwurfGeschwindigkeit);
        RoboterBefehl = Befehl.Abwurf;
    }

    public override void InStartposition(float[] startRotation, float[] startGeschwindigkeit)
    {
        SetzeSollrotation(startRotation);
        SetzeSollRotationsGeschwindigkeit(startGeschwindigkeit);
        RoboterBefehl = Befehl.Start;
    }

    public override RotationsAchse[] GetAchsen()
    {
        return achse;
    }
}
