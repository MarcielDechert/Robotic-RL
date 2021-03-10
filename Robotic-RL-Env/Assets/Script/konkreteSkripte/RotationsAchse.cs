using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum RotationsRichtung { Neutral = 0, Positiv = 1, Negativ = -1 };

public class RotationsAchse : MonoBehaviour
{
<<<<<<< Updated upstream
    public float achsengeschwindigkeit = 30.0f; // Grad/Sekunde
    private float sollRotation = 30.0f;
    private float toleranz = 5.0f;
    private bool wirft = false;
    public bool Wirft { get => wirft; set => wirft = value; }
=======
    [SerializeField] private float achsenGeschwindigkeit = 30.0f; // Grad/Sekunde
    private float sollRotation;
    private float toleranz = 5.0f;
    private ArticulationBody articulation;

    private bool isWirft = false;
    public bool IsWirft { get => isWirft; set => isWirft = value; }
>>>>>>> Stashed changes

    public RotationsRichtung rotationState = RotationsRichtung.Neutral;
    private ArticulationBody articulation;


    void Start()
    {
        articulation = GetComponent<ArticulationBody>();
    }

    public void FixedUpdate()
    {
        if (rotationState != RotationsRichtung.Neutral)
        {
<<<<<<< Updated upstream
            float rotationAenderung = (float)rotationState * achsengeschwindigkeit * Time.fixedDeltaTime;
            float rotationZiel = AktuelleRotationDerAchse() + rotationAenderung;
            RotiereAchse(rotationZiel, sollRotation);
=======
            // Roationsänderung nach Zeitintervall abhängig
            float rotationAenderung = (float)rotationState * achsenGeschwindigkeit * Time.fixedDeltaTime;
            float aktuellesRotationziel = AktuelleRotationDerAchse() + rotationAenderung;
            RotiereAchse(aktuellesRotationziel, sollRotation);
>>>>>>> Stashed changes
        }

    }

    public float AktuelleRotationDerAchse()
    {
        
        // float currentRotationRads = articulation.jointPosition[0];
        // float currentRotation = Mathf.Rad2Deg * currentRotationRads;
        // return currentRotation;
        return articulation.xDrive.target;
    }

    // Rotiert um xx Grad um die x Achse
    private void RotiereAchse(float zielRotation, float sollRotation)
    {

        if (rotationState == RotationsRichtung.Negativ)
        {
<<<<<<< Updated upstream
            if(wirft)
            { 
                Time.fixedDeltaTime = 0.005f;
=======
            // wenn geworfen wird
            if(isWirft)
            { 
                //verändert den Zeitintervall zwischen den Aufruf der FixedUpdate() Methode beim Werfen, um genauer den Abwurfpunkt anzufahren => mehr Berechnugen
                Time.fixedDeltaTime = 0.001f;
>>>>>>> Stashed changes
            }
            else if (AktuelleRotationDerAchse() <= sollRotation + toleranz)
            {
<<<<<<< Updated upstream
                //achsengeschwindigkeit = (Mathf.Abs(sollRotation-AktuelleRotationDerAchse())* 100 /toleranz) + 0.001f;
                achsengeschwindigkeit = 2.0f;
=======
                //Achsengeschwindikeit wird verlangsamt
                achsenGeschwindigkeit = 2.0f;
>>>>>>> Stashed changes
            }
            if (AktuelleRotationDerAchse() <= sollRotation)
            {
<<<<<<< Updated upstream
                Time.fixedDeltaTime = 0.02f;
=======
                //verändert den Zeitintervall zwischen den Aufruf der FixedUpdate() Methode für weniger Berechnung um Leistung zu sparen
                Time.fixedDeltaTime = 0.01f;
                //stoppt Rotation
>>>>>>> Stashed changes
                rotationState = RotationsRichtung.Neutral;
                var drive = articulation.xDrive;
                drive.target = sollRotation;
                articulation.xDrive = drive;
            }
            else
            {
                var drive = articulation.xDrive;
                drive.target = zielRotation;
                articulation.xDrive = drive;
            }
        }
        else
        {
            if(isWirft)
            {
                Time.fixedDeltaTime = 0.005f;
            }
            else if (AktuelleRotationDerAchse() >= sollRotation - toleranz)
            {
<<<<<<< Updated upstream
                //achsengeschwindigkeit = (Mathf.Abs(sollRotation-AktuelleRotationDerAchse())* 100 /toleranz) + 0.001f;
                achsengeschwindigkeit = 2.0f;
            }
            if (AktuelleRotationDerAchse() >= sollRotation)
            {
                Time.fixedDeltaTime = 0.02f;
=======
                achsenGeschwindigkeit = 2.0f;
            }
            if (AktuelleRotationDerAchse() >= sollRotation)
            {
                Time.fixedDeltaTime = 0.01f;
>>>>>>> Stashed changes
                rotationState = RotationsRichtung.Neutral;
                var drive = articulation.xDrive;
                drive.target = sollRotation;
                articulation.xDrive = drive;
            }
            else
            {
                var drive = articulation.xDrive;
                drive.target = zielRotation;
                articulation.xDrive = drive;
            }

        }
    }

    public void RotiereSofort(float sollWinkel)
    {
        var drive = articulation.xDrive;
        drive.target = sollWinkel;
        articulation.xDrive = drive;
    }

    // Rotiert um xx Grad um die x Achse
    public void RotiereAchseBis(float sollRotationsZiel, float sollRotaionsGeschwindigkeit)
    {
        achsenGeschwindigkeit = sollRotaionsGeschwindigkeit;
        sollRotation = sollRotationsZiel;
        if (AktuelleRotationDerAchse() - sollRotationsZiel < 0)
        {
            rotationState = RotationsRichtung.Positiv;
        }
        else
        {
            rotationState = RotationsRichtung.Negativ;
        }

    }

    public Vector3 GetSpeed()
    {
        return articulation.velocity;
    }
}
