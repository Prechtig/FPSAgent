using UnityEngine;
using System.Collections;

//A normal mouse look script, modified for networking.

public class MouseLook : MonoBehaviour
{

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationY = 0F;

    public float mouseX;
    public float mouseY;


    void Update()
	{
		mouseX = Input.GetAxis ("Mouse X");
		mouseY = Input.GetAxis ("Mouse Y");

		if (axes == RotationAxes.MouseXAndY) {
			float rotationX = transform.localEulerAngles.y + mouseX * sensitivityX;

			rotationY += mouseY * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

			transform.localEulerAngles = new Vector3 (-rotationY, rotationX, 0);
		} else if (axes == RotationAxes.MouseX) {
			transform.Rotate (0, mouseX * sensitivityX*Time.deltaTime, 0);
		} else {
			rotationY += mouseY * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

			transform.localEulerAngles = new Vector3 (-rotationY, transform.localEulerAngles.y, 0);

		}
	}

    void Start()
    {
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;

		Cursor.lockState = CursorLockMode.Locked;

    }
}