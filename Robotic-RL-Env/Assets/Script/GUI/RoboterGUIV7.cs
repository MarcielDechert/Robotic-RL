using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Diese Klasse ist für die Verarbeitung und Darstellungen von Daten aus 
/// der GUI zuständig.
/// </summary>

public class RoboterGUIV7 : MonoBehaviour
{
    [SerializeField] private Button abwurfButton;
    [SerializeField] private Button startButton;

    [SerializeField] private Text roboterStatusText;
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
    [SerializeField] private Slider sliderJ6;
    [SerializeField] private Slider sliderGeschwindigkeitScene;

    [SerializeField] private InputField inputJ1;
    [SerializeField] private InputField inputJ2;
    [SerializeField] private InputField inputJ3;
    [SerializeField] private InputField inputJ4;
    [SerializeField] private InputField inputJ5;
    [SerializeField] private InputField inputJ6;
    [SerializeField] private InputField wurfgeschwindigkeitJ3;
    [SerializeField] private InputField abwurfwinkelJ3;

    [SerializeField] private Toggle toggleFlugbahn;
    [SerializeField] private Toggle toggleLuftwiderstand;
    [SerializeField] private Toggle toggleMenue;
    [SerializeField] private GameObject panel;
    [SerializeField] private Dropdown dropdownModi;
    [SerializeField] private LineRenderer flugbahn;
    [SerializeField] private int segmente = 100;
    [SerializeField] private RobotsLearningArea area;

    private Vector3 letzteBallposition = Vector3.zero;
    private int count;
    private bool abwurfbereit;
    private float[] startRotation;
    private float[] abwurfRotation;
    private float[] startGeschwindigkeit;
    private float[] abwurfGeschwindigkeit;
    private RotationsAchse[] achsen;

    /// <summary>
    /// Wird beim Start einmalig aufgerufen
    /// </summary>
    void Start()
    {
        Init();
    }
    /// <summary>
    /// Initialisiert Methoden und deklariert Attribute
    /// </summary>
    private void Init()
    {

        toggleFlugbahn.onValueChanged.AddListener(delegate
        {
            FlugbahnAktivieren(toggleFlugbahn);
        });

        toggleLuftwiderstand.onValueChanged.AddListener(delegate
        {
            LuftwiderstandAktivieren(toggleLuftwiderstand);
        });

        toggleMenue.onValueChanged.AddListener(delegate
        {
            MenueEinblenden(toggleMenue);
        });

        /*
        dropdownModi.onValueChanged.AddListener(delegate
        {
            WechselModi(dropdownModi);
        });
        */
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

        sliderJ6.onValueChanged.AddListener(delegate
        {
            RotiereJ6(sliderJ6);
        });

        sliderGeschwindigkeitScene.onValueChanged.AddListener(delegate
        {
            SetzeGeschwindikeitDerScene(sliderGeschwindigkeitScene);
        });

        startRotation = new float[6];

        abwurfRotation = new float[6];

        startGeschwindigkeit = new float[6];

        abwurfGeschwindigkeit = new float[6];

        startButton.onClick.AddListener(StartButtonGedrueckt);
        abwurfButton.onClick.AddListener(AbwurfButtonGedrueckt);

        inputJ1.text = "90";
        inputJ2.text = "0";
        inputJ3.text = "170";
        inputJ4.text = "0";
        inputJ5.text = "-50";
        inputJ6.text = "0";

        wurfgeschwindigkeitJ3.text = "180";
        abwurfwinkelJ3.text = "-15";

        flugbahn.positionCount = segmente;
        flugbahn.enabled = false;
        toggleFlugbahn.isOn = false;
        abwurfbereit = false;
        count = 0;
    }

    /// <summary>
    /// Wird einmal pro Frame aufgerufen und führt 3 Methoden aus die der GUI ein Verhalten geben
    /// </summary>
    public void LateUpdate()
    {
        SetzeTextfelder();
        FlugbahnZeichnen();
        AktiviereManuelleAusrichtung();
    }

