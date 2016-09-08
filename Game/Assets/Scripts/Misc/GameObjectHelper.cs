using System;
using UnityEngine;

public class GameObjectHelper
{
	public static bool IsObjectWithinSight (Camera viewer, GameObject obj) {
		// The bottom-left of the camera is (0,0); the top-right is (1,1). The z position is in world units from the camera.
		var viewPos = viewer.WorldToViewportPoint (obj.transform.position);

		return 	0 <= viewPos.x && viewPos.x <= 1 &&
			0 <= viewPos.y && viewPos.y <= 1 &&
			0 <= viewPos.z;
	}

	public static float AngleTo (Camera viewer, GameObject obj) {
		return Vector3.Angle (viewer.transform.forward, DirectionTo (viewer, obj));
	}

	public static float VerticalAngleTo (Camera viewer, GameObject obj) {
		float botY = DirectionTo (viewer, obj).normalized.y;
		float playerY = viewer.transform.forward.normalized.y;

		return RadiansToDegree(Math.Asin (botY) - Math.Asin (playerY));
	}

	public static float VerticalAngleTo (Transform viewer, Transform obj) {
		float botY = DirectionTo (viewer, obj).normalized.y;
		float playerY = viewer.forward.normalized.y;

		return RadiansToDegree(Math.Asin (botY) - Math.Asin (playerY));
	}

	public static float HorizontalAngleTo (Camera viewer, GameObject obj) {
		float angle = Vector3.Angle (viewer.transform.forward, DirectionTo (viewer, obj));

		switch (AngleDir (viewer.transform.forward, DirectionTo (viewer, obj), Vector3.up)) {
		case Direction.Left:
			return -angle;
		case Direction.Right:
			return angle;
		case Direction.Forward:
			return 0;
		}
		throw new InvalidOperationException ();
	}

	public static float HorizontalAngleTo (Transform viewer, Transform obj) {
		float angle = Vector3.Angle (viewer.transform.forward, DirectionTo (viewer, obj));

		switch (AngleDir (viewer.transform.forward, DirectionTo (viewer, obj), Vector3.up)) {
		case Direction.Left:
			return -angle;
		case Direction.Right:
			return angle;
		case Direction.Forward:
			return 0;
		}
		throw new InvalidOperationException ();
	}

	public static float DistanceTo(Camera viewer, GameObject obj) {
		return Vector3.Distance (viewer.transform.position, obj.transform.position);
	}

	private static Vector3 DirectionTo(Camera viewer, GameObject obj) {
		// Gets a vector that points from the player's position to the bot's.
		return obj.transform.position - viewer.transform.position;
	}

	private static Vector3 DirectionTo(Transform from, Transform to) {
		// Gets a vector that points from from's position to the to's position.
		return to.position - from.position;
	}

	private static Direction AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
		Vector3 right = Vector3.Cross(up, fwd);        // right vector
		float dir = Vector3.Dot(right, targetDir);

		if (dir > 0f) {
			return Direction.Right;
		} else if (dir < 0f) {
			return Direction.Left;
		} else {
			return Direction.Forward;
		}
	}

	public static float RadiansToDegree(double radians) {
		return (float) (radians * 180 / Math.PI);
	}

	private enum Direction { Left, Right, Forward };
}
