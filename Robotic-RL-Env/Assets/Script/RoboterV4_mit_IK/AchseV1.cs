using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchseV1 : MonoBehaviour
{
    [SerializeField] private AchseV1 m_child;
    private float Winkel = 0;

    public AchseV1 GetChild()
    {
        return m_child;
    }


    public void Rotate(float rx, float ry , float rz)
    {
        Vector3 p = new Vector3(rx, ry, rz);

       // transform.Rotate(p, 45 * Time.deltaTime); rotiert mit 45 Grad pro Sekunde
        transform.Rotate(p);
    }

    public float getRotation()
    {

        //float Winkel = transform.localEulerAngles.normalized.z *(180/Mathf.PI);
        return this.Winkel;
    }

    public void setRotation(float winkel)
    {
        Winkel += winkel;
        if (Winkel >= 360)
        {
            Winkel -= 360;
        }
        else if(Winkel <= 0)
        {
            Winkel += 360;
        }
    }
}
