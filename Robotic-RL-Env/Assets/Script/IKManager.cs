using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKManager : MonoBehaviour
{

    [SerializeField] private Armbewegung m_root;
    [SerializeField] private Armbewegung m_end;
    [SerializeField] private GameObject m_target;

    [SerializeField] private float m_rate = 5.0f;
    [SerializeField] private float m_threshold = 0.05f;
    [SerializeField] private int m_steps = 20;
    

    float CalculateSlope(Armbewegung _joint)
    {
        float deltaTheta = 0.01f;
        float distance1 = GetDistance(m_end.transform.position, m_target.transform.position);

        _joint.Rotate(deltaTheta);

        float distance2 = GetDistance(m_end.transform.position, m_target.transform.position);

        _joint.Rotate(-deltaTheta);

        return (distance2 - distance1) / deltaTheta;

    }


    // Update is called once per frame
    void Update()
    {
        for( int i = 0; i < m_steps; ++i)
        {
            // solange bis das Ziel mit einer gewissen Abweichung erreicht wurde
            if(GetDistance(m_end.transform.position, m_target.transform.position) > m_threshold)
            {
                Armbewegung current = m_root;

                //silange bis es keine Achse mehr zum rotieren gibt
                while(current != null)
                {
                    float slope = CalculateSlope(current);
                    // dreht die aktuelle Achse mit der berechneten Steigung mit einer Drehrate
                    current.Rotate(-slope * m_rate);

                    // holt sich die nächste Achse/Verbindungspunkt
                    current = current.GetChild();
                }
            }

        }

        
    }

    float GetDistance(Vector3 _point1, Vector3 _point2)
    {
        return Vector3.Distance(_point1, _point2);
    }
}
