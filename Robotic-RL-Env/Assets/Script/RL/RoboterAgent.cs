using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class RoboterAgent : Agent
{
    public GameObject roboter;
    public GameObject target;
    public GameObject ball;

    RoboterManagerRL r_robot;
    Rigidbody r_target;
    Rigidbody r_ball;

    // Start is called before the first frame update
    void Start()
    {
        r_robot = roboter.GetComponent<RoboterManagerRL>();
        r_target = target.GetComponent<Rigidbody>();
        r_ball = ball.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        r_robot.StartPosition();
        r_target.transform.localPosition = new Vector3((float)(-1 * (Random.value * 2 + 0.2)), 0, 0);
        //r_robot.start = true;
        r_robot.abwurf = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(r_target.transform.localPosition);
        sensor.AddObservation(r_robot.transform.localPosition);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        // Actions, size = 2
        r_robot.abwurfwinkel = vectorAction[0];
        r_robot.wurfgeschwindigkeit = vectorAction[1];
        r_robot.abwurf = true;

        // Rewards
        float distanceToTarget = Vector3.Distance(r_ball.position, r_target.position);

        // Getroffen
        if (distanceToTarget < 0.1f)
        {
            SetReward(1.0f);
            EndEpisode();
        }
        // Danebengeworfen
        else if(r_ball.position.y < -0.1f)
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
