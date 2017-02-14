using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using System.Linq;
using AssemblyCSharp;
using System;
using Assets.Scripts;

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

    private static bool loaded = false;
    private static bool _useCNN;
    private static bool _frameControl;
    private static int _cnnFrameRefreshRate;
    private static bool _useVPR;

    private static bool UseCNN
    {
        get
        {
            if (!loaded)
            {
                LoadFromProperty();               
            }
            return _useCNN;
        }
    }



    private static bool FrameControl
    {
        get
        {
            if (!loaded)
            {
                LoadFromProperty();
            }
            return _frameControl;
        }
    }

    private static int CNNFrameRefreshRate
    {
        get
        {
            if (!loaded)
            {
                LoadFromProperty();
            }
            return _cnnFrameRefreshRate;
        }
    }

    public static bool UseVPR
    {
        get
        {
            return _useVPR;
        }
        set
        {
            _useVPR = value;
        }
    }

    private int frameCounter = 0;
	private double[] EmptyDoubleArray;
    private double[] output;
    private HeatMap heatMap;

	// Use this for initialization
	void Start () {
		playerCam = gameObject.GetComponentInChildren<Camera> ();
		EmptyDoubleArray = new double [box.InputCount];
        weapon.boxId = box.Id;
        output = new double[box.OutputSignalArray.Length];
        heatMap = GameObject.FindWithTag("PlayerHUD").GetComponent<Canvas>().GetComponentInChildren<HeatMap>();
        //frameControl = "true".Equals(PropertiesReader.GetPropertyFile (PropertyFile.Project).GetProperty ("game.neat.training.frameControl"));
        //useCNN = "true".Equals(PropertiesReader.GetPropertyFile (PropertyFile.Project).GetProperty ("game.neat.training.use.cnn"));
        //if(frameControl) {
        //	cnnFrameRefreshRate = int.Parse (PropertiesReader.GetPropertyFile (PropertyFile.Project).GetProperty ("game.neat.training.use.cnn.frameRefreshRate"));
        //}
    }

    // Update is called once per frame
    public double tpsCount;
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
        bool activated = false;
        if (IsRunning)
        {
            if (FrameControl)
            {
                if (frameCounter++ % CNNFrameRefreshRate == 0)
                {
                    tpsCount++;
                    ActivateBox();
                    activated = true;
                }
            }
            else
            {
                //No framecrontrol
                ActivateBox();
            }

            //Mouse movement
            mouseX = (float)(output[0] - output[1]);
            mouseY = (float)(output[2] - output[3]);

            rotationX += -mouseY * sensitivityY * Time.fixedDeltaTime;
            rotationX = Mathf.Clamp(rotationX, -90, 90);

            rotationY += mouseX * sensitivityX * Time.fixedDeltaTime;
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

            //if (activated || !FrameControl)
            //{
                if (output[4] > shootThreshold)
                {
                    weapon.FireOneShot();
                }
                else if (output[5] > reloadThreshold)
                {
                    weapon.Reload();
                }
            //}
        }
    }

    private void ActivateBox()
    {
        
        ISignalArray inputArr = box.InputSignalArray;
        //activate
        if (UseCNN)
        {
            double[] fromCNN;
            if (UseVPR)
            {
                fromCNN = GroundTruthCNN.CalculateFeaturesVPR(playerCam);
                //heatMap.UpdateColors(fromCNN);

                fromCNN = ArrayTool.Binarize(fromCNN);
            }
            else
            {
                fromCNN = GroundTruthCNN.CalculateFeaturesAngular(playerCam);
            }
            inputArr.CopyFrom(fromCNN, 0);
        }
        else if (Arena.BotSpawn.Bots.Count == 0)
        {
            inputArr.CopyFrom(EmptyDoubleArray, 0);
        }
        else if (UseVPR)
        {
            double[] d = GroundTruth.CalculateFeatures(playerCam, Arena.BotSpawn.Bots[0]);
            //ArrayTool.Binarize(GroundTruthCNN.CalculateFeaturesVPR(playerCam));
            //heatMap.UpdateColors(d);
            inputArr.CopyFrom(d, 0);
        }
        else
        {
            GroundTruthCNN.CalculateFeaturesAngular(playerCam);
            inputArr.CopyFrom(GroundTruth.CalculateGroundTruthsScaledAngleSplit(playerCam, 1), 0);
        }
        //Activate network
        box.Activate();
        //Obtain output
        ISignalArray outputArr = box.OutputSignalArray;
        

        //double[] output = new double[outputArr.Length];
        outputArr.CopyTo(output, 0);
        //return output;
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

    private static void LoadFromProperty()
    {
        _useCNN = "true".Equals(PropertiesReader.GetPropertyFile(PropertyFile.Project).GetProperty("game.neat.training.use.cnn"));
        _frameControl = "true".Equals(PropertiesReader.GetPropertyFile(PropertyFile.Project).GetProperty("game.neat.training.frameControl"));
        _cnnFrameRefreshRate = int.Parse(PropertiesReader.GetPropertyFile(PropertyFile.Project).GetProperty("game.neat.training.use.cnn.frameRefreshRate"));
        loaded = true;
    }

    public override float GetFitness()
	{
        //THIS IS NOT USED!
        return -1;
	}
}
