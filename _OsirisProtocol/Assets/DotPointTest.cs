using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotPointTest : MonoBehaviour
{
	[Header("References")]
	public Transform player;
	public Transform coverPos;
	public Vector3 offset;

	[Header("View Settings")]
	public float fieldOfView;
	public float viewDistance;

	[Header("Results")]
	public int _visibilityPercentage;
	public bool isVisible;

	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		EvaluateCovePositions();
	}

	void EvaluateCovePositions()
	{
		_visibilityPercentage = VisibiltyPercentage(coverPos.position, player);
	}
	int VisibiltyPercentage(Vector3 position, Transform player)
	{
		// Calcular la dirección de la cobertura desde el jugador.
		Vector3 directionToCover = (position - player.position).normalized;

		// Calcular el ángulo del campo de visión en radianes.
		float halfFieldOfViewRadians = fieldOfView * 0.5f * Mathf.Deg2Rad;

		// Calcular el coseno de la mitad del campo de visión.
		float cosHalfFieldOfView = Mathf.Cos(halfFieldOfViewRadians);

		// Calcular el producto punto (coseno del ángulo) entre la dirección a la que el jugador está mirando y la dirección hacia la cobertura.
		float dotProduct = Vector3.Dot(player.transform.forward, directionToCover);

		// Asegurarse de que el porcentaje no sea negativo.
		float percentage = Mathf.Max((dotProduct - cosHalfFieldOfView) / (1 - cosHalfFieldOfView), 0f) * 100;

		// Retornar el porcentaje, que será al menos 0.
		return Mathf.CeilToInt(percentage);
	}

	void OnDrawGizmos()
	{
		if (player != null)
		{
			Gizmos.matrix = Matrix4x4.TRS(player.position, player.rotation * Quaternion.Euler(offset), player.localScale);
			Gizmos.color = Color.blue;
			Vector3 center = player.position;
			Gizmos.DrawFrustum(center, fieldOfView, viewDistance, 0.001f, 0f);
		}
	}
}
