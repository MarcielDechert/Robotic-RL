using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoboterManagerV6 : MonoBehaviour
{
    [SerializeField] private Button abwurfbutton;
    [SerializeField] private Button startbutton;

    [SerializeField] private AchseV6 j0;
    [SerializeField] private AchseV6 j1;
    [SerializeField] private AchseV6 j2;
    [SerializeField] private AchseV6 j3;
    [SerializeField] private AchseV6 j4;


    [SerializeField] private float startRotationJ0 = 0.0f;
    [SerializeField] private float startRotationJ1 = 0.0f;
    [SerializeField] private float startRotationJ2 = 0.0f;
    [SerializeField] private float startRotationJ3 = 0.0f;
    [SerializeField] private float startRotationJ4 = 0.0f;

    [SerializeField] private float abwurfwinkel = 0.0f;
    [SerializeField] private float abwurfgeschwindigkeit = 0.0f;
    [SerializeField] private float toleranzwinkel = 0.5f;

    // [SerializeField] private float speed = 0.0f;

    private float[] startRotation;

    private AchseV6[] achsen;

    private bool start;
    private bool abwurf;

    // Start is called before the first frame update
    void Start()
    {

        abwurfbutton.onClick.AddListener(AbwurfButtonGeklickt);
        startbutton.onClick.AddListener(StartButtonGeglickt);

        startRotation = new float[5];
        achsen = new AchseV6[5];
        Init();
    }
    void Init()
    {
        start = false;
        abwurf = false;

        startRotation[0] = startRotationJ0;
        startRotation[1] = startRotationJ1;
        startRotation[2] = startRotationJ2;
        startRotation[3] = startRotationJ3;
        startRotation[4] = startRotationJ4;

        achsen[0] = j0;
        achsen[1] = j1;
        achsen[2] = j2;
        achsen[3] = j3;
        achsen[4] = j4;
        // Debug.Log(StartRotationJ1);
    }

    public void AbwurfButtonGeklickt()
    {
        abwurf = !abwurf;
    }

    public void StartButtonGeglickt()
    {
        start = !start;
    }

    private void Update()
    {
        if (start)
        {

            //Debug.Log(j1.CurrentPrimaryAxisRotation());
            
            for (int i = 0; i < 5; i++)
            {
                if(startRotation[i] < 0)
                {
                    if(!(achsen[i].CurrentPrimaryAxisRotation() <= startRotation[i] + toleranzwinkel && achsen[i].CurrentPrimaryAxisRotation() >= startRotation[i] - toleranzwinkel))
                    {
                         achsen[i].rotationState = RotationDirection.Negative;


                        Debug.Log(achsen[i].CurrentPrimaryAxisRotation());
                    }
                    else
                    {
                        achsen[i].rotationState = RotationDirection.None;
                    }
                }
                else
                {
                    if (!(achsen[i].CurrentPrimaryAxisRotation() >= startRotation[i] - toleranzwinkel && achsen[i].CurrentPrimaryAxisRotation() <= startRotation[i] + toleranzwinkel))
                    {
                        achsen[i].rotationState = RotationDirection.Positive;
                    }
                    else
                    {
                        achsen[i].rotationState = RotationDirection.None;
                    }

                }
            }

           // Debug.Log(J1.CurrentPrimaryAxisRotation());
            
        }
        if (abwurf)
        {
            if (!(j2.CurrentPrimaryAxisRotation() >= - toleranzwinkel && j2.CurrentPrimaryAxisRotation() <= toleranzwinkel))
            {
                j2.speed = abwurfgeschwindigkeit;
                j2.rotationState = RotationDirection.Positive;
            }
            else
            {
                j2.rotationState = RotationDirection.None;
                abwurf = !abwurf;
            }
        }
        
    }
}
