using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotsLearningArea : MonoBehaviour
{
    [Header("Learning Parts")]
    [SerializeField] public RoboterController r_robot;
    public RoboterController R_robot { get => r_robot; }
    [SerializeField] private Rigidbody target;
    [SerializeField] private Rigidbody ball;
    [SerializeField] private RoboterAgent agent;
    public RoboterAgent Agent { get => agent; set => agent = value; }
    [SerializeField] private RoboterGUIV7 gui;
    private BallControllerV7 r_ball;
    public BallControllerV7 R_ball { get => r_ball; set => r_ball = value; }

    private float wurfweite;

    public float Wurfweite { get => wurfweite; set => wurfweite = value; }

    private float abwurfhoehe;

    public float Abwurfhoehe { get => abwurfhoehe; set => abwurfhoehe = value; }



    // Start is called before the first frame update
    void Start()
    {
        r_ball = ball.GetComponent<BallControllerV7>();

    }

    public void Reset()
    {
        if( agent.GetComponent<RoboterAgent>().enabled == true)
        {
            target.transform.localPosition = new Vector3((float)(-0.5 * (Random.value + 1)), 0, 0);
            ball.transform.localPosition = new Vector3(0, 2f, 0);
        }

        ball.velocity = Vector3.zero;
        ball.angularVelocity = Vector3.zero;
        ball.useGravity = false;

        r_robot.AbwurfgeschwindigkeitVector3 = Vector3.zero;
        r_robot.Abwurfgeschwindigkeit= 0.0f;
        r_robot.AbwurfwinkelBall = 0.0f;

        r_ball.Kollidiert = false;
        r_ball.KollisionsListe.Clear();

        wurfweite = 0.0f;
        abwurfhoehe = 0.0f;
    }

    public float DistanceToTarget()
    {
        return Vector3.Distance(r_robot.transform.position, target.position);
    }

    public void BerechneWurfweite()
    {
        wurfweite = Mathf.Abs(r_ball.transform.position.x - r_robot.transform.position.x);
    }

    public void BerechneAbwurfhoehe()
    {
        abwurfhoehe = Mathf.Abs(r_robot.AbwurfPosition.position.y);
    }

    private void FixedUpdate()
    {
        r_robot.Step();
        r_ball.Step();
        gui.Step();
    }
}
