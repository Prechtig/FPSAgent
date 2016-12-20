using UnityEngine;
using System.Collections;
using System;
using Kajabity.Tools.Java;

public class TrainingDataCapturer : MonoBehaviour
{
	private static int saveInterval;
	private int frameCounter = 0;

	private string cameraTag = "MainCamera";
	private Camera playerCam;
	private static int width, height;

	static TrainingDataCapturer() {
		JavaProperties projectProperties = PropertiesReader.GetPropertyFile (PropertyFile.Project);
		saveInterval = int.Parse(projectProperties.GetProperty("datageneration.screenshot.save.interval"));
		width = int.Parse (projectProperties.GetProperty ("training.image.width"));
		height = int.Parse (projectProperties.GetProperty ("training.image.height"));
	}

	// Use this for initialization
	void Start ()
	{
		DatabaseWriter.Initialize ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!IsCameraInitialized ()) {
			InitializeCamera ();
		} else {
			frameCounter = ++frameCounter % saveInterval;

			if (frameCounter == 0) {
				TrainingData trainingData = CaptureTrainingData ();
				DatabaseWriter.InsertTrainingData (trainingData);
			}
		}
	}

	public TrainingData CaptureTrainingData() {
		float[] groundTruths = GroundTruth.CalculateGroundTruths (playerCam, 1);
		Screenshot screenshot = ScreenSnapper.SnapScreenshot (playerCam);
		return new TrainingData (groundTruths, screenshot);
	}

	private bool IsCameraInitialized() {
		return playerCam != null;
	}

	private void InitializeCamera() {
		GameObject cam = GameObject.FindGameObjectWithTag (cameraTag);
		if(cam != null) {
			Screen.SetResolution (width, height, false);
			playerCam = cam.GetComponent<Camera> ();
			playerCam.rect = new Rect (0f, 0f, 1f, 1f);
			playerCam.targetTexture = new RenderTexture (width, height, 24);
			playerCam.aspect = 1f;
			playerCam.nearClipPlane = 0.001f;
			playerCam.farClipPlane = 1000f;
		}
	}
}
