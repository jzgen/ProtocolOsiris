using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class CoverDetection : MonoBehaviour
{
	[Header("Cover Detection")]
	public LayerMask coverMask;
	public float radius = 5f;
	public Collider[] covers;
	public List<Vector3> coverPoints;

	[Header("References")]
	public Transform player;

	//Cover Point Generation
	private float pointSpacing = 1.0f;

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			SearchCovers();
		}
	}
	void SearchCovers()
	{
		covers = Physics.OverlapSphere(transform.position, radius, coverMask);

		if (covers.Length > 0)
		{
			foreach (Collider collider in covers)
			{
				Vector3 upDirection = collider.transform.position + (Vector3.up * 2);
				Debug.DrawLine(collider.transform.position, upDirection, Color.yellow);
			}

			coverPoints = GenerateCoverPoints();
		}
	}
	public List<Vector3> GenerateCoverPoints()
	{
		List<Vector3> coverPoints = new List<Vector3>();

		foreach (Collider coverCollider in covers)
		{
			// Obtenemos el centro del collider y su rotación en el eje Y
			Vector3 colliderCenter = coverCollider.bounds.center;
			Quaternion colliderRotation = Quaternion.Euler(0, coverCollider.transform.eulerAngles.y, 0);

			//Obtenemos la medida de box collider
			Vector3 coverSize = CoverSizeToWorld(coverCollider as BoxCollider);
			int gridSizeX = Mathf.CeilToInt(coverSize.x + 1.1f);
			int gridSizeZ = Mathf.CeilToInt(coverSize.z + 1.1f);

			// Calculamos el punto de inicio del grid para que esté centrado en el collider.
			Vector3 gridOrigin = colliderCenter - new Vector3((gridSizeX - 1) * pointSpacing / 2, 0, (gridSizeZ - 1) * pointSpacing / 2);

			for (int x = 0; x < gridSizeX; x++)
			{
				for (int z = 0; z < gridSizeZ; z++)
				{
					// Calculamos la posición del punto en el espacio local del grid
					Vector3 pointLocal = new Vector3(x * pointSpacing, 0, z * pointSpacing);

					// Transformamos el punto al espacio mundial sin aplicar rotación
					Vector3 pointWorld = gridOrigin + pointLocal;

					// Aplicamos la rotación del collider al punto
					pointWorld = colliderCenter + colliderRotation * (pointWorld - colliderCenter);

					// Verificamos si el punto está obstruido
					if (!Physics.CheckSphere(pointWorld, 0.1f, coverMask))
					{
						coverPoints.Add(pointWorld);
					}
				}
			}
		}

		return coverPoints;
	}
	Vector3 CoverSizeToWorld(BoxCollider boxCollider)
	{
		Vector3 worldScale = boxCollider.transform.lossyScale;
		float width = RoundToDecimal(boxCollider.size.x * worldScale.x, 3);
		float length = RoundToDecimal(boxCollider.size.z * worldScale.z, 3);
		float height = RoundToDecimal(boxCollider.size.y * worldScale.y, 3);
		return new Vector3(width, height, length);
	}
	float RoundToDecimal(float value, int decimalPlaces)
	{
		float multiplier = Mathf.Pow(10f, decimalPlaces);
		return Mathf.Round(value * multiplier) / multiplier;
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position, radius);

		if (covers.Length > 0)
		{
			foreach (Vector3 point in coverPoints)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(point, 0.1f); // Dibuja una esfera en cada punto de cobertura
			}

			foreach (BoxCollider boxCollider in covers)
			{
				Vector3 size = CoverSizeToWorld(boxCollider);
				Gizmos.matrix = Matrix4x4.TRS(boxCollider.transform.position, boxCollider.transform.rotation, Vector3.one * 1.001f);
				Gizmos.color = new Color(1, 0, 1, 0.5f);
				Gizmos.DrawCube(Vector3.zero, size); // Dibuja en el origen porque la matriz ya tiene en cuenta el centro
				Gizmos.color = Color.magenta;
				Gizmos.DrawWireCube(Vector3.zero, size);
			}
		}
		// Restablece la matriz de Gizmos a la identidad después de dibujar
		Gizmos.matrix = Matrix4x4.identity;
	}
}