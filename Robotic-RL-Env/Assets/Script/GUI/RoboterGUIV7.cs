using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoboterGUIV7 : MonoBehaviour
{
    [SerializeField] private RobotsLearningArea area;

    [SerializeField] private Button abwurfButton;
    [SerializeField] private Button StartfButton;
    [SerializeField] private Text abwurfgeschwindigkeitText;
    [SerializeField] private Text abwurfwinkelBallText;
    [SerializeField] private Text abwurfwinkelJ3Text;
    [SerializeField] private Text einwurfwinkelText;
    [SerializeField] private Text wurfweiteText;
    [SerializeField] private Text abwurfhoeheText;
    [SerializeField] private Text radiusJ3TCP;

    [SerializeField] private Text j1RotationText;

    [SerializeField] private Text j2RotationText;

    [SerializeField] private Text j3RotationText;

    [SerializeField] private Text j4RotationText;

    [SerializeField] private Text j5RotationText;

    [SerializeField] private Text j6RotationText;

    [SerializeField] private Slider sliderJ1;

    [SerializeField] private Slider sliderJ2;

    [SerializeField] private Slider sliderJ3;

    [SerializeField] private Slider sliderJ4;

    [SerializeField] private Slider sliderJ5;

    [SerializeField] private Slider sliderGeschwindigkeitScene;

    [SerializeField] private InputField inputJ1;

    [SerializeField] private InputField inputJ2;

    [SerializeField] private InputField inputJ3;

    [SerializeField] private InputField inputJ4;

    [SerializeField] private InputField inputJ5;

    [SerializeField] private InputField wurfgeschwindigkeitJ3;

    [SerializeField] private InputField abwurfwinkelJ3;

    [SerializeField] private Toggle toggleFlugbahn;
<<<<<<< Updated upstream

    [SerializeField] private Toggle toggleLuftwidertand;

=======
    [SerializeField] private Toggle toggleLuftwiderstand;
    [SerializeField] private Toggle toggleMenue;

    [SerializeField] private GameObject panel;
>>>>>>> Stashed changes

    [SerializeField] private Dropdown dropdownModi;

    [SerializeField] private LineRenderer flugbahn;

    [SerializeField] private int segmente = 100;

<<<<<<< Updated upstream
    [SerializeField] private RobotsLearningArea area;
    private Vector3 letzteBallposition = Vector3.zero;
=======
>>>>>>> Stashed changes
    private int count;
    private bool abwurfbereit;
    private float[] startRotation;
    private float[] abwurfRotation;
    private float[] startGeschwindigkeit;
    private float[] abwurfGeschwindigkeit;

    private RotationsAchse[] achsen;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {

        toggleFlugbahn.onValueChanged.AddListener(delegate
        {
            AktiviereFlugbahn(toggleFlugbahn);
        });

        toggleLuftwidertand.onValueChanged.AddListener(delegate
        {
<<<<<<< Updated upstream
            LuftwiderstandAktivieren(toggleLuftwidertand);
        });

=======
            AktiviereLuftwiderstand(toggleLuftwiderstand);
        });

        toggleMenue.onValueChanged.AddListener(delegate
        {
            EinblendenMenue(toggleMenue);
        });

        /*
>>>>>>> Stashed changes
        dropdownModi.onValueChanged.AddListener(delegate
        {
            WechselModi(dropdownModi);
        });

        sliderJ1.onValueChanged.AddListener(delegate
        {
            RotiereJ1(sliderJ1);
        });

        sliderJ2.onValueChanged.AddListener(delegate
        {
            RotiereJ2(sliderJ2);
        });

        sliderJ3.onValueChanged.AddListener(delegate
        {
            RotiereJ3(sliderJ3);
        });

        sliderJ4.onValueChanged.AddListener(delegate
        {
            RotiereJ4(sliderJ4);
        });

        sliderJ5.onValueChanged.AddListener(delegate
        {
            RotiereJ5(sliderJ5);
        });

        sliderGeschwindigkeitScene.onValueChanged.AddListener(delegate
        {
            SetzeGeschwindikeitDerScene(sliderGeschwindigkeitScene);
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
<<<<<<< Updated upstream
        inputJ5.text = "40";
=======
        inputJ5.text = "-50";
        inputJ6.text = "90";

>>>>>>> Stashed changes
        wurfgeschwindigkeitJ3.text = "180";
        abwurfwinkelJ3.text = "-60";

        flugbahn.positionCount = segmente;
        flugbahn.enabled = false;
        toggleFlugbahn.isOn = false;
        abwurfbereit = false;
        count = 0;

    }

    // Update is called once per frame
    public void LateUpdate()
    {
<<<<<<< Updated upstream
        SetzeTextfelder();
        FlugbahnZeichnen();
        SliderAktivieren();
=======
        SetTextfelder();
        SetFlugbahn();
        AktiviereManuelleAusrichtung();
>>>>>>> Stashed changes
    }

    private void SliderAktivieren()
    {
        if (area.R_robot.RoboterStatus == RoboterStatus.Abwurfbereit)
        {
            sliderJ1.enabled = true;
            sliderJ2.enabled = true;
            sliderJ3.enabled = true;
            sliderJ4.enabled = true;
            sliderJ5.enabled = true;
        }else
        {
            sliderJ1.enabled = false;
            sliderJ2.enabled = false;
            sliderJ3.enabled = false;
            sliderJ4.enabled = false;
            sliderJ5.enabled = false;
        }
    }

<<<<<<< Updated upstream
    private void FlugbahnZeichnen()
=======
    /// <summary>
    /// Zeichnet die Flugbahn des Balls
    /// </summary>
    private void SetFlugbahn()
>>>>>>> Stashed changes
    {

        if (area.R_robot.RoboterStatus == RoboterStatus.Wirft && count < segmente && abwurfbereit)
        {
            flugbahn.SetPosition(count, area.R_ball.transform.position);
            letzteBallposition = area.R_ball.transform.position;
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

<<<<<<< Updated upstream
    private void SetzeStartRotation()
=======
    private void SetRoboterStatus()
    {
        switch (area.R_robot.RoboterStatus)
        {
            case RoboterStatus.Neutral:
                roboterStatusText.text = "Roboter Status: Neutral";
                break;
            case RoboterStatus.Faehrt:
                roboterStatusText.text = "Roboter Status: Fährt";
                break;
            case RoboterStatus.Wirft:
                roboterStatusText.text = "Roboter Status: Wirft";
                break;
            case RoboterStatus.Abwurfbereit:
                roboterStatusText.text = "Roboter Status: Abwurfbereit";
                break;
        }

    }

    /// <summary>
    /// Füllt das Array startRotaion mit dem Inhalt der Eingabefelder
    /// </summary>
    private void SetStartRotation()
>>>>>>> Stashed changes
    {
        startRotation[0] = float.Parse(inputJ1.text);
        startRotation[1] = float.Parse(inputJ2.text);
        startRotation[2] = float.Parse(inputJ3.text);
        startRotation[3] = float.Parse(inputJ4.text);
        startRotation[4] = float.Parse(inputJ5.text);
        startRotation[5] = 0.0f;
    }
<<<<<<< Updated upstream

    private void SetzeStartGeschwindigkeit()
=======
    /// <summary>
    ///  Füllt das Array startGeschwindigkeit mit den Startwinkelgeschwindigkeiten in Grad/s
    /// </summary>
    private void SetStartGeschwindigkeit()
>>>>>>> Stashed changes
    {
        startGeschwindigkeit[0] = 180.0f;
        startGeschwindigkeit[1] = 180.0f;
        startGeschwindigkeit[2] = 180.0f;
        startGeschwindigkeit[3] = 180.0f;
        startGeschwindigkeit[4] = 180.0f;
        startGeschwindigkeit[5] = 180.0f;

<<<<<<< Updated upstream
    }
    private void SetzeAbwurfRotation()
=======
    /// <summary>
    /// Füllt das Array abwurfRotaion mit dem Inhalt der Eingabefelder
    /// </summary>
    private void SetAbwurfRotation()
>>>>>>> Stashed changes
    {
        abwurfRotation[0] = float.Parse(inputJ1.text);
        abwurfRotation[1] = float.Parse(inputJ2.text);
        abwurfRotation[2] = float.Parse(abwurfwinkelJ3.text);
        abwurfRotation[3] = float.Parse(inputJ4.text);
        abwurfRotation[4] = float.Parse(inputJ5.text);
        abwurfRotation[5] = 0.0f;
    }

<<<<<<< Updated upstream
    private void SetzeAbwurfGeschwindigkeit()
=======
    /// <summary>
    /// Füllt das Array abwurfGeschwindigkeit mit den Abwurfwinkelgeschwindigkeiten in Grad/s
    /// </summary>
    private void SetAbwurfGeschwindigkeit()
>>>>>>> Stashed changes
    {
        abwurfGeschwindigkeit[0] = 80.0f;
        abwurfGeschwindigkeit[1] = 80.0f;
        abwurfGeschwindigkeit[2] = float.Parse(wurfgeschwindigkeitJ3.text);
        abwurfGeschwindigkeit[3] = 80.0f;
        abwurfGeschwindigkeit[4] = 80.0f;
        abwurfGeschwindigkeit[5] = 80.0f;
    }

<<<<<<< Updated upstream
    private void SetzeTextfelder()
    {
=======
    /// <summary>
    /// Aktualisiert Anzeigeelemente
    /// </summary>
    private void SetTextfelder()
    {
        SetRoboterStatus();
>>>>>>> Stashed changes
        achsen = area.R_robot.GetAchsen();
        if (area.R_robot.RoboterStatus == RoboterStatus.Abwurfbereit)
        {

            abwurfwinkelBallText.text = "Abwurfwinkel: 0.0 Grad";
            abwurfgeschwindigkeitText.text = "Abwurfgeschwindigkeit: 0.0 ms";
            einwurfwinkelText.text = "Einwurfwinkel: 0.0 Grad";
            wurfweiteText.text = "Wurfweite: 0.0 m";
            abwurfhoeheText.text = "Abwurfhoehe: 0.0 m";
        }else
        {
<<<<<<< Updated upstream
            abwurfwinkelBallText.text = "Abwurfwinkel: " + area.R_robot.AbwurfwinkelBall + " Grad";
            abwurfgeschwindigkeitText.text = "Abwurfgeschwindigkeit: " + area.R_robot.Abwurfgeschwindigkeit + " ms";
=======
            // Setzt Textanzeigen auf aktuellen Wert
            abwurfwinkelBallText.text = "Abwurfwinkel: " + area.R_robot.AbwurfWinkelBall + " Grad";
            abwurfgeschwindigkeitText.text = "Abwurfgeschwindigkeit: " + area.R_robot.AbwurfGeschwindigkeit + " ms";
>>>>>>> Stashed changes
            einwurfwinkelText.text = "Einwurfwinkel: " + area.R_ball.EinwurfWinkel + " Grad";
            wurfweiteText.text = "Wurfweite: " + area.Wurfweite + " m";
            abwurfhoeheText.text = "Abwurfhoehe: " + area.Abwurfhoehe + " m";
            radiusJ3TCP.text = "Radius J3-TCP: " + BerechneRadiusJ3TCP() + " m";
        }        

        j1RotationText.text = "J1: " + Mathf.Round(achsen[0].AktuelleRotationDerAchse()) + " Grad";
        j2RotationText.text = "J2: " + Mathf.Round(achsen[1].AktuelleRotationDerAchse()) + " Grad";
        j3RotationText.text = "J3: " + Mathf.Round(achsen[2].AktuelleRotationDerAchse()) + " Grad";
        j4RotationText.text = "J4: " + Mathf.Round(achsen[3].AktuelleRotationDerAchse()) + " Grad";
        j5RotationText.text = "J5: " + Mathf.Round(achsen[4].AktuelleRotationDerAchse()) + " Grad";

    }

    private void StartButtonGedrueckt()
    {
        SetStartRotation();
        SetStartGeschwindigkeit();
        area.R_robot.InStartposition(startRotation, startGeschwindigkeit);
        ResetFlugbahn();

    }

    private void AbwurfButtonGedrueckt()
    {
        SetAbwurfRotation();
        SetAbwurfGeschwindigkeit();
        area.R_robot.StarteAbwurf(abwurfRotation, abwurfGeschwindigkeit);
        abwurfbereit = true;

    }

<<<<<<< Updated upstream
    private void FlugbahnAktivieren(Toggle change)
=======
    /// <summary>
    /// Aktiviert und deaktiviert die Flugbahnaufzeichnung
    /// </summary>
    /// <param name="change"> Toggle</param>
    private void AktiviereFlugbahn(Toggle change)
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
    private void LuftwiderstandAktivieren(Toggle change)
=======
    /// <summary>
    /// Aktiviert und deaktiviert den Lufwiderstands Flag im BallController
    /// </summary>
    /// <param name="change"> Toggle</param>
    private void AktiviereLuftwiderstand(Toggle change)
>>>>>>> Stashed changes
    {
        if (change.isOn)
        {
            area.R_ball.IsLuftwiderstandAktiv = true;
        }
        else
        {
            area.R_ball.IsLuftwiderstandAktiv = false;
        }

    }
    private float BerechneRadiusJ3TCP()
    {
        return Vector3.Distance(achsen[2].transform.GetChild(0).transform.position,achsen[5].transform.GetChild(1).transform.position);
    }

    private void WechselModi(Dropdown change)
    {
        switch (change.value)
        {
            case 0:
                Debug.Log("Manuell");
                area.Agent.GetComponent<RoboterAgent>().enabled = false;
                // Skripte ausschalten
                break;
            case 1:
                Debug.Log("KI");
                area.Agent.GetComponent<RoboterAgent>().enabled = true;
                // Skripte einschalten
                break;
            default:
                break;
        }
    }
<<<<<<< Updated upstream
=======

    */
    /// <summary>
    /// Blendet die untere Menüleiste über den Toggle ein und aus
    /// </summary>
    /// <param name="change"></param>
    private void EinblendenMenue(Toggle change)
    {   
        // Wenn Toggle aktiviert
        if (change.isOn)
        {
            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
        }
    }

    /// <summary>
    /// Verändert die Geschwindigkeit der Scene 
    /// </summary>
    /// <param name="change"> Inhaltswert des Sliders</param>
>>>>>>> Stashed changes
    private void SetzeGeschwindikeitDerScene(Slider change)
    {
        Time.timeScale = change.value;
    }

    private void RotiereJ1(Slider change)
    {
        area.R_robot.GetAchsen()[0].RotiereSofort(change.value);
        inputJ1.text = "" + change.value;
    }

    private void RotiereJ2(Slider change)
    {
        area.R_robot.GetAchsen()[1].RotiereSofort(change.value);
        inputJ2.text = "" + change.value;
    }

    private void RotiereJ3(Slider change)
    {
        area.R_robot.GetAchsen()[2].RotiereSofort(change.value);
        inputJ3.text = "" + change.value;
    }

    private void RotiereJ4(Slider change)
    {
        area.R_robot.GetAchsen()[3].RotiereSofort(change.value);
        inputJ4.text = "" + change.value;
    }

    private void RotiereJ5(Slider change)
    {   
        
        area.R_robot.GetAchsen()[4].RotiereSofort(change.value);
        inputJ5.text = "" + change.value;
    }
}
