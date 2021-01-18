using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum KollisionsLayer { Neutral = 0, Wand = 1, Boden = 2, Decke = 3, Becherwand = 4, Becherboden = 5, Einwurfzone = 6, Roboter = 7 };

public class TTBallController : BallController
{
    private float luftwiderstand;
    private float flaeche;
    private float reynoldzahl;
    private float cw;
    private float ballDurchmesser;
    private float luftDichte = 1.2041f;
    private float viskoseLuft = 0.0000182f;

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
        if(LuftwiderstandAktiv)
        {
            reynoldzahl = BerechneReynoldszahl(luftDichte,Ballgeschwindigkeit.magnitude,ballDurchmesser, viskoseLuft);
            cw = BerechneCwWert(reynoldzahl);
            luftwiderstand = BerechneLuftwiderstand(cw,flaeche,luftDichte,Ballgeschwindigkeit.magnitude);

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

    private float BerechneLuftwiderstand(float _cw, float _flaeche, float _luftDichte, float _geschwindigkeit)
    {
        return 0.5f * _cw * _flaeche * _luftDichte * Mathf.Pow(_geschwindigkeit,2) * 1000;
    }

    private float BerechneBallFlaeche(float _durchmesser)
    {
        return Mathf.Pow(_durchmesser,2) / 4* Mathf.PI;
    }

    private float BerechneReynoldszahl(float _luftDichte, float _geschwindigkeit,float _durchmesser, float _viskoseLuft)
    {
        return _luftDichte * _geschwindigkeit * _durchmesser / _viskoseLuft ;
    }

    private float BerechneCwWert(float _reynoldzahl)
    {
        return 24/_reynoldzahl + 4/Mathf.Sqrt(_reynoldzahl) + 0.4f  ;
    }
    

}
