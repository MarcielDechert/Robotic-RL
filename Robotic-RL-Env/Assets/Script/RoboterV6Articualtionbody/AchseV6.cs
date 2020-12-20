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
        float aktuelleRotationRads = articulation.jointPosition[0]; // Aktuelle Drehung der x Achse 
        float aktuelleRoatationGrad = Mathf.Rad2Deg * aktuelleRotationRads; // Rotation in Grad umrechnen
        return aktuelleRoatationGrad;
    }

    // Rotiert um xx Grad um die x Achse
    public void RotiereAchse(float zielRotation)
    {
        var drive = articulation.xDrive;
        drive.target = zielRotation;
        articulation.xDrive = drive;
    }
}
