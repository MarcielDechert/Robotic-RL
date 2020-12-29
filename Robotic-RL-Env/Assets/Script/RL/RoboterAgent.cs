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

    RoboterControllerV7 robot;
    RobotsLearningArea area;

    // Start is called before the first frame update
    public override void Initialize()
    {
        robot = g_roboter.GetComponent<RoboterControllerV7>();
        area = g_area.GetComponent<RobotsLearningArea>();
    }

    public override void OnEpisodeBegin()
    {
        area.Reset();
        Abwurfvorgang = false;

        float[] sollgeschwindigkeit = new float[] { 180f, 180f, 180f, 180f, 180f, 180f };
        float[] sollwinkel = new float[] { 180f, 0, 90f, 0, 0, 0 };
        robot.InStartposition(sollwinkel, sollgeschwindigkeit);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(area.DistanceToTarget());
        sensor.AddObservation(robot.IstRotation);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var continuousActions = actionBuffers.ContinuousActions;
        continuousActions[0] = (float)((continuousActions[0] / 2) + 0.5);
        continuousActions[1] = (float)((continuousActions[1] / 2) + 0.5);
        float kigeschwindigkeit = Mathf.Lerp(50f, 500f, continuousActions[0]);
        float kiwinkel = Mathf.Lerp(-120f, 45f, continuousActions[1]);
        Debug.Log("KI Übergabe: " + continuousActions[0] + " und Winkel: " + continuousActions[1]);

        float[] geschwindigkeit = new float[] { 0, 0, kigeschwindigkeit, 0, 0, 0 };
        float[] winkel = new float[] { 180f, 0f, kiwinkel, 0f, 0f, 0f }; // Startposition J3 = 90 Abwurf = -90
        robot.StarteAbwurf(winkel, geschwindigkeit);
        Debug.Log("Befehl in StartAbwurf mit Geschwindigkeit: " + kigeschwindigkeit + " und Winkel: " + kiwinkel);
    }

    public void FixedUpdate()
    {
        var distanceToTarget = area.DistanceToTarget();

        if(Abwurfvorgang == false && robot.RoboterStatus == RoboterStatus.Abwurfbereit)
        {
            RequestDecision();
            Abwurfvorgang = true;
        }

        if (distanceToTarget < 0.05f && area.r_ball.Kollidiert == true)
        {
            SetReward(10.0f);
            EndEpisode();
        }
        else if (area.r_ball.Kollidiert == true)
        {
            SetReward(1 / distanceToTarget);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }
}
