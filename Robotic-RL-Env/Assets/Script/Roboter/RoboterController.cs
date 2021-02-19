﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoboterController : MonoBehaviour, IRobotControl, IStep
{

    private RoboterStatus roboterStatus = RoboterStatus.Neutral;
    public RoboterStatus RoboterStatus { get => roboterStatus; set => roboterStatus = value; }

    [SerializeField] protected Transform abwurfPosition;
    public Transform AbwurfPosition { get => abwurfPosition; set => abwurfPosition = value; }

    private Befehl befehl = Befehl.Neutral;
    public Befehl RoboterBefehl  { get => befehl; set => befehl = value; }

    private Vector3 abwurfgeschwindigkeitVector3;
    public Vector3  AbwurfgeschwindigkeitVector3 { get => abwurfgeschwindigkeitVector3; set => abwurfgeschwindigkeitVector3 = value; }
   

    private float abwurfgeschwindigkeit;
    public float Abwurfgeschwindigkeit { get => abwurfgeschwindigkeit; set => abwurfgeschwindigkeit = value; }

    private float abwurfwinkelBall;
    public float AbwurfwinkelBall { get => abwurfwinkelBall; set => abwurfwinkelBall = value; }

    private Vector3 letztePosition;

    public abstract void Step();

    public abstract void StarteAbwurf(float[] abwurfRotation, float[] abwurfGeschwindigkeit);

    public abstract void InStartposition(float[] startRotation, float[] startGeschwindigkeit);

    public abstract RotationsAchse[] GetAchsen();

    protected void BerechneAbwurfgeschwindigkeit()
    {
        AbwurfgeschwindigkeitVector3 = (AbwurfPosition.position - letztePosition) / Time.fixedDeltaTime;
        letztePosition = AbwurfPosition.position;
        //abwurfgeschwindigkeitVector3 = achse[anzahlAchsen - 1].GetSpeed();
    }
    protected float BerechneAbwurfwinkel()
    {
        return Mathf.Rad2Deg * Mathf.Asin((AbwurfgeschwindigkeitVector3.y) / AbwurfgeschwindigkeitVector3.magnitude);
    }


}
