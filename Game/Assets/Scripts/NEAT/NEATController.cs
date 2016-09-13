using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;

public class NEATController : UnitController {

	IBlackBox box;
	bool IsRunning;

	Transform lookRoot;

	public float minimumY = -60F;
	public float maximumY = 60F;
	float rotationY = 0F;
	public float sensitivityX;
	public float sensitivityY;
	public float mouseX;
	public float mouseY;
	public float shootThreshold;
	public float reloadThreshold;
	public NEATWeapon weapon;
	private int once = 0;
	// Use this for initialization
	void Start () {
		//lookRoot = gameObject.transform.GetChild(2).GetChild(0);
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (IsRunning) {
			System.Random rnd = new System.Random ();
			//Debug.Log (box);
			//Optain input array from convolutional neural network / java

			ISignalArray inputArr = box.InputSignalArray;
			inputArr [0] = rnd.NextDouble ();

			//Activate network
			box.Activate ();

			//Obtain output
			ISignalArray outputArr = box.OutputSignalArray;

			double[] output = new double[outputArr.Length];
			outputArr.CopyTo (output, 0);

			if (once < 10) {
				transform.Rotate (2f, 0, 0);
				once++;
			}
			//Mouse movement
			/*mouseX = (float) output [0]; 
			Input.GetAxis ("Mouse X");
			float turnDirection = (float)(output [0] - output [1]);
			if (turnDirection > 0) { //Turn left
				transform.Rotate (0f, mouseX * sensitivityX, 0f); // * Time.deltaTime
			} else { // Turn right
				transform.Rotate (0, mouseX * sensitivityX, 0); //-mouseX?
			}*/
			/*
			rotationY += 10 * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

			lookRoot.transform.localEulerAngles = new Vector3 (rotationY, transform.localEulerAngles.y, 0);
			*/
			/*
			mouseY = (float) output [2]; 
			Input.GetAxis ("Mouse Y");
			turnDirection = (float)(output [2] - output [3]);
			if (turnDirection > 0) { //Turn up
				rotationY += mouseY * sensitivityY;
				rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

				lookRoot.transform.localEulerAngles = new Vector3 (-rotationY, transform.localEulerAngles.y, 0);
			} else { // Turn down
				rotationY += mouseY * sensitivityY;
				rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

				lookRoot.transform.localEulerAngles = new Vector3 (rotationY, transform.localEulerAngles.y, 0);
			}


			if (output [4] > shootThreshold) {
				weapon.FireOneShot ();
			}
			if (output [5] > reloadThreshold) {
				weapon.Reload ();
			}
			*/

			//var turnAngle = outputArr [0];

			//Debug.Log (turnAngle);
			//transform.Rotate (new Vector3 (0, (float)turnAngle, 0));
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
		//return 360f - System.Math.Abs (transform.rotation.eulerAngles.y - 180f);
		return 100f;
	}
}
