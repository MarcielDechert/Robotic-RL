using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum KollisionsLayer { Neutral = 0, Wand = 1, Boden = 2, Decke = 3, Becherwand = 4, Becherboden = 5, Einwurfzone = 6, Roboter = 7 };

public class TTBallController : BallController
{
<<<<<<< Updated upstream
    private float luftwiderstand;
    private float flaeche;
    private float reynoldzahl;
    private float cw;
    private float ballDurchmesser;
    private float luftDichte = 1.2041f;
    private float viskoseLuft = 0.0000182f;

=======
    private const double  luftDichte = 1.2041;          // Luftdruck in kg/m^3 bei 20°C Umgebungstemp.
    private const double  luftViskositaet = 0.0000182;  // Viskosität der Luft in Pa*s
    private const double  reynoldzahlKrit = 3692.0;     // kritische Reynoldszahl: Laminar --> Turbulent

    private double luftWiderstand;                      // in Newton
    private double flaeche;                             // m² 
    private double reynoldzahl;                         // dimensionale Strömungsgeschwindigkeit
    private double cw;                                  // Widerstandsbeiwert
    private double ballDurchmesser;

    /*
     * Export Flugdaten in csv Datei
    private string path = "Assets/test1.csv";
    private StreamWriter writer;
    private float stopZeit = 0.0f;

    */

