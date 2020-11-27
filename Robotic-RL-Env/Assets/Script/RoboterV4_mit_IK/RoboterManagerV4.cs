using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoboterManagerV4 : MonoBehaviour
{

    [SerializeField] private AchseV1 r_Ausgangsachse;
    [SerializeField] private AchseV1 r_Endpunkt;
    [SerializeField] private Button Abwurfbutton;
    [SerializeField] private Button Startbutton;

    [SerializeField] private float r_Drehrate = 5.0f;
    [SerializeField] private float r_Schwelle = 0.05f;
    [SerializeField] private int r_Schritte = 20;

    private ArticulationBody Joint;

    private bool aktion = false;
    private Vector3 aktullesZiel;

    private AchseV1 J01;
    private AchseV1 J02;
    private AchseV1 J03;
    private AchseV1 J04;
    private AchseV1 J05;
    private AchseV1 J06;

    private void Start()
    {
        Abwurfbutton.onClick.AddListener(AbwurfButtonGeklickt);
        Startbutton.onClick.AddListener(StartButtonGeglickt);


        J01 = r_Ausgangsachse;
        J02 = J01.GetChild();
        J03 = J02.GetChild();
        J04 = J03.GetChild();
        J05 = J04.GetChild();
        J06 = J05.GetChild();

        Debug.Log(J02.getRotation().ToString());
        J02.Rotate(0, 25, 0);
        Debug.Log(J02.getRotation().ToString());
        J03.Rotate(0, -180, 0);
    }

    public void AbwurfButtonGeklickt()
    {
    }
    
    public void StartButtonGeglickt()
    {
        aktion = !aktion;
        //r_Ball.useGravity = true;
    }

    public void FahrZuAusgangsposition()
    {
    }

    float BerechneSteigung(AchseV1 _achse)
    {
        float deltaTheta = 0.01f;
        float distanz1 = BerechneDistanz(r_Endpunkt.transform.position, aktullesZiel);

        _achse.Rotate(0,deltaTheta,0);

        float distanz2 = BerechneDistanz(r_Endpunkt.transform.position, aktullesZiel);

        _achse.Rotate(0,-deltaTheta,0);

        return (distanz2 - distanz1) / deltaTheta;
    }


   
    void FixedUpdate()
    {
        //Debug.Log(J02.getRotation().ToString());
        if (aktion)
        {
            //if (BerechneDistanz(r_Endpunkt.transform.position, r_Zielpunkt.transform.position) < r_Schwelle)
            //{
            //    StartCoroutine(Verzoegerung(1));
            //}

            /*
            for ( int i = 0; i < r_Schritte; ++i)
            {
                // solange bis das Ziel mit einer gewissen Abweichung erreicht wurde
                if(BerechneDistanz(r_Endpunkt.transform.position, aktullesZiel) > r_Schwelle)
                {
                    AchseV1 aktuelleAchse = r_Ausgangsachse;

                    //solange bis es keine Achse mehr zum rotieren gibt
                    while(aktuelleAchse != null)
                    {
                        float steigung = BerechneSteigung(aktuelleAchse);
                        // dreht die aktuelle Achse mit der berechneten Steigung mit einer Drehrate
                        aktuelleAchse.Rotate(0, -steigung * r_Drehrate, 0);

                        // holt sich die nächste Achse/Verbindungspunkt
                        aktuelleAchse = aktuelleAchse.GetChild();
                    }
                }
            }
            */

        }
    }

    IEnumerator Verzoegerung(float time)
    {
        yield return new WaitForSeconds(time);
        FahrZuAusgangsposition();
        yield return new WaitForSeconds(time);
        AbwurfButtonGeklickt();

    }
    float BerechneDistanz(Vector3 _punkt1, Vector3 _punkt2)
    {
        return Vector3.Distance(_punkt1, _punkt2);
    }
}
