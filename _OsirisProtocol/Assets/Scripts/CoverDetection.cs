using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The CoverDetection class is responsible for detecting potential cover points around objects
public class CoverDetection : MonoBehaviour
{
	[Header("Cover Detection")]
	public LayerMask coverMask;
	public float radius = 5f;
	public Collider[] covers;

	[Header("Cover Points")]
	public float pointSpacing = 1.25f;
	public List<CoverPoint> coverPoints = new List<CoverPoint>();

	[Header("References")]
	public Transform player;

	private void Update()
	{
		SearchCovers();
	}
	void SearchCovers()
	{
		covers = Physics.OverlapSphere(transform.position, radius, coverMask);

		coverPoints.Clear();
		if (covers.Length > 0 && pointSpacing > 0)
		{
			coverPoints = GenerateCoverPoints();
			AssignPotentialCoversPoints();
		}
	}
	public List<CoverPoint> GenerateCoverPoints()
	{
		List<CoverPoint> coverPoints = new List<CoverPoint>();

		foreach (Collider coverCollider in covers)
		{
			// Obtenemos el centro del collider y su rotación en el eje Y
			Vector3 colliderCenter = coverCollider.bounds.center;
			Quaternion colliderRotation = Quaternion.Euler(0, coverCollider.transform.eulerAngles.y, 0);

			//Obtenemos la medida de box collider
			Vector3 coverSize = CoverSizeToWorld(coverCollider as BoxCollider);
			int gridSizeX = Mathf.CeilToInt(((coverSize.x + 1f) / pointSpacing) + 0.5f);
			int gridSizeZ = Mathf.CeilToInt(((coverSize.z + 1f) / pointSpacing) + 0.5f);
			float gridHeight = coverSize.y / 2;

			// Calculamos el punto de inicio del grid para que esté centrado en el collider.
			Vector3 gridOrigin = colliderCenter - new Vector3((gridSizeX - 1) * pointSpacing / 2, gridHeight, (gridSizeZ - 1) * pointSpacing / 2);

			for (int x = 0; x < gridSizeX; x++)
			{
				for (int z = 0; z < gridSizeZ; z++)
				{
					// Comprueba si el punto está en una esquina y omítelo
					bool isCorner = (x == 0 && z == 0) || // Esquina inferior izquierda
									(x == gridSizeX - 1 && z == 0) || // Esquina inferior derecha
									(x == 0 && z == gridSizeZ - 1) || // Esquina superior izquierda
									(x == gridSizeX - 1 && z == gridSizeZ - 1); // Esquina superior derecha

					if (!isCorner)
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
							float lineOfSightValue = LineOfSight(pointWorld, coverCollider);
							coverPoints.Add(new CoverPoint(pointWorld, coverCollider, lineOfSightValue));
						}
					}
				}
			}
		}

		return coverPoints;
	}
	float RoundToDecimal(float value, int decimalPlaces)
	{
		float multiplier = Mathf.Pow(10f, decimalPlaces);
		return Mathf.Round(value * multiplier) / multiplier;
	}
	Vector3 CoverSizeToWorld(BoxCollider boxCollider)
	{
		Vector3 worldScale = boxCollider.transform.lossyScale;
		float width = RoundToDecimal(boxCollider.size.x * worldScale.x, 3);
		float length = RoundToDecimal(boxCollider.size.z * worldScale.z, 3);
		float height = RoundToDecimal(boxCollider.size.y * worldScale.y, 3);
		return new Vector3(width, height, length);
	}
	float LineOfSight(Vector3 coverPoint, Collider cover)
	{
		Vector3 coverToPlayer = (player.transform.position - coverPoint).normalized;
		Vector3 coverToPoint = (coverPoint - cover.transform.position).normalized;
		float dotProduct = Vector3.Dot(coverToPoint, coverToPlayer);
		return dotProduct;
	}
	private void AssignPotentialCoversPoints()
	{
		foreach (CoverPoint point in coverPoints)
		{
			Vector3 pointToPlayer = (player.transform.position - point.position).normalized;
			float dotProduct = LineOfSight(point.position, point.coverCollider);
			float distance = Vector3.Distance(point.position, player.transform.position);

			Color lineColor;

			if (dotProduct < -0.1f)
			{
				lineColor = Color.yellow;

				if (Physics.Raycast(point.position, pointToPlayer, out RaycastHit hit, distance))
				{
					lineColor = Color.blue;
				}
			}
			else
			{
				lineColor = Color.white;
			}

			// Cambia el color de la línea basado en si el punto está detrás de la cobertura o no
			Debug.DrawLine(point.position, point.position + Vector3.up * 2, lineColor);
		}
	}
	private void OnDrawGizmos() //Visual debug
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position, radius);

		if (covers.Length > 0)
		{
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
//Class to represents a potential cover point in the game world.
[System.Serializable]
public class CoverPoint
{
	public Vector3 position;
	public Collider coverCollider;
	public float lineOfSight;
	public CoverPoint(Vector3 position, Collider coverCollider, float lineOfSight)
	{
		this.position = position;
		this.coverCollider = coverCollider;
		this.lineOfSight = lineOfSight;
	}
}