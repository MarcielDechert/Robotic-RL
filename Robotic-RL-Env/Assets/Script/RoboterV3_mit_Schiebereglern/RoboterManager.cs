using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoboterManager : MonoBehaviour
{
    [SerializeField] private Achse j0;
    [SerializeField] private Slider s0;
    [SerializeField] private Achse j1;
    [SerializeField] private Slider s1;
    [SerializeField] private Achse j2;
    [SerializeField] private Slider s2;
    [SerializeField] private Achse j3;
    [SerializeField] private Slider s3;
    [SerializeField] private Achse j4;
    [SerializeField] private Slider s4;


    private float diffJ0, diffJ1, diffJ2, diffJ3, diffJ4;
    private float aktuellerWinkelJ0, aktuellerWinkelJ1, aktuellerWinkelJ2, aktuellerWinkelJ3, aktuellerWinkelJ4;

    private void Start()
    {
       aktuellerWinkelJ0 = 0f;
       aktuellerWinkelJ1 = 0f;
       aktuellerWinkelJ2 = 0f;
       aktuellerWinkelJ3 = 0f;
       aktuellerWinkelJ4 = 0f;
       diffJ0 = 0f;
       diffJ1 = 0f;
       diffJ2 = 0f;
       diffJ3 = 0f;
       diffJ4 = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        ManageJ0();
        ManageJ1();
        ManageJ2();
        ManageJ3();
        ManageJ4();
    }

    private void ManageJ0()
    {
        diffJ0 = s0.value - aktuellerWinkelJ0;
        aktuellerWinkelJ0 = s0.value;

        if(diffJ0 != 0)
        {
            j0.Rotate(diffJ0);
        }
    }

    private void ManageJ1()
    {
        diffJ1 = s1.value - aktuellerWinkelJ1;
        aktuellerWinkelJ1 = s1.value;

        if (diffJ1 != 0)
        {
            j1.Rotate(diffJ1);
        }
    }

    private void ManageJ2()
    {
        diffJ2 = s2.value - aktuellerWinkelJ2;
        aktuellerWinkelJ2 = s2.value;

        if (diffJ2 != 0)
        {
            j2.Rotate(-diffJ2);
        }
    }

    private void ManageJ3()
    {
        diffJ3 = s3.value - aktuellerWinkelJ3;
        aktuellerWinkelJ3 = s3.value;

        if (diffJ3 != 0)
        {
            j3.Rotate(diffJ3);
        }
    }

    private void ManageJ4()
    {
        diffJ4 = s4.value - aktuellerWinkelJ4;
        aktuellerWinkelJ4 = s4.value;

        if (diffJ4 != 0)
        {
            j4.Rotate(-diffJ4);
        }
    }
}
