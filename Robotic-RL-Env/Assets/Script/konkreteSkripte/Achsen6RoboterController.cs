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

    private List<RotationsAchse> achseV7s;

    private float[] sollRotation;

    private float[] sollGeschwindigkeit;

<<<<<<< Updated upstream
    private float[] istRotation;
    public float[] IstRotation { get => istRotation; set => istRotation = value; }
    private bool abwurfSignal;
=======
    private bool isAbwurfSignal;
>>>>>>> Stashed changes



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

        isAbwurfSignal = false;
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
<<<<<<< Updated upstream
                if (sollIstErreicht())
                {
                    if (abwurfSignal)
                    {
                        RoboterStatus = RoboterStatus.Wirft;
                        area.R_ball.Abwurf(AbwurfPosition.position,AbwurfgeschwindigkeitVector3);
                        WerteSetzen();
=======

                // Wenn die Sollrotation der Achsen erreicht ist
                if (IsSollErreicht())
                {
                    //wenn die Flag abwurfSingnal gesetzt ist
                    if (isAbwurfSignal)
                    {
                        //Ball wird losgelassen
                        area.R_ball.Abwurf(AbwurfPosition.position,AbwurfGeschwindigkeitVector3);
                        SetWerte();
                        RoboterStatus = RoboterStatus.Wirft;
>>>>>>> Stashed changes
                    }
                    else
                    {
                        RoboterStatus = RoboterStatus.Abwurfbereit;
                        area.BallReset();
                    }
                }
                else
                {
<<<<<<< Updated upstream
=======
                    //wenn die Flag abwurfSingnal gesetzt ist
                    if (isAbwurfSignal)
                    {
                        //Position des Balls wird verändert
                        area.R_ball.GetComponent<Rigidbody>().MovePosition(AbwurfPosition.position); 
                    }
>>>>>>> Stashed changes
                    BerechneAbwurfgeschwindigkeit();
                }
                break;

            case RoboterStatus.Abwurfbereit:
<<<<<<< Updated upstream
                abwurfSignal = true;
                if (Befehl == Befehl.Abwurf)
                {
                    //sollIst = false;
                    j3.Wirft = true;
=======
                isAbwurfSignal = true;

                //wenn der RoboterBefehl den Zustand Abwurf hat 
                if (RoboterBefehl == Befehl.Abwurf)
                {
                    j3.IsWirft = true;
>>>>>>> Stashed changes
                    RoboterStatus = RoboterStatus.Faehrt;
                    RotiereAlleAchsen();
                }
                break;

            case RoboterStatus.Wirft:
<<<<<<< Updated upstream
                abwurfSignal = false;
                j3.Wirft = false;
                if (Befehl == Befehl.Start)
=======
                isAbwurfSignal = false;
                j3.IsWirft = false;

                //wenn der RoboterBefehl den Zustand Start hat 
                if (RoboterBefehl == Befehl.Start)
>>>>>>> Stashed changes
                {
                    RoboterStatus = RoboterStatus.Faehrt;
                    RotiereAlleAchsen();
                    //sollIst = false;
                }
                break;
        }
    }

<<<<<<< Updated upstream
    private void WerteSetzen()
=======
    /// <summary>
    /// Setzt die Werte für Abwurfgeschwindigkeit und Abwurfwinkel des TCPs bzw den Ball
    /// </summary>
    private void SetWerte()
>>>>>>> Stashed changes
    {
        AbwurfGeschwindigkeit = AbwurfGeschwindigkeitVector3.magnitude;
        AbwurfWinkelBall = BerechneAbwurfwinkel();
    }

<<<<<<< Updated upstream
    private void SetzeSollrotation(float[] sollWinkel)
=======
    /// <summary>
    /// Füllt das Array sollRotation mit dem Array was übergeben wurde
    /// </summary>
    /// <param name="sollWinkel"> Array mit den Sollwinkeln</param>
    private void SetSollrotation(float[] sollWinkel)
>>>>>>> Stashed changes
    {
        // foreach (var item in achseV7s)
        // {


        // }
        for (int i = 0; i < anzahlAchsen; i++)
        {
            sollRotation[i] = sollWinkel[i];
        }
    }

<<<<<<< Updated upstream
    private void SetzeSollRotationsGeschwindigkeit(float[] sollRotaionsGeschwindigkeit)
=======
    /// <summary>
    /// Füllt das Array sollGeschwindigkeit mit dem Array was übergeben wurde
    /// </summary>
    /// <param name="sollRotaionsGeschwindigkeit">Array mit den Sollgeschwindigkeiten</param>
    private void SetSollRotationsGeschwindigkeit(float[] sollRotaionsGeschwindigkeit)
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
    private bool sollIstErreicht()
=======
    /// <summary>
    /// Überpüft ob die Achsen sich bewegen, also ob die Sollrotationen erreicht sind
    /// </summary>
    /// <returns> true wenn sich alle Achsen nicht mehr bewegen</returns>
    private bool IsSollErreicht()
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        SetzeSollrotation(abwurfRotation);
        SetzeSollRotationsGeschwindigkeit(abwurfGeschwindigkeit);
        Befehl = Befehl.Abwurf;
=======
        SetSollrotation(abwurfRotation);
        SetSollRotationsGeschwindigkeit(abwurfGeschwindigkeit);
        RoboterBefehl = Befehl.Abwurf;
>>>>>>> Stashed changes
    }

    public override void InStartposition(float[] startRotation, float[] startGeschwindigkeit)
    {
<<<<<<< Updated upstream
        SetzeSollrotation(startRotation);
        SetzeSollRotationsGeschwindigkeit(startGeschwindigkeit);
        Befehl = Befehl.Start;
=======
        SetSollrotation(startRotation);
        SetSollRotationsGeschwindigkeit(startGeschwindigkeit);
        RoboterBefehl = Befehl.Start;
>>>>>>> Stashed changes
    }

    public override RotationsAchse[] GetAchsen()
    {
        return achse;
    }
}
