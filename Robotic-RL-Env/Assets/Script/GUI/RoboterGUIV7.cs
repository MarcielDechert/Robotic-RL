﻿using System.Collections;
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
    [SerializeField] private Text abwurfhoeheText;

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

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {

        toggleFlugbahn.onValueChanged.AddListener(delegate
        {
            FlugbahnAktivieren(toggleFlugbahn);
        });

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
    public void Step()
    {
        SetzeTextfelder();
        FlugbahnZeichnen();
        SliderAktivieren();
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

    private void FlugbahnZeichnen()
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
        if (area.R_robot.RoboterStatus == RoboterStatus.Abwurfbereit)
        {

            abwurfwinkelBallText.text = "Abwurfwinkel: 0.0 Grad";
            abwurfgeschwindigkeitText.text = "Abwurfgeschwindigkeit: 0.0 ms";
            einwurfwinkelText.text = "Einwurfwinkel: 0.0 Grad";
            wurfweiteText.text = "Wurfweite: 0.0 m";
            abwurfhoeheText.text = "Abwurfhoehe: 0.0 m";
        }else
        {
            abwurfwinkelBallText.text = "Abwurfwinkel: " + area.R_robot.AbwurfwinkelBall + " Grad";
            abwurfgeschwindigkeitText.text = "Abwurfgeschwindigkeit: " + area.R_robot.Abwurfgeschwindigkeit + " ms";
            einwurfwinkelText.text = "Einwurfwinkel: " + area.R_ball.EinwurfWinkel + " Grad";
            wurfweiteText.text = "Wurfweite: " + area.Wurfweite + " m";
            abwurfhoeheText.text = "Abwurfhoehe: " + area.Abwurfhoehe + " m";
        }

        j1RotationText.text = "J1: " + Mathf.Round(area.R_robot.AchseV7[0].AktuelleRotationDerAchse()) + " Grad";
        j2RotationText.text = "J2: " + Mathf.Round(area.R_robot.AchseV7[1].AktuelleRotationDerAchse()) + " Grad";
        j3RotationText.text = "J3: " + Mathf.Round(area.R_robot.AchseV7[2].AktuelleRotationDerAchse()) + " Grad";
        j4RotationText.text = "J4: " + Mathf.Round(area.R_robot.AchseV7[3].AktuelleRotationDerAchse()) + " Grad";
        j5RotationText.text = "J5: " + Mathf.Round(area.R_robot.AchseV7[4].AktuelleRotationDerAchse()) + " Grad";

    }

    private void StartButtonGedrueckt()
    {
        SetzeStartRotation();
        SetzeStartGeschwindigkeit();
        area.R_robot.InStartposition(startRotation, startGeschwindigkeit);
        ResetFlugbahn();

    }

    private void AbwurfButtonGedrueckt()
    {
        SetzeAbwurfRotation();
        SetzeAbwurfGeschwindigkeit();
        area.R_robot.StarteAbwurf(abwurfRotation, abwurfGeschwindigkeit);
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
    private void SetzeGeschwindikeitDerScene(Slider change)
    {
        Time.timeScale = change.value;
    }

    private void RotiereJ1(Slider change)
    {
        area.R_robot.AchseV7[0].RotiereSofort(change.value);
        inputJ1.text = "" + change.value;
    }

    private void RotiereJ2(Slider change)
    {
        area.R_robot.AchseV7[1].RotiereSofort(change.value);
        inputJ2.text = "" + change.value;
    }

    private void RotiereJ3(Slider change)
    {
        area.R_robot.AchseV7[2].RotiereSofort(change.value);
        inputJ3.text = "" + change.value;
    }

    private void RotiereJ4(Slider change)
    {
        area.R_robot.AchseV7[3].RotiereSofort(change.value);
        inputJ4.text = "" + change.value;
    }

    private void RotiereJ5(Slider change)
    {
        area.R_robot.AchseV7[4].RotiereSofort(change.value);
        inputJ5.text = "" + change.value;
    }
}
