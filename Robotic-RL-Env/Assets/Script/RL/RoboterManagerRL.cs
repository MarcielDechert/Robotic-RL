using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoboterManagerRL : MonoBehaviour
{ 
    [SerializeField] private AchseV1 r_Ausgangsachse;
    [SerializeField] private AchseV1 r_Endpunkt;

    [SerializeField] private float r_Drehrate = 0.1f;
    [SerializeField] private float r_Schwelle = 0.05f;

    private List<AchseV1> Joints = new List<AchseV1>();
    private List<float> TargetAngles = new List<float>();
    private bool rotate = false;

    private void Start()
    {
        GetJoints();
        RotateJointsAngle(0, 25, 180, 0, 0, 0);
    }
   
    float BerechneDistanz(Vector3 _punkt1, Vector3 _punkt2)
    {
        return Vector3.Distance(_punkt1, _punkt2);
    }

    private void RotateJointsAngle(float J01_Angle, float J02_Angle, float J03_Angle, float J04_Angle, float J05_Angle, float J06_Angle)
    {
        if(TargetAngles.Count == 0)
        {
            TargetAngles.Add(J01_Angle);
            TargetAngles.Add(J02_Angle);
            TargetAngles.Add(J03_Angle);
            TargetAngles.Add(J04_Angle);
            TargetAngles.Add(J05_Angle);
            TargetAngles.Add(J06_Angle);
        }
        else
        {
            TargetAngles[0] = J01_Angle;
            TargetAngles[1] = J02_Angle;
            TargetAngles[2] = J03_Angle;
            TargetAngles[3] = J04_Angle;
            TargetAngles[4] = J05_Angle;
            TargetAngles[5] = J06_Angle;
        }
        rotate = true;
    }

    private void GetJoints()
    {
        Joints.Add(r_Ausgangsachse);
        Joints.Add(Joints[Joints.Count - 1].GetChild());
        Joints.Add(Joints[Joints.Count - 1].GetChild());
        Joints.Add(Joints[Joints.Count - 1].GetChild());
        Joints.Add(Joints[Joints.Count - 1].GetChild());
        Joints.Add(Joints[Joints.Count - 1].GetChild());
    }

    public void Update()
    {
        if(rotate)
        {
            foreach (var Joint in Joints)
            {
                float target = TargetAngles[Joints.IndexOf(Joint)];
                if (target != Joint.getRotation()) 
                {
                    RotateJoint(Joint, target);
                }
            }
        }
    }

    private void RotateJoint(AchseV1 joint, float target)
    {
        joint.Rotate(0, target, 0);
        joint.setRotation(target);
    }
}
