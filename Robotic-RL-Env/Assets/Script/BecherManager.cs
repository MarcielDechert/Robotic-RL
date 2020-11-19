using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BecherManager : MonoBehaviour
{
    [SerializeField] private Text trefferTextfeld;

    private int punkte = 0;

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            trefferTextfeld.text = "Punkte: " + punkte++;
            Destroy(other.gameObject);
        }
    }
}
