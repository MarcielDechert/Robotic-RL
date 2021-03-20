using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stellt zwei Methoden für die Fahrt in die Abwurfposition und Startposition bereit 
/// </summary>
public interface IRobotControl
{
    void StarteAbwurf(float[] abwurfRotation, float[] abwurfGeschwindigkeit);

    void InStartposition(float[] startRotation, float[] startGeschwindigkeit);
}