    /// <summary>
    /// Aktiviert und deaktiviert die Slider für die manuelle Ausrichtung des Roboters
    /// </summary>
    private void AktiviereManuelleAusrichtung()
    {
        // wenn der Roboter in der Abwurfposition ist
        if (area.R_robot.RoboterStatus == RoboterStatus.Abwurfbereit)
        {
            sliderJ1.enabled = true;
            sliderJ2.enabled = true;
            sliderJ3.enabled = true;
            sliderJ4.enabled = true;
            sliderJ5.enabled = true;
            sliderJ6.enabled = true;
        }
        else
        {
            sliderJ1.enabled = false;
            sliderJ2.enabled = false;
            sliderJ3.enabled = false;
            sliderJ4.enabled = false;
            sliderJ5.enabled = false;
            sliderJ6.enabled = false;
        }
        // wenn der Roboter in der Abwurfposition ist
        if (area.R_robot.RoboterStatus == RoboterStatus.Wirft || area.R_robot.RoboterStatus == RoboterStatus.Neutral)
        {
            inputJ1.enabled = true;
            inputJ2.enabled = true;
            inputJ3.enabled = true;
            inputJ4.enabled = true;
            inputJ5.enabled = true;
            inputJ6.enabled = true;
        }
        else
        {
            inputJ1.enabled = false;
            inputJ2.enabled = false;
            inputJ3.enabled = false;
            inputJ4.enabled = false;
            inputJ5.enabled = false;
            inputJ6.enabled = false;
        }
    }

    /// <summary>
    /// Zeichnet die Flugbahn des Balls
    /// </summary>
    private void FlugbahnZeichnen()
    {
        // wenn der Roboter im Status Wirft ist, die Segemete vom Line Renderer nicht voll sind und die Flag abwurfbereit gestezt ist
        if (area.R_robot.RoboterStatus == RoboterStatus.Wirft && count < segmente && abwurfbereit)
        {
            // zeichnet ein Punkt an der Position des Balls
            flugbahn.SetPosition(count, area.R_ball.transform.position);
            letzteBallposition = area.R_ball.transform.position;
            count++;
        }
    }
    /// <summary>
    /// Löscht die gezeichnete Flugbahn des Balls
    /// </summary>
    private void ResetFlugbahn()
    {
        // Schleife um alle Punkte der gezeichneten Flugbahn zurückzusetzen
        for (int i = 0; i < segmente; i++)
        {
            flugbahn.SetPosition(i, Vector3.zero);
        }
        abwurfbereit = false;
        count = 0;
    }

    private void SetzeRoboterStatus()
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
    private void SetzeStartRotation()
    {
        startRotation[0] = float.Parse(inputJ1.text);
        startRotation[1] = float.Parse(inputJ2.text);
        startRotation[2] = float.Parse(inputJ3.text);
        startRotation[3] = float.Parse(inputJ4.text);
        startRotation[4] = float.Parse(inputJ5.text);
        startRotation[5] = float.Parse(inputJ6.text);
    }
    /// <summary>
    ///  Füllt das Array startGeschwindigkeit mit den Startwinkelgeschwindigkeiten in Grad/s
    /// </summary>
    private void SetzeStartGeschwindigkeit()
    {
        startGeschwindigkeit[0] = 180.0f;
        startGeschwindigkeit[1] = 180.0f;
        startGeschwindigkeit[2] = 180.0f;
        startGeschwindigkeit[3] = 180.0f;
        startGeschwindigkeit[4] = 180.0f;
        startGeschwindigkeit[5] = 180.0f;
    }

    /// <summary>
    /// Füllt das Array abwurfRotaion mit dem Inhalt der Eingabefelder
    /// </summary>
    private void SetzeAbwurfRotation()
    {
        abwurfRotation[0] = float.Parse(inputJ1.text);
        abwurfRotation[1] = float.Parse(inputJ2.text);
        abwurfRotation[2] = float.Parse(abwurfwinkelJ3.text);
        abwurfRotation[3] = float.Parse(inputJ4.text);
        abwurfRotation[4] = float.Parse(inputJ5.text);
        abwurfRotation[5] = float.Parse(inputJ6.text);
    }

    /// <summary>
    /// Füllt das Array abwurfGeschwindigkeit mit den Abwurfwinkelgeschwindigkeiten in Grad/s
    /// </summary>
    private void SetzeAbwurfGeschwindigkeit()
    {
        abwurfGeschwindigkeit[0] = 80.0f;
        abwurfGeschwindigkeit[1] = 80.0f;
        abwurfGeschwindigkeit[2] = float.Parse(wurfgeschwindigkeitJ3.text);
        abwurfGeschwindigkeit[3] = 80.0f;
        abwurfGeschwindigkeit[4] = 80.0f;
        abwurfGeschwindigkeit[5] = 80.0f;
    }

