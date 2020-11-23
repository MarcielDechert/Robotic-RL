using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achse : MonoBehaviour
{
   

    public void Rotate(float _angle)
    {
        transform.Rotate(Vector3.up * _angle);
    }
}
