using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RoboterAgent : Agent
{
    public GameObject g_roboter;
    public GameObject g_area;
    private bool Abwurfvorgang = false;

    RoboterManagerV6 robot;
    RobotsLearningArea area;

    // Start is called before the first frame update
    public override void Initialize()
    {
        robot = g_roboter.GetComponent<RoboterManagerV6>();
        area = g_area.GetComponent<RobotsLearningArea>();
    }

    public override void OnEpisodeBegin()
    {
        area.Reset();
        Abwurfvorgang = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(area.DistanceToTarget());
        sensor.AddObservation(robot.achsen[0].AktuelleRotationDerAchse());
        sensor.AddObservation(robot.achsen[1].AktuelleRotationDerAchse());
        sensor.AddObservation(robot.achsen[2].AktuelleRotationDerAchse());
        sensor.AddObservation(robot.achsen[3].AktuelleRotationDerAchse());
        sensor.AddObservation(robot.achsen[4].AktuelleRotationDerAchse());
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        float geschwindigkeit = Mathf.Clamp(vectorAction[0], 0.5f, 1f);
        float winkel = Mathf.Clamp(vectorAction[1], 0.5f, 1f);
        robot.StarteAbwurfMitKI(geschwindigkeit, winkel);
    }

    public void FixedUpdate()
    {
        var distanceToTarget = area.DistanceToTarget();

        if(Abwurfvorgang == false && robot.abwurfStatus == AbwurfStatus.Neutral)
        {
            area.Reset();
        }
        else if(Abwurfvorgang == false && robot.abwurfStatus == AbwurfStatus.Abwurfbereit)
        {
            RequestDecision();
            Abwurfvorgang = true;
        }

        if (distanceToTarget < 0.15f && area.r_ball.Kollidiert == true)
        {
            SetReward(100.0f);
            EndEpisode();
        }
        else if (area.r_ball.Kollidiert == true)
        {
            SetReward(1 / distanceToTarget);
            EndEpisode();
        }
        else if (area.r_ball.transform.position.y < -10f)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }
}
