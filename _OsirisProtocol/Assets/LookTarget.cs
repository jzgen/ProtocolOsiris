using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTarget : MonoBehaviour
{
	public Transform target;
	public Vector3 offset;
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		Vector3 targetPosition = transform.position - offset;
		Vector3 lookDirection = ((target.position - targetPosition) / 2).normalized;
		transform.rotation = Quaternion.LookRotation(lookDirection);

		Ray debugRay = new(transform.position, lookDirection);

		Debug.DrawLine(debugRay.origin, debugRay.GetPoint(10), Color.blue, 0.01f);
	}
}
