using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoboterGUIV7 : MonoBehaviour
{
    [SerializeField] private Button abwurfButton;
    [SerializeField] private Button StartfButton;
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

    [SerializeField] private InputField inputJ1;

    [SerializeField] private InputField inputJ2;

    [SerializeField] private InputField inputJ3;

    [SerializeField] private InputField inputJ4;

    [SerializeField] private InputField inputJ5;

    [SerializeField] private InputField wurfgeschwindigkeitJ3;

    [SerializeField] private InputField abwurfwinkelJ3;


    [SerializeField] private  GameObject roboter;

    [SerializeField] private  GameObject ball;
    RoboterControllerV7 roboterManager;

    BallControllerV7 ballManager;

    private float[] startRotation;
    private float[] abwurfRotation;
    private float[] startGeschwindigkeit;
    private float[] abwurfGeschwindigkeit;

    // Start is called before the first frame update
    void Start()
    {
        
        roboterManager = roboter.GetComponent<RoboterControllerV7>();
        ballManager = ball.GetComponent<BallControllerV7>();

        startRotation = new float[6];

        abwurfRotation = new float[6];

        startGeschwindigkeit = new float[6];

        abwurfGeschwindigkeit = new float[6];

        StartfButton.onClick.AddListener(StartButtonGedrueckt);
        abwurfButton.onClick.AddListener(AbwurfButtonGedrueckt);
        inputJ1.text = "180";
        inputJ2.text = "0";
        inputJ3.text = "80";         
        inputJ4.text = "0";        
        inputJ5.text = "60";          
        wurfgeschwindigkeitJ3.text = "500";         
        abwurfwinkelJ3.text = "-80";

        //inputJ1.onValueChanged.AddListener(SetzeTextfelder);

    }

    // Update is called once per frame
    void Update()
    {
        abwurfwinkelBallText.text = "Abwurfwinkel: " + roboterManager.AbwurfwinkelBall + " Grad";
        abwurfgeschwindigkeitText.text = "Abwurfgeschwindigkeit: " + roboterManager.Abwurfgeschwindigkeit + " ms";
        einwurfwinkelText.text = "Einwurfwinkel: " + ballManager.Einwurfwinkel + " Grad";
       
        j1RotationText.text = "J1: "+ Mathf.Round(roboterManager.IstRotation[0]) + " Grad";
        j2RotationText.text = "J2: "+ Mathf.Round(roboterManager.IstRotation[1]) + " Grad";
        j3RotationText.text = "J3: "+ Mathf.Round(roboterManager.IstRotation[2]) + " Grad";
        j4RotationText.text = "J4: "+ Mathf.Round(roboterManager.IstRotation[3]) + " Grad";
        j5RotationText.text = "J5: "+ Mathf.Round(roboterManager.IstRotation[4]) + " Grad";
       
       
    }
    private void SetzeStartRotation()
    {
        startRotation[0] = float.Parse(inputJ1.text);
        startRotation[1] = float.Parse(inputJ2.text);
        startRotation[2] = float.Parse(inputJ3.text);
        startRotation[3] = float.Parse(inputJ4.text);
        startRotation[4] = float.Parse(inputJ5.text);
        startRotation[5] = 0.0f;
    }

    private void SetzeStartGeschwindigkeit()
    {
        startGeschwindigkeit[0] = 500.0f;
        startGeschwindigkeit[1] = 500.0f;
        startGeschwindigkeit[2] = 500.0f;
        startGeschwindigkeit[3] = 500.0f;
        startGeschwindigkeit[4] = 500.0f;
        startGeschwindigkeit[5] = 500.0f;

    }
    private void SetzeAbwurfRotation()
    {
        abwurfRotation[0] = float.Parse(inputJ1.text);
        abwurfRotation[1] = float.Parse(inputJ2.text);
        abwurfRotation[2] = float.Parse(abwurfwinkelJ3.text);
        abwurfRotation[3] = float.Parse(inputJ4.text);
        abwurfRotation[4] = float.Parse(inputJ5.text);
        abwurfRotation[5] = 0.0f;
    }

    private void SetzeAbwurfGeschwindigkeit()
    {
        abwurfGeschwindigkeit[0] = 500.0f;
        abwurfGeschwindigkeit[1] = 500.0f;
        abwurfGeschwindigkeit[2] = float.Parse(wurfgeschwindigkeitJ3.text);
        abwurfGeschwindigkeit[3] = 500.0f;
        abwurfGeschwindigkeit[4] = 500.0f;
        abwurfGeschwindigkeit[5] = 500.0f;
    }

    private void SetzeTextfelder(){

    }

    private void StartButtonGedrueckt()
    {
        SetzeStartRotation();
        SetzeStartGeschwindigkeit();
        roboterManager.InStartposition(startRotation,startGeschwindigkeit);

    }

    private void AbwurfButtonGedrueckt()
    {
        SetzeAbwurfRotation();
        SetzeAbwurfGeschwindigkeit();
        roboterManager.StarteAbwurf(abwurfRotation,abwurfGeschwindigkeit);

    }
}
