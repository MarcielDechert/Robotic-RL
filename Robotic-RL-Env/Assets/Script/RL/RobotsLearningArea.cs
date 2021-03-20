using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Entält Methoden und Komponeten zu Steuerung der Simulation
/// </summary>
public class RobotsLearningArea : MonoBehaviour
{
    [Header("Learning Parts")]
    [SerializeField] private RoboterController r_robot;
    public RoboterController R_robot { get => r_robot;}

    [SerializeField] private BallController r_ball;
    public BallController R_ball { get => r_ball;}

    [SerializeField] private RoboterAgent agent;
    public RoboterAgent Agent { get => agent;}

    [SerializeField] private Rigidbody target;

    public Rigidbody Target { get => target;}


    [SerializeField] private Rigidbody ball;

    private float wurfweite;
    public float Wurfweite { get => wurfweite; set => wurfweite = value; }

    private float abwurfhoehe;
    public float Abwurfhoehe { get => abwurfhoehe; set => abwurfhoehe = value; }


    private Vector3 defaultBallposition;

    /*
     * Für mögliche Zeitanzeigen => Bsp. Flugdauer
    private float stopZeit = 0.0f;

    public float StopZeit { get => stopZeit; set => stopZeit = value; }
    */



    /// <summary>
    /// Wird beim Start einmalig aufgerufen. Deklariert Attribute
    /// </summary>
    private void Start()
    {
        defaultBallposition = r_ball.transform.position;
        r_ball = ball.GetComponent<TTBallController>();
    }

    /// <summary>
    /// Setzt Werte von Attribute zurück
    /// </summary>
    public void AreaReset()
    {
        // wenn der Agent der KI aktiv ist
        if(agent.enabled)
        {
            // Versetzt den Becher auf ein begrenzten Zufallswert in x Richtung
            target.transform.localPosition = new Vector3((float)(-0.25*Random.value - 0.5f), 0.06f, 0);
        }
        // Setzt die Position des Balls zurück
        ball.transform.localPosition = defaultBallposition;

        // Setzt Eigenschaften des Balls zurück
        ball.velocity = Vector3.zero;
        ball.angularVelocity = Vector3.zero;
        ball.useGravity = false;

        // Setzt Abwurfparameter des Roboters zurück
        r_robot.AbwurfGeschwindigkeitVector3 = Vector3.zero;
        r_robot.AbwurfGeschwindigkeit= 0.0f;
        r_robot.AbwurfWinkelBall = 0.0f;

        // Löscht den Inhalt der Kollisionsliste
        r_ball.IsKollidiert = false;
        r_ball.KollisionsListe.Clear();

        // Setzt Ergebnis Werte zurück
        wurfweite = 0.0f;
        abwurfhoehe = 0.0f;
        r_ball.EinwurfWinkel = 0.0f;
        //stopZeit = 0.0f;

    }

    /// <summary>
    /// Berechnet die Distanz zwischen Roboter und Becher
    /// </summary>
    /// <returns> absolute Distanz als Float</returns>
    public float DistanceToTarget()
    {
        return Vector3.Distance(r_robot.transform.position, target.position);
    }

    /// <summary>
    /// Berechnet die Distanz zwischen Ball und Becher
    /// </summary>
    /// <returns></returns>
    public float DistanceBallToTarget() 
    {
        return Vector3.Distance(r_ball.transform.position, target.position);
    }

    /// <summary>
    /// Berechnet absolute Wurfweite in x Richtung
    /// </summary>
    public void BerechneWurfweite()
    {
        wurfweite = Mathf.Abs(r_ball.transform.position.x - r_robot.AbwurfPosition.transform.position.x);
    }

    /// <summary>
    /// Berechnet absolute Abwurfhöhe in y Richtung
    /// </summary>
    public void BerechneAbwurfhoehe()
    {
        abwurfhoehe = Mathf.Abs(r_robot.AbwurfPosition.position.y)+ r_robot.transform.position.y;
    }

    /// <summary>
    /// Verschiebt den Becher in x Richtung 
    /// </summary>
    /// <param name="xPosition">beliebiger Wert</param>
    public void SetBecherposition(float xPosition)
    {
        target.position = new Vector3( xPosition, target.position.y);
    }

    /// <summary>
    /// Steuert/Koordiniert die Schritte der Komponenten Roboter, Ball und Agent
    /// </summary>
    private void FixedUpdate()
    {
        r_robot.Step();

        // Wenn sich der Roboter im Status Wirft befindet
        if(r_robot.RoboterStatus == RoboterStatus.Wirft)
        {
            r_ball.Step();
            //stopZeit += Time.fixedDeltaTime;
        }

        // wenn das Agent-Skript aktiv ist
        if (agent.enabled)
        {
            agent.Step();
        }
    }
}
