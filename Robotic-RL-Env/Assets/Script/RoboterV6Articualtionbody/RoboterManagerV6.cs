using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoboterManagerV6 : MonoBehaviour
{
    [SerializeField] private Button abwurfbutton;
    [SerializeField] private Button startbutton;
    [SerializeField] private Text geschwindigkeitText;
    [SerializeField] private Rigidbody ball;

    [SerializeField] private AchseV6 j0;
    [SerializeField] private AchseV6 j1;
    [SerializeField] private AchseV6 j2;
    [SerializeField] private AchseV6 j3;
    [SerializeField] private AchseV6 j4;

    [SerializeField] private Rigidbody r_Ball;
    [SerializeField] private Transform abwurfPosition;


    [SerializeField] private float startRotationJ0 = 0.0f;
    [SerializeField] private float startRotationJ1 = 0.0f;
    [SerializeField] private float startRotationJ2 = 0.0f;
    [SerializeField] private float startRotationJ3 = 0.0f;
    [SerializeField] private float startRotationJ4 = 0.0f;

    [SerializeField] private float abwurfwinkel = 0.0f;
    [SerializeField] private float wurfgeschwindigkeit = 0.0f;
    [SerializeField] private float toleranzwinkel = 0.5f;

    // [SerializeField] private float speed = 0.0f;

    private Vector3 abwurfgeschwindigkeit;
    private Vector3 letztePosition = Vector3.zero;

    private bool abwurfpositionErreicht;

    private float[] startRotation;

    private AchseV6[] achsen;

    private bool start;
    private bool abwurf;

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
        start = false;
        abwurf = false;
        abwurfpositionErreicht = false;

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
        abwurf = !abwurf;
    }

    public void StartButtonGeglickt()
    {
        start = !start;
    }

    private void FixedUpdate()
    {
        BerechneGeschwindigkeit();
        Startvorgang();
        Abwurfvorgang();

    }


    void BerechneGeschwindigkeit()
    {
        // aktuelle Geschwindigkeit des Abwurfpunktes am Greifer
        abwurfgeschwindigkeit = (abwurfPosition.transform.position - letztePosition) / Time.deltaTime;
        letztePosition = abwurfPosition.transform.position;
        Debug.Log(abwurfgeschwindigkeit);

    }

    void Abwurf()
    {
        abwurfPosition.rotation = Quaternion.LookRotation(abwurfgeschwindigkeit);
        Rigidbody obj = Instantiate(r_Ball, abwurfPosition.position, Quaternion.identity);
        obj.velocity = abwurfgeschwindigkeit;
    }
    

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
        if (abwurf)
        {
            if (!(j2.CurrentPrimaryAxisRotation() >= -toleranzwinkel && j2.CurrentPrimaryAxisRotation() <= toleranzwinkel))
            {
                j2.speed = wurfgeschwindigkeit;
                j2.rotationState = RotationDirection.Positive;
            }
            else
            {
                j2.rotationState = RotationDirection.None;
                abwurf = !abwurf;
                abwurfpositionErreicht = true;
            }
        }
        // ist der vorher angegebene Abwurfwinkel mit einer Toleranz erreicht wird der Abwurf des Balls gestartet
        if (abwurf && j2.CurrentPrimaryAxisRotation() <= abwurfwinkel + toleranzwinkel && j2.CurrentPrimaryAxisRotation() >= abwurfwinkel - toleranzwinkel && abwurfpositionErreicht)
        {
            Abwurf();
           // ball.useGravity = true;
           // ball.AddForce(abwurfgeschwindigkeit);
            geschwindigkeitText.text = "Abwurfgeschwindigkeit: " + abwurfgeschwindigkeit.magnitude + " ms";
            abwurfpositionErreicht = false;

        }


    }
}
