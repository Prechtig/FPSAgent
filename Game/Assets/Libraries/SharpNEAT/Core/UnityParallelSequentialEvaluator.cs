using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using System.Collections;
using UnityEngine;

namespace SharpNEAT.Core
{
	class UnityParallelSequentialEvaluator<TGenome, TPhenome> : IGenomeListEvaluator<TGenome>
		where TGenome : class, IGenome<TGenome>
		where TPhenome : class
	{

		readonly IGenomeDecoder<TGenome, TPhenome> _genomeDecoder;
		IPhenomeEvaluator<TPhenome> _phenomeEvaluator;
		//readonly IPhenomeEvaluator<TPhenome> _phenomeEvaluator;
		Optimizer _optimizer;

		#region Constructor

		/// <summary>
		/// Construct with the provided IGenomeDecoder and IPhenomeEvaluator.
		/// </summary>
		public UnityParallelSequentialEvaluator (IGenomeDecoder<TGenome, TPhenome> genomeDecoder,
		                                       IPhenomeEvaluator<TPhenome> phenomeEvaluator,
		                                       Optimizer opt)
		{
			_genomeDecoder = genomeDecoder;
			_phenomeEvaluator = phenomeEvaluator;
			_optimizer = opt;
		}

		#endregion

		public ulong EvaluationCount {
			get { return _phenomeEvaluator.EvaluationCount; }
		}

		public bool StopConditionSatisfied {
			get { return _phenomeEvaluator.StopConditionSatisfied; }
		}

		public IEnumerator Evaluate (IList<TGenome> genomeList)
		{
			yield return Coroutiner.StartCoroutine (evaluateList (genomeList));
		}

		//The one we use, Mikkel
		private IEnumerator evaluateList (IList<TGenome> genomeList)
		{
			/* Generation timer
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch ();
			sw.Start ();
			*/

			Dictionary<TGenome, TPhenome> dict = new Dictionary<TGenome, TPhenome> ();
			Dictionary<TGenome, FitnessInfo[]> fitnessDict = new Dictionary<TGenome, FitnessInfo[]> ();
			int maxParallel = _optimizer.MaxParallel;
			int parallel = 0;
			if (genomeList.Count % maxParallel != 0) {
				Debug.Log ("Population do not match the max parallel setting!!!!!");
			}
			for (int i = 0; i < _optimizer.Trials; i++) {
				_phenomeEvaluator.Reset ();

				dict = new Dictionary<TGenome, TPhenome> ();
				foreach (TGenome genome in genomeList) {
					TPhenome phenome = _genomeDecoder.Decode (genome);
					if (null == phenome) {   // Non-viable genome.
						genome.EvaluationInfo.SetFitness (0.0);
						genome.EvaluationInfo.AuxFitnessArr = null;
					} else {
						if (i == 0) {
							fitnessDict.Add (genome, new FitnessInfo[_optimizer.Trials]);
						}
						dict.Add (genome, phenome);
						//if (!dict.ContainsKey(genome))
						//{
						//    dict.Add(genome, phenome);
						//    fitnessDict.Add(phenome, new FitnessInfo[_optimizer.Trials]);
						//}

						if (parallel < maxParallel-1) {
							Coroutiner.StartCoroutine (_phenomeEvaluator.Evaluate (phenome));
							parallel++;
						} else {
							yield return Coroutiner.StartCoroutine(_phenomeEvaluator.Evaluate(phenome));
							//BotSpawn.iteration++;
							parallel = 0;
							NEATArena.ResetYOffset ();
						}
					}
				}

				//yield return new WaitForSeconds (_optimizer.TrialDuration);

				foreach (TGenome genome in dict.Keys) {
					TPhenome phenome = dict [genome];
					if (phenome != null) {

						FitnessInfo fitnessInfo = _phenomeEvaluator.GetLastFitness (phenome);

						fitnessDict [genome] [i] = fitnessInfo;
					}
				}
			}
			foreach (TGenome genome in dict.Keys) {
				TPhenome phenome = dict [genome];
				if (phenome != null) {
					double fitness = 0;

					for (int i = 0; i < _optimizer.Trials; i++) {

						fitness += fitnessDict [genome] [i]._fitness;

					}
					var fit = fitness;
					fitness /= _optimizer.Trials; // Averaged fitness

					if (fit > _optimizer.StoppingFitness) {
						//  Utility.Log("Fitness is " + fit + ", stopping now because stopping fitness is " + _optimizer.StoppingFitness);
						//  _phenomeEvaluator.StopConditionSatisfied = true;
					}
					genome.EvaluationInfo.SetFitness (fitness);
					genome.EvaluationInfo.AuxFitnessArr = fitnessDict [genome] [0]._auxFitnessArr;
				}
			}

			/* Generation timer
			sw.Stop();
			// Get the elapsed time as a TimeSpan value.
			TimeSpan ts = sw.Elapsed;
			// Format and display the TimeSpan value.
			string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
				ts.Hours, ts.Minutes, ts.Seconds,
				ts.Milliseconds / 10);
			Debug.Log("Generation time " + elapsedTime);
			*/
		}

		public void Reset ()
		{
			_phenomeEvaluator.Reset ();
		}
	}
}