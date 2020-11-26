using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoboterManagerV5 : MonoBehaviour
{


    [SerializeField] private Button Abwurfbutton;
    [SerializeField] private Button Startbutton;

    [SerializeField] private AchseV5 J0;
    [SerializeField] private AchseV5 J1;
    [SerializeField] private AchseV5 J2;
    [SerializeField] private AchseV5 J3;
    [SerializeField] private AchseV5 J4;


    [SerializeField] private float StartRotationJ0 = 0.0f;
    [SerializeField] private float StartRotationJ1 = 0.0f;
    [SerializeField] private float StartRotationJ2 = 0.0f;
    [SerializeField] private float StartRotationJ3 = 0.0f;
    [SerializeField] private float StartRotationJ4 = 0.0f;
    
    private float[] StartRotation;

    private AchseV5[] Achsen;

    // Start is called before the first frame update
    void Start()
    {

        Abwurfbutton.onClick.AddListener(AbwurfButtonGeklickt);
        Startbutton.onClick.AddListener(StartButtonGeglickt);

        StartRotation = new float[5];
        Achsen = new AchseV5[5];
        Init();
    }
    void Init()
    {
        StartRotation[0] = StartRotationJ0;
        StartRotation[1] = StartRotationJ1;
        StartRotation[2] = StartRotationJ2;
        StartRotation[3] = StartRotationJ3;
        StartRotation[4] = StartRotationJ4;

        Achsen[0] = J0;
        Achsen[1] = J1;
        Achsen[2] = J2;
        Achsen[3] = J3;
        Achsen[4] = J4;
       // Debug.Log(StartRotationJ1);
    }

    public void AbwurfButtonGeklickt()
    {
    }

    public void StartButtonGeglickt()
    {

        for (int i = 0; i < 5; i++)
        {
            Achsen[i].Rotate(0, StartRotation[i], 0);
        }

        Debug.Log(J3.transform.rotation.eulerAngles.y);

    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log(J3.transform.localEulerAngles.y);

    }
}
