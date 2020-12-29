using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotsLearningArea : MonoBehaviour
{
    [Header("Learning Parts")]
    public Transform roboter;
    public Rigidbody target;
    public Rigidbody ball;
    public GameObject Agent;

    public RoboterControllerV7 r_robot;
    public Rigidbody r_target;
    public BallControllerV7 r_ball;


    // Start is called before the first frame update

    void Start()
    {
        r_robot = roboter.GetComponent<RoboterControllerV7>();
        r_target = target.GetComponent<Rigidbody>();
        r_ball = ball.GetComponent<BallControllerV7>();
    }

    void LateUpdate()
    {
        //Agent.GetComponent<RoboterAgent>().enabled = true;
    }

    public void Reset()
    {
        r_target.transform.localPosition = new Vector3((float)(-1 * (Random.value + 0.3)), 0, 0);
        ball.transform.localPosition = new Vector3(0, 2f, 0);
        ball.velocity = Vector3.zero;
        ball.angularVelocity = Vector3.zero;
        ball.useGravity = false;
        r_ball.Kollidiert = false;
    }

    public float DistanceToTarget()
    {
        return Vector3.Distance(r_robot.transform.position, r_target.position);
    }
}
