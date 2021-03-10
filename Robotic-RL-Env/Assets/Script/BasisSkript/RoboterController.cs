using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoboterController : MonoBehaviour, IRobotControl, IStep
{

    private RoboterStatus roboterStatus = RoboterStatus.Neutral;
    public RoboterStatus RoboterStatus { get => roboterStatus; set => roboterStatus = value; }
    [SerializeField] protected Transform abwurfPosition;
    public Transform AbwurfPosition { get => abwurfPosition; set => abwurfPosition = value; }

    private Befehl befehl = Befehl.Neutral;
<<<<<<< Updated upstream
    public Befehl Befehl  { get => befehl; set => befehl = value; }
    private Vector3 abwurfgeschwindigkeitVector3;
    public Vector3  AbwurfgeschwindigkeitVector3 { get => abwurfgeschwindigkeitVector3; set => abwurfgeschwindigkeitVector3 = value; }
    private Vector3 letztePosition;
    private float abwurfgeschwindigkeit;
    public float Abwurfgeschwindigkeit { get => abwurfgeschwindigkeit; set => abwurfgeschwindigkeit = value; }
=======
    public Befehl RoboterBefehl  { get => befehl; set => befehl = value; }

    private Vector3 abwurfGeschwindigkeitVector3;
    public Vector3  AbwurfGeschwindigkeitVector3 { get => abwurfGeschwindigkeitVector3; set => abwurfGeschwindigkeitVector3 = value; }
   

    private float abwurfGeschwindigkeit;
    public float AbwurfGeschwindigkeit { get => abwurfGeschwindigkeit; set => abwurfGeschwindigkeit = value; }
>>>>>>> Stashed changes

    private float abwurfWinkelBall;
    public float AbwurfWinkelBall { get => abwurfWinkelBall; set => abwurfWinkelBall = value; }

    public abstract void Step();

    public abstract void StarteAbwurf(float[] abwurfRotation, float[] abwurfGeschwindigkeit);

    public abstract void InStartposition(float[] startRotation, float[] startGeschwindigkeit);

    public abstract RotationsAchse[] GetAchsen();


    protected void BerechneAbwurfgeschwindigkeit()
    {
        AbwurfGeschwindigkeitVector3 = (AbwurfPosition.position - letztePosition) / Time.fixedDeltaTime;
        letztePosition = AbwurfPosition.position;
        //abwurfgeschwindigkeitVector3 = achse[anzahlAchsen - 1].GetSpeed();
    }
    protected float BerechneAbwurfwinkel()
    {
        return Mathf.Rad2Deg * Mathf.Asin((AbwurfGeschwindigkeitVector3.y) / AbwurfGeschwindigkeitVector3.magnitude);
    }


}
