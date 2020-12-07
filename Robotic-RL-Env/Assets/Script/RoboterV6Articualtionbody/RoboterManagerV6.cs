﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoboterManagerV6 : MonoBehaviour
{
    [SerializeField] private Button abwurfbutton;
    [SerializeField] private Button startbutton;
    [SerializeField] private Text geschwindigkeitText;
    [SerializeField] private Text abwurfwinkelText;
    [SerializeField] private Text abwurfwinkelJ2Text;
    [SerializeField] public Text einwurfwinkelText;
    [SerializeField] private Rigidbody ball;
    [SerializeField] private Rigidbody r_Ball;
    [SerializeField] private Transform abwurfPosition;

    [SerializeField] private AchseV6 j0;
    [SerializeField] private AchseV6 j1;
    [SerializeField] private AchseV6 j2;
    [SerializeField] private AchseV6 j3;
    [SerializeField] private AchseV6 j4;


    [SerializeField] private float startRotationJ0 = 0.0f;
    [SerializeField] private float startRotationJ1 = 0.0f;
    [SerializeField] private float startRotationJ2 = 0.0f;
    [SerializeField] private float startRotationJ3 = 0.0f;
    [SerializeField] private float startRotationJ4 = 0.0f;

    [SerializeField] private float abwurfwinkel = 0.0f;
    [SerializeField] private float wurfgeschwindigkeit = 0.0f;
    [SerializeField] private float toleranzwinkel = 0.5f;

    private Vector3 abwurfgeschwindigkeit;
    private Vector3 letztePosition = Vector3.zero;
    private bool abwurfSignal;

    private float[] startRotation;

    private AchseV6[] achsen;
    

    // Start is called before the first frame update
    void Start()
    {

        abwurfbutton.onClick.AddListener(AbwurfButtonGeklickt);
        startbutton.onClick.AddListener(StartButtonGeglickt);

        startRotation = new float[5];
        achsen = new AchseV6[5];
        Init();
    }
    void Init()
    {
        abwurfSignal = false;
        //abwurfpositionErreicht = false;

        startRotation[0] = startRotationJ0;
        startRotation[1] = startRotationJ1;
        startRotation[2] = startRotationJ2;
        startRotation[3] = startRotationJ3;
        startRotation[4] = startRotationJ4;

        achsen[0] = j0;
        achsen[1] = j1;
        achsen[2] = j2;
        achsen[3] = j3;
        achsen[4] = j4;
        // Debug.Log(StartRotationJ1);
        
    }

    public void AbwurfButtonGeklickt()
    {
        AbwurfSignalSetzen();
    }

    public void StartButtonGeglickt()
    {
        InStartpositionFahren();
    }

    private void FixedUpdate()
    {
        
        if(abwurfSignal)
        {   
            BerechneGeschwindigkeit();
            AbwurfvorgangV1();
        }

    }


    private void BerechneGeschwindigkeit()
    {
        // aktuelle Geschwindigkeit des Abwurfpunktes am Greifer
        abwurfgeschwindigkeit = (abwurfPosition.transform.position - letztePosition) / Time.fixedDeltaTime;
        letztePosition = abwurfPosition.transform.position;
        //Debug.Log(abwurfgeschwindigkeit);

    }

    private void Abwurf()
    {
        abwurfPosition.rotation = Quaternion.LookRotation(abwurfgeschwindigkeit);
        Rigidbody obj = Instantiate(r_Ball, abwurfPosition.position, Quaternion.identity);
        obj.velocity = abwurfgeschwindigkeit;
    }

    private void AbwurfV1()
    {
        ball.MovePosition(abwurfPosition.position);
        ball.useGravity = true;
        ball.velocity =(abwurfgeschwindigkeit);
    }

    public void InStartpositionFahren()
    {
            for (int i = 0; i < 5; i++)
            {
                achsen[i].RotateTo(startRotation[i]);
            }
        geschwindigkeitText.text = "Abwurfgeschwindigkeit: -- ms";
        abwurfwinkelText.text = "Abwurfwinkel: --  Grad";
        abwurfwinkelJ2Text.text = "AbwurfwinkelJ2: --  Grad";
        einwurfwinkelText.text = "Einwurfwinkel: --  Grad";
        ball.velocity = Vector3.zero;
        ball.useGravity = false;

    }
    
    /*
    void Startvorgang()
    {

        // ist der Startbutton betätigt werden alle Achsen in die voher bestimmte Startposition gefahren
        if (start)
        {

            //Debug.Log(j1.CurrentPrimaryAxisRotation());

            for (int i = 0; i < 5; i++)
            {
                if (startRotation[i] < 0)
                {
                    if (!(achsen[i].CurrentPrimaryAxisRotation() <= startRotation[i] + toleranzwinkel && achsen[i].CurrentPrimaryAxisRotation() >= startRotation[i] - toleranzwinkel))
                    {
                        achsen[i].rotationState = RotationDirection.Negative;


                       // Debug.Log(achsen[i].CurrentPrimaryAxisRotation());
                    }
                    else
                    {
                        achsen[i].rotationState = RotationDirection.None;
                    }
                }
                else
                {
                    if (!(achsen[i].CurrentPrimaryAxisRotation() >= startRotation[i] - toleranzwinkel && achsen[i].CurrentPrimaryAxisRotation() <= startRotation[i] + toleranzwinkel))
                    {
                        achsen[i].rotationState = RotationDirection.Positive;
                    }
                    else
                    {
                        achsen[i].rotationState = RotationDirection.None;
                    }

                }
            }

            // Debug.Log(J1.CurrentPrimaryAxisRotation());

        }

    }
    void Abwurfvorgang ()
    {
        // ist der Abwurfbutton betätigt wird der Wurfvorgang gestartet bis die Ausgangsposition wieder ereicht  ist
        if (abgeworfen)
        {
            if (!(j2.CurrentPrimaryAxisRotation() >= -toleranzwinkel && j2.CurrentPrimaryAxisRotation() <= toleranzwinkel))
            {
                j2.speed = wurfgeschwindigkeit;
                j2.rotationState = RotationDirection.Positive;
            }
            else
            {
                j2.rotationState = RotationDirection.None;
                //abwurf = !abwurf;
                abgeworfen = false;
            }
        }
        // ist der vorher angegebene Abwurfwinkel mit einer Toleranz erreicht wird der Abwurf des Balls gestartet
        if ( j2.CurrentPrimaryAxisRotation() <= abwurfwinkel + toleranzwinkel && j2.CurrentPrimaryAxisRotation() >= abwurfwinkel - toleranzwinkel)
        {
            Abwurf();
           // ball.useGravity = true;
           // ball.AddForce(abwurfgeschwindigkeit);
            geschwindigkeitText.text = "Abwurfgeschwindigkeit: " + abwurfgeschwindigkeit.magnitude + " ms";

        }


    }
    */
    private void AbwurfvorgangV1()
    {
            
            j2.speed = wurfgeschwindigkeit;
            j2.rotationState = RotationDirection.Positive;
        /*
        if (j2.CurrentPrimaryAxisRotation() >= -toleranzwinkel && j2.CurrentPrimaryAxisRotation() <= toleranzwinkel)
        {
            j2.rotationState = RotationDirection.None;
            abwurfSignal = false;
        }
        */
        // ist der vorher angegebene Abwurfwinkel mit einer Toleranz erreicht wird der Abwurf des Balls gestartet
        if (j2.CurrentPrimaryAxisRotation() <= abwurfwinkel + toleranzwinkel && j2.CurrentPrimaryAxisRotation() >= abwurfwinkel - toleranzwinkel )
        {
            
            //Abwurf();
            AbwurfV1();
            geschwindigkeitText.text = "Abwurfgeschwindigkeit: " + abwurfgeschwindigkeit.magnitude + " ms";
            abwurfwinkelJ2Text.text = "AbwurfwinkelJ2: " + j2.CurrentPrimaryAxisRotation() + " Grad";
            abwurfwinkelText.text = "Abwurfwinkel: " + Mathf.Rad2Deg * Mathf.Acos(-abwurfgeschwindigkeit.x/abwurfgeschwindigkeit.magnitude) + " Grad";
            abwurfSignal = false;
            j2.rotationState = RotationDirection.None;
        }

    }
    // Startet den Ballabwurf mit der Wurfgschwindigkeit des Arms von 0-1 und dem Abwurfwinkel von 0-1
    public void StarteAbwurf(float geschwindigkeit, float winkel)
    {
        abwurfwinkel = UebersetzeWinkel(winkel);
        wurfgeschwindigkeit = UebersetzGeschwindigkeit(geschwindigkeit);
        abwurfSignal = true;
    }
    private void AbwurfSignalSetzen()
    {
        abwurfSignal = true;
    }

    // Rechnet die den Winkel von -50 bis -200 um
    private float UebersetzeWinkel(float winkel)
    {
        if (winkel < 0 || winkel > 1) return 0;
        return -1 * (winkel * 150 + 50);
    }

    // Rechnet die den Geschwindigkeit von 0 bis 500 um
    private float UebersetzGeschwindigkeit(float geschwindigkeit)
    {
        if (geschwindigkeit < 0 || geschwindigkeit > 1) return 0;
        return  geschwindigkeit * 500;
    }


}
