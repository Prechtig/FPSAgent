using System;
using SharpNeat.Domains;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Decoders;
using System.Xml;
using System.Collections.Generic;
using SharpNeat.Phenomes;
using SharpNeat.Core;
using SharpNeat.Decoders.Neat;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.DistanceMetrics;
using SharpNeat.SpeciationStrategies;
using SharpNEAT.Core;
using UnityEngine;
using Kajabity.Tools.Java;
using System.IO;

public class Experiment : INeatExperiment
{
	NeatEvolutionAlgorithmParameters _eaParams;
	NeatGenomeParameters _neatGenomeParams;
	string _name;
	int _populationSize;
	int _specieCount;
	NetworkActivationScheme _activationScheme;
	string _complexityRegulationStr;
	int? _complexityThreshold;
	string _description;
	Optimizer _optimizer;
	int _inputCount;
	int _outputCount;
    JavaProperties _neatParameters;

	public string Name
	{
		get { return _name; }
	}

	public string Description
	{
		get { return _description; }
	}

	public int InputCount
	{
		get { return _inputCount; }
	}

	public int OutputCount
	{
		get { return _outputCount; }
	}

	public int DefaultPopulationSize
	{
		get { return _populationSize; }
	}

	public NeatEvolutionAlgorithmParameters NeatEvolutionAlgorithmParameters
	{
		get { return _eaParams; }
	}

	public NeatGenomeParameters NeatGenomeParameters
	{
		get { return _neatGenomeParams; }
	}

	public void SetOptimizer(Optimizer se)
	{
		this._optimizer = se;
	}


	public void Initialize(string name, XmlElement xmlConfig)
	{
		Initialize(name, xmlConfig, 1, 6);
	}

	public void Initialize(string name, XmlElement xmlConfig, int input, int output)
	{
		_name = name;
		_populationSize = XmlUtils.GetValueAsInt(xmlConfig, "PopulationSize");
		_specieCount = XmlUtils.GetValueAsInt(xmlConfig, "SpecieCount");
		_activationScheme = ExperimentUtils.CreateActivationScheme(xmlConfig, "Activation");
		_complexityRegulationStr = XmlUtils.TryGetValueAsString(xmlConfig, "ComplexityRegulationStrategy");
		_complexityThreshold = XmlUtils.TryGetValueAsInt(xmlConfig, "ComplexityThreshold");
		_description = XmlUtils.TryGetValueAsString(xmlConfig, "Description");


        _neatParameters = PropertiesReader.GetPropertyFile(PropertyFile.NEAT);
        _eaParams = new NeatEvolutionAlgorithmParameters();
        _eaParams.SpecieCount = _specieCount;
        _eaParams.ElitismProportion = Double.Parse(_neatParameters.GetProperty("ElitismProportion"));
        _eaParams.SelectionProportion = Double.Parse(_neatParameters.GetProperty("SelectionProportion"));
        _eaParams.OffspringAsexualProportion = Double.Parse(_neatParameters.GetProperty("OffspringAsexualProportion"));
        _eaParams.OffspringSexualProportion = Double.Parse(_neatParameters.GetProperty("OffspringSexualProportion"));
        _eaParams.InterspeciesMatingProportion = Double.Parse(_neatParameters.GetProperty("InterspeciesMatingProportion"));
        _eaParams.BestFitnessMovingAverageHistoryLength = Int32.Parse(_neatParameters.GetProperty("BestFitnessMovingAverageHistoryLength"));
        _eaParams.ComplexityMovingAverageHistoryLength = Int32.Parse(_neatParameters.GetProperty("ComplexityMovingAverageHistoryLength"));
        _eaParams.MeanSpecieChampFitnessMovingAverageHistoryLength = Int32.Parse(_neatParameters.GetProperty("MeanSpecieChampFitnessMovingAverageHistoryLength"));

        _neatGenomeParams = new NeatGenomeParameters();
        _neatGenomeParams.FeedforwardOnly = _activationScheme.AcyclicNetwork; //Is it correct that the network is feed forwrd?
        _neatGenomeParams.ConnectionWeightRange = Double.Parse(_neatParameters.GetProperty("ConnectionWeightRange"));
        _neatGenomeParams.InitialInterconnectionsProportion = Double.Parse(_neatParameters.GetProperty("InitialInterconnectionsProportion"));
        _neatGenomeParams.DisjointExcessGenesRecombinedProbability = Double.Parse(_neatParameters.GetProperty("DisjointExcessGenesRecombinedProbability"));
        _neatGenomeParams.ConnectionWeightMutationProbability = Double.Parse(_neatParameters.GetProperty("ConnectionWeightMutationProbability"));
        _neatGenomeParams.AddNodeMutationProbability = Double.Parse(_neatParameters.GetProperty("AddNodeMutationProbability"));
        _neatGenomeParams.AddConnectionMutationProbability = Double.Parse(_neatParameters.GetProperty("AddConnectionMutationProbability"));
        _neatGenomeParams.DeleteConnectionMutationProbability = Double.Parse(_neatParameters.GetProperty("DeleteConnectionMutationProbability"));
        _neatGenomeParams.NodeAuxStateMutationProbability = Double.Parse(_neatParameters.GetProperty("NodeAuxStateMutationProbability"));

        _inputCount = input;
		_outputCount = output;
	}

