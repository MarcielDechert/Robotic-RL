using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stellt zwei Methoden bereit für die Fahrt in die Abwurfposition und Startposition
/// </summary>
public interface IRobotControl
{
    void StarteAbwurf(float[] abwurfRotation, float[] abwurfGeschwindigkeit);

    void InStartposition(float[] startRotation, float[] startGeschwindigkeit);
}
