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
	public GameObject floorPrefab;

	private int NUM_INPUTS;
	private int NUM_OUTPUTS;

	bool EARunning;
	//string popFileSavePath, champFileSavePath;

	static Experiment experiment;
	static NeatEvolutionAlgorithm<NeatGenome> _ea;

	public GameObject Unit;
	public bool RunBestNetwork = false;
	public int MaxParallel;
	//public NEATArena arena;

	Dictionary<IBlackBox, UnitController> ControllerMap = new Dictionary<IBlackBox, UnitController>();
	Dictionary<IBlackBox, NEATArena> ArenaMap = new Dictionary<IBlackBox, NEATArena>();
	Dictionary<IBlackBox, Pair<float, float>> FitnessMap = new Dictionary<IBlackBox, Pair<float, float>>();

    Dictionary<uint, int> WrongReloadsMap = new Dictionary<uint, int>();
    Dictionary<uint, int> ShotsMap = new Dictionary<uint, int>();
    Dictionary<uint, int> MissedMap = new Dictionary<uint, int>();

    //private DateTime startTime;
    private float timeLeft;
	private float accum;
	private int frames;
	private float updateInterval = 12;

	private double Fitness;
	private float PreviousTimeScale;
	private static int PersistNGenerations;
	//Persist populationprivate Boolean Persisted;

	private int trials;
	private float trialDuration;
	private float stoppingFitness;

	private bool AutomaticTimeScaleOn = "true".Equals(PropertiesReader.GetPropertyFile(PropertyFile.Project).GetProperty("game.neat.training.automaticTimescale"));
	private bool Started = false;
	private bool FirstUpdate = true;

    private static string _resultSavePath;
    public bool BestNetworkIsRunning = false;
    private float _runBestTime = 1;
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
        if (Input.GetKeyDown("left"))
        {
            if (_runBestTime == 25)
            {
                _runBestTime = 1;
            }
            else
            {
                _runBestTime = 25;
            }
            print("_runBestTime = " + _runBestTime);
        }
    }

	public void StartEA()
	{
        //QualitySettings.vSyncCount = 0;  // VSync must be disabled
        //Application.targetFrameRate = 30;

        char dirSepChar = Path.DirectorySeparatorChar;
		_resultSavePath = Application.persistentDataPath + dirSepChar + DateTime.Now.ToString("dd-MM-yy--HH-mm-ss") + dirSepChar;
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

        /*
        string folderName = "14-12-16--15-13-45"; // angualar no recoil tha bomb
        string generationName = "505";
        */
        /*
        string folderName = "15-12-16--09-13-16";  //fixedUpdate
        string generationName = "263";  //fixedupdate
        
        string location = Application.persistentDataPath + dirSepChar + folderName + dirSepChar + generationName + dirSepChar + "FPSAgent.champ.xml";
        //string location = Application.persistentDataPath + dirSepChar + folderName + dirSepChar + generationName + dirSepChar + "FPSAgent.pop.xml";
        _ea = experiment.CreateEvolutionAlgorithm(location);
        */
        _ea = experiment.CreateEvolutionAlgorithm();
        _ea.UpdateEvent += new EventHandler(ea_UpdateEvent);
		var evoSpeed = int.Parse (PropertiesReader.GetPropertyFile(PropertyFile.Project).GetProperty("game.neat.training.evolutionSpeed"));
        
        Started = true;
		Time.timeScale = evoSpeed;
		_ea.StartContinue();
		EARunning = true;
	}

	void ea_UpdateEvent(object sender, EventArgs e)
	{
		Fitness = _ea.Statistics._maxFitness;

		if (!FirstUpdate) {
            uint id = _ea.CurrentChampGenome.Id;
			Utility.Log (string.Format ("gen={0:N0} bestFitness={1:N6}", _ea.CurrentGeneration, Fitness));
            LocalLogger.Write (string.Format("{0:N0}\t{1:N6}\t{2:N6}\t{3:N6}\t{4:N0}\t{5:N0}\t{6:N0}",
                _ea.CurrentGeneration, Fitness, 
                Fitness - _ea.CurrentChampGenome.EvaluationInfo.AuxFitnessArr[0]._value, 
                _ea.CurrentChampGenome.EvaluationInfo.AuxFitnessArr[0]._value,
                ShotsMap[id],
                MissedMap[id],
                WrongReloadsMap[id]));
        } else {
            LocalLogger.Write(string.Format("Generation\tFitness\tShooting fitness\tAiming fitness\tShots\tMisses\tWrong reloads"));
            FirstUpdate = false;
		}

        //Utility.Log(string.Format("Moving average: {0}, N: {1}", _ea.Statistics._bestFitnessMA.Mean, _ea.Statistics._bestFitnessMA.Length));
        //Utility.Log ("maxSpecieSize=" + _ea.Statistics._maxSpecieSize + "\nChampion id: " + _ea.CurrentChampGenome.Id);
        //Debug.Log("Champions specie id: " + _ea.CurrentChampGenome.SpecieIdx);
        FitnessMap = new Dictionary<IBlackBox, Pair<float, float>>();
        WrongReloadsMap = new Dictionary<uint, int>();
        ShotsMap = new Dictionary<uint, int>();
        MissedMap = new Dictionary<uint, int>();
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
	public static void PersistPopulation(){
		string champFileSavePath = _resultSavePath + string.Format ("{0}/{1}.champ.xml", _ea.CurrentGeneration - 1, "FPSAgent");
		string popFileSavePath = _resultSavePath + string.Format ("{0}/{1}.pop.xml", _ea.CurrentGeneration - 1, "FPSAgent");

		XmlWriterSettings _xwSettings = new XmlWriterSettings ();
		_xwSettings.Indent = true;
		// Save genomes to xml file.        
		DirectoryInfo dirInf = new DirectoryInfo (_resultSavePath + string.Format ("{0}", _ea.CurrentGeneration - 1));
		if (!dirInf.Exists) {
			dirInf.Create ();
		}

		if (PersistNGenerations != 0 && (_ea.CurrentGeneration - 1 == 1 || ((_ea.CurrentGeneration - 1) % PersistNGenerations == 0 && _ea.CurrentGeneration - 1 != 0))) {
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
		arena.FloorPrefab = floorPrefab;
		arena.Init ();

		GameObject obj = arena.GetPlayer ();
		UnitController controller = obj.GetComponent<UnitController>();

		ControllerMap.Add (box, controller);
		ArenaMap.Add (box, arena);

		controller.Activate(box);
	}

    public int RunBestCount = 1;
    private int runs = 0;
    private float bestFitness = 0;
    public void StopEvaluation(IBlackBox box)
	{
		UnitController ct = ControllerMap[box];
		NEATArena nt = ArenaMap [box];

        if (BestNetworkIsRunning)
        {
            bestFitness += nt.GetFitness().First;
            runs++;
            if (runs == RunBestCount)
            {
                Debug.Log("Results for speed: " + _runBestTime);
                Debug.Log("Best Network fitness: " + bestFitness / RunBestCount);
                runs = 0;
                bestFitness = 0;
                if (_runBestTime == 1)
                {
                    _runBestTime = 25;
                }
                else
                {
                    _runBestTime = 1;
                }
                print("Changing _runBestTime = " + _runBestTime);
            }
            BestNetworkIsRunning = false;
            AutomaticTimeScaleOn = prevAutomaticTimeScale;
        }
        else
        {
            //Add statistics
            NEATWeapon nw = nt.GetPlayer().GetComponentInChildren<NEATWeapon>();

            //Add wrong reloads
            if (WrongReloadsMap.ContainsKey(box.Id))
            {
                WrongReloadsMap[box.Id] += nw.WrongReloads;
            }
            else
            {
                WrongReloadsMap.Add(box.Id, nw.WrongReloads);
            }
            //Add shots
            if (ShotsMap.ContainsKey(box.Id))
            {
                ShotsMap[box.Id] += nw.Shots;
            }
            else
            {
                ShotsMap.Add(box.Id, nw.Shots);
            }
            //Add misses
            if (MissedMap.ContainsKey(box.Id))
            {
                MissedMap[box.Id] += nw.Misses;
            }
            else
            {
                MissedMap.Add(box.Id, nw.Misses);
            }


            //SharpNeat.Core.TPhenome bestPhenome = (TPhenome)box;
            if (FitnessMap.ContainsKey(box))
            {
                FitnessMap[box] = nt.GetFitness();
            }
            else
            {
                FitnessMap.Add(box, nt.GetFitness());
            }
        }

        ControllerMap.Remove (box);
		ArenaMap.Remove (box);

		Destroy (ct.gameObject);
		Destroy (nt);
	}
    
    private bool prevAutomaticTimeScale;
	public IBlackBox GetBestPhenome(){
		if (_ea.CurrentGeneration > 1) {
			Time.timeScale = _runBestTime;
            prevAutomaticTimeScale = AutomaticTimeScaleOn;
            AutomaticTimeScaleOn = false;

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

            /*
            if()
            string s = "";
            foreach (var item in genome.ConnectionList)
            {
                s += item.SourceNodeId + " - " + item.TargetNodeId + " - " + item.Weight + "\n";
            }

            Debug.Log(s);
            */
            return phenome;
		}
		print ("Cannot run best network in first generation!");
		RunBestNetwork = false;
		return null;
	}

	public Pair<float, float> GetFitness(IBlackBox box)
	{
		if (FitnessMap.ContainsKey (box)) {
			return FitnessMap [box];
		}
		return null;
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
            if (runs != 0)
            {
                GUI.Button(new Rect(300, Screen.height - 70, 150, 60), string.Format("Current run: {0}\nCurrent average fitness: {1}", runs, bestFitness/runs));
            }
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
		//NUM_INPUTS = XmlUtils.GetValueAsInt (config, "inputs");
		NUM_OUTPUTS = XmlUtils.GetValueAsInt (config, "outputs");
		MaxParallel = XmlUtils.GetValueAsInt (config, "parallelAgents");

        NEATWeapon.recoil = "true".Equals(PropertiesReader.GetPropertyFile(PropertyFile.Project).GetProperty("game.neat.training.use.recoil"));
        bool useVPR = "true".Equals(PropertiesReader.GetPropertyFile(PropertyFile.Project).GetProperty("game.neat.training.use.vpr"));
        NEATController.UseVPR = useVPR;
        if (useVPR)
        {
            NUM_INPUTS = 26;
        }
        else
        {
            NUM_INPUTS = 6;
        }
    }
}