using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;

public class RoboterAgent : Agent
{
    public RobotsLearningArea area;
    private bool Abwurfvorgang = false;
    private float elapsedTime;

    public override void OnEpisodeBegin()
    {
        area.Reset();
        area.BallReset();
        Abwurfvorgang = false;

        float[] sollgeschwindigkeit = new float[] { 100f, 100f, 100f, 25f, 25f, 25f };
        float[] sollwinkel = new float[] { 180f, 0, 80f, 0, 60f, 0 };
        area.R_robot.InStartposition(sollwinkel, sollgeschwindigkeit);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(area.DistanceToTarget());
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var continuousActions = actionBuffers.ContinuousActions;
        continuousActions[0] = (float)((continuousActions[0] / 2) + 0.5);
        continuousActions[1] = (float)((continuousActions[1] / 2) + 0.5);
        float kigeschwindigkeit = Mathf.Lerp(10f, 500f, continuousActions[0]);
        float kiwinkel = Mathf.Lerp(70, -110f, continuousActions[1]);
        Debug.Log("KI Übergabe: " + continuousActions[0] + " und Winkel: " + continuousActions[1]);

        float[] geschwindigkeit = new float[] {0.01f, 0.01f, kigeschwindigkeit, 0.01f, 0.01f, 0.01f };
        float[] winkel = new float[] { 180f, 0f, kiwinkel, 0f, 60f, 0f };
        area.R_robot.StarteAbwurf(winkel, geschwindigkeit);
        Debug.Log("Befehl in StartAbwurf mit Geschwindigkeit: " + kigeschwindigkeit + " und Winkel: " + kiwinkel);
    }

    public void Step()
    {
        if (Abwurfvorgang == false && area.R_robot.RoboterStatus == RoboterStatus.Abwurfbereit)
        {
            RequestDecision();
            Abwurfvorgang = true;
        }

        if (area.R_ball.Kollidiert == true)
        {
            if(area.R_ball.KollisionsListe.Contains(KollisionsLayer.Becherboden))
            {

                if (area.R_ball.KollisionsListe.Contains(KollisionsLayer.Einwurfzone))
                {
                    if (area.R_ball.EinwurfWinkel >= 60)
                    {
                        Debug.Log("Reward= " + 1f + " mit einem Einwurfwinkel von über 45 Grad = " + area.R_ball.EinwurfWinkel);
                        SetReward(1f);
                        EndEpisode();
                    }
                    else
                    {
                        Debug.Log("Reward= " + 0.5f + area.R_ball.EinwurfWinkel / 120 + " mit einem Einwurfwinkel von " + area.R_ball.EinwurfWinkel);
                        SetReward(0.5f + area.R_ball.EinwurfWinkel / 120);
                        EndEpisode();
                    }
                }
            }
            else if (area.R_ball.KollisionsListe.Contains(KollisionsLayer.Boden) || area.R_ball.KollisionsListe.Contains(KollisionsLayer.Roboter) ||
                area.R_ball.KollisionsListe.Contains(KollisionsLayer.Decke) || area.R_ball.KollisionsListe.Contains(KollisionsLayer.Wand))
            {
                 var distanceBallToTarget = area.DistanceBallToTarget();
                 Debug.Log("Reward= " + 1 / (4 + distanceBallToTarget));
                 SetReward(1 / (2 + distanceBallToTarget));
                 EndEpisode();
            }
            area.R_ball.Kollidiert = false;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }
}
