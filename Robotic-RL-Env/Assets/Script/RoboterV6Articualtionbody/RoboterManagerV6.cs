using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public enum AbwurfStatus { Neutral = 0, Abwurfbereit = 1,InStartposition = 2, Wirft = 3,  Stopp= -1 };

public class RoboterManagerV6 : MonoBehaviour
{
    [SerializeField] private Button abwurfbutton;
    [SerializeField] private Button startbutton;
    [SerializeField] private Text abwurfgeschwindigkeitText;
    [SerializeField] private Text abwurfwinkelText;
    [SerializeField] private Text abwurfwinkelJ3Text;
    [SerializeField] public Text einwurfwinkelText;
    [SerializeField] private Rigidbody ball;
    [SerializeField] private Transform abwurfPosition;

    [SerializeField] private AchseV6 j1;
    [SerializeField] private AchseV6 j2;
    [SerializeField] private AchseV6 j3;
    [SerializeField] private AchseV6 j4;
    [SerializeField] private AchseV6 j5;


    [SerializeField] private float startRotationJ1 = 0.0f;
    [SerializeField] private float startRotationJ2 = 0.0f;
    [SerializeField] private float startRotationJ3 = 0.0f;
    [SerializeField] private float startRotationJ4 = 0.0f;
    [SerializeField] private float startRotationJ5 = 0.0f;

    [SerializeField] private float abwurfwinkel = 0.0f;
    [SerializeField] private float wurfgeschwindigkeit = 0.0f;
    [SerializeField] private float toleranzwinkel = 0.5f;
    [SerializeField] private int anzahlAchsen = 0;
    [SerializeField] private int WurfAchse = 3;

    private AbwurfStatus abwurfStatus = AbwurfStatus.Neutral;
    private Vector3 abwurfgeschwindigkeit;
    private Vector3 letztePosition = Vector3.zero;
    private bool abwurfSignal;

    private float[] startRotation;

    private AchseV6[] achsen;




    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    void Init()
    {

        abwurfbutton.onClick.AddListener(AbwurfButtonGeklickt);
        startbutton.onClick.AddListener(StartButtonGeglickt);

        startRotation = new float[5];
        achsen = new AchseV6[5];

        
        //abwurfpositionErreicht = false;

        startRotation[0] = startRotationJ1;
        startRotation[1] = startRotationJ2;
        startRotation[2] = startRotationJ3;
        startRotation[3] = startRotationJ4;
        startRotation[4] = startRotationJ5;

        achsen[0] = j1;
        achsen[1] = j2;
        achsen[2] = j3;
        achsen[3] = j4;
        achsen[4] = j5;
        // Debug.Log(StartRotationJ1);
        if(WurfAchse == 0)
        {
            WurfAchse = 1;
        }
        
        abwurfSignal = false;
        
    }

    private void FixedUpdate()
    {

        switch(abwurfStatus)
        {

            case AbwurfStatus.Neutral: 
                                                break;

            case AbwurfStatus.InStartposition:    
                                                abwurfSignal = false;
                                                InStartpositionFahren();
                                                abwurfStatus = AbwurfStatus.Abwurfbereit;
                                                break;

            case AbwurfStatus.Abwurfbereit:
                                                if(abwurfSignal)
                                                {
                                                    abwurfStatus = AbwurfStatus.Wirft;
                                                }
                                                break;

            case AbwurfStatus.Wirft:   
                                                abwurfSignal = false;
                                                BerechneGeschwindigkeit();
                                                AbwurfvorgangV1(); 
                                                break;

        }

    }

    public void AbwurfButtonGeklickt()
    {
        AbwurfSignalSetzen();
    }

    public void StartButtonGeglickt()
    {
        abwurfStatus = AbwurfStatus.InStartposition;
    }


    private void BerechneGeschwindigkeit()
    {
        // aktuelle Geschwindigkeit des Abwurfpunktes am Greifer
        abwurfgeschwindigkeit = (abwurfPosition.transform.position - letztePosition) / Time.fixedDeltaTime;
        letztePosition = abwurfPosition.transform.position;
        //Debug.Log(abwurfgeschwindigkeit);

    }
    