    /// <summary>
    /// Aktualisiert Anzeigeelemente
    /// </summary>
    private void SetzeTextfelder()
    {
        SetzeRoboterStatus();
        achsen = area.R_robot.GetAchsen();

        // Wenn der Roboter im Status Abwurfberit ist
        if (area.R_robot.RoboterStatus == RoboterStatus.Abwurfbereit)
        {
            // Setzt Textanzeigen auf 0
            abwurfwinkelBallText.text = "Abwurfwinkel: 0.0 Grad";
            abwurfgeschwindigkeitText.text = "Abwurfgeschwindigkeit: 0.0 ms";
            einwurfwinkelText.text = "Einwurfwinkel: 0.0 Grad";
            wurfweiteText.text = "Wurfweite: 0.0 m";
            abwurfhoeheText.text = "Abwurfhoehe: 0.0 m";
            // Richtet die Slider neu aus
            sliderJ1.value = Mathf.Round(achsen[0].AktuelleRotationDerAchse());
            sliderJ2.value = Mathf.Round(achsen[1].AktuelleRotationDerAchse());
            sliderJ3.value = Mathf.Round(achsen[2].AktuelleRotationDerAchse());
            sliderJ4.value = Mathf.Round(achsen[3].AktuelleRotationDerAchse());
            sliderJ5.value = Mathf.Round(achsen[4].AktuelleRotationDerAchse());
            sliderJ6.value = Mathf.Round(achsen[5].AktuelleRotationDerAchse());
        }
        else
        {
            // Setzt Textanzeigen auf aktuellen Wert
            abwurfwinkelBallText.text = "Abwurfwinkel: " + area.R_robot.AbwurfwinkelBall + " Grad";
            abwurfgeschwindigkeitText.text = "Abwurfgeschwindigkeit: " + area.R_robot.Abwurfgeschwindigkeit + " ms";
            einwurfwinkelText.text = "Einwurfwinkel: " + area.R_ball.EinwurfWinkel + " Grad";
            wurfweiteText.text = "Wurfweite: " + area.Wurfweite + " m";
            abwurfhoeheText.text = "Abwurfhoehe: " + area.Abwurfhoehe + " m";
            radiusJ3TCP.text = "Radius J3-TCP: " + BerechneRadiusJ3TCP() + " m";
        }
        // Setzt Textanzeigen auf aktuellen Wert
        j1RotationText.text = "J1: " + Mathf.Round(achsen[0].AktuelleRotationDerAchse()) + " Grad";
        j2RotationText.text = "J2: " + Mathf.Round(achsen[1].AktuelleRotationDerAchse()) + " Grad";
        j3RotationText.text = "J3: " + Mathf.Round(achsen[2].AktuelleRotationDerAchse()) + " Grad";
        j4RotationText.text = "J4: " + Mathf.Round(achsen[3].AktuelleRotationDerAchse()) + " Grad";
        j5RotationText.text = "J5: " + Mathf.Round(achsen[4].AktuelleRotationDerAchse()) + " Grad";
        j6RotationText.text = "J6: " + Mathf.Round(achsen[5].AktuelleRotationDerAchse()) + " Grad";
    }

    /// <summary>
    /// Ruft Methoden auf damit der Roboter in Startposition fährt
    /// </summary>
    private void StartButtonGedrueckt()
    {
        SetzeStartRotation();
        SetzeStartGeschwindigkeit();
        area.R_robot.InStartposition(startRotation, startGeschwindigkeit);
        ResetFlugbahn();
        area.AreaReset();
    }

    /// <summary>
    /// Ruft Methoden für auf damit der Roboter in Abwurfposition fährt
    /// </summary>
    private void AbwurfButtonGedrueckt()
    {
        SetzeAbwurfRotation();
        SetzeAbwurfGeschwindigkeit();
        area.R_robot.StarteAbwurf(abwurfRotation, abwurfGeschwindigkeit);
        abwurfbereit = true;
    }

