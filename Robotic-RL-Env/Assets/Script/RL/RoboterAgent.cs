using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;

public class RoboterAgent : Agent
{
    public GameObject g_roboter;
    public GameObject g_area;
    private bool Abwurfvorgang = false;

    RoboterControllerV7 robot;
    RobotsLearningArea area;

    private bool wand;

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

        float[] sollgeschwindigkeit = new float[] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] sollwinkel = new float[] { 180f, 0, 80f, 0, 60f, 0 };
        robot.InStartposition(sollwinkel, sollgeschwindigkeit);
        wand = false;
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
        float kigeschwindigkeit = Mathf.Lerp(250f, 500f, continuousActions[0]);
        float kiwinkel = Mathf.Lerp(-120f, 80f, continuousActions[1]);
        Debug.Log("KI Übergabe: " + continuousActions[0] + " und Winkel: " + continuousActions[1]);

        float[] geschwindigkeit = new float[] {0.05f, 0.05f, kigeschwindigkeit, 0.05f, 0.05f, 0.05f };
        float[] winkel = new float[] { 180f, 0f, kiwinkel, 0f, 60f, 0f }; // Startposition J3 = 90 Abwurf = -90
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

        if (area.R_ball.Kollidiert == true)
        {
            if (area.R_ball.KollisionsStatus == KollisionsLayer.Becherwand && wand == false)
            {
                Debug.Log("Reward= " + 0.25f);
                AddReward(0.25f);
                wand = true;
            }
            else if(area.R_ball.KollisionsStatus == KollisionsLayer.Einwurfzone)
            {
                Debug.Log("Reward= " + (1 / (4 + (1 / area.R_ball.Einwurfwinkel))) + " mit einem Einwurfwinkel von " + area.R_ball.Einwurfwinkel);
                AddReward(1 / (4 + (1/area.R_ball.Einwurfwinkel)));
            }
            else if(area.R_ball.KollisionsStatus == KollisionsLayer.Becherboden)
            {
                Debug.Log("Reward= " + 0.5f );
                AddReward(0.5f);
                EndEpisode();
            }
            else if (area.R_ball.KollisionsStatus == KollisionsLayer.Boden ||
                area.R_ball.KollisionsStatus == KollisionsLayer.Roboter)
            {
                Debug.Log("Reward= " + (1 / (4 + distanceToTarget)));
                AddReward(1 / (4 + distanceToTarget));
                EndEpisode();
            }
            area.R_ball.KollisionsStatus = KollisionsLayer.Neutral;
            area.R_ball.Kollidiert = false;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }
}
