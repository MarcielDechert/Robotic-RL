using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum RotationsRichtung { Neutral = 0, Positiv = 1, Negativ = -1 };
public class AchseV6 : MonoBehaviour
{
    public float achsengeschwindigkeit = 30.0f; // Grad/Frame

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
            RotiereAchse(rotationZiel);
        }

    }

    public float AktuelleRotationDerAchse()
    {
        return articulation.xDrive.target;
    }

    public float GetVelocity()
    {
        return articulation.xDrive.targetVelocity;
    }

    // Rotiert um xx Grad um die x Achse
    public void RotiereAchse(float zielRotation)
    {
        var drive = articulation.xDrive;
        drive.target = zielRotation;
        drive.targetVelocity = 3.4f;
        articulation.xDrive = drive;
    }
}
