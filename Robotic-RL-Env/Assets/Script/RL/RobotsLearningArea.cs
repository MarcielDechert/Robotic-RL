using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotsLearningArea : MonoBehaviour
{
    [Header("Learning Parts")]
    public GameObject roboter;
    public GameObject target;
    public GameObject ball;
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
        Reset();
    }

    void LateUpdate()
    {
        //Agent.GetComponent<RoboterAgent>().enabled = true;
    }

    public void Reset()
    {
        r_target.transform.localPosition = new Vector3((float)(-1 * (Random.value + 0.3)), 0, 0);
        r_robot.InStartpositionFahren();
        r_ball.Kollidiert = false;
    }

    public float DistanceToTarget()
    {
        return Vector3.Distance(r_robot.transform.position, r_target.position);
    }
}
