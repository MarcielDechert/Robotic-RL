using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RoboterAgent : Agent
{
    public GameObject roboter;
    public GameObject target;
    public GameObject ball;

    RoboterManagerV6 r_robot;
    Rigidbody r_target;
    Rigidbody r_ball;

    // Start is called before the first frame update
    void Start()
    {
        r_robot = roboter.GetComponent<RoboterManagerV6>();
        r_target = target.GetComponent<Rigidbody>();
        r_ball = ball.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        r_robot.InStartpositionFahren();
        r_target.transform.localPosition = new Vector3((float)(-1 * (Random.value * 2 + 0.2)), 0, 0);
        r_robot.Abwurfvorgangbool = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(r_target.transform.localPosition);
        sensor.AddObservation(r_robot.transform.localPosition);
        sensor.AddObservation(r_robot.achsen[2].CurrentPrimaryAxisRotation());
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float distanceToTarget = Vector3.Distance(r_ball.position, r_target.position);

        if (r_robot.Abwurfvorgangbool == false)
        {
            var continuousActions = actionBuffers.ContinuousActions;
            r_robot.abwurfwinkel = continuousActions[0];
            r_robot.wurfgeschwindigkeit = continuousActions[1];
            r_robot.SetzeAbwurfSignal();
        }
        else if (distanceToTarget < 0.1f)
        {
            SetReward(1.0f);
            EndEpisode();
        }
        else if (r_ball.position.y < -0.1f)
        {
            EndEpisode();
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }
}
