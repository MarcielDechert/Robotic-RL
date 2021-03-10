using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// globaler Zustand des Roboters
public enum RoboterStatus { Neutral = 0, Abwurfbereit = 1, Wirft = 2, Faehrt = 3, Stopp = -1 };

// globaler Befehlszustand
public enum Befehl { Neutral = 0, Abwurf = 1, Start = 2 };

/// <summary>
///  Klasse für die Steuerung des Roboters. Erbt von der abstrakten Klasse RoboterController
/// </summary>
public class Achsen6RoboterController : RoboterController
{
    [SerializeField] private RobotsLearningArea area;

    private RotationsAchse[] rotationsAchse;
    public RotationsAchse[] RotationsAchse { get => rotationsAchse; set => rotationsAchse = value; }

    [SerializeField] private RotationsAchse j1;
    [SerializeField] private RotationsAchse j2;
    [SerializeField] private RotationsAchse j3;
    [SerializeField] private RotationsAchse j4;
    [SerializeField] private RotationsAchse j5;
    [SerializeField] private RotationsAchse j6;
    [SerializeField] private int anzahlAchsen = 6;

    private float[] sollRotation;

    private float[] sollGeschwindigkeit;

    private bool abwurfSignal;

    /// <summary>
    /// Wird beim Start einmalig aufgerufen
    /// </summary>
    void Start()
    {
        Init();
    }

    /// <summary>
    /// Initialisiert und deklariert Attribute
    /// </summary>
    void Init()
    {
        sollRotation = new float[anzahlAchsen];
        sollGeschwindigkeit = new float[anzahlAchsen];

        rotationsAchse = new RotationsAchse[anzahlAchsen];

        rotationsAchse[0] = j1;
        rotationsAchse[1] = j2;
        rotationsAchse[2] = j3;
        rotationsAchse[3] = j4;
        rotationsAchse[4] = j5;
        rotationsAchse[5] = j6;

        abwurfSignal = false;
    }

    /// <summary>
    /// Kontrolliert und wechselt die Roboterzustände. Ersetzt die FixedUpdate() Methode
    /// </summary>
    public override void Step()
    {

        switch (RoboterStatus)
        {
            // Neutraler Zustand
            case RoboterStatus.Neutral:

                //wenn der RoboterBefehl den Zustand Start hat
                if (RoboterBefehl == Befehl.Start)
                {
                    RoboterStatus = RoboterStatus.Faehrt;
                    RotiereAlleAchsen();
                }
                break;

            // Faehrt Zustand
            case RoboterStatus.Faehrt:

                // Wenn die Sollrotation der Achsen erreicht ist
                if (SollIstErreicht())
                {
                    //wenn die Flag abwurfSingnal gesetzt ist
                    if (abwurfSignal)
                    {
                        //Ball wird losgelassen
                        area.R_ball.Abwurf(AbwurfPosition.position,AbwurfgeschwindigkeitVector3);
                        WerteSetzen();
                        RoboterStatus = RoboterStatus.Wirft;
                    }
                    else
                    {
                        RoboterStatus = RoboterStatus.Abwurfbereit;
                       // area.AreaReset();
                    }
                }
                else
                {
                    //wenn die Flag abwurfSingnal gesetzt ist
                    if (abwurfSignal)
                    {
                        //Position des Balls wird verändert
                        area.R_ball.GetComponent<Rigidbody>().MovePosition(AbwurfPosition.position); 
                    }
                    BerechneAbwurfgeschwindigkeit();
                }
                break;

            // Abwurfbereit Zustand
            case RoboterStatus.Abwurfbereit:
                abwurfSignal = true;

                //wenn der RoboterBefehl den Zustand Abwurf hat 
                if (RoboterBefehl == Befehl.Abwurf)
                {
                    j3.Wirft = true;
                    RoboterStatus = RoboterStatus.Faehrt;
                    RotiereAlleAchsen();
                }
                break;

            // Abwurfbereit Zustand
            case RoboterStatus.Wirft:
                abwurfSignal = false;
                j3.Wirft = false;

                //wenn der RoboterBefehl den Zustand Start hat 
                if (RoboterBefehl == Befehl.Start)
                {
                    RoboterStatus = RoboterStatus.Faehrt;
                    RotiereAlleAchsen();
                }
                break;
        }
    }

