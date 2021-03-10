using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// globaler Zustand für die Rotaionsrichtung der Achsen
public enum RotationsRichtung { Neutral = 0, Positiv = 1, Negativ = -1 };

/// <summary>
/// Enthält Mehtoden damit sich die Achsen rotieren lassen können
/// </summary>
public class RotationsAchse : MonoBehaviour
{
    [SerializeField] private float achsenGeschwindigkeit = 30.0f; // Grad/Sekunde
    private float sollRotation;
    private float toleranz = 5.0f;
    private ArticulationBody articulation;
    private bool isWirft = false;
    public bool IsWirft { get => isWirft; set => isWirft = value; }

    private RotationsRichtung rotationState = RotationsRichtung.Neutral;
    public RotationsRichtung RotationState { get => rotationState; set => rotationState = value; }

    /// <summary>
    /// Wird beim Start einmalig aufgerufen. Initialisiert Attribute
    /// </summary>
    void Start()
    {
        articulation = GetComponent<ArticulationBody>();
    }

    /// <summary>
    /// Wird einmal im Frame aufgerufen. Steuert die Rotation der Achsen
    /// </summary>
    protected void FixedUpdate()
    {
        // wenn die RotationsRichtung ungleich Neutral ist 
        if (rotationState != RotationsRichtung.Neutral)
        {
            // Roationsänderung nach Zeitintervall abhängig
            float rotationAenderung = (float)rotationState * achsenGeschwindigkeit * Time.fixedDeltaTime;
            float aktuellesRotationziel = AktuelleRotationDerAchse() + rotationAenderung;
            RotiereAchse(aktuellesRotationziel, sollRotation);
        }
    }


    /// <summary>
    /// Liefert den aktuelle Rotation der Achse
    /// </summary>
    /// <returns> Grad in float</returns>
    public float AktuelleRotationDerAchse()
    {
        return articulation.xDrive.target;
    }

    /// <summary>
    /// Rotiert Achse bis zum vorgegebenen Wert
    /// </summary>
    /// <param name="aktuellesZiel"> aktuelles Ziel. Je nach Zeitintervall abhängig</param>
    /// <param name="sollRotation"> Ziel der Endrotation</param>
    private void RotiereAchse(float aktuellesZiel, float sollRotation)
    {
        // wenn die Rotationsrichtung Negativ ist
        if (rotationState == RotationsRichtung.Negativ)
        {
            // wenn geworfen wird
            if(isWirft)
            { 
                //verändert den Zeitintervall zwischen den Aufruf der FixedUpdate() Methode beim Werfen, um genauer den Abwurfpunkt anzufahren => mehr Berechnugen
                Time.fixedDeltaTime = 0.01f;
            }
            // wenn die Rotation der Achse den Toleranzbereich betritt
            else if (AktuelleRotationDerAchse() <= sollRotation + toleranz)
            {
                //Achsengeschwindikeit wird verlangsamt
                achsenGeschwindigkeit = 2.0f;
            }
            // wenn aktuelle Rotation den Sollwert erreicht
            if (AktuelleRotationDerAchse() <= sollRotation)
            {
                //verändert den Zeitintervall zwischen den Aufruf der FixedUpdate() Methode für weniger Berechnung um Leistung zu sparen
                Time.fixedDeltaTime = 0.01f;
                //stoppt Rotation
                rotationState = RotationsRichtung.Neutral;
                // Sollroation wird genau ausgerichtetet
                var drive = articulation.xDrive;
                drive.target = sollRotation;
                articulation.xDrive = drive;
            }
            else
            {
                // Zielroatioen wird auf aktuelles Ziel gesezt. Drehung um ein Intervall
                var drive = articulation.xDrive;
                drive.target = aktuellesZiel;
                articulation.xDrive = drive;
            }
        }
        else
        {
            if(isWirft)
            {
                Time.fixedDeltaTime = 0.01f;
            }
            else if (AktuelleRotationDerAchse() >= sollRotation - toleranz)
            {
                achsenGeschwindigkeit = 2.0f;
            }
            if (AktuelleRotationDerAchse() >= sollRotation)
            {
                Time.fixedDeltaTime = 0.01f;
                rotationState = RotationsRichtung.Neutral;
                var drive = articulation.xDrive;
                drive.target = sollRotation;
                articulation.xDrive = drive;
            }
            else
            {
                var drive = articulation.xDrive;
                drive.target = aktuellesZiel;
                articulation.xDrive = drive;
            }
        }
    }

    /// <summary>
    /// Rotiert Achse sofort zum angegbenen Ziel
    /// </summary>
    /// <param name="sollWinkel"> Sollrotation in Grad</param>
    public void RotiereSofort(float sollWinkel)
    {
        var drive = articulation.xDrive;
        drive.target = sollWinkel;
        articulation.xDrive = drive;
    }

    /// <summary>
    /// Setzt Sollrotationen und Sollgeschwindigkeiten und ändert die Rotationsrichtungen in Abhängikeit der Sollrotation
    /// </summary>
    /// <param name="sollRotationsZiel"> Sollrotation in Grad</param>
    /// <param name="sollRotaionsGeschwindigkeit">Sollrotationsgeschwindigkeit in Grad/s</param>
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
}
