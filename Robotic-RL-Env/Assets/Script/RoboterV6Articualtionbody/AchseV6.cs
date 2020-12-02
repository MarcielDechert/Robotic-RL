using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum RotationDirection { None = 0, Positive = 1, Negative = -1 };
public class AchseV6 : MonoBehaviour
{
    public float speed = 30.0f;

    public RotationDirection rotationState = RotationDirection.None;
    private ArticulationBody articulation;

    void Start()
    {
        articulation = GetComponent<ArticulationBody>();
    }

    public void FixedUpdate()
    {
        if (rotationState != RotationDirection.None)
        {
            float rotationChange = (float)rotationState * speed * Time.fixedDeltaTime;
            float rotationGoal = CurrentPrimaryAxisRotation() + rotationChange;
            RotateTo(rotationGoal);
        }

    }

    public float CurrentPrimaryAxisRotation()
    {
        float currentRotationRads = articulation.jointPosition[0]; // Aktuelle Drehung der x Achse 
        float currentRotation = Mathf.Rad2Deg * currentRotationRads; // Rotation in Grad umrechnen
        return currentRotation;
    }

    // Rotiert um xx Grad um die x Achse
    public void RotateTo(float primaryAxisRotation)
    {
        var drive = articulation.xDrive;
        drive.target = primaryAxisRotation;
        articulation.xDrive = drive;
    }
}
