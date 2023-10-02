using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quaternionsPractice : MonoBehaviour
{
    public Transform target;
    public bool FrontFace = false;
    public Quaternion rotationTarget;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rayOrigin = transform.position;
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, transform.forward, out hit, 10))
        {
            // Obtén la normal de la superficie impactada.
            Vector3 surfaceNormal = hit.normal;
            // Transforma la normal local a valores mundiales.
            Vector3 globalNormal = hit.collider.transform.TransformDirection(surfaceNormal);
            // Define un umbral para determinar la orientación.
            float threshold = 0.0f; // Puedes ajustar este valor según tus necesidades.
                                    // Comprueba si la normal global está cerca de la dirección hacia adelante del mundo.
            
            if (Vector3.Dot(globalNormal, Vector3.forward) >= threshold)
            {
                FrontFace=true;
                rotationTarget = (Quaternion.FromToRotation(transform.forward, hit.collider.transform.forward)) * Quaternion.Euler(0, 125, 0);
            }
            else
            {
                FrontFace = false; ;
                rotationTarget = (Quaternion.FromToRotation(transform.forward, hit.collider.transform.forward)) * Quaternion.Euler(0, 125, 0);
            }

            Debug.DrawLine(rayOrigin, hit.point, Color.blue);
        }
        else
        {
            Debug.DrawLine(rayOrigin, transform.position + transform.forward * 10, Color.red);
        }
    }
}
