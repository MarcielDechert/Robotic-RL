using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armbewegung : MonoBehaviour
{
    public Armbewegung m_child;

    public Armbewegung GetChild()
    {
        return m_child;
    }

    public void Rotate(float _angle)
    {
        transform.Rotate(Vector3.up * _angle);
    }
}
