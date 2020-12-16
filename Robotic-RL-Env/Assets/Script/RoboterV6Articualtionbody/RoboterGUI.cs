using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoboterGUI : MonoBehaviour
{
    [SerializeField] private Text abwurfgeschwindigkeitText;
    [SerializeField] private Text abwurfwinkelBallText;
    [SerializeField] private Text abwurfwinkelJ3Text;
    [SerializeField] private Text einwurfwinkelText;
    
    [SerializeField] private Text j1RotationText;
    
    [SerializeField] private Text j2RotationText;
    
    [SerializeField] private Text j3RotationText;
    
    [SerializeField] private Text j4RotationText;
    
    [SerializeField] private Text j5RotationText;
    
    [SerializeField] private Text j6RotationText;

    [SerializeField] private  GameObject roboter;

    [SerializeField] private  GameObject ball;
    RoboterManagerV6 roboterManager;

    BallManager ballManager;

    // Start is called before the first frame update
    void Start()
    {
        
        roboterManager = roboter.GetComponent<RoboterManagerV6>();
        ballManager = ball.GetComponent<BallManager>();
    }

    // Update is called once per frame
    void Update()
    {

        abwurfwinkelJ3Text.text = "Abwurfwinkel J3: "+ roboterManager.AbwurfwinkelJ3 + " Grad";
        abwurfwinkelBallText.text = "Abwurfwinkel: " + roboterManager.AbwurfwinkelBall + " Grad";
        abwurfgeschwindigkeitText.text = "Abwurfgeschwindigkeit: " + roboterManager.Abwurfgeschwindigkeit + " ms";
        einwurfwinkelText.text = "Einwurfwinkel: " + ballManager.Einwurfwinkel + " Grad";
       
    }
}
