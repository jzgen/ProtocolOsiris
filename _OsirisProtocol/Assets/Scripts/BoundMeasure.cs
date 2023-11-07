using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundMeasure : MonoBehaviour
{
	public float radius;
	public float sizeX;
	public float sizeZ;
	public LayerMask covermask;
	public Collider[] colliders;
	void Update()
	{
		colliders = Physics.OverlapSphere(transform.position, radius, covermask);
		if (colliders.Length == 1)
		{
			foreach (Collider coverCollider in colliders)
			{
				Vector3 size = CoverSizeToWorld(coverCollider as BoxCollider);
				sizeX = size.x;
				sizeZ = size.z;
			}
		}
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
		Gizmos.color = Color.black;
		Gizmos.DrawWireSphere(transform.position, radius);

		if (colliders.Length > 0)
		{
			foreach (BoxCollider boxCollider in colliders)
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
