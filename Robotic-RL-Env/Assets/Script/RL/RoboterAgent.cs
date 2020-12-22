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

        float[] sollgeschwindigkeit = new float[] { 180f, 180f, 180f, 180f, 180f };
        float[] sollwinkel = new float[] { 180f, 0, 90f, 0, 0 };
        robot.InStartposition(sollwinkel, sollgeschwindigkeit);

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
        float kigeschwindigkeit = Mathf.Clamp(vectorAction[0], 100f, 500f);
        float kiwinkel = Mathf.Clamp(vectorAction[0], -80f, 120f);

        float[] geschwindigkeit = new float[] { 180f, 180f, kigeschwindigkeit, 180f, 180f };
        float[] winkel = new float[] { 180f, 0f, kiwinkel, 0f, 0f }; // Startposition J3 = 90 Abwurf = -90

        robot.StarteAbwurf(winkel, geschwindigkeit);
        Debug.Log("Befehl in StartAbwurf mit Geschwindigkeit: " + kigeschwindigkeit + " und Winkel: " + kiwinkel);
    }

    public void FixedUpdate()
    {
        var distanceToTarget = area.DistanceToTarget();

        if(Abwurfvorgang == false && robot.abwurfStatus == RoboterStatus.Abwurfbereit)
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

    public override void Heuristic(float[] actionsOut)
    {

    }
}
