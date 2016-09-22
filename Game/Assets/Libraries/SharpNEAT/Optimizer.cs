﻿using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using System.Collections.Generic;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using System;
using System.Xml;
using System.IO;
using SharpNeat.Domains;

public class Optimizer : MonoBehaviour {
	public GameObject wallPrefab;

	const int NUM_INPUTS = 20;
	const int NUM_OUTPUTS = 6;

	bool EARunning;
	//string popFileSavePath, champFileSavePath;

	Experiment experiment;
	static NeatEvolutionAlgorithm<NeatGenome> _ea;

	public GameObject Unit;
	//public NEATArena arena;

	Dictionary<IBlackBox, UnitController> ControllerMap = new Dictionary<IBlackBox, UnitController>();
	Dictionary<IBlackBox, NEATArena> ArenaMap = new Dictionary<IBlackBox, NEATArena>();
	Dictionary<IBlackBox, float> FitnessMap = new Dictionary<IBlackBox, float>();

	private DateTime startTime;
	private float timeLeft;
	private float accum;
	private int frames;
	private float updateInterval = 12;

	private uint Generation;
	private double Fitness;
	private float PreviousTimeScale;
	private int PersistNGenerations;
	private Boolean Persisted;

	private int trials;
	private float trialDuration;
	private float stoppingFitness;

	public int Trials
	{
		get { return trials; }
	}

	public float TrialDuration {
		get { return trialDuration; }
	}

	public float StoppingFitness {
		get { return stoppingFitness; }
	}

	// Use this for initialization
	void Start () {
		Utility.DebugLog = true;
		experiment = new Experiment();
		XmlDocument xmlConfig = new XmlDocument();
		TextAsset textAsset = (TextAsset)Resources.Load("experiment.config");
		xmlConfig.LoadXml(textAsset.text);

		InitFromConfig (xmlConfig.DocumentElement);

		experiment.SetOptimizer(this);

		experiment.Initialize("FPS Experiment", xmlConfig.DocumentElement, NUM_INPUTS, NUM_OUTPUTS);

		//champFileSavePath = Application.persistentDataPath + string.Format("/{0}.champ.xml", "FPSAgent");
		//popFileSavePath = Application.persistentDataPath + string.Format("/{0}.pop.xml", "FPSAgent");

		//print(champFileSavePath);
	}

	// Update is called once per frame
	void Update()
	{
		//  evaluationStartTime += Time.deltaTime;
		Persisted = false;

		timeLeft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		++frames;
		//Time.timeScale = 1; //TODO REmove this

		if (timeLeft <= 0.0)
		{
			var fps = accum / frames;
			timeLeft = updateInterval;
			accum = 0.0f;
			frames = 0;
			//   print("FPS: " + fps);
			if (fps < 10 && Time.timeScale > 1)
			{
				Time.timeScale = Time.timeScale - 1;
				print("Lowering time scale to " + Time.timeScale);
			}
		}
	}

	public void StartEA()
	{
		Utility.DebugLog = true;
		Utility.Log("Starting FPS Agent experiment");
		FitnessMap = new Dictionary<IBlackBox, float> ();
		//_ea = experiment.CreateEvolutionAlgorithm(popFileSavePath);
		_ea = experiment.CreateEvolutionAlgorithm();
		startTime = DateTime.Now;

		_ea.UpdateEvent += new EventHandler(ea_UpdateEvent);
		//_ea.PausedEvent += new EventHandler(ea_PauseEvent);
		var evoSpeed = 25;

		//   Time.fixedDeltaTime = 0.045f;
		Time.timeScale = evoSpeed;       
		_ea.StartContinue();
		EARunning = true;
	}

	void ea_UpdateEvent(object sender, EventArgs e)
	{
		Utility.Log(string.Format("gen={0:N0} bestFitness={1:N6}",
			_ea.CurrentGeneration, _ea.Statistics._maxFitness));

		Fitness = _ea.Statistics._maxFitness;
		Generation = _ea.CurrentGeneration;

		//    Utility.Log(string.Format("Moving average: {0}, N: {1}", _ea.Statistics._bestFitnessMA.Mean, _ea.Statistics._bestFitnessMA.Length));
	}

	/*
	void ea_PauseEvent(object sender, EventArgs e)
	{
		Time.timeScale = 1;
		Utility.Log("Done ea'ing (and neat'ing)");

		XmlWriterSettings _xwSettings = new XmlWriterSettings();
		_xwSettings.Indent = true;
		// Save genomes to xml file.        
		DirectoryInfo dirInf = new DirectoryInfo(Application.persistentDataPath);
		if (!dirInf.Exists)
		{
			Debug.Log("Creating subdirectory");
			dirInf.Create();
		}
		using (XmlWriter xw = XmlWriter.Create(popFileSavePath, _xwSettings))
		{
			experiment.SavePopulation(xw, _ea.GenomeList);
		}
		// Also save the best genome

		using (XmlWriter xw = XmlWriter.Create(champFileSavePath, _xwSettings))
		{
			experiment.SavePopulation(xw, new NeatGenome[] { _ea.CurrentChampGenome });
		}
		DateTime endTime = DateTime.Now;
		Utility.Log("Total time elapsed: " + (endTime - startTime));

		System.IO.StreamReader stream = new System.IO.StreamReader(popFileSavePath);

		EARunning = false;
	}
	*/

	void Pause()
	{
		if (EARunning) { //Unpause
			Time.timeScale = PreviousTimeScale;
			EARunning = true;
		} else { //Pause
			PreviousTimeScale = Time.timeScale;
			Time.timeScale = 0;

			PersistPopulation ();
			EARunning = false;
		}
	}

