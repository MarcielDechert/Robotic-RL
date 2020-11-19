using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbwurfManager : MonoBehaviour
{

    [SerializeField] private Rigidbody r_Ball;
    [SerializeField] private Text geschwindigkeitText;

    private Vector3 abwurfGeschwindigkeit;
    private Vector3 letztePosition = Vector3.zero;
    private bool abwurfpositionErreicht;

    // Start is called before the first frame update
    void Start()
    {
        abwurfpositionErreicht = true;
    }
    private void Update()
    {
        BerechneGeschwindigkeit();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            if (abwurfpositionErreicht)
            {
                //BerechneGeschwindigkeit();
                Abwurf();
                geschwindigkeitText.text = "Abwurfgeschwindigkeit: "+abwurfGeschwindigkeit.magnitude+" ms";
                //geschwindigkeitText.text = "" + Mathf.Acos(this.transform.position.y / abwurfGeschwindigkeit.magnitude);
            }
            abwurfpositionErreicht = !abwurfpositionErreicht;
        }
        
    }


    void BerechneGeschwindigkeit()
    {
        // aktuelle Geschwindigkeit des Abwurfpunktes am Greifer
        abwurfGeschwindigkeit = (this.transform.position - letztePosition) / Time.deltaTime;
        letztePosition = this.transform.position;
        Debug.Log(abwurfGeschwindigkeit);

    }

    void Abwurf()
    {
        this.transform.rotation = Quaternion.LookRotation(abwurfGeschwindigkeit);
        Rigidbody obj = Instantiate(r_Ball, this.transform.position, Quaternion.identity);
        obj.velocity = abwurfGeschwindigkeit;
    }
}