    /*
    private void Abwurf()
    {
        abwurfPosition.rotation = Quaternion.LookRotation(abwurfgeschwindigkeit);
        Rigidbody obj = Instantiate(r_Ball, abwurfPosition.position, Quaternion.identity);
        obj.velocity = abwurfgeschwindigkeit;
    }
    */

    private void AbwurfV1()
    {
        ball.MovePosition(abwurfPosition.position);
        ball.useGravity = true;
        ball.velocity =(abwurfgeschwindigkeit);
    }

    public void InStartpositionFahren()
    {
            for (int i = 0; i < anzahlAchsen; i++)
            {
                achsen[i].RotiereAchse(startRotation[i]);
            }
            
        achsen[WurfAchse-1].rotationState = RotationsRichtung.Neutral;
        abwurfgeschwindigkeitText.text = "Abwurfgeschwindigkeit: -- ms";
        abwurfwinkelText.text = "Abwurfwinkel: --  Grad";
        abwurfwinkelJ3Text.text = "AbwurfwinkelJ3: --  Grad";
        einwurfwinkelText.text = "Einwurfwinkel: --  Grad";
        ball.velocity = Vector3.zero;
        ball.useGravity = false;
    }
    private void AbwurfvorgangV1()
    {
            
            achsen[WurfAchse-1].achsengeschwindigkeit = wurfgeschwindigkeit;
            achsen[WurfAchse-1].rotationState = RotationsRichtung.Positiv;
        // ist der vorher angegebene Abwurfwinkel mit einer Toleranz erreicht wird der Abwurf des Balls gestartet
        if (achsen[WurfAchse-1].AktuelleRotationDerAchse() <= abwurfwinkel + toleranzwinkel && achsen[WurfAchse-1].AktuelleRotationDerAchse() >= abwurfwinkel - toleranzwinkel )
        {
            
            //Abwurf();
            AbwurfV1();
            abwurfgeschwindigkeitText.text = "Abwurfgeschwindigkeit: " + abwurfgeschwindigkeit.magnitude + " ms";
            abwurfwinkelJ3Text.text = "AbwurfwinkelJ2: " + achsen[WurfAchse-1].AktuelleRotationDerAchse() + " Grad";
            abwurfwinkelText.text = "Abwurfwinkel: " + BerechneAbwurfwinkel() + " Grad";
            achsen[WurfAchse-1].rotationState = RotationsRichtung.Neutral;
            abwurfStatus = AbwurfStatus.Neutral;
        }

    }

    private float BerechneAbwurfwinkel(){
        return  Mathf.Rad2Deg * Mathf.Acos(-abwurfgeschwindigkeit.x/abwurfgeschwindigkeit.magnitude);
    }
    // Startet den Ballabwurf mit der Wurfgschwindigkeit des Arms von 0-1 und dem Abwurfwinkel von 0-1
    public void StarteAbwurf(float geschwindigkeit, float winkel)
    {
        abwurfwinkel = UebersetzeWinkel(winkel);
        wurfgeschwindigkeit = UebersetzGeschwindigkeit(geschwindigkeit);
        abwurfSignal = true;
    }
    private void AbwurfSignalSetzen()
    {
       abwurfSignal = true;
    }

    // Rechnet die den Winkel von -50 bis -200 um
    private float UebersetzeWinkel(float winkel)
    {
        if (winkel < 0 || winkel > 1) return 0;
        return -1 * (winkel * 150 + 50);
    }

    // Rechnet die den Geschwindigkeit von 0 bis 500 um
    private float UebersetzGeschwindigkeit(float geschwindigkeit)
    {
        if (geschwindigkeit < 0 || geschwindigkeit > 1) return 0;
        return  geschwindigkeit * 500;
    }

    public void SetzeGeschwindikeitDerScene(float geschwindigkeit){
        Time.timeScale = geschwindigkeit;
    }


}
