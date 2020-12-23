using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchseV7 : MonoBehaviour
{
    public float achsengeschwindigkeit = 30.0f; // Grad/Frame
    private float sollRotations = 30.0f; // Grad/Frame

    public RotationsRichtung rotationState = RotationsRichtung.Neutral;
    private ArticulationBody articulation;


    void Start()
    {
        articulation = GetComponent<ArticulationBody>();
    }

    public void FixedUpdate()
    {
        if (rotationState != RotationsRichtung.Neutral)
        {
            float rotationAenderung = (float)rotationState * achsengeschwindigkeit * Time.fixedDeltaTime;
            float rotationZiel = AktuelleRotationDerAchse() + rotationAenderung;
            RotiereAchse(rotationZiel,sollRotations);
        }

    }

    public float AktuelleRotationDerAchse()
    {
        return articulation.xDrive.target;
    }

    // Rotiert um xx Grad um die x Achse
    private void RotiereAchse(float zielRotation, float sollRotation)
    {
        if(rotationState == RotationsRichtung.Negativ)
        {
            if(AktuelleRotationDerAchse() <= sollRotation)
            {
                rotationState = RotationsRichtung.Neutral;
            }
            else
            {
                var drive = articulation.xDrive;
                drive.target = zielRotation;
                articulation.xDrive = drive;
            }
        }
        else
        {
            if(AktuelleRotationDerAchse() >= sollRotation)
            {
                rotationState = RotationsRichtung.Neutral;
            }
            else
            {
                var drive = articulation.xDrive;
                drive.target = zielRotation;
                articulation.xDrive = drive;
            }

        }
    }

    // Rotiert um xx Grad um die x Achse
    public void RotiereAchseBis(float sollRotationsZiel, float sollRotaionsGeschwindigkeit)
    {
        achsengeschwindigkeit = sollRotaionsGeschwindigkeit;
        sollRotations = sollRotationsZiel;
        if( AktuelleRotationDerAchse() - sollRotationsZiel < 0)
        {
            rotationState = RotationsRichtung.Positiv;
        }
        else
        {
            rotationState = RotationsRichtung.Negativ;
        }
       
    }

    public Vector3 GetSpeed()
    {
        return articulation.velocity;
    }
}
