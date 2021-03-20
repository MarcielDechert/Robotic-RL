using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// globaler Zustand für die Rotaionsrichtung der Achsen
public enum RotationsRichtung { Neutral = 0, Positiv = 1, Negativ = -1 };

/// <summary>
/// Klasse um die Rotationen der Achsen zu realisieren
/// </summary>
public class RotationsAchse : MonoBehaviour
{
    [SerializeField] private float achsenGeschwindigkeit = 30.0f;    // Default Achsengeschwindigkeit => Grad/Sekunde
    private float sollRotation;
    private ArticulationBody articulation;

    private float annaehrungsGeschwindigkeit = 2.0f;                 // Grad/Sekunde => Toleranzbereich wird mit der Geschwindigkeit angefahren => nur für Startpositionsausrichtung
    private float toleranz = 5.0f;                                   // Bereich, wo die genaue Annährung der Sollstellung einsetzt => in Grad
    private float hoheBerechungsrate = 0.01f;                        // Hohe Berechnungen pro Sekunde => empfohlen 0.005 - 0.01 
    private float niedrigeBerechnungsrate = 0.01f;                   // Niedrige Berechnungen pro Sekunde  => empfohlen 0.02 - 0.01 


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
    /// Liefert die aktuelle Rotation der Achse
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
                Time.fixedDeltaTime = hoheBerechungsrate;
            }
            // wenn die Rotation der Achse den Toleranzbereich betritt
            else if (AktuelleRotationDerAchse() <= sollRotation + toleranz)
            {
                //Achsengeschwindikeit wird verlangsamt
                achsenGeschwindigkeit = annaehrungsGeschwindigkeit;
            }
            // wenn aktuelle Rotation den Sollwert erreicht
            if (AktuelleRotationDerAchse() <= sollRotation)
            {
                //verändert den Zeitintervall zwischen den Aufruf der FixedUpdate() Methode für weniger Berechnung um Leistung zu sparen
                Time.fixedDeltaTime = niedrigeBerechnungsrate;
                //stoppt Rotation
                rotationState = RotationsRichtung.Neutral;
                // Sollroation wird genau ausgerichtetet
                var drive = articulation.xDrive;
                drive.target = sollRotation;
                articulation.xDrive = drive;
            }
            else
            {
                // Zielrotatioen wird auf aktuelles Ziel gesetzt. Drehung um ein Intervall
                var drive = articulation.xDrive;
                drive.target = aktuellesZiel;
                articulation.xDrive = drive;
            }
        }
        // Vorghen wie oben nur für positive Drehrichtung
        else
        {
            if(isWirft)
            {
                Time.fixedDeltaTime = hoheBerechungsrate;
            }
            else if (AktuelleRotationDerAchse() >= sollRotation - toleranz)
            {
                achsenGeschwindigkeit = annaehrungsGeschwindigkeit;
            }
            if (AktuelleRotationDerAchse() >= sollRotation)
            {
                Time.fixedDeltaTime = niedrigeBerechnungsrate;
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
