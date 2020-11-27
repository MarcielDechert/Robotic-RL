using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class RoboterAgent : Agent
{
    RoboterManagerRL Roboter;
    Rigidbody Target;
    Rigidbody ball;

    // Start is called before the first frame update
    void Start()
    {
        Roboter = GetComponent<RoboterManagerRL>();
        Target = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        Target.transform.localPosition = new Vector3((float)((Random.value - 0.5) * 2), 0.2f, 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Target.transform.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        //sensor.AddObservation(Roboter.Abwurfwinkel);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        // Actions, size = 2
        //Roboter.AbwurfWinkel = vectorAction[0];
        //Roboter.Abwurfgeschwinidgkeit = vectorAction[1];

        // Rewards
        float distanceToTarget = Vector3.Distance(ball.position, Target.position);

        // Getroffen
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }
        // Danebengeworfen
        else
        {
            EndEpisode();
        }
    }
}
