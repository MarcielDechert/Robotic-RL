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

    public RoboterManagerV6 r_robot;
    public Rigidbody r_target;
    public BallManager r_ball;


    // Start is called before the first frame update

    void Start()
    {
        r_robot = roboter.GetComponent<RoboterManagerV6>();
        r_target = target.GetComponent<Rigidbody>();
        r_ball = ball.GetComponent<BallManager>();
    }

    void LateUpdate()
    {
        //Agent.GetComponent<RoboterAgent>().enabled = true;
    }

    public void Reset()
    {
        r_target.transform.localPosition = new Vector3((float)(-1 * (Random.value + 0.3)), 0, 0);
        ball.transform.localPosition = new Vector3(0, 1f, 0);
        ball.velocity = Vector3.zero;
        ball.useGravity = false;
        r_ball.Kollidiert = false;
    }

    public float DistanceToTarget()
    {
        return Vector3.Distance(r_robot.transform.position, r_target.position);
    }
}
