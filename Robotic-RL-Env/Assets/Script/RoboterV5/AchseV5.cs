using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchseV5 : MonoBehaviour
{

    public AchseV5 GetAngle()
    {
        return this;
    }


    public void Rotate(float rx, float ry, float rz)
    {
        Vector3 p = new Vector3(rx, ry, rz);
        // transform.Rotate(p, 45 * Time.deltaTime); rotiert mit 45 Grad pro Sekunde
        transform.Rotate(p);
    }
}
