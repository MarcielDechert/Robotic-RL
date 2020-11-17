using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abwurfpunkt : MonoBehaviour
{

    [SerializeField] private Rigidbody r_Ball;

    private Vector3 abwurfGeschwindigkeit;
    private Vector3 letztePosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Update()
    {
        BerechneGeschwindigkeit();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            Abwurf();
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
