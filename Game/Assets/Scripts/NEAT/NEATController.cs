﻿using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using System.Linq;

public class NEATController : UnitController {

	IBlackBox box;
	bool IsRunning;

	//Transform lookRoot;

	float rotationX = 0;
	float rotationY = 0;

	public float sensitivityX;
	public float sensitivityY;
	public float mouseX;
	public float mouseY;
	public float shootThreshold;
	public float reloadThreshold;
	public NEATWeapon weapon;
	private Camera playerCam;
	// Use this for initialization
	void Start () {
		playerCam = gameObject.GetComponentInChildren<Camera> ();
		//lookRoot = gameObject.transform.GetChild(2).GetChild(0);
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (IsRunning) {
			ISignalArray inputArr = box.InputSignalArray;
			float[] groundTruths = GroundTruth.CalculateGroundTruthsScaled (playerCam, 1);
			inputArr.CopyFrom(groundTruths.Select(f => (double)f).ToArray(), 0);

			//Activate network
			box.Activate ();
			//Obtain output
			ISignalArray outputArr = box.OutputSignalArray;

			double[] output = new double[outputArr.Length];
			outputArr.CopyTo (output, 0);

			//Set outputs
			/*
			output[0] = 0;
			output[1] = 0;
			output[2] = 0;
			output[3] = 0;
			output[4] = 0; //shooting
			output[5] = 0;
			*/


			//Mouse movement
			mouseX = (float)(output [0] - output [1]);
			mouseY = (float)(output [2] - output [3]);

			rotationX += -mouseY * sensitivityY * Time.deltaTime;
			rotationX = Mathf.Clamp (rotationX, -90, 90);

			rotationY += mouseX * sensitivityX * Time.deltaTime;
			//rotationY = Mathf.Clamp (rotationY, -90, 90);
			if (rotationY > 180) {
				rotationY -= 360;
			} else if (rotationY < -180) {
				rotationY += 360;
			}
			transform.localEulerAngles = new Vector3(rotationX, rotationY, transform.localEulerAngles.z);

			if (output [4] > shootThreshold) {
				weapon.FireOneShot ();
			} else if (output [5] > reloadThreshold) {
				weapon.Reload ();
			}
		}
	}

	public override void Stop()
	{
		this.IsRunning = false;
	}

	public override void Activate(IBlackBox box)
	{
		this.box = box;
		this.IsRunning = true;
	}

	public override float GetFitness()
	{
		//THIS IS NOT USED!
		return -1;
	}
}
