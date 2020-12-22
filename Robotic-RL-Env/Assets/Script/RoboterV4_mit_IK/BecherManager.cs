using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BecherManager : MonoBehaviour
{
    [SerializeField] private Text trefferTextfeld;

    private int punkte = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            //trefferTextfeld.text = "Punkte: " + punkte++;
            Destroy(other.gameObject);
        }
    }

    public void RandomMoveCup()
    {

    }
}