	public List<NeatGenome> LoadPopulation(XmlReader xr)
	{
		NeatGenomeFactory genomeFactory = (NeatGenomeFactory)CreateGenomeFactory();
		return NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, genomeFactory);
	}

	public void SavePopulation(XmlWriter xw, IList<NeatGenome> genomeList)
	{
		NeatGenomeXmlIO.WriteComplete(xw, genomeList, false);
	}

	public IGenomeDecoder<NeatGenome, IBlackBox> CreateGenomeDecoder()
	{
		return new NeatGenomeDecoder(_activationScheme);
	}

	public IGenomeFactory<NeatGenome> CreateGenomeFactory()
	{
		return new NeatGenomeFactory(InputCount, OutputCount, _neatGenomeParams);
	}

	public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(string fileName)
	{
		List<NeatGenome> genomeList = null;
		IGenomeFactory<NeatGenome> genomeFactory = CreateGenomeFactory();
		try
		{
			if (fileName.Contains("/.pop.xml"))
			{
				throw new Exception();
			}
			using (XmlReader xr = XmlReader.Create(fileName))
			{
				genomeList = LoadPopulation(xr);
			}
		}
		catch (Exception e1)
		{
			Utility.Log(fileName + " Error loading genome from file!\nLoading aborted.\n"
				+ e1.Message + "\nJoe: " + fileName);

			genomeList = genomeFactory.CreateGenomeList(_populationSize, 0);

		}



		return CreateEvolutionAlgorithm(genomeFactory, genomeList);
	}

	public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm()
	{
		return CreateEvolutionAlgorithm(_populationSize);
	}

	public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(int populationSize)
	{
		IGenomeFactory<NeatGenome> genomeFactory = CreateGenomeFactory();

		List<NeatGenome> genomeList = genomeFactory.CreateGenomeList(populationSize, 0);

		return CreateEvolutionAlgorithm(genomeFactory, genomeList);
	}

	public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(IGenomeFactory<NeatGenome> genomeFactory, List<NeatGenome> genomeList)
	{
        double c1 = Double.Parse(_neatParameters.GetProperty("c1"));
        double c2 = Double.Parse(_neatParameters.GetProperty("c2"));
        double c3 = Double.Parse(_neatParameters.GetProperty("c3"));
        IDistanceMetric distanceMetric = new ManhattanDistanceMetric(c1, c2, c3);
		ISpeciationStrategy<NeatGenome> speciationStrategy = new KMeansClusteringStrategy<NeatGenome>(distanceMetric);
		//ISpeciationStrategy<NeatGenome> speciationStrategy = new RandomClusteringStrategy<NeatGenome>();

		IComplexityRegulationStrategy complexityRegulationStrategy = ExperimentUtils.CreateComplexityRegulationStrategy(_complexityRegulationStr, _complexityThreshold);

		NeatEvolutionAlgorithm<NeatGenome> ea = new NeatEvolutionAlgorithm<NeatGenome>(_eaParams, speciationStrategy, complexityRegulationStrategy);

		// Create black box evaluator       
		Evaluator evaluator = new Evaluator(_optimizer);

		IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder = CreateGenomeDecoder();


		//IGenomeListEvaluator<NeatGenome> innerEvaluator = new UnityListEvaluator<NeatGenome, IBlackBox>(genomeDecoder, evaluator, _optimizer);
		IGenomeListEvaluator<NeatGenome> innerEvaluator = new UnityParallelSequentialEvaluator<NeatGenome, IBlackBox>(genomeDecoder, evaluator, _optimizer);


		new SelectiveGenomeListEvaluator<NeatGenome>(innerEvaluator,
			SelectiveGenomeListEvaluator<NeatGenome>.CreatePredicate_OnceOnly());

		//ea.Initialize(selectiveEvaluator, genomeFactory, genomeList);
		ea.Initialize(innerEvaluator, genomeFactory, genomeList);

		return ea;
	}
}