    /// <summary>
    /// Aktiviert und deaktiviert die Flugbahnaufzeichnung
    /// </summary>
    /// <param name="change"> Toggle</param>
    private void FlugbahnAktivieren(Toggle change)
    {
        // Wenn der Toogle betätigt ist
        if (change.isOn)
        {
            flugbahn.enabled = true;
        }
        else
        {
            flugbahn.enabled = false;
        }
    }

    /// <summary>
    /// Aktiviert und deaktiviert den Lufwiderstands Flag im BallController
    /// </summary>
    /// <param name="change"> Toggle</param>
    private void LuftwiderstandAktivieren(Toggle change)
    {
        // Wenn der Toogle betätigt ist
        if (change.isOn)
        {
            area.R_ball.LuftwiderstandAktiv = true;
        }
        else
        {
            area.R_ball.LuftwiderstandAktiv = false;
        }
    }

    /// <summary>
    /// Berechnet den Radius zwischen dem TCP und der J3 Achse des Roboters
    /// </summary>
    /// <returns> absoluten Distanzwert als Float </returns>
    private float BerechneRadiusJ3TCP()
    {
        return Vector3.Distance(achsen[2].transform.GetChild(0).GetChild(0).transform.position,achsen[5].transform.GetChild(1).transform.position);
    }

    /*
    /// <summary>
    /// Wechselt zwischen manuellen und KI Betrieb
    /// </summary>
    /// <param name="change"> Dropdown</param>
    private void WechselModi(Dropdown change)
    {
        switch (change.value)
        {   
            // Schaltet das Agent Skript der KI aus
            case 0:
                area.Agent.GetComponent<RoboterAgent>().enabled = false;
                break;
            // Schaltet das Agent Skript der KI ein
            case 1:
                area.Agent.GetComponent<RoboterAgent>().enabled = true;
                // Skripte einschalten
                break;
            default:
                break;
        }
    }

    */
    /// <summary>
    /// Blendet die untere Menüleiste über den Toggle ein und aus
    /// </summary>
    /// <param name="change"></param>
    private void MenueEinblenden(Toggle change)
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
    private void SetzeGeschwindikeitDerScene(Slider change)
    {
        Time.timeScale = change.value;
    }

    /// <summary>
    /// Rotiert die Achse J1 des Roboters und aktualisiert Anzeigefeld für J1 
    /// </summary>
    /// <param name="change">Inhaltswert des Sliders</param>
    private void RotiereJ1(Slider change)
    {
        area.R_robot.GetAchsen()[0].RotiereSofort(change.value);
        inputJ1.text = "" + change.value;
    }

    /// <summary>
    /// Rotiert die Achse J2 des Roboters und aktualisiert Anzeigefeld für J2 
    /// </summary>
    /// <param name="change">Inhaltswert des Sliders</param>
    private void RotiereJ2(Slider change)
    {
        area.R_robot.GetAchsen()[1].RotiereSofort(change.value);
        inputJ2.text = "" + change.value;
    }

    /// <summary>
    /// Rotiert die Achse J3 des Roboters und aktualisiert Anzeigefeld für J3 
    /// </summary>
    /// <param name="change">Inhaltswert des Sliders</param>
    private void RotiereJ3(Slider change)
    {
        area.R_robot.GetAchsen()[2].RotiereSofort(change.value);
        inputJ3.text = "" + change.value;
    }

    /// <summary>
    /// Rotiert die Achse J4 des Roboters und aktualisiert Anzeigefeld für J4 
    /// </summary>
    /// <param name="change">Inhaltswert des Sliders</param>
    private void RotiereJ4(Slider change)
    {
        area.R_robot.GetAchsen()[3].RotiereSofort(change.value);
        inputJ4.text = "" + change.value;
    }

    /// <summary>
    /// Rotiert die Achse J5 des Roboters und aktualisiert Anzeigefeld für J5 
    /// </summary>
    /// <param name="change">Inhaltswert des Sliders</param>
    private void RotiereJ5(Slider change)
    {   
        area.R_robot.GetAchsen()[4].RotiereSofort(change.value);
        inputJ5.text = "" + change.value;
    }

    /// <summary>
    /// Rotiert die Achse J6 des Roboters und aktualisiert Anzeigefeld für J6 
    /// </summary>
    /// <param name="change">Inhaltswert des Sliders</param>
    private void RotiereJ6(Slider change)
    {
        area.R_robot.GetAchsen()[5].RotiereSofort(change.value);
        inputJ6.text = "" + change.value;
    }
}