	/// <summary>
	/// Persists the population if it is the Nth generation specified in the xml config
	/// </summary>
	public void CheckPersistPopulation(){
		if (_ea.CurrentGeneration == 1 || _ea.CurrentGeneration % PersistNGenerations == 0) {
			PersistPopulation ();
		}
	}

	public void PersistPopulation(){
		string champFileSavePath = Application.persistentDataPath + string.Format("/{0}/{1}.champ.xml", _ea.CurrentGeneration, "FPSAgent");
		string popFileSavePath = Application.persistentDataPath + string.Format("/{0}/{1}.pop.xml", _ea.CurrentGeneration, "FPSAgent");

		XmlWriterSettings _xwSettings = new XmlWriterSettings ();
		_xwSettings.Indent = true;
		// Save genomes to xml file.        
		DirectoryInfo dirInf = new DirectoryInfo (Application.persistentDataPath + string.Format("/{0}", _ea.CurrentGeneration));
		if (!dirInf.Exists) {
			//Debug.Log ("Creating subdirectory");
			dirInf.Create ();
		}
		using (XmlWriter xw = XmlWriter.Create (popFileSavePath, _xwSettings)) {
			experiment.SavePopulation (xw, _ea.GenomeList);
		}
		// Also save the best genome

		using (XmlWriter xw = XmlWriter.Create (champFileSavePath, _xwSettings)) {
			experiment.SavePopulation (xw, new NeatGenome[] { _ea.CurrentChampGenome });
		}
		DateTime endTime = DateTime.Now;
		//Utility.Log ("Total time elapsed: " + (endTime - startTime));

		//System.IO.StreamReader stream = new System.IO.StreamReader (popFileSavePath);

	}

	public void StopEA()
	{

		if (_ea != null && _ea.RunState == SharpNeat.Core.RunState.Running)
		{
			_ea.Stop();
		}
	}

	public void Evaluate(IBlackBox box)
	{
		
		//Time.timeScale = 1;
		//NEATArena arena = new NEATArena ();
		//NEATArena arena = NEATArena.CreateInstance<NEATArena>();
		NEATArena arena = gameObject.AddComponent<NEATArena>();
		arena.wallPrefab = wallPrefab;
		arena.Init ();
		//Instantiate (arena, arena.transform.position, arena.transform.rotation);

		GameObject obj = arena.GetPlayer ();
		//GameObject obj = Instantiate(Unit, Unit.transform.position, Unit.transform.rotation) as GameObject;
		UnitController controller = obj.GetComponent<UnitController>();

		ControllerMap.Add (box, controller);
		ArenaMap.Add (box, arena);

		controller.Activate(box);

		//PersistPopulation ();
	}

	public void StopEvaluation(IBlackBox box)
	{
		UnitController ct = ControllerMap[box];
		NEATArena nt = ArenaMap [box];

		FitnessMap.Add (box, nt.GetFitness ());

		ControllerMap.Remove (box);
		ArenaMap.Remove (box);

		Destroy (ct.gameObject);
		Destroy (nt);
	}

	public void RunBest()
	{
		Time.timeScale = 1;

		NeatGenome genome = null;
		string champFileSavePath = Application.persistentDataPath + string.Format("/{0}/{1}.champ.xml", Generation, "FPSAgent");
		// Try to load the genome from the XML document.
		try
		{
			using (XmlReader xr = XmlReader.Create(champFileSavePath))
				genome = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, (NeatGenomeFactory)experiment.CreateGenomeFactory())[0];
		}
		catch (Exception e1)
		{
			// print(champFileLoadPath + " Error loading genome from file!\nLoading aborted.\n"
			//						  + e1.Message + "\nJoe: " + champFileLoadPath);
			return;
		}

		// Get a genome decoder that can convert genomes to phenomes.
		var genomeDecoder = experiment.CreateGenomeDecoder();

		// Decode the genome into a phenome (neural network).
		var phenome = genomeDecoder.Decode(genome);

		GameObject obj = Instantiate(Unit, Unit.transform.position, Unit.transform.rotation) as GameObject;
		UnitController controller = obj.GetComponent<UnitController>();

		ControllerMap.Add(phenome, controller);

		controller.Activate(phenome);
	}

	public float GetFitness(IBlackBox box)
	{
		if (FitnessMap.ContainsKey (box)) {
			return FitnessMap [box];
		}
		return 0;
	}

	void OnGUI()
	{
		if (GUI.Button(new Rect(10, 10, 100, 40), "Start EA"))
		{
			StartEA();
		}
		if (GUI.Button(new Rect(10, 60, 100, 40), "Stop EA"))
		{
			StopEA();
		}
		/*
		if (GUI.Button(new Rect(10, 110, 100, 40), "Run best"))
		{
			RunBest();
		}
		*/
		if (GUI.Button(new Rect(10, 160, 100, 40), "Pause EA"))
		{
			Pause();
		}

		GUI.Button(new Rect(10, Screen.height - 70, 100, 60), string.Format("Generation: {0}\nFitness: {1:0.00}", Generation, Fitness));
	}

	private void InitFromConfig(XmlElement config) {
		trials = XmlUtils.GetValueAsInt (config, "TrialCount");
		trialDuration = Convert.ToSingle(XmlUtils.GetValueAsDouble (config, "TrialDuration"));
		stoppingFitness = Convert.ToSingle (XmlUtils.GetValueAsDouble (config, "StoppingFitness"));
		PersistNGenerations = XmlUtils.GetValueAsInt(config, "PersistNGenerations");
	}
}