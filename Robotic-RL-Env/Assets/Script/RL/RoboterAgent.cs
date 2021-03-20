using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;

public enum Belohnungssystem { Linear = 0, LinearMitToleranz = 1, LinearMitNegativ = 2, LinearNegativToleranz = 3};

/// <summary>
/// Diese Klasse beinhaltet die Steuerung des Ablaufes des Lernprozesses. Hierzu wird die Klasse Agent des ML-Agent-Toolkit
/// vererbt und die Funktionalitäten genutzt um die Schnittstelle zu externen Lernalgorithmen zur Verfügung zu stellen.
/// </summary>
public class RoboterAgent : Agent, IStep
{
    public RobotsLearningArea area;
    private bool Abwurfvorgang = false;
    private float elapsedTime;
    public Belohnungssystem Belohnung = Belohnungssystem.LinearMitToleranz; // Hier kann das Belohnungssystem ausgewählt werden, nachdem die Punkte verteilt werden.


    /// <summary>
    /// Die Methode OnEpisodeBegin() wird immer dann aufgerufen, wenn die Methode EndEpisode() aufgerufen wird oder zu Beginn
    /// des Lernprozesses. Innerhalb dieser Methode wird die Umgebung in einen einheitlichen Startzustand gebracht.
    /// </summary>
    public override void OnEpisodeBegin()
    {
        area.AreaReset(); // Hier wird die Umgebung, daher der Ball und der Becher an eine definierte Position verschoben.
        Abwurfvorgang = false;

        float[] sollgeschwindigkeit = new float[] { 100f, 100f, 100f, 25f, 25f, 25f };
        float[] sollwinkel = new float[] { 90f, 0f, 170f, 0, -50, 90 };
        area.R_robot.InStartposition(sollwinkel, sollgeschwindigkeit); // Fahren des Roboters in seine Startposition
    }

