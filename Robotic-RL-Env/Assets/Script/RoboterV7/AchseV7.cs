using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum RotationsRichtung { Neutral = 0, Positiv = 1, Negativ = -1 };

public class AchseV7 : MonoBehaviour
{
    public float achsengeschwindigkeit = 30.0f; // Grad/Sekunde
    private float sollRotation = 30.0f;

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
            RotiereAchse(rotationZiel, sollRotation);
        }

    }

    public float AktuelleRotationDerAchse()
    {
        
        // float currentRotationRads = articulation.jointPosition[0];
        // float currentRotation = Mathf.Rad2Deg * currentRotationRads;
        // return currentRotation;
        return articulation.xDrive.target;
    }

    // Rotiert um xx Grad um die x Achse
    private void RotiereAchse(float zielRotation, float sollRotation)
    {
        if (rotationState == RotationsRichtung.Negativ)
        {
            if (AktuelleRotationDerAchse() <= sollRotation)
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
            if (AktuelleRotationDerAchse() >= sollRotation)
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

    public void RotiereSofort(float sollWinkel)
    {
        var drive = articulation.xDrive;
        drive.target = sollWinkel;
        articulation.xDrive = drive;
    }

    // Rotiert um xx Grad um die x Achse
    public void RotiereAchseBis(float sollRotationsZiel, float sollRotaionsGeschwindigkeit)
    {
        achsengeschwindigkeit = sollRotaionsGeschwindigkeit;
        sollRotation = sollRotationsZiel;
        if (AktuelleRotationDerAchse() - sollRotationsZiel < 0)
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
