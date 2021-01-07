﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotsLearningArea : MonoBehaviour
{
    [Header("Learning Parts")]
    public GameObject roboter;
    public GameObject target;
    public Rigidbody ball;

    private RoboterControllerV7 r_robot;
    private BallControllerV7 r_ball;

    public BallControllerV7 R_ball { get => r_ball; set => r_ball = value; }


    // Start is called before the first frame update
    void Start()
    {
        r_robot = roboter.GetComponent<RoboterControllerV7>();
        r_ball = ball.GetComponent<BallControllerV7>();
    }

    public void Reset()
    {
        target.transform.localPosition = new Vector3((float)((-0.25 * Random.value) - 0.5), 0, 0);
        ball.transform.localPosition = new Vector3(0, 2f, 0);
        ball.velocity = Vector3.zero;
        ball.angularVelocity = Vector3.zero;
        ball.useGravity = false;
        r_ball.Kollidiert = false;
        r_ball.KollisionsListe.Clear();
    }

    public List<float> DistanceToTarget()
    {
        List<float> temp = new List<float>();
        temp.Add(Vector3.Distance(r_robot.transform.position, target.transform.position));
        for (int i = 0; i < target.transform.childCount; i++)
        {
            temp.Add(Vector3.Distance(r_robot.transform.position, target.transform.GetChild(i).transform.position));
        }
        return temp;
    }

    public float DistanceBallToTarget()
    {
        return Vector3.Distance(r_ball.transform.position, target.transform.position);
    }
}