    /// <summary>
    /// Wird beim Start einmalig aufgerufen. Initialisiert Attribute
    /// </summary>
>>>>>>> Stashed changes
    private void Start()
    {
        flaeche = BerechneBallFlaeche(transform.localScale.y);
        ballDurchmesser = transform.localScale.y;
    }
    public override void Step()
    {
        BerechneBallgeschwindigkeit();
        SetLuftwidertand();
    }

<<<<<<< Updated upstream
    private void SetzeLuftwidertand()
    {
        if(LuftwiderstandAktiv)
        {
            reynoldzahl = BerechneReynoldszahl(luftDichte,Ballgeschwindigkeit.magnitude,ballDurchmesser, viskoseLuft);
            cw = BerechneCwWert(reynoldzahl);
            luftwiderstand = BerechneLuftwiderstand(cw,flaeche,luftDichte,Ballgeschwindigkeit.magnitude);
=======
    /// <summary>
    /// Lässt ein Luftwidertand dem Ball entgegenwirken
    /// </summary>
    private void SetLuftwidertand()
    {
        // wenn die Luftwidertands Flag aktiv ist
        if (IsLuftwiderstandAktiv)
        {
            // wenn die Geschwindigkeit des Balls ungleich 0 ist 
            if ( BallGeschwindigkeit.magnitude != 0 ) 
            {
                // Reynoldszahl wird neu geseztzt
                reynoldzahl = BerechneReynoldszahl( BallGeschwindigkeit.magnitude, ballDurchmesser);

                // wenn die aktuelle Reynoldszahl größer der kritischen Reynoldszahl ist
                // Turbulente Strömung
                if(reynoldzahl >= reynoldzahlKrit)
                {
                    // Widerstandsbeiwert wird neu gesetzt
                    cw = BerechneCwWert(reynoldzahl);
                    // Luftwidertand wird neu gesetzt
                    luftWiderstand = BerechneLuftwiderstandTurbulent(cw,flaeche, BallGeschwindigkeit.magnitude);
                }
                // Laminare Strömung
                else
                {
                     //Luftwidertand wird neu gesetzt
                     luftWiderstand = BerechneLuftwiderstandLaminar( ballDurchmesser, BallGeschwindigkeit.magnitude);
                }

                //Debug.Log("V_Betrag:"+Ballgeschwindigkeit.magnitude);
                //Debug.Log("F_Betrag:" + luftwiderstand);
                //Debug.Log(stopZeit);

                //stopZeit += Time.fixedDeltaTime;

                // der Luftwidertand wird dem Ball entgegengewirkt
                area.R_ball.GetComponent<Rigidbody>().AddForce(-BallGeschwindigkeit.normalized * (float)luftWiderstand, ForceMode.Force);

                
                 /* Export der Flugdaten
                  
                if (this.transform.position.y > 0.015)
                {
                    writer = new StreamWriter(path, true);
                    writer.WriteLine(Ballgeschwindigkeit.magnitude+" "+ Mathf.Abs(transform.position.x - area.r_robot.AbwurfPosition.transform.position.x) + " "+ this.transform.position.y + " " +luftwiderstand);
                    writer.Close();

                    //Write some text to the test.txt file
                    //
>>>>>>> Stashed changes

            if(luftwiderstand < 10.0f && area.r_robot.RoboterStatus == RoboterStatus.Wirft)
            {
                area.R_ball.GetComponent<Rigidbody>().drag = luftwiderstand;
                //Debug.Log(luftwiderstand);
            }
        }
        else
        {
            area.R_ball.GetComponent<Rigidbody>().drag = 0.0f;
        }
    }

<<<<<<< Updated upstream
    private float BerechneLuftwiderstand(float _cw, float _flaeche, float _luftDichte, float _geschwindigkeit)
    {
        return 0.5f * _cw * _flaeche * _luftDichte * Mathf.Pow(_geschwindigkeit,2) * 1000;
=======
    /// <summary>
    /// Berechnet den Luftwiderstand für turbulente Strömung
    /// </summary>
    /// <param name="_cw"> Widerstandsbeiwert</param>
    /// <param name="_flaeche"> Querschnitsfläsche des Balls</param>
    /// <param name="_luftDichte"> Luftdichte</param>
    /// <param name="_geschwindigkeit"> Betrag der Geschwindigkeit des Balls </param>
    /// <returns> Widertand in Newton als Double</returns>
    private double BerechneLuftwiderstandTurbulent(double _cw, double _flaeche, double _geschwindigkeit)
    {
        return 0.5 * _cw * _flaeche * luftDichte * Mathf.Pow( (float)_geschwindigkeit,2);
    }

    /// <summary>
    /// Berechnet den Luftwiderstand für laminare Strömung
    /// </summary>
    /// <param name="_viskoseLuft"> Viskosität der Luft</param>
    /// <param name="_ballDurchmesser"> Durchmesser des Balls</param>
    /// <param name="_geschwindigkeit"> Betrag der Geschwindigkeit des Balls</param>
    /// <returns>Widertand in Newton als Double</returns>
    private double BerechneLuftwiderstandLaminar( double _ballDurchmesser, double _geschwindigkeit)
    {
        return 6.0 * Mathf.PI * luftViskositaet * _ballDurchmesser / 2 * _geschwindigkeit;
>>>>>>> Stashed changes
    }

    private float BerechneBallFlaeche(float _durchmesser)
    {
        return Mathf.Pow(_durchmesser,2) / 4* Mathf.PI;
    }

<<<<<<< Updated upstream
    private float BerechneReynoldszahl(float _luftDichte, float _geschwindigkeit,float _durchmesser, float _viskoseLuft)
=======
    /// <summary>
    /// Berechnet die Reynoldszahl
    /// </summary>
    /// <param name="_luftDichte"> Luftdichte</param>
    /// <param name="_geschwindigkeit">Betrag der Geschwindigkeit des Balls</param>
    /// <param name="_durchmesser"> Durchmesser des Balls</param>
    /// <param name="_viskoseLuft"> Viskosität des Balls</param>
    /// <returns> Reynoldszahl als Double</returns>
    private double BerechneReynoldszahl( double _geschwindigkeit, double _durchmesser)
>>>>>>> Stashed changes
    {
        return luftDichte * _geschwindigkeit * _durchmesser / luftViskositaet ;
    }

    private float BerechneCwWert(float _reynoldzahl)
    {
        return 24/_reynoldzahl + 4/Mathf.Sqrt(_reynoldzahl) + 0.4f  ;
    }
    

}