    /// <summary>
    /// Setzt die Werte für Abwurfgeschwindigkeit und Abwurfwinkel des TCPs bzw den Ball
    /// </summary>
    private void WerteSetzen()
    {
        Abwurfgeschwindigkeit = AbwurfgeschwindigkeitVector3.magnitude;
        AbwurfwinkelBall = BerechneAbwurfwinkel();
    }

    /// <summary>
    /// Füllt das Array sollRotation mit dem Array was übergeben wurde
    /// </summary>
    /// <param name="sollWinkel"> Array mit den Sollwinkeln</param>
    private void SetzeSollrotation(float[] sollWinkel)
    {
        for (int i = 0; i < anzahlAchsen; i++)
        {
            sollRotation[i] = sollWinkel[i];
        }
    }

    /// <summary>
    /// Füllt das Array sollGeschwindigkeit mit dem Array was übergeben wurde
    /// </summary>
    /// <param name="sollRotaionsGeschwindigkeit">Array mit den Sollgeschwindigkeiten</param>
    private void SetzeSollRotationsGeschwindigkeit(float[] sollRotaionsGeschwindigkeit)
    {
        for (int i = 0; i < anzahlAchsen; i++)
        {
            sollGeschwindigkeit[i] = sollRotaionsGeschwindigkeit[i];
        }
    }

    /// <summary>
    /// Ruft die Methoden der RotationsAchsen Klasse auf um alle Achsen zum Sollwinkel mit der Sollgeschwindigkeit anzufahren
    /// </summary>
    private void RotiereAlleAchsen()
    {
        for (int i = 0; i < anzahlAchsen; i++)
        {
            rotationsAchse[i].RotiereAchseBis(sollRotation[i], sollGeschwindigkeit[i]);
        }
    }

    /// <summary>
    /// Überpüft ob die Achsen sich bewegen, also ob die Sollrotationen erreicht sind
    /// </summary>
    /// <returns> true wenn sich alle Achsen nicht mehr bewegen</returns>
    private bool SollIstErreicht()
    {
        bool sollIst = false;
        bool fix = true;
        for (int i = 0; i < anzahlAchsen; i++)
        {
            if (rotationsAchse[i].RotationState != RotationsRichtung.Neutral)
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

    /// <summary>
    /// Setzt Abwurfparameter und wechselt der RoboterBefehl in Abwurf
    /// </summary>
    /// <param name="abwurfRotation"> Array mit den Abwurfrotationen der Achsen</param>
    /// <param name="abwurfGeschwindigkeit">Array mit den Abwurfgeschwindigkeiten der Achsen</param>
    public override void StarteAbwurf(float[] abwurfRotation, float[] abwurfGeschwindigkeit)
    {
        SetzeSollrotation(abwurfRotation);
        SetzeSollRotationsGeschwindigkeit(abwurfGeschwindigkeit);
        RoboterBefehl = Befehl.Abwurf;
    }

    /// <summary>
    /// Setzt Startparameter und wechselt der RoboterBefehl in Start
    /// </summary>
    /// <param name="startRotation">Array mit den Startrotationen der Achsen</param>
    /// <param name="startGeschwindigkeit">Array mit den Startgeschwindigkeiten der Achsen</param>
    public override void InStartposition(float[] startRotation, float[] startGeschwindigkeit)
    {
        SetzeSollrotation(startRotation);
        SetzeSollRotationsGeschwindigkeit(startGeschwindigkeit);
        RoboterBefehl = Befehl.Start;
    }

    /// <summary>
    /// Gibt die Achsen als Array zurück
    /// </summary>
    /// <returns> Array vom Typ RotationsAchse</returns>
    public override RotationsAchse[] GetAchsen()
    {
        return rotationsAchse;
    }
}
