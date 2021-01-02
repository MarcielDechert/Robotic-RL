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
    [SerializeField] private Text wurfweiteText;

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

    [SerializeField] private Toggle toggleFlugbahn;

    [SerializeField] private Dropdown dropdownModi;

    [SerializeField] private LineRenderer flugbahn;

    [SerializeField] private int segmente = 10;
    [SerializeField] private RoboterControllerV7 roboter;

    [SerializeField] private BallControllerV7 ballRoboter;


    private Vector3 letzteBallposition = Vector3.zero;
    private int count;
    private bool abwurfbereit;
    private float[] startRotation;
    private float[] abwurfRotation;
    private float[] startGeschwindigkeit;
    private float[] abwurfGeschwindigkeit;

    // Start is called before the first frame update
    void Start()
    {

        toggleFlugbahn.onValueChanged.AddListener(delegate
        {
            FlugbahnAktivieren(toggleFlugbahn);
        });

        dropdownModi.onValueChanged.AddListener(delegate
        {
            WechselModi(dropdownModi);
        });

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

        flugbahn.positionCount = segmente;
        flugbahn.enabled = false;
        toggleFlugbahn.isOn = false;
        abwurfbereit = false;
        count = 0;

    }

    // Update is called once per frame
    void Update()
    {
        SetzeTextfelder();
        FlugbahnZeichnen();
    }

    private void FlugbahnZeichnen()
    {

        if (roboter.RoboterStatus == RoboterStatus.Wirft && count < segmente && abwurfbereit)
        {
            flugbahn.SetPosition(count, ballRoboter.transform.position);
            letzteBallposition = ballRoboter.transform.position;
            count++;
        }

    }

    private void ResetFlugbahn()
    {
        for (int i = 0; i < segmente; i++)
        {
            flugbahn.SetPosition(i, Vector3.zero);
        }
        abwurfbereit = false;
        count = 0;
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

    private void SetzeTextfelder()
    {

        abwurfwinkelBallText.text = "Abwurfwinkel: " + roboter.AbwurfwinkelBall + " Grad";
        abwurfgeschwindigkeitText.text = "Abwurfgeschwindigkeit: " + roboter.Abwurfgeschwindigkeit + " ms";
        einwurfwinkelText.text = "Einwurfwinkel: " + ballRoboter.EinwurfWinkel + " Grad";
        wurfweiteText.text = "Wurfweite: " + ballRoboter.Wurfweite + " m";

        j1RotationText.text = "J1: " + Mathf.Round(roboter.AchseV7[0].AktuelleRotationDerAchse()) + " Grad";
        j2RotationText.text = "J2: " + Mathf.Round(roboter.AchseV7[1].AktuelleRotationDerAchse()) + " Grad";
        j3RotationText.text = "J3: " + Mathf.Round(roboter.AchseV7[2].AktuelleRotationDerAchse()) + " Grad";
        j4RotationText.text = "J4: " + Mathf.Round(roboter.AchseV7[3].AktuelleRotationDerAchse()) + " Grad";
        j5RotationText.text = "J5: " + Mathf.Round(roboter.AchseV7[4].AktuelleRotationDerAchse()) + " Grad";

    }

    private void StartButtonGedrueckt()
    {
        SetzeStartRotation();
        SetzeStartGeschwindigkeit();
        roboter.InStartposition(startRotation, startGeschwindigkeit);
        ResetFlugbahn();

    }

    private void AbwurfButtonGedrueckt()
    {
        SetzeAbwurfRotation();
        SetzeAbwurfGeschwindigkeit();
        roboter.StarteAbwurf(abwurfRotation, abwurfGeschwindigkeit);
        abwurfbereit = true;

    }

    private void FlugbahnAktivieren(Toggle change)
    {
        if (change.isOn)
        {
            flugbahn.enabled = true;
        }
        else
        {
            flugbahn.enabled = false;
        }

    }

    private void WechselModi(Dropdown change)
    {
        switch (change.value)
        {
            case 0:
                Debug.Log("Manuell");
                // Skripte ausschalten
                break;
            case 1:
                Debug.Log("KI");
                // Skripte einschalten
                break;
            default:
                break;
        }
    }
    public void SetzeGeschwindikeitDerScene(float geschwindigkeit)
    {
        Time.timeScale = geschwindigkeit;
    }

    public void RotiereJ1(float winkel)
    {
        roboter.AchseV7[0].RotiereSofort(winkel);
        inputJ1.text = "" + winkel;
    }

    public void RotiereJ2(float winkel)
    {
        roboter.AchseV7[1].RotiereSofort(winkel);
        inputJ2.text = "" + winkel;
    }

    public void RotiereJ3(float winkel)
    {
        roboter.AchseV7[2].RotiereSofort(winkel);
        inputJ3.text = "" + winkel;
    }

    public void RotiereJ4(float winkel)
    {
        roboter.AchseV7[3].RotiereSofort(winkel);
        inputJ4.text = "" + winkel;
    }

    public void RotiereJ5(float winkel)
    {
        roboter.AchseV7[4].RotiereSofort(winkel);
        inputJ5.text = "" + winkel;
    }
}
