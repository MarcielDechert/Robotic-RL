﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKManagerRobot : MonoBehaviour
{

    [SerializeField] private AchseV1 r_Ausgangsachse;
    [SerializeField] private AchseV1 r_Endpunkt;
    [SerializeField] private GameObject r_Zielpunkt;
    [SerializeField] private Rigidbody r_Ball;

    [SerializeField] private float r_Drehrate = 5.0f;
    [SerializeField] private float r_Schwelle = 0.05f;
    [SerializeField] private int r_Schritte = 20;

    private void Start()
    {
        //r_Ball = GetComponent<Rigidbody>();
    }

    float BerechneSteigung(AchseV1 _achse)
    {
        float deltaTheta = 0.01f;
        float distanz1 = BerechneDistanz(r_Endpunkt.transform.position, r_Zielpunkt.transform.position);

        _achse.Rotate(0,deltaTheta,0);

        float distanz2 = BerechneDistanz(r_Endpunkt.transform.position, r_Zielpunkt.transform.position);

        _achse.Rotate(0,-deltaTheta,0);

        return (distanz2 - distanz1) / deltaTheta;

    }


    // Update is called once per frame
    void Update()
    {
        for( int i = 0; i < r_Schritte; ++i)
        {
            // solange bis das Ziel mit einer gewissen Abweichung erreicht wurde
            if(BerechneDistanz(r_Endpunkt.transform.position, r_Zielpunkt.transform.position) > r_Schwelle)
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
            else
            {
                r_Ball.useGravity = true;
            }

        }

        
    }

    float BerechneDistanz(Vector3 _punkt1, Vector3 _punkt2)
    {
        return Vector3.Distance(_punkt1, _punkt2);
    }
}
