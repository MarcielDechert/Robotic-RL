using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stellt eine Methode bereit die für die Synchronisation zuständig sein soll
/// </summary>
public interface IStep
{
    void Step();
}
