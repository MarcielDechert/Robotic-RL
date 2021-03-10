﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotsLearningArea : MonoBehaviour
{
    [Header("Learning Parts")]
    [SerializeField] public RoboterController r_robot;
    public RoboterController R_robot { get => r_robot;}

    [SerializeField] private BallController r_ball;
    public BallController R_ball { get => r_ball;}

    [SerializeField] private RoboterAgent agent;
    public RoboterAgent Agent { get => agent;}

    [SerializeField] public Rigidbody target;
    [SerializeField] public Rigidbody ball;

    private float wurfweite;
    public float Wurfweite { get => wurfweite; set => wurfweite = value; }

    private float abwurfhoehe;
    public float Abwurfhoehe { get => abwurfhoehe; set => abwurfhoehe = value; }

    private float stopZeit = 0.0f;

    public float StopZeit { get => stopZeit; set => stopZeit = value; }



    // Start is called before the first frame update
   private void Start()
    {
        r_ball = ball.GetComponent<TTBallController>();
    }
    public void AreaReset()
    {
        ball.transform.position = r_robot.AbwurfPosition.position;

        ball.velocity = Vector3.zero;
        ball.angularVelocity = Vector3.zero;
        ball.useGravity = false;

        r_robot.AbwurfgeschwindigkeitVector3 = Vector3.zero;
        r_robot.Abwurfgeschwindigkeit= 0.0f;
        r_robot.AbwurfwinkelBall = 0.0f;

        r_ball.Kollidiert = false;
        r_ball.KollisionsListe.Clear();

        wurfweite = 0.0f;
        abwurfhoehe = 0.0f;
        r_ball.EinwurfWinkel = 0.0f;
        stopZeit = 0.0f;

    }
    public void ResetZeit()
    {
    }
    public float DistanceToTarget()
    {
        return Vector3.Distance(r_robot.transform.position, target.position);
    }

    public float DistanceBallToTarget() 
    {
        return Vector3.Distance(r_ball.transform.position, target.position);
    }

    public void BerechneWurfweite()
    {
        wurfweite = Mathf.Abs(r_ball.transform.position.x - r_robot.AbwurfPosition.transform.position.x);
    }

    public void BerechneAbwurfhoehe()
    {
        abwurfhoehe = Mathf.Abs(r_robot.AbwurfPosition.position.y)+ r_robot.transform.position.y;
    }

    private void FixedUpdate()
    {
        r_robot.Step();
        if(r_robot.RoboterStatus == RoboterStatus.Wirft)
        {
            r_ball.Step();
            //Debug.Log(r_ball.Ballgeschwindigkeit.magnitude);
           // Debug.Log(stopZeit);
            //stopZeit += Time.fixedDeltaTime;
            //Debug.Log(stopZeit);
        }

        if (agent.enabled)
        {
            agent.Step();
        }
    }
}
