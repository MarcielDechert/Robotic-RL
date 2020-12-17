using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RoboterAgent : Agent
{
    [Header("Learning Parts")]
    public GameObject roboter;
    public GameObject area;
    private bool Abwurfvorgang = false;

    RoboterManagerV6 r_robot;
    RobotsLearningArea r_area;

    // Start is called before the first frame update
    public override void Initialize()
    {
        r_robot = roboter.GetComponent<RoboterManagerV6>();
        r_area = area.GetComponent<RobotsLearningArea>();
    }

    public override void OnEpisodeBegin()
    {
        r_area.Reset();
        Abwurfvorgang = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(r_area.DistanceToTarget());
        sensor.AddObservation(r_robot.achsen[0].CurrentPrimaryAxisRotation());
        sensor.AddObservation(r_robot.achsen[1].CurrentPrimaryAxisRotation());
        sensor.AddObservation(r_robot.achsen[2].CurrentPrimaryAxisRotation());
        sensor.AddObservation(r_robot.achsen[3].CurrentPrimaryAxisRotation());
        sensor.AddObservation(r_robot.achsen[4].CurrentPrimaryAxisRotation());
    }

    public override void OnActionReceived(float[] actions)
    {
        float geschwindigkeit = ScaleAction(actions[0], 0.2f, 1f);
        float winkel = ScaleAction(actions[1], 0.2f, 1f);

        r_robot.StarteAbwurf(geschwindigkeit, winkel);
        Abwurfvorgang = true;
    }

    public void FixedUpdate()
    {
        if (Abwurfvorgang == false)
        {
            RequestDecision();
        }

        float distanceToTarget = r_area.DistanceToTarget();

        if (distanceToTarget < 0.15f && r_area.r_ball.Kollidiert == true)
        {
            SetReward(100.0f);
            EndEpisode();
        }
        else if (r_area.r_ball.Kollidiert == true)
        {
            SetReward(1 / distanceToTarget);
            EndEpisode();
        }
        else if (r_area.r_ball.transform.position.y < -10f)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }
}
