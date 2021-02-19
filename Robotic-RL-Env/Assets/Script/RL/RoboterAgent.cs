using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;

public enum Belohnungssystem { Linear = 0, LinearMitToleranz = 1, LinearMitNegativ = 2, LinearNegativToleranz = 3};

public class RoboterAgent : Agent
{
    public RobotsLearningArea area;
    private bool Abwurfvorgang = false;
    private float elapsedTime;
    public Belohnungssystem Belohnung = Belohnungssystem.LinearMitToleranz;

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
        float kiwinkel = Mathf.Lerp(0f, 70f, continuousActions[1]);
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
            if (Belohnung == Belohnungssystem.Linear)
            {
                if (area.R_ball.KollisionsListe.Contains(KollisionsLayer.Becherboden))
                {

                    if (area.R_ball.KollisionsListe.Contains(KollisionsLayer.Einwurfzone))
                    {
                        if (area.R_ball.EinwurfWinkel >= 60)
                        {
                            Debug.Log("Reward= " + 1f + " mit einem Einwurfwinkel von über 60 Grad = " + area.R_ball.EinwurfWinkel);
                            SetReward(1f);
                            EndEpisode();
                        }
                        else
                        {
                            Debug.Log("Reward= " + (0.5f + area.R_ball.EinwurfWinkel / 120) + " mit einem Einwurfwinkel von " + area.R_ball.EinwurfWinkel);
                            SetReward(0.5f + area.R_ball.EinwurfWinkel / 120);
                            EndEpisode();
                        }
                    }
                }
                else if ((area.R_ball.KollisionsListe.Contains(KollisionsLayer.Boden) || area.R_ball.KollisionsListe.Contains(KollisionsLayer.Roboter) ||
                    area.R_ball.KollisionsListe.Contains(KollisionsLayer.Decke)) && !area.R_ball.KollisionsListe.Contains(KollisionsLayer.Einwurfzone))
                {
                    float distanceBallToTarget = area.DistanceBallToTarget();
                    float reward = (float)(0.5 - 0.5 * distanceBallToTarget);
                    if (reward <= 0f)
                    {
                        Debug.Log("Reward= " + 0);
                        SetReward(0);
                    }
                    else
                    {
                        Debug.Log("Reward= " + reward);
                        SetReward(reward);
                    }
                    EndEpisode();
                }
            }
            else if (Belohnung == Belohnungssystem.LinearMitToleranz)
            {
                if (area.R_ball.KollisionsListe.Contains(KollisionsLayer.Becherboden))
                {

                    if (area.R_ball.KollisionsListe.Contains(KollisionsLayer.Einwurfzone))
                    {
                        if (area.R_ball.EinwurfWinkel >= 60)
                        {
                            Debug.Log("Reward= " + 1f + " mit einem Einwurfwinkel von über 60 Grad = " + area.R_ball.EinwurfWinkel);
                            SetReward(1f);
                            EndEpisode();
                        }
                        else
                        {
                            Debug.Log("Reward= " + (0.5f + area.R_ball.EinwurfWinkel / 120) + " mit einem Einwurfwinkel von " + area.R_ball.EinwurfWinkel);
                            SetReward(0.5f + area.R_ball.EinwurfWinkel / 120);
                            EndEpisode();
                        }
                    }
                }
                else if ((area.R_ball.KollisionsListe.Contains(KollisionsLayer.Boden) || area.R_ball.KollisionsListe.Contains(KollisionsLayer.Roboter) ||
                    area.R_ball.KollisionsListe.Contains(KollisionsLayer.Decke)) && !area.R_ball.KollisionsListe.Contains(KollisionsLayer.Einwurfzone))
                {
                    float distanceBallToTarget = area.DistanceBallToTarget();
                    float reward = (float)(0.5 - 0.5 * distanceBallToTarget);
                    if (area.R_ball.KollisionsListe.Contains(KollisionsLayer.Becherwand))
                    {
                        Debug.Log("Reward= " + 0.5f);
                        SetReward(0.5f);
                    }
                    else if (reward <= 0f)
                    {
                        Debug.Log("Reward= " + 0);
                        SetReward(0);
                    }
                    else
                    {
                        Debug.Log("Reward= " + reward);
                        SetReward(reward);
                    }
                    EndEpisode();
                }
            }
            else if (Belohnung == Belohnungssystem.LinearMitNegativ)
            {
                if (area.R_ball.KollisionsListe.Contains(KollisionsLayer.Becherboden))
                {

                    if (area.R_ball.KollisionsListe.Contains(KollisionsLayer.Einwurfzone))
                    {
                        if (area.R_ball.EinwurfWinkel >= 60)
                        {
                            Debug.Log("Reward= " + 1f + " mit einem Einwurfwinkel von über 60 Grad = " + area.R_ball.EinwurfWinkel);
                            SetReward(1f);
                            EndEpisode();
                        }
                        else
                        {
                            Debug.Log("Reward= " + (area.R_ball.EinwurfWinkel / 60) + " mit einem Einwurfwinkel von " + area.R_ball.EinwurfWinkel);
                            SetReward(area.R_ball.EinwurfWinkel / 60);
                            EndEpisode();
                        }
                    }
                }
                else if ((area.R_ball.KollisionsListe.Contains(KollisionsLayer.Boden) || area.R_ball.KollisionsListe.Contains(KollisionsLayer.Roboter) ||
                    area.R_ball.KollisionsListe.Contains(KollisionsLayer.Decke)) && !area.R_ball.KollisionsListe.Contains(KollisionsLayer.Einwurfzone))
                {
                    float distanceBallToTarget = area.DistanceBallToTarget();
                    float reward = (float)(-1 * distanceBallToTarget);
                    if (reward <= -1f)
                    {
                        Debug.Log("Reward= " + (-1f));
                        SetReward(-1f);
                    }
                    else
                    {
                        Debug.Log("Reward= " + reward);
                        SetReward(reward);
                    }
                    EndEpisode();
                }
            }
            else if (Belohnung == Belohnungssystem.LinearNegativToleranz)
            {
                if (area.R_ball.KollisionsListe.Contains(KollisionsLayer.Becherboden))
                {

                    if (area.R_ball.KollisionsListe.Contains(KollisionsLayer.Einwurfzone))
                    {
                        if (area.R_ball.EinwurfWinkel >= 60)
                        {
                            Debug.Log("Reward= " + 1f + " mit einem Einwurfwinkel von über 60 Grad = " + area.R_ball.EinwurfWinkel);
                            SetReward(1f);
                        }
                        else if (area.R_ball.EinwurfWinkel <= 30)
                        {
                            Debug.Log("Reward= " + 0f + " mit einem Einwurfwinkel von unter 30 Grad = " + area.R_ball.EinwurfWinkel);
                            SetReward(0f);
                        }
                        else
                        {
                            var reward = -1 + area.R_ball.EinwurfWinkel / 30;
                            Debug.Log("Reward= " + reward + " mit einem Einwurfwinkel von " + area.R_ball.EinwurfWinkel);
                            SetReward(reward);
                        }
                        EndEpisode();
                    }
                }
                else if ((area.R_ball.KollisionsListe.Contains(KollisionsLayer.Boden) || area.R_ball.KollisionsListe.Contains(KollisionsLayer.Roboter) ||
                    area.R_ball.KollisionsListe.Contains(KollisionsLayer.Decke)) && !area.R_ball.KollisionsListe.Contains(KollisionsLayer.Einwurfzone))
                {
                    float distanceBallToTarget = area.DistanceBallToTarget();
                    float reward = (float)(-1 * distanceBallToTarget);
                    if (reward <= -1f)
                    {
                        Debug.Log("Reward= " + (-1f));
                        SetReward(-1f);
                    }
                    else if (area.R_ball.KollisionsListe.Contains(KollisionsLayer.Becherwand))
                    {
                        Debug.Log("Reward= " + 0f);
                        SetReward(0f);
                    }
                    else
                    {
                        Debug.Log("Reward= " + reward);
                        SetReward(reward);
                    }
                    EndEpisode();
                }
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }
}