    /// <summary>
    /// Diese Methode wird dann aufgerufen, wenn die Methode RequestDecision() aufgerufen wird. Hier werden die aktuellen Zustände,
    /// daher die Entfernung des Bechers dem Lernalgorithmus zur Verfügung gestellt.
    /// </summary>
    /// <param name="sensor">Der Parameter sensor beinhaltet anschließend die Entfernung des Bechers in m</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(area.DistanceToTarget());
    }

    /// <summary>
    /// Diese Methode wird dann aufgerufen, wenn die Antwort des Lernalgorithmus nach der Methode RequestDecision() erhalten wurde. 
    /// Die auszuführenden Aktionen befinden sich in der Variable actionBuffers. Die Aktionen müssen nun von dem Intervall [-1, 1] auf
    /// das jeweilige Intervall des Roboters verschoben werden. Mit den Aktionen wird anschließend der Wurf mit der Methode StarteAbwurf
    /// gestartet
    /// </summary>
    /// <param name="actionBuffers">Auszuführende Aktionen im Intervall von [-1, 1]</param>
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Verschieben des Intervalls der Aktionen.
        var continuousActions = actionBuffers.ContinuousActions;
        continuousActions[0] = (float)((continuousActions[0] / 2) + 0.5);
        continuousActions[1] = (float)((continuousActions[1] / 2) + 0.5);
        float kigeschwindigkeit = Mathf.Lerp(10f, 360f, continuousActions[0]);
        float kiwinkel = Mathf.Lerp(-30f, 170, continuousActions[1]);
        Debug.Log("KI Übergabe: " + continuousActions[0] + " und Winkel: " + continuousActions[1]);

        // Nach Anpassen der Geschwindigkeit und des Winkels wird eine Liste der einzelnen Werte für die Achsen erstellt und der Abwurf gestartet.
        float[] geschwindigkeit = new float[] {0.01f, 0.01f, kigeschwindigkeit, 0.01f, 0.01f, 0.01f };
        float[] winkel = new float[] { 90f, 0f, kiwinkel, 0, -50, 90 };
        area.R_robot.StarteAbwurf(winkel, geschwindigkeit);
        Debug.Log("Befehl in StartAbwurf mit Geschwindigkeit: " + kigeschwindigkeit + " und Winkel: " + kiwinkel);
    }

    /// <summary>
    /// Diese Methode wird in jedem Frame einmal aufgerufen. Daher ist diese Methode mit der FixedUpdated-Methode zu vergleichen, wird jedoch genutzt
    /// um einen koordinierten Ablauf der Umgebung zu gewährleisten und in gewissen Situatiionen die Methode zu überspringen um Performance zu sparen.
    /// Diese Methode gehört zu der Schnittstelle iStep.
    /// </summary>
    public void Step()
    {
        // Ist der Roboter in der Startposition wird der Lernalgorithmus angestoßen die Aktionen zu berechnen.
        if (Abwurfvorgang == false && area.R_robot.RoboterStatus == RoboterStatus.Abwurfbereit)
        {
            RequestDecision();
            Abwurfvorgang = true;
        }

        // In jedem Frame wird hier überprüft, ob der geworfene Ball mit einem weiteren Objekt kollidiert ist. Anschließend wird je nach Belohnungssystem
        // unterscheidliche Überprfungen durchgeführt
        if (area.R_ball.IsKollidiert == true)
        {
            if (Belohnung == Belohnungssystem.Linear)
            {
                // Ist der Ball mit dem Becherboden kollidiert, wurde der Ball in den Becher geworfen. Anschließend wird überprüft mit welchem Winkel die
                // Einwurfzone durchgquert wurde im daraufaufbauend eine Belohnung zu vergeben. Mit der Methode EndEpisode() wird die Episode beendet und
                // die Methode onEpisodeBegin wird wiederaufgerufen.
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
                // Wurde der Ball deneben geworfen, wird eine Kollision mit einem weiteren KollisionLayer erkannt. Mit dieser Information wird auf Basis der
                // Distanz zum Becher eine Belohnung vergeben, die nicht höher als 0.5 Punkte ist. Die Kollision mit dem Boden, Roboter oder Decke hat das
                // Ender der Episode zur Folge.
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
                // Ist der Ball mit dem Becherboden kollidiert, wurde der Ball in den Becher geworfen. Anschließend wird überprüft mit welchem Winkel die
                // Einwurfzone durchgquert wurde im daraufaufbauend eine Belohnung zu vergeben. Mit der Methode EndEpisode() wird die Episode beendet und
                // die Methode onEpisodeBegin wird wiederaufgerufen.

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
                // Wurde der Ball deneben geworfen, wird eine Kollision mit einem weiteren KollisionLayer erkannt. Mit dieser Information wird auf Basis der
                // Distanz zum Becher eine Belohnung vergeben, die nicht höher als 0.5 Punkte ist. Die Kollision mit dem Boden, Roboter oder Decke hat das
                // Ender der Episode zur Folge.
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
                // Ist der Ball mit dem Becherboden kollidiert, wurde der Ball in den Becher geworfen. Anschließend wird überprüft mit welchem Winkel die
                // Einwurfzone durchgquert wurde im daraufaufbauend eine Belohnung zu vergeben. Mit der Methode EndEpisode() wird die Episode beendet und
                // die Methode onEpisodeBegin wird wiederaufgerufen.
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
                // Wurde der Ball deneben geworfen, wird eine Kollision mit einem weiteren KollisionLayer erkannt. Mit dieser Information wird auf Basis der
                // Distanz zum Becher eine Belohnung vergeben, die nicht höher als 0.5 Punkte ist. Die Kollision mit dem Boden, Roboter oder Decke hat das
                // Ender der Episode zur Folge.
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
                // Ist der Ball mit dem Becherboden kollidiert, wurde der Ball in den Becher geworfen. Anschließend wird überprüft mit welchem Winkel die
                // Einwurfzone durchgquert wurde im daraufaufbauend eine Belohnung zu vergeben. Mit der Methode EndEpisode() wird die Episode beendet und
                // die Methode onEpisodeBegin wird wiederaufgerufen.
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
                // Wurde der Ball deneben geworfen, wird eine Kollision mit einem weiteren KollisionLayer erkannt. Mit dieser Information wird auf Basis der
                // Distanz zum Becher eine Belohnung vergeben, die nicht höher als 0.5 Punkte ist. Die Kollision mit dem Boden, Roboter oder Decke hat das
                // Ender der Episode zur Folge.
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

    /// <summary>
    /// Diese Methode wird ebenfalls von dem ML-Agents-Toolkit bzw. von der Klasse Agent übernommen. Diese Methode beeinhaltet normalerweise die Steuerung des Agenten
    /// wie z.B. vorwärts und Rückwärts. Dies ist jedoch in der Step-Methode implementiert, wird jedoch benötigt da sonst eine Fehlermeldung getriggert wird.
    /// </summary>
    /// <param name="actionsOut">Aktion sind hier ebenfalls die Aktionen die ausgeführt werden sollen</param>
    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }
}
