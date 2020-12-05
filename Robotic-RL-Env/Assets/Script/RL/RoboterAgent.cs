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
    private bool Abwurfvorgang = false;

    RoboterManagerV6 r_robot;
    Rigidbody r_target;
    BallManager r_ball;

    // Start is called before the first frame update
    public override void Initialize()
    {
        r_robot = roboter.GetComponent<RoboterManagerV6>();
        r_target = target.GetComponent<Rigidbody>();
        r_ball = ball.GetComponent<BallManager>();
    }

    public override void OnEpisodeBegin()
    {
        r_robot.InStartpositionFahren();
        r_ball.Kollidiert = false;
        //r_target.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        r_target.transform.localPosition = new Vector3((float)(-1 * (Random.value + 0.3)), 0.08f, 0);
        r_target.velocity = Vector3.zero;
        Abwurfvorgang = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(r_target.transform.localPosition);
        sensor.AddObservation(r_robot.transform.localPosition);
        sensor.AddObservation(r_robot.achsen[2].CurrentPrimaryAxisRotation());
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        float distanceToTarget = Vector3.Distance(r_ball.transform.position, r_target.position);

        if (Abwurfvorgang == false)
        {
            float geschwindigkeit = Mathf.Clamp(vectorAction[0], 0.3f, 1f);
            float winkel = Mathf.Clamp(vectorAction[1], 0f, 1f);

            r_robot.StarteAbwurf(geschwindigkeit, winkel);
            Abwurfvorgang = true;
        }
        else if (distanceToTarget < 0.5f && r_ball.Kollidiert == true)
        {
            SetReward(20.0f);
            EndEpisode();
        }
        else if (r_ball.Kollidiert == true)
        {
            SetReward(1 / distanceToTarget);
            EndEpisode();
        }
        //else if (distanceToTarget > 0.1f && r_ball.Kollidiert == true)
        //{
        //    SetReward(-1f);
        //    EndEpisode();
        //}
        else if (r_ball.transform.position.y < -10f)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Geschwindigkeit");
        actionsOut[1] = Input.GetAxis("Winkel");
    }
}
