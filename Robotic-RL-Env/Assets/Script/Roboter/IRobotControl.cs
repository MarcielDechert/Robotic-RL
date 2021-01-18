using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRobotControl
{
    void StarteAbwurf(float[] abwurfRotation, float[] abwurfGeschwindigkeit);

    void InStartposition(float[] startRotation, float[] startGeschwindigkeit);
}
