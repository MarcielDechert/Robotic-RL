using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchseV7 : MonoBehaviour
{
    public float achsengeschwindigkeit = 30.0f; // Grad/Frame
    //public float rotationsZiel = 30.0f; // Grad/Frame

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

    // Rotiert um xx Grad um die x Achse
    public void RotiereAchse(float zielRotation)
    {
        var drive = articulation.xDrive;
        drive.target = zielRotation;
        articulation.jointVelocity = new ArticulationReducedSpace(3.6f);
        articulation.xDrive = drive;
    }

    public Vector3 GetSpeed()
    {
        return articulation.velocity;
    }
}
