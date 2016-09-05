using UnityEngine;
using System.Collections;

[System.Serializable]
public class stateNew
{
	public string name = "Stand";
	public float speed;
	public float height;
	public Vector3 center;
	public Vector3 camPos;
}

public class NEATPlayerController : MonoBehaviour
{
	//Movement related
	/*
	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	private Vector3 moveDirection = Vector3.zero;
	public CharacterController controller;
	public PlayerVitals pv;

	public float hor;
	public float ver;
	public int state;
	public bool running;

	public state[] states = new state[3];
	private state curState;

	public Transform adjTrans;

	private float adjvar = 1;
	*/


	//Mouse related
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX;
	public float sensitivityY;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	float rotationY = 0F;

	public float mouseX;
	public float mouseY;


	public float shootThreshold;
	public float reloadThreshold;
	public Weapon weapon;

	private float[] simulatedInputs;
	void Start(){
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;

		Cursor.lockState = CursorLockMode.Locked;


		simulatedInputs = new float[4];
		simulatedInputs [0] = 1;
		simulatedInputs [1] = 0;
		simulatedInputs [2] = 1;
		simulatedInputs [3] = 0;
	}

	void Update()
	{
		//Run network

		//Mouse movement
		mouseX = simulatedInputs [0]; Input.GetAxis ("Mouse X");
		float turnDirection = simulatedInputs [0] - simulatedInputs [1];
		if (turnDirection > 0) { //Turn left
			transform.Rotate (0, mouseX * sensitivityX * Time.deltaTime, 0);
		} else { // Turn right
			transform.Rotate (0, mouseX * sensitivityX, 0);
		}

		if (simulatedInputs [2] > shootThreshold) {
			weapon.FireOneShot ();
		}
		if (simulatedInputs [3] > reloadThreshold) {
			weapon.Reload ();
		}
		simulatedInputs [3] += 0.1f;
		simulatedInputs [3] = simulatedInputs [3] % 2;


		/*CheckState ();
		CheckInput ();
		if (controller.isGrounded) {
			ver = Input.GetAxis ("Vertical");
			hor = Input.GetAxis ("Horizontal");
			if (Mathf.Abs (ver) > 0.1f && Mathf.Abs (hor) > 0.1f)
				adjvar = 0.701f;
			else
				adjvar = 1f;
			moveDirection = new Vector3 (hor * adjvar, -2f, ver * adjvar);
			moveDirection = transform.TransformDirection (moveDirection);
			moveDirection *= speed;
			if (Input.GetButtonDown ("Jump")) {// && Screen.lockCursor)
				if (state == 0)
					moveDirection.y = jumpSpeed;
				else if (state == 1)
					state = 0;
				else if (state == 2)
					state = 1;
			}
		}
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move (moveDirection * Time.deltaTime);*/

		/*

		mouseX = Input.GetAxis ("Mouse X");
		mouseY = Input.GetAxis ("Mouse Y");

		if (axes == RotationAxes.MouseXAndY) {
			float rotationX = transform.localEulerAngles.y + mouseX * sensitivityX;

			rotationY += mouseY * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

			transform.localEulerAngles = new Vector3 (-rotationY, rotationX, 0);
		} else if (axes == RotationAxes.MouseX) {
			transform.Rotate (0, mouseX * sensitivityX, 0);
		} else {
			rotationY += mouseY * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

			transform.localEulerAngles = new Vector3 (-rotationY, transform.localEulerAngles.y, 0);

		}


		*/
	}

	/*
	void CheckInput()
	{
		if (Input.GetKeyDown(KeyCode.C) && controller.isGrounded)
		{
			if (state == 0) state = 1;
			else if (state == 1) state = 0;
		}
		running = (controller.isGrounded && controller.velocity.magnitude > 1 && state == 0 && Input.GetKey (KeyCode.LeftShift) && Input.GetKey (KeyCode.W) && !Input.GetKey (KeyCode.S));// && Screen.lockCursor);
	}

	void CheckState()
	{
		if (running)
		{
			curState = states[0];
		}
		else if (state == 0)
		{
			curState = states[1];
		}
		else if (state == 1)
		{
			curState = states[2];
		}

		speed = curState.speed;
		controller.height = curState.height;
		controller.center = curState.center;
		adjTrans.localPosition = Vector3.Lerp(adjTrans.localPosition, curState.camPos, Time.deltaTime * 10);
	}
	*/
}
