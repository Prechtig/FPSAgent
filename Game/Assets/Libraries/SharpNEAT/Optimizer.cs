using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using SharpNeat.Core;
using System.Collections.Generic;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using System;
using System.Xml;
using System.IO;
using SharpNeat.Domains;
using Assets.Scripts.TrainingDataGeneration;
using Assets.Scripts;

public class Optimizer : MonoBehaviour {
	public GameObject wallPrefab;

	private int NUM_INPUTS;
	private int NUM_OUTPUTS;

	bool EARunning;
	//string popFileSavePath, champFileSavePath;

	Experiment experiment;
	static NeatEvolutionAlgorithm<NeatGenome> _ea;

	public GameObject Unit;
	public bool RunBestNetwork = false;
	public int MaxParallel;
	//public NEATArena arena;

	Dictionary<IBlackBox, UnitController> ControllerMap = new Dictionary<IBlackBox, UnitController>();
	Dictionary<IBlackBox, NEATArena> ArenaMap = new Dictionary<IBlackBox, NEATArena>();
	Dictionary<IBlackBox, float> FitnessMap = new Dictionary<IBlackBox, float>();

	//private DateTime startTime;
	private float timeLeft;
	private float accum;
	private int frames;
	private float updateInterval = 12;

	private double Fitness;
	private float PreviousTimeScale;
	private int PersistNGenerations;
	//Persist populationprivate Boolean Persisted;

	private int trials;
	private float trialDuration;
	private float stoppingFitness;

	private bool AutomaticTimeScaleOn = false;
	private bool Started = false;
	private bool FirstUpdate = true;

