using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class BoundMeasure : MonoBehaviour
{
	public float radius;
	public Vector3 offset;
	public Transform player;
	public Transform cover;
	public Vector3 direction;
	public List<Vector3> pointPositions;
	public List<float> results;
	void Update()
	{
		results.Clear();

		direction = (player.position - cover.position).normalized;

		if (results.Count < pointPositions.Count)
		{
			foreach (Vector3 point in pointPositions)
			{
				float dotProduct = Vector3.Dot(point, direction);
				results.Add(dotProduct);
			}
		}

		Ray rayDirection = new(cover.position, direction);
		Debug.DrawLine(rayDirection.origin, rayDirection.GetPoint(3));
	}
	private void OnDrawGizmos()
	{
		if (pointPositions.Count != results.Count) return;

		for (int i = 0; i < pointPositions.Count; i++)
		{
			float dotProduct = results[i];
			Gizmos.color = dotProduct < 0 ? Color.blue : Color.red;

			Vector3 gizmoPosition = pointPositions[i];
			Gizmos.DrawSphere(gizmoPosition, radius);
		}
	}
}
