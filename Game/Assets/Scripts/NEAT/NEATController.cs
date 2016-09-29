using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using System.Linq;

public class NEATController : UnitController {

	IBlackBox box;
	bool IsRunning;

	Transform lookRoot;

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
		lookRoot = gameObject.transform.GetChild(2).GetChild(0);
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (IsRunning) {
			//System.Random rnd = new System.Random ();
			//Debug.Log (box);
			//Optain input array from convolutional neural network / java

			ISignalArray inputArr = box.InputSignalArray;
			float[] groundTruths = GroundTruth.CalculateGroundTruthsScaled (playerCam, 3);
			//inputArr.CopyFrom ();
			inputArr.CopyFrom(groundTruths.Select(f => (double)f).ToArray(), 0);

			//Activate network
			box.Activate ();
			//Obtain output
			ISignalArray outputArr = box.OutputSignalArray;

			double[] output = new double[outputArr.Length];
			outputArr.CopyTo (output, 0);

			/*
			output[0] = 0.1;
			output[1] = 0;
			output[2] = 0.1;
			output[3] = 0;
			output[4] = 1;
			output[5] = 1;
			*/


			//Mouse movement
			mouseX = (float)(output [0] - output [1]);
			mouseY = (float)(output [2] - output [3]);

			//mouseX = Input.GetAxis ("Mouse X");
			//mouseY = Input.GetAxis ("Mouse Y");
			//Cursor.lockState = CursorLockMode.Locked;

			//transform.Rotate (0, mouseX * sensitivityX * Time.deltaTime, 0, Space.World);


			rotationX += -mouseY * sensitivityY * Time.deltaTime;
			rotationX = Mathf.Clamp (rotationX, -90, 90);

			rotationY += mouseX * sensitivityX * Time.deltaTime;
			rotationY = Mathf.Clamp (rotationY, -90, 90);

			transform.localEulerAngles = new Vector3(rotationX, rotationY, transform.localEulerAngles.z);

			//Debug.Log (transform.localEulerAngles.x);
			//Debug.Log (mouseY);
			//transform.Rotate (new Vector3(rotationY, 0, 0));

			/*if (rotationY < 0) { //down
				/*if (transform.eulerAngles.x > 90 && transform.eulerAngles.x < 270) {
					transform.Rotate ((transform.eulerAngles.x + 90), 0, 0);
				}
			} else if (rotationY > 0) { //up
				if (transform.eulerAngles.x > 90 && transform.eulerAngles.x < 270) {
					transform.Rotate (-(transform.eulerAngles.x - 90), 0, 0);
				}
			}
			if (transform.eulerAngles.x > 90) {
			//	transform.Rotate (-(transform.eulerAngles.x - 90), 0, 0);
			} else if (transform.eulerAngles.x < -90) {
			//	transform.Rotate ((transform.eulerAngles.x + 90), 0, 0);
			}
			*/

			/*float rotationX = transform.localEulerAngles.y + mouseX * sensitivityX;

			rotationY += mouseY * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

			transform.localEulerAngles = new Vector3 (-rotationY, rotationX, 0);

			//transform.Rotate (rotationY, rotationX, 0);
			*/

			/*if (once < 10000) {
				transform.Rotate (2f, 0, 0);
				once++;
			}*/

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
			*/


		//Debug.Log ("Shoot: " + output[4]);
		if (output [4] > shootThreshold) {
			weapon.FireOneShot ();
		} else if (output [5] > reloadThreshold) {
			weapon.Reload ();
		}


			/*if (output [5] > reloadThreshold) {
				weapon.Reload ();
			} else if (output [4] > shootThreshold) {
				weapon.FireOneShot ();
			}*/


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
		//THIS IS NOT USED!
		return -1;
	}
}
