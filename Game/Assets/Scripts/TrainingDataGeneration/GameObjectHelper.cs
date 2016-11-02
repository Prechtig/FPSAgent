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

	public static float AngleTo (Transform viewer, Transform obj) {
		return Vector3.Angle (viewer.transform.forward, DirectionTo (viewer, obj));
	}

	public static float VerticalAngleTo (Transform viewer, Transform obj) {
		float botY = DirectionTo (viewer, obj).normalized.y;
		float playerY = viewer.forward.normalized.y;

		return RadiansToDegree(Math.Asin (botY) - Math.Asin (playerY));
	}

	public static float HorizontalAngleTo (Transform viewer, Transform obj) {
		Vector3 viewerForward = nullifyY (viewer.forward);
		float angle = Vector3.Angle (viewerForward, HorizontalDirectionTo (viewer.position, obj.position));

		switch (AngleDir (viewerForward, HorizontalDirectionTo (viewer.position, obj.position), Vector3.up)) {
		case Direction.Left:
			return -angle;
		case Direction.Right:
			return angle;
		case Direction.Forward:
			return 0;
		}
		throw new InvalidOperationException ();
	}

	public static float DistanceTo(Transform viewer, Transform obj) {
		return Vector3.Distance (viewer.position, obj.position);
	}

	private static Vector3 DirectionTo(Transform viewer, Transform obj) {
		// Gets a vector that points from from's position to the to's position.
		return obj.position - viewer.position;
	}

	private static Vector3 HorizontalDirectionTo(Vector3 viewer, Vector3 obj) {
		return nullifyY(obj) - nullifyY(viewer);
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

	private static Vector3 nullifyY(Vector3 v) {
		return new Vector3 (v.x, 0, v.z);
	}

	private enum Direction { Left, Right, Forward };
}
