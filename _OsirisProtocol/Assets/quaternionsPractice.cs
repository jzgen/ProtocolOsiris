using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quaternionsPractice : MonoBehaviour
{
    public Transform objeto1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Calcula la direcci�n "forward" de objetoB en el espacio mundial
        Vector3 forwardDirection = objeto1.forward;

        // Calcula la rotaci�n necesaria para alinear objetoA con forwardDirection
        Quaternion rotation = Quaternion.LookRotation(forwardDirection);

        // Aplica la rotaci�n a objetoA
        transform.rotation = rotation;
    }
}
