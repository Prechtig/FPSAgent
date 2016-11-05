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
		Vector3 viewerYZPlaneNormal = Vector3.Cross (viewer.forward, viewer.up);
		Vector3 viewerToObj = DirectionTo (viewer, obj);
		Vector3 viewerToObjOnViewerYZPlane = Vector3.ProjectOnPlane (viewerToObj, viewerYZPlaneNormal);

		float angle = Vector3.Angle (viewer.forward, viewerToObjOnViewerYZPlane);

		switch(VerticalAngleDir(viewer.up, viewerToObj)) {
		case VerticalDirection.Down:
			return -angle;
		case VerticalDirection.Up:
			return angle;
		case VerticalDirection.Forward:
			return 0;
		}
		throw new InvalidOperationException ();
	}

	public static float HorizontalAngleTo (Transform viewer, Transform obj) {
		Vector3 planeNormal = viewer.up;
		Vector3 viewerToObj = DirectionTo(viewer, obj);
		Vector3 viewerToObjOnViewerXZPlane = Vector3.ProjectOnPlane (viewerToObj, planeNormal);

		float angle = Vector3.Angle (viewer.forward, viewerToObjOnViewerXZPlane);

		switch (HorizontalAngleDir (viewer.forward, DirectionTo (viewer, obj), viewer.up)) {
		case HorizontalDirection.Left:
			return -angle;
		case HorizontalDirection.Right:
			return angle;
		case HorizontalDirection.Forward:
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

	private static VerticalDirection VerticalAngleDir(Vector3 up, Vector3 targetDir) {
		float dir = Vector3.Dot (up, targetDir);

		if (dir > 0f) {
			return VerticalDirection.Up;
		} else if (dir < 0f) {
			return VerticalDirection.Down;
		} else {
			return VerticalDirection.Forward;
		}
	}

	private static HorizontalDirection HorizontalAngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
		Vector3 right = Vector3.Cross(up, fwd);        // right vector
		float dir = Vector3.Dot(right, targetDir);

		if (dir > 0f) {
			return HorizontalDirection.Right;
		} else if (dir < 0f) {
			return HorizontalDirection.Left;
		} else {
			return HorizontalDirection.Forward;
		}
	}

	public static float RadiansToDegree(double radians) {
		return (float) (radians * 180 / Math.PI);
	}

	private enum HorizontalDirection { Left, Right, Forward };

	private enum VerticalDirection { Up, Down, Forward };
}
