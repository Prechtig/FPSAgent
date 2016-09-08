using System;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class GameObjectHelperTest
{

	private readonly double delta = 1E-3;

	[Test]
	public void testHorizontalAngleDirections() {
		GameObject original = new GameObject ();
		float angle;

		GameObject viewer = GameObject.Instantiate (original);
		viewer.transform.position = new Vector3 (0, 0, 0);
		viewer.transform.forward = Vector3.forward;

		GameObject frontRight = GameObject.Instantiate (original);
		frontRight.transform.position = new Vector3 (1, 0, 1);
		angle = GameObjectHelper.HorizontalAngleTo (viewer.transform, frontRight.transform);
		Assert.AreEqual (45d, (double) angle, delta);

		GameObject frontLeft = GameObject.Instantiate (original);
		frontLeft.transform.position = new Vector3 (-1, 0, 1);
		angle = GameObjectHelper.HorizontalAngleTo (viewer.transform, frontLeft.transform);
		Assert.AreEqual (-45d, (double) angle, delta);

		GameObject backRight = GameObject.Instantiate (original);
		backRight.transform.position = new Vector3 (1, 0, -1);
		angle = GameObjectHelper.HorizontalAngleTo (viewer.transform, backRight.transform);
		Assert.AreEqual (135d, (double) angle, delta);

		GameObject backLeft = GameObject.Instantiate (original);
		backLeft.transform.position = new Vector3 (-1, 0, -1);
		angle = GameObjectHelper.HorizontalAngleTo (viewer.transform, backLeft.transform);
		Assert.AreEqual (-135d, (double) angle, delta);
	}

	[Test]
	public void testHorizontalAngle() {
		GameObject original = new GameObject ();
		GameObject other = GameObject.Instantiate (original);
		double calculatedAngle, trueAngle;
		float a, b;

		GameObject viewer = GameObject.Instantiate (original);
		viewer.transform.position = new Vector3 (0, 0, 0);
		viewer.transform.forward = Vector3.forward;

		a = 1;
		b = 2;
		other.transform.position = new Vector3 (a, 0, b);
		calculatedAngle = GameObjectHelper.HorizontalAngleTo (viewer.transform, other.transform);
		trueAngle = GameObjectHelper.RadiansToDegree(Math.Atan (a / b));
		Assert.AreEqual (trueAngle, calculatedAngle, delta);

		a = 2;
		b = 1;
		other.transform.position = new Vector3 (a, 0, b);
		calculatedAngle = GameObjectHelper.HorizontalAngleTo (viewer.transform, other.transform);
		trueAngle = GameObjectHelper.RadiansToDegree(Math.Atan (a / b));
		Assert.AreEqual (trueAngle, calculatedAngle, delta);
	}

	[Test]
	public void testVerticalAngleDirections() {
		GameObject original = new GameObject ();
		GameObject other = GameObject.Instantiate (original);
		double calculatedAngle;

		GameObject viewer = GameObject.Instantiate (original);
		viewer.transform.position = new Vector3 (0, 0, 0);
		viewer.transform.forward = Vector3.forward;

		other.transform.position = new Vector3 (0, 1, 1);
		calculatedAngle = GameObjectHelper.VerticalAngleTo (viewer.transform, other.transform);
		Assert.AreEqual (45, calculatedAngle, delta);

		other.transform.position = new Vector3 (0, -1, 1);
		calculatedAngle = GameObjectHelper.VerticalAngleTo (viewer.transform, other.transform);
		Assert.AreEqual (-45, calculatedAngle, delta);
	}

	[Test]
	public void testVerticalAngle() {
		GameObject original = new GameObject ();
		GameObject other = GameObject.Instantiate (original);
		double calculatedAngle, trueAngle;
		float a, b;

		GameObject viewer = GameObject.Instantiate (original);
		viewer.transform.position = new Vector3 (0, 0, 0);
		viewer.transform.forward = Vector3.forward;

		a = 1;
		b = 2;
		other.transform.position = new Vector3 (0, b, a);
		calculatedAngle = GameObjectHelper.VerticalAngleTo (viewer.transform, other.transform);
		trueAngle = GameObjectHelper.RadiansToDegree(Math.Atan (b / a));
		Assert.AreEqual (trueAngle, calculatedAngle, delta);

		a = 2;
		b = 1;
		other.transform.position = new Vector3 (0, b, a);
		calculatedAngle = GameObjectHelper.VerticalAngleTo (viewer.transform, other.transform);
		trueAngle = GameObjectHelper.RadiansToDegree(Math.Atan (b / a));
		Assert.AreEqual (trueAngle, calculatedAngle, delta);

	}
}
