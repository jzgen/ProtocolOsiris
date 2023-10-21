using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveAround : MonoBehaviour
{
    public Transform center; // El centro alrededor del cual rotar� el objeto.
    public float rotationSpeed = 30.0f; // Velocidad de rotaci�n en grados por segundo.

    private Vector3 rotationAxis; // Eje de rotaci�n.

    private void Start()
    {
        if (center == null)
        {
            Debug.LogError("El centro no est� asignado. Asigna un Transform al campo 'Center' en el Inspector.");
            enabled = false; // Deshabilita el script si el centro no est� asignado.
            return;
        }
    }

    private void Update()
    {
        // Rota el objeto alrededor del centro.
        transform.RotateAround(center.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