    private string _resultSavePath;

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
	}

	// Update is called once per frame
	void Update()
	{
		//  evaluationStartTime += Time.deltaTime;

		timeLeft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		++frames;

		if (timeLeft <= 0.0)
		{
			var fps = accum / frames;
			timeLeft = updateInterval;
			accum = 0.0f;
			frames = 0;
			//   print("FPS: " + fps);
			if(EARunning) {
				if (AutomaticTimeScaleOn && fps < 10 && Time.timeScale > 1)
				{
					Time.timeScale = Time.timeScale - 1;
					print("Lowering time scale to " + Time.timeScale);
				} else if (AutomaticTimeScaleOn && fps > 50 && Time.timeScale < 25 && !RunBestNetwork) {
					Time.timeScale++;
					print("Increasing time scale to " + Time.timeScale);
				}
			}
		}

		if (Input.GetKeyDown("up")) {
			AutomaticTimeScaleOn = false;
			Time.timeScale++;
			print("Increasing time scale to " + Time.timeScale);
		} else if (Input.GetKeyDown("down")) {
			AutomaticTimeScaleOn = false;
			if (Time.timeScale > 1) {
				Time.timeScale--;
				print("Lowering time scale to " + Time.timeScale);
			} else {
				print ("Cannot lower time scale to 0");
			}
		}

		if (Input.GetKeyDown ("right")) {
			AutomaticTimeScaleOn = true;
			print ("Automatic timescale enabled");
		}
	}

	public void StartEA()
	{
		char dirSepChar = Path.DirectorySeparatorChar;
		_resultSavePath = Application.persistentDataPath + dirSepChar + DateTime.Now.ToString("dd-MM-yy--HH-mm") + dirSepChar;
        print("Data save path: " + _resultSavePath);

        Utility.DebugLog = true;
        LocalLogger.Initialize(_resultSavePath);
        //Copy Settings to result folder
        string currentDir = Environment.CurrentDirectory;
		File.Copy(currentDir + dirSepChar + "Assets" + dirSepChar + "Resources" + dirSepChar + "experiment.config.xml", _resultSavePath + "experiment.config.xml", true);
        File.Copy(PropertiesReader.GetPropertyFilePath(PropertyFile.Project), _resultSavePath + "project.properties", true);
        File.Copy(PropertiesReader.GetPropertyFilePath(PropertyFile.NEAT), _resultSavePath + "neat.properties", true);


        Utility.Log("Starting FPS Agent experiment");

		VisualPartitionClassifier.GetInstance ().InitializeFromProperties ();
		FitnessMap = new Dictionary<IBlackBox, float> ();
		//_ea = experiment.CreateEvolutionAlgorithm(popFileSavePath);
		_ea = experiment.CreateEvolutionAlgorithm();
		_ea.UpdateEvent += new EventHandler(ea_UpdateEvent);
		var evoSpeed = 1;
		Started = true;
		Time.timeScale = evoSpeed;
		_ea.StartContinue();
		EARunning = true;
	}

	void ea_UpdateEvent(object sender, EventArgs e)
	{
		Fitness = _ea.Statistics._maxFitness;

		if (!FirstUpdate) {
			Utility.Log (string.Format ("gen={0:N0} bestFitness={1:N6}", _ea.CurrentGeneration, Fitness));
            LocalLogger.Write (string.Format("gen={0:N0} bestFitness={1:N6}", _ea.CurrentGeneration, Fitness));
        } else {
			FirstUpdate = false;
		}

		//Utility.Log(string.Format("Moving average: {0}, N: {1}", _ea.Statistics._bestFitnessMA.Mean, _ea.Statistics._bestFitnessMA.Length));
		//Utility.Log ("maxSpecieSize=" + _ea.Statistics._maxSpecieSize + "\nChampion id: " + _ea.CurrentChampGenome.Id);
		//Debug.Log("Champions specie id: " + _ea.CurrentChampGenome.SpecieIdx);

	}

	void PauseUnpause()
	{
		if (!EARunning) { //Unpause
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
		if (_ea.CurrentGeneration != 0){ // || ((_ea.CurrentGeneration - 1) % PersistNGenerations == 0 && _ea.CurrentGeneration != 1)) {
			PersistPopulation ();
		}
	}

	public void PersistPopulation(){
		string champFileSavePath = _resultSavePath + string.Format ("{0}/{1}.champ.xml", _ea.CurrentGeneration, "FPSAgent");
		string popFileSavePath = _resultSavePath + string.Format ("{0}/{1}.pop.xml", _ea.CurrentGeneration, "FPSAgent");

		XmlWriterSettings _xwSettings = new XmlWriterSettings ();
		_xwSettings.Indent = true;
		// Save genomes to xml file.        
		DirectoryInfo dirInf = new DirectoryInfo (_resultSavePath + string.Format ("{0}", _ea.CurrentGeneration));
		if (!dirInf.Exists) {
			dirInf.Create ();
		}

		if (PersistNGenerations != 0 && (_ea.CurrentGeneration == 1 || ((_ea.CurrentGeneration) % PersistNGenerations == 0 && _ea.CurrentGeneration != 0))) {
			using (XmlWriter xw = XmlWriter.Create (popFileSavePath, _xwSettings)) {
				experiment.SavePopulation (xw, _ea.GenomeList);
			}
		}
		// Also save the best genome
		using (XmlWriter xw = XmlWriter.Create (champFileSavePath, _xwSettings)) {
			experiment.SavePopulation (xw, new NeatGenome[] { _ea.CurrentChampGenome });
		}
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
		NEATArena arena = gameObject.AddComponent<NEATArena>();
		arena.WallPrefab = wallPrefab;
		arena.Init ();

		GameObject obj = arena.GetPlayer ();
		UnitController controller = obj.GetComponent<UnitController>();

		ControllerMap.Add (box, controller);
		ArenaMap.Add (box, arena);

		controller.Activate(box);
	}

	public void StopEvaluation(IBlackBox box)
	{
		UnitController ct = ControllerMap[box];
		NEATArena nt = ArenaMap [box];
		Debug.Log (nt.GetFitness ());
		if (!RunBestNetwork) {
			if (FitnessMap.ContainsKey (box)) {
				FitnessMap [box] = nt.GetFitness ();
			} else {
				FitnessMap.Add (box, nt.GetFitness ());
			}
		}
		ControllerMap.Remove (box);
		ArenaMap.Remove (box);

		Destroy (ct.gameObject);
		Destroy (nt);
	}

	public IBlackBox GetBestPhenome(){
		if (_ea.CurrentGeneration > 1) {
			Time.timeScale = 1;

			NeatGenome genome = null;
			string champFileLoadPath = _resultSavePath + string.Format ("{0}/{1}.champ.xml", _ea.CurrentGeneration - 1, "FPSAgent");
            //string champFileLoadPath = _resultSavePath + string.Format ("{0}/{1}.champ.xml", 248, "FPSAgent");

            // Try to load the genome from the XML document.
            try
            {
				using (XmlReader xr = XmlReader.Create (champFileLoadPath))
					genome = NeatGenomeXmlIO.ReadCompleteGenomeList (xr, false, (NeatGenomeFactory)experiment.CreateGenomeFactory ()) [0];
			} catch (Exception e1) {
				print (champFileLoadPath + " Error loading genome from file!\nLoading aborted.\n"
				+ e1.Message + "\nJoe: " + champFileLoadPath);
				return null;
			}

			// Get a genome decoder that can convert genomes to phenomes.
			var genomeDecoder = experiment.CreateGenomeDecoder ();

			// Decode the genome into a phenome (neural network).
			var phenome = genomeDecoder.Decode (genome);
			return phenome;
		}
		print ("Cannot run best network in first generation!");
		RunBestNetwork = false;
		return null;
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
		if (EARunning) {
			//StopEA not imlemented
			/*if (GUI.Button(new Rect(10, 60, 100, 40), "Stop EA"))
			{
				StopEA();
			}*/
			if (GUI.Button (new Rect (10, 160, 100, 40), "Pause EA")) {
				PauseUnpause ();
			}
			if (GUI.Button(new Rect(10, 110, 100, 40), "Run best"))
			{
				RunBestNetwork = true;
			}
			//Show current generation and max fitness from last generation
			GUI.Button(new Rect(10, Screen.height - 70, 150, 60), string.Format("Current generation: {0}\nMax fitness: {1:0.00}", _ea.CurrentGeneration, Fitness));
		} else {
			if (!Started) {
				if (GUI.Button (new Rect (10, 10, 100, 40), "Start EA")) {
					StartEA ();
				}
			}
			else {
				if (GUI.Button(new Rect(10, 110, 100, 40), "Run best"))
				{
					RunBestNetwork = true;
				}
				if (GUI.Button (new Rect (10, 160, 100, 40), "Resume EA")) {
					PauseUnpause ();
				}
			}
		}
	}

	private void InitFromConfig(XmlElement config) {
		trials = XmlUtils.GetValueAsInt (config, "TrialCount");
		trialDuration = Convert.ToSingle(XmlUtils.GetValueAsDouble (config, "TrialDuration"));
		stoppingFitness = Convert.ToSingle (XmlUtils.GetValueAsDouble (config, "StoppingFitness"));
		PersistNGenerations = XmlUtils.GetValueAsInt(config, "PersistNGenerations");
		NUM_INPUTS = XmlUtils.GetValueAsInt (config, "inputs");
		NUM_OUTPUTS = XmlUtils.GetValueAsInt (config, "outputs");
		MaxParallel = XmlUtils.GetValueAsInt (config, "parallelAgents");
	}
}