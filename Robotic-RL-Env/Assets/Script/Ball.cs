using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Ball : MonoBehaviour
{

    [SerializeField] private Rigidbody bullet;
    [SerializeField] private GameObject curser;
    [SerializeField] private LayerMask layer;
    [SerializeField] private Transform shootPoint;

    private Camera cam;


    private float time = 0.0f;
    private bool isMoving = false;
    private bool isJumpPressed = false;


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        LaunchProjectile();
        
    }

    void LaunchProjectile()
    {
        Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(camRay, out hit, 100f, layer))
        {
            curser.SetActive(true);
            curser.transform.position = hit.point + Vector3.up * 0.1f;

            Vector3 Vo = CalculateVelocity(hit.point, shootPoint.position, 1f);

            if (Input.GetMouseButtonDown(0))
            {
                Rigidbody obj = Instantiate(bullet, shootPoint.position, Quaternion.identity);
                obj.velocity = Vo;
            }
        } else
        {
            curser.SetActive(false);
        }

    }
    /*

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(gameObject.name + " kollidiert mit " + collision.gameObject.name);
        rb.velocity = new Vector3(-15, 0, 0);
        isMoving = true;
    }
    */

    Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time)
    {
        // define the distance x and y first
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0f;

        // create a float the represent our distance
        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;

        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;

        return result;
    }
}
