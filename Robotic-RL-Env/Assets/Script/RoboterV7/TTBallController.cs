using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public enum KollisionsLayer { Neutral = 0, Wand = 1, Boden = 2, Decke = 3, Becherwand = 4, Becherboden = 5, Einwurfzone = 6, Roboter = 7, TCP = 8};

public class TTBallController : BallController
{
    private double luftwiderstand;
    private double luftwiderstandfx;
    private double luftwiderstandfy;
    private double flaeche;
    private double reynoldzahl;
    private double reynoldzahlKrit = 4852.0;
    private double cw;
    private double ballDurchmesser;
    private double luftDichte = 1.2041;
    private double viskoseLuft = 0.0000182;
    private float cd = .47f;
    private string path = "Assets/test1.csv";
    private StreamWriter writer;
    private float stopZeit = 0.0f;

    private void Start()
    {
        flaeche = BerechneBallFlaeche(transform.localScale.y);
        ballDurchmesser = transform.localScale.y;
    }
    public override void Step()
    {
        BerechneBallgeschwindigkeit();
        SetzeLuftwidertand();
    }

    private void SetzeLuftwidertand()
    {

        if (LuftwiderstandAktiv)
        {
            if ( Ballgeschwindigkeit.magnitude != 0 && area.r_robot.RoboterStatus == RoboterStatus.Wirft) 
            {
                
                reynoldzahl = BerechneReynoldszahl(luftDichte, Ballgeschwindigkeit.magnitude, ballDurchmesser, viskoseLuft);

                if(reynoldzahl >= reynoldzahlKrit)
                {

                    cw = BerechneCwWert(reynoldzahl);
                    luftwiderstand = BerechneLuftwiderstandTurbulent(cw,flaeche,luftDichte, Ballgeschwindigkeit.magnitude);
                }
                else
                {
                     luftwiderstand = BerechneLuftwiderstandLaminar(viskoseLuft, ballDurchmesser, Ballgeschwindigkeit.magnitude);

                }

                Debug.Log("V_Betrag:"+Ballgeschwindigkeit.magnitude);
                Debug.Log("F_Betrag:" + luftwiderstand);
                Debug.Log(stopZeit);

                stopZeit += Time.fixedDeltaTime;

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

    private double BerechneLuftwiderstandTurbulent(double _cw, double _flaeche, double _luftDichte, double _geschwindigkeit)
    {
        return 0.5 * _cw * _flaeche * _luftDichte * Mathf.Pow( (float)_geschwindigkeit,2);
    }

    private double BerechneLuftwiderstandLaminar(double _viskoseLuft, double _ballDurchmesser, double _geschwindigkeit)
    {
        return 6.0 * Mathf.PI * _viskoseLuft * _ballDurchmesser / 2 * _geschwindigkeit;
    }

    private double BerechneBallFlaeche(double _durchmesser)
    {
        return  Mathf.PI * Mathf.Pow( (float) _durchmesser,2) / 4 ;
    }

    private double BerechneReynoldszahl(double _luftDichte, double _geschwindigkeit, double _durchmesser, double _viskoseLuft)
    {
        return _luftDichte * _geschwindigkeit * _durchmesser / _viskoseLuft ;
    }

    private double BerechneCwWert(double _reynoldzahl)
    {
        return 24/_reynoldzahl + 4/Mathf.Sqrt( (float) _reynoldzahl) + 0.4  ;
    }
    

}
