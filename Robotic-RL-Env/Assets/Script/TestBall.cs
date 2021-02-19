using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBall : MonoBehaviour
{
    private double luftwiderstand;
    private double luftwiderstandfx;
    private double luftwiderstandfy;
    private double flaeche;
    private double reynoldzahl;
    private double reynoldzahlKrit = 4852.0;
    private double cw;
    private double ballDurchmesser;
    private double luftDichte = 1.2041;
    private double viskoseLuft = 0.0000182;
    private Vector3 luftwiderstandV3;
    private double alpha = 0.0;
    // Start is called before the first frame update
    void Start()
    {
        flaeche = BerechneBallFlaeche(transform.localScale.y);
        ballDurchmesser = transform.localScale.y;
        this.GetComponent<Rigidbody>().useGravity = true;
        this.GetComponent<Rigidbody>().velocity = new Vector3(1.729f,0.0f,0.0f);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Luft();
        
    }

    private void Luft()
    {
        if (GetComponent<Rigidbody>().velocity.magnitude != 0)
        {
            reynoldzahl = BerechneReynoldszahl(luftDichte, GetComponent<Rigidbody>().velocity.magnitude, ballDurchmesser, viskoseLuft);

            if (reynoldzahl >= reynoldzahlKrit)
            {

                cw = BerechneCwWert(reynoldzahl);
                luftwiderstand = BerechneLuftwiderstandTurbulent(cw, flaeche, luftDichte, GetComponent<Rigidbody>().velocity.magnitude);
                Debug.Log(GetComponent<Rigidbody>().transform.position);
                // Debug.Log("Turbulent");
            }
            else
            {
                luftwiderstand = BerechneLuftwiderstandLaminar(viskoseLuft, ballDurchmesser, GetComponent<Rigidbody>().velocity.magnitude);
                //Debug.Log("Fy" + luftwiderstandfy);
                //Debug.Log("Laminar");

            }
            //luftwiderstand *= 4.7f;
            luftwiderstandfx = -(luftwiderstand * GetComponent<Rigidbody>().velocity.x / GetComponent<Rigidbody>().velocity.magnitude);

            luftwiderstandfy = -(luftwiderstand * GetComponent<Rigidbody>().velocity.y / GetComponent<Rigidbody>().velocity.magnitude);
            Debug.Log(GetComponent<Rigidbody>().velocity);
            // Debug.Log("V_Betrag:"+Ballgeschwindigkeit.magnitude);
            // Debug.Log("Reynold:" + reynoldzahl);
            //Debug.Log("F_Betrag:" + luftwiderstand);
            //Debug.Log("X:" + GetComponent<Rigidbody>().transform.position.x);
            //Debug.Log("Fx:" + luftwiderstandfx);
            //Debug.Log("Fy" + luftwiderstandfy);


            //  luftwiderstandV3 = new Vector3( (float) luftwiderstandfx, (float) luftwiderstandfy, 0);


            GetComponent<Rigidbody>().AddForce((float)luftwiderstandfx, (float)luftwiderstandfy, 0);

        }

    }

    private double BerechneLuftwiderstandTurbulent(double _cw, double _flaeche, double _luftDichte, double _geschwindigkeit)
    {
        return 0.5 * _cw * _flaeche * _luftDichte * Mathf.Pow((float)_geschwindigkeit, 2);
    }

    private double BerechneLuftwiderstandLaminar(double _viskoseLuft, double _ballDurchmesser, double _geschwindigkeit)
    {
        return 6.0 * Mathf.PI * _viskoseLuft * _ballDurchmesser / 2 * _geschwindigkeit;
    }

    private double BerechneBallFlaeche(double _durchmesser)
    {
        return Mathf.Pow((float)_durchmesser, 2) / 4 * Mathf.PI;
    }

    private double BerechneReynoldszahl(double _luftDichte, double _geschwindigkeit, double _durchmesser, double _viskoseLuft)
    {
        return _luftDichte * _geschwindigkeit * _durchmesser / _viskoseLuft;
    }

    private double BerechneCwWert(double _reynoldzahl)
    {
        return 24 / _reynoldzahl + 4 / Mathf.Sqrt((float)_reynoldzahl) + 0.4;
    }

    protected void OnCollisionEnter(Collision other)
    {


        this.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
        this.GetComponent<Rigidbody>().useGravity = false;
        Debug.Log(this.transform.position);

    }

}

