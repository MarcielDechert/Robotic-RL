using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

// Globaler Zustand für die Kollisions Layer
public enum KollisionsLayer { Neutral = 0, Wand = 1, Boden = 2, Decke = 3, Becherwand = 4, Becherboden = 5, Einwurfzone = 6, Roboter = 7, TCP = 8};

/// <summary>
/// Steuert das Verhalten des Balls. Erbt von der Klasse BallController
/// </summary>
public class TTBallController : BallController
{
    private double luftwiderstand; // in Newton
    private double flaeche;        // m² 
    private double reynoldzahl;
    private double reynoldzahlKrit = 3692.0;
    private double cw;
    private double ballDurchmesser;
    private double luftDichte = 1.2041;
    private double viskoseLuft = 0.0000182;
    private string path = "Assets/test1.csv";
    private StreamWriter writer;
    private float stopZeit = 0.0f;

    /// <summary>
    /// Wird beim Start einmalig aufgerufen. Initialisiert Attribute
    /// </summary>
    private void Start()
    {
        flaeche = BerechneBallFlaeche(transform.localScale.y);
        ballDurchmesser = transform.localScale.y;
    }

    /// <summary>
    /// Führt das Verhalten des Balls aus. Ersetzt die FixedUpdate() Methode
    /// </summary>
    public override void Step()
    {
        BerechneBallgeschwindigkeit();
        SetzeLuftwidertand();
    }

    /// <summary>
    /// Lässt ein Luftwidertand dem Ball entgegenwirken
    /// </summary>
    private void SetzeLuftwidertand()
    {
        // wenn die Luftwidertands Flag aktiv ist
        if (LuftwiderstandAktiv)
        {
            // wenn die Geschwindigkeit des Balls ungleich 0 ist und der RoboterStatus Wirft ist
            if ( Ballgeschwindigkeit.magnitude != 0 && area.r_robot.RoboterStatus == RoboterStatus.Wirft) 
            {
                // Reynoldszahl wird neu geseztzt
                reynoldzahl = BerechneReynoldszahl(luftDichte, Ballgeschwindigkeit.magnitude, ballDurchmesser, viskoseLuft);

                // wenn die aktuelle Reynoldszahl größer der kritischen Reynoldszahl ist
                // Turbulente Strömung
                if(reynoldzahl >= reynoldzahlKrit)
                {
                    // Widerstandsbeiwert wird neu gesetzt
                    cw = BerechneCwWert(reynoldzahl);
                    // Luftwidertand wird neu gesetzt
                    luftwiderstand = BerechneLuftwiderstandTurbulent(cw,flaeche,luftDichte, Ballgeschwindigkeit.magnitude);
                }
                // Laminare Strömung
                else
                {
                     //Luftwidertand wird neu gesetzt
                     luftwiderstand = BerechneLuftwiderstandLaminar(viskoseLuft, ballDurchmesser, Ballgeschwindigkeit.magnitude);
                }

                //Debug.Log("V_Betrag:"+Ballgeschwindigkeit.magnitude);
                //Debug.Log("F_Betrag:" + luftwiderstand);
                //Debug.Log(stopZeit);

                stopZeit += Time.fixedDeltaTime;

                // der Luftwidertand wird dem Ball entgegengewirkt
                area.R_ball.GetComponent<Rigidbody>().AddForce(-Ballgeschwindigkeit.normalized * (float)luftwiderstand, ForceMode.Force);

                
                 /*
                  
                if (this.transform.position.y > 0.015)
                {
                    writer = new StreamWriter(path, true);
                    writer.WriteLine(Ballgeschwindigkeit.magnitude+" "+ Mathf.Abs(transform.position.x - area.r_robot.AbwurfPosition.transform.position.x) + " "+ this.transform.position.y + " " +luftwiderstand);
                    writer.Close();

                    //Write some text to the test.txt file
                    //

                    //Re-import the file to update the reference in the editor
                    //AssetDatabase.ImportAsset(path);
                    // TextAsset asset = (TextAsset)Resources.Load("test");

                    //Print the text from the file
                    //Debug.Log(asset.text);

                }
                
                */
            }
        }
    }

    /// <summary>
    /// Berechnet den Luftwiderstand für turbulente Strömung
    /// </summary>
    /// <param name="_cw"> Widerstandsbeiwert</param>
    /// <param name="_flaeche"> Querschnitsfläsche des Balls</param>
    /// <param name="_luftDichte"> Luftdichte</param>
    /// <param name="_geschwindigkeit"> Betrag der Geschwindigkeit des Balls </param>
    /// <returns> Widertand in Newton als Double</returns>
    private double BerechneLuftwiderstandTurbulent(double _cw, double _flaeche, double _luftDichte, double _geschwindigkeit)
    {
        return 0.5 * _cw * _flaeche * _luftDichte * Mathf.Pow( (float)_geschwindigkeit,2);
    }

    /// <summary>
    /// Berechnet den Luftwiderstand für laminare Strömung
    /// </summary>
    /// <param name="_viskoseLuft"> Viskosität der Luft</param>
    /// <param name="_ballDurchmesser"> Durchmesser des Balls</param>
    /// <param name="_geschwindigkeit"> Betrag der Geschwindigkeit des Balls</param>
    /// <returns>Widertand in Newton als Double</returns>
    private double BerechneLuftwiderstandLaminar(double _viskoseLuft, double _ballDurchmesser, double _geschwindigkeit)
    {
        return 6.0 * Mathf.PI * _viskoseLuft * _ballDurchmesser / 2 * _geschwindigkeit;
    }

    /// <summary>
    /// Berechnet die Querschnittsfläche des Balls
    /// </summary>
    /// <param name="_durchmesser"> Durchmesser des Balls</param>
    /// <returns> Fläche in m² als Double</returns>
    private double BerechneBallFlaeche(double _durchmesser)
    {
        return  Mathf.PI * Mathf.Pow( (float) _durchmesser,2) / 4 ;
    }

    /// <summary>
    /// Berechnet die Reynoldszahl
    /// </summary>
    /// <param name="_luftDichte"> Luftdichte</param>
    /// <param name="_geschwindigkeit">Betrag der Geschwindigkeit des Balls</param>
    /// <param name="_durchmesser"> Durchmesser des Balls</param>
    /// <param name="_viskoseLuft"> Viskosität des Balls</param>
    /// <returns> Reynoldszahl als Double</returns>
    private double BerechneReynoldszahl(double _luftDichte, double _geschwindigkeit, double _durchmesser, double _viskoseLuft)
    {
        return _luftDichte * _geschwindigkeit * _durchmesser / _viskoseLuft ;
    }

    /// <summary>
    /// Berechnet den Widerstandsbeiwert
    /// </summary>
    /// <param name="_reynoldzahl"> Reynoldszahl</param>
    /// <returns> Widerstandbeiwert als Double</returns>
    private double BerechneCwWert(double _reynoldzahl)
    {
        return 24/_reynoldzahl + 4/Mathf.Sqrt( (float) _reynoldzahl) + 0.4  ;
    }
    

}
