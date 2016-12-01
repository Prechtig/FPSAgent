using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using System.Linq;
using AssemblyCSharp;

public class NEATController : UnitController {

	IBlackBox box;
	bool IsRunning;

	//Transform lookRoot;

	public float rotationX = 0;
	public float rotationY = 0;

	public float sensitivityX;
	public float sensitivityY;
	public float mouseX;
	public float mouseY;
	public float shootThreshold;
	public float reloadThreshold;
	public NEATWeapon weapon;
	public NEATArena Arena;

	private Camera playerCam;
	private bool useCNN;
	private bool frameControl;
	private int cnnFrameRefreshRate;
	private int frameCounter = 0;

	private double[] EmptyDoubleArray;

	// Use this for initialization
	void Start () {
		playerCam = gameObject.GetComponentInChildren<Camera> ();
		EmptyDoubleArray = new double [box.InputCount];
		frameControl = "true".Equals(PropertiesReader.GetPropertyFile (PropertyFile.Project).GetProperty ("game.neat.training.frameControl"));
		useCNN = "true".Equals(PropertiesReader.GetPropertyFile (PropertyFile.Project).GetProperty ("game.neat.training.use.cnn"));
		if(frameControl) {
			cnnFrameRefreshRate = int.Parse (PropertiesReader.GetPropertyFile (PropertyFile.Project).GetProperty ("game.neat.training.use.cnn.frameRefreshRate"));
		}
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
		if (Input.GetKey (KeyCode.Mouse0)) {
			weapon.FireOneShot ();
		}
		if (Input.GetKey (KeyCode.R)) {
			weapon.Reload ();
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			Cursor.lockState = CursorLockMode.Locked;
		}
		*/
        //count++;
        if ((IsRunning && !frameControl) ||
            (IsRunning && frameControl && frameCounter++ % cnnFrameRefreshRate == 0))
        {
            ISignalArray inputArr = box.InputSignalArray;

            if (useCNN)
            {
                double[] fromCNN = GroundTruthCNN.CalculateFeatures(playerCam);
                double[] result = ArrayTool.Binarize(fromCNN);

                Debug.Log(ArrayTool.ToString(result));

                inputArr.CopyFrom(result, 0);
            }
            else
            {
                if (Arena.BotSpawn.Bots.Count == 0)
                {
                    inputArr.CopyFrom(EmptyDoubleArray, 0);
                }
                else
                {
                    inputArr.CopyFrom(GroundTruth.CalculateFeatures(playerCam, Arena.BotSpawn.Bots[0]), 0);
                }
            }

            //Activate network
            box.Activate();
            //Obtain output
            ISignalArray outputArr = box.OutputSignalArray;

            double[] output = new double[outputArr.Length];
            outputArr.CopyTo(output, 0);

            /*
			output [0] = 0;
			output [1] = 0;
			output [2] = 0;
			output [3] = 0;
			output [4] = 1;
			output [5] = 0;
			*/


            //Mouse movement
            mouseX = (float)(output[0] - output[1]);
            mouseY = (float)(output[2] - output[3]);

            rotationX += -mouseY * sensitivityY * Time.deltaTime;
            rotationX = Mathf.Clamp(rotationX, -90, 90);

            rotationY += mouseX * sensitivityX * Time.deltaTime;
            //rotationY = Mathf.Clamp (rotationY, -90, 90);
            if (rotationY > 180)
            {
                rotationY -= 360;
            }
            else if (rotationY < -180)
            {
                rotationY += 360;
            }
            transform.localEulerAngles = new Vector3(rotationX, rotationY, transform.localEulerAngles.z);
            if (output[4] > shootThreshold)
            {
                weapon.FireOneShot();
            }
            else if (output[5] > reloadThreshold)
            {
                weapon.Reload();
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
