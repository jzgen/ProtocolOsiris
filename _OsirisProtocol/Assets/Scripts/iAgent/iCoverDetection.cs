using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The CoverDetection class is responsible for detecting potential cover points around objects
public class iCoverDetection : MonoBehaviour
{
	[Header("Cover Detection")]
	public LayerMask coverMask;
	public float radius = 5f;
	public Collider[] covers;

	[Header("Cover Points")]
	public float pointSpacing = 1.25f;
	public List<CoverPoint> coverPoints = new List<CoverPoint>();
	public List<Vector3> potentialCoverPoint;

	[Header("Cover Position Priority")]
	public float distanceToAgentWeight;
	public float visibilityWeight;
	public float distanceToPlayerWeight;

	public Vector3 bestCoverPos;

	[Header("Player References")]
	public Transform player;
	public float fieldOfView;
	public float viewDistance;
	public Vector3 rotationOffset;
	public void SearchCovers()
	{
		// Use a sphere to detect all covers within an estimated radius.
		covers = Physics.OverlapSphere(transform.position, radius, coverMask);

		// Clear the lists to update and evaluate only the cover points within the established radius.
		coverPoints.Clear();
		potentialCoverPoint.Clear();

		// If the size of the covers array is greater than 0 and the variable pointSpacing is greater than 0, generate the position lists. 
		if (covers.Length > 0 && pointSpacing > 0)
		{
			coverPoints = GenerateCoverPoints(); // Generate a list of points that are positioned around the cover.
			AssignPotentialCoversPoints(); // Based on the previously generated list, assign potential points to a new Vector3 list.

			bestCoverPos = EvaluateCoverPositions();
		}
	}
	public List<CoverPoint> GenerateCoverPoints()
	{
		List<CoverPoint> coverPoints = new List<CoverPoint>();

		foreach (Collider coverCollider in covers)
		{
			// We obtain the center of the collider and its rotation on the Y axis.
			Vector3 colliderCenter = coverCollider.bounds.center;
			Quaternion colliderRotation = Quaternion.Euler(0, coverCollider.transform.eulerAngles.y, 0);

			// Obtain the dimensions of the box collider with the function CoverSizeToWorld.
			Vector3 coverSize = CoverSizeToWorld(coverCollider as BoxCollider);

			// Convert the coverSize measurements to int to determine the size of a grid that fits the collider size, taking into account the pointSpacing.
			int gridSizeX = Mathf.CeilToInt(((coverSize.x + 1f) / pointSpacing) + 0.5f); // Add +1 so that the grid generates a perimeter.
			int gridSizeZ = Mathf.CeilToInt(((coverSize.z + 1f) / pointSpacing) + 0.5f); // And 0.5 to avoid issues with half dimensions.
			float gridHeight = coverSize.y / 2;

			// Calculate the starting point of the grid allign to the center collider.
			Vector3 gridOrigin = colliderCenter - new Vector3((gridSizeX - 1) * pointSpacing / 2, gridHeight, (gridSizeZ - 1) * pointSpacing / 2);

			// Generate a loop to form the grid of positions.
			for (int x = 0; x < gridSizeX; x++)
			{
				for (int z = 0; z < gridSizeZ; z++)
				{
					// Check if the point is on a corner and skip it.
					bool isCorner = (x == 0 && z == 0) || // Bottom left corner
									(x == gridSizeX - 1 && z == 0) || // Bottom right corner.
									(x == 0 && z == gridSizeZ - 1) || // Upper left corner
									(x == gridSizeX - 1 && z == gridSizeZ - 1); // Upper right corner

					if (!isCorner)
					{
						// Calculate the position of the point in the grid's local space.
						Vector3 pointLocal = new Vector3(x * pointSpacing, 0, z * pointSpacing);

						// Transform the point to world space without applying rotation.
						Vector3 pointWorld = gridOrigin + pointLocal;

						// Apply the collider's rotation to the point.
						pointWorld = colliderCenter + colliderRotation * (pointWorld - colliderCenter);

						// Check if the point is obstructed.
						if (!Physics.CheckSphere(pointWorld, 0.1f, ~LayerMask.GetMask("Default")))
						{
							// If the point is not obstructed, we evaluate whether it is behind or in front of the cover based on the player's position.
							float lineOfSightValue = LineOfSight(pointWorld, coverCollider);
							// Add the cover point to the list with its position, the collider it was generated with, and the line of sight.
							coverPoints.Add(new CoverPoint(pointWorld, coverCollider, lineOfSightValue));
						}
					}
				}
			}
		}

		return coverPoints;
	}

	// Rounds a floating-point value to a specified number of decimal places.
	float RoundToDecimal(float value, int decimalPlaces)
	{
		float multiplier = Mathf.Pow(10f, decimalPlaces); // Calculate the multiplier for the specified decimal places.
		return Mathf.Round(value * multiplier) / multiplier; // Round the value to the nearest integer, then divide by the multiplier to get the rounded decimal value.
	}

