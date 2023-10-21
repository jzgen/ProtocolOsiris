using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveAround : MonoBehaviour
{
    public Transform center; // El centro alrededor del cual rotará el objeto.
    public float rotationSpeed = 30.0f; // Velocidad de rotación en grados por segundo.

    private Vector3 rotationAxis; // Eje de rotación.

    private void Start()
    {
        if (center == null)
        {
            Debug.LogError("El centro no está asignado. Asigna un Transform al campo 'Center' en el Inspector.");
            enabled = false; // Deshabilita el script si el centro no está asignado.
            return;
        }
    }

    private void Update()
    {
        // Rota el objeto alrededor del centro.
        transform.RotateAround(center.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
