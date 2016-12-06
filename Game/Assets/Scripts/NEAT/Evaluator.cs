using UnityEngine;
using System.Collections;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using System.Collections.Generic;

public class Evaluator : IPhenomeEvaluator<IBlackBox> {

	ulong _evalCount;
	bool _stopConditionSatisfied;
	Optimizer optimizer;
	FitnessInfo fitness;

	public static int RunCount = 0;

	Dictionary<IBlackBox, FitnessInfo> dict = new Dictionary<IBlackBox, FitnessInfo>();

	public ulong EvaluationCount
	{
		get { return _evalCount; }
	}

	public bool StopConditionSatisfied
	{
		get { return _stopConditionSatisfied; }
	}

	public Evaluator(Optimizer se)
	{
		this.optimizer = se;
	}

	public IEnumerator Evaluate(IBlackBox box)
	{
		if (optimizer != null) {
			optimizer.Evaluate (box);
			yield return new WaitForSeconds (optimizer.TrialDuration);
			optimizer.StopEvaluation (box);

			if (!optimizer.RunBestNetwork) {
				float fit = optimizer.GetFitness (box).First;
                float auxFit = optimizer.GetFitness(box).Second;

                FitnessInfo fitness = new FitnessInfo (fit, auxFit);
				dict.Add (box, fitness);
			}
		} else {
			Debug.Log ("test");
		}
		RunCount--;
	}

	public void Reset()
	{
		this.fitness = FitnessInfo.Zero;
		dict = new Dictionary<IBlackBox, FitnessInfo>();
	}

	public FitnessInfo GetLastFitness()
	{
		return this.fitness;
	}


	public FitnessInfo GetLastFitness(IBlackBox phenome)
	{
		if (dict.ContainsKey(phenome))
		{
			FitnessInfo fit = dict[phenome];
			dict.Remove(phenome);

			return fit;
		}
		return FitnessInfo.Zero;
	}
}