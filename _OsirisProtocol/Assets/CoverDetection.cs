using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class CoverDetection : MonoBehaviour
{
	public LayerMask coverMask;
	private void Start()
	{
		int NavMeshMask = 1 << NavMesh.GetAreaFromName(coverMask.ToString());

		NavMeshHit hit;
		if (NavMesh.FindClosestEdge(transform.position, out hit, NavMeshMask))
		{
			Debug.DrawLine(hit.position, Vector3.up * 3, Color.yellow, float.PositiveInfinity);
			Debug.Log("Posición cercana válida: " + hit.position);
		}

	}
	private void Update()
	{

	}
}