using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;

public class NEATControllerBox : UnitController {

	IBlackBox box;
	bool IsRunning;


	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (IsRunning) {
			System.Random rnd = new System.Random ();
			Debug.Log (box);
			ISignalArray inputArr = box.InputSignalArray;
			inputArr [0] = rnd.NextDouble ();
			box.Activate ();
			ISignalArray outputArr = box.OutputSignalArray;
			var turnAngle = outputArr [0];

			Debug.Log (turnAngle);
			transform.Rotate (new Vector3 (0, (float)turnAngle, 0));
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
		return 360f - System.Math.Abs (transform.rotation.eulerAngles.y - 180f);
	}
}