	//Converts the local size of a BoxCollider to its world size, accounting for the object's global scale.
	Vector3 CoverSizeToWorld(BoxCollider boxCollider)
	{
		// Get the global scale of the object to which the collider is attached.
		Vector3 worldScale = boxCollider.transform.lossyScale;

		// Calculate the world size for each dimension of the collider, rounding to three decimal places for precision.
		float width = RoundToDecimal(boxCollider.size.x * worldScale.x, 3);
		float length = RoundToDecimal(boxCollider.size.z * worldScale.z, 3);
		float height = RoundToDecimal(boxCollider.size.y * worldScale.y, 3);

		// Return the world size as a new Vector3.
		return new Vector3(width, height, length);
	}

	// Calculates the dot product between two vectors to determine the line of sight from a cover point to the player.
	float LineOfSight(Vector3 coverPoint, Collider cover)
	{
		// Calculate the normalized directions
		Vector3 coverToPlayer = (player.transform.position - coverPoint).normalized;
		Vector3 coverToPoint = (coverPoint - cover.transform.position).normalized;

		// Compute the dot product of the two vectors direction.
		float dotProduct = Vector3.Dot(coverToPoint, coverToPlayer);

		// Return the dot product, which indicates how aligned the cover point is with the player's position.
		return dotProduct;
	}

	//Evaluates each cover point to determine potential cover points based on line of sight and distance to the player.
	private void AssignPotentialCoversPoints()
	{
		if (coverPoints.Count > 0)
		{
			foreach (CoverPoint point in coverPoints)
			{
				// Calculate the elements to perform the raycast
				Vector3 pointToPlayer = ((player.transform.position + (Vector3.up * 2)) - point.position).normalized;
				float distance = Vector3.Distance(point.position, player.transform.position);

				Color lineColor;

				if (point.lineOfSight < -0.1f)
				{
					// Set the line color to yellow to indicate cover point without physical cover between the player
					lineColor = Color.yellow;

					// Perform a raycast to check if there is a clear line of sight to the player.
					if (Physics.Raycast(point.position, pointToPlayer, out RaycastHit hit, distance))
					{
						// If the raycast hits something, set the line color to blue and add the point to potential covers.
						lineColor = Color.blue;
						potentialCoverPoint.Add(point.position);
					}
				}
				else
				{
					// If the point is not behind cover, set the line color to white.
					lineColor = Color.white;
				}

				// Draw a debug line in the scene view to visualize the cover point status.
				Debug.DrawLine(point.position, point.position + Vector3.up * 2, lineColor);
			}
		}
	}

	//Evaluates potential cover positions by calculating their distance to the agent and player, and the visibility percentage from the player's position.
	Vector3 EvaluateCoverPositions()
	{
		Vector3 bestCoverPosition = Vector3.zero;
		float bestScore = float.MinValue;

		foreach (Vector3 position in potentialCoverPoint)
		{
			float distanceToAgentScore = 1 / Vector3.Distance(position, transform.position);
			int visibilityScore = VisibiltyPercentage(position, player);
			float distanceToPlayerScore = Vector3.Distance(position, player.position);

			float totalScore = distanceToAgentWeight * distanceToAgentScore +
							   visibilityWeight * visibilityScore +
							   distanceToPlayerWeight * distanceToPlayerScore;

			if (totalScore > bestScore)
			{
				bestScore = totalScore;
				bestCoverPosition = position; // Almacenamos la posición en lugar del objeto CoverPoint
			}
		}

		// Devuelve la posición de la mejor cobertura
		return bestCoverPosition;
	}

	//Calculates the visibility percentage of a cover position from the player's perspective, based on the player's field of view.
	int VisibiltyPercentage(Vector3 position, Transform player)
	{
		// Calculate the direction to the cover from the player's position.
		Vector3 directionToCover = (position - player.position).normalized;

		// Convert the field of view angle to radians and calculate half of it.
		float halfFieldOfViewRadians = fieldOfView * 0.5f * Mathf.Deg2Rad;

		// Calculate the cosine of the half field of view.
		float cosHalfFieldOfView = Mathf.Cos(halfFieldOfViewRadians);

		// Calculate the dot product (cosine of the angle) between the player's forward direction and the direction to the cover.
		float dotProduct = Vector3.Dot(Camera.main.transform.forward, directionToCover);

		// Ensure that the visibility percentage is not negative and scale it to a 0-100 range.
		float percentage = Mathf.Max((dotProduct - cosHalfFieldOfView) / (1 - cosHalfFieldOfView), 0f) * 100;

		// Round the percentage to the nearest integer and ensure it is at least 0.
		return Mathf.CeilToInt(percentage);
	}

	private void OnDrawGizmos() //Visual debug
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position, radius);

		//Debug best coverPostion
		if (bestCoverPos != Vector3.zero)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawSphere(bestCoverPos, 0.1f);
		}

		//Debug the cover detected by the iAgent
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

		//Debug the player cone vision;
		if (player != null)
		{
			Gizmos.matrix = Matrix4x4.TRS(player.position, player.rotation * Quaternion.Euler(rotationOffset), Vector3.one * 1.001f);
			Gizmos.color = Color.blue;
			Vector3 center = player.position;
			Gizmos.DrawFrustum(Vector3.zero + Vector3.right * 0.1f, fieldOfView, viewDistance, 0.001f, 0f);
		}
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