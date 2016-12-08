using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using System.Collections;
using UnityEngine;
using SharpNeat.Phenomes;

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
            Dictionary<TGenome, TPhenome> dict = new Dictionary<TGenome, TPhenome> ();
			Dictionary<TGenome, FitnessInfo[]> fitnessDict = new Dictionary<TGenome, FitnessInfo[]> ();

			int maxParallel = _optimizer.MaxParallel;
			int parallel = 0;
			if (genomeList.Count % maxParallel != 0) {
				Debug.Log ("Population do not match the max-parallel setting!!!!!");
			}
			for (int i = 0; i < _optimizer.Trials; i++) {
				_phenomeEvaluator.Reset ();
				dict = new Dictionary<TGenome, TPhenome> ();
				foreach (TGenome genome in genomeList) {
					//Run best netowork
					if (_optimizer.RunBestNetwork && parallel == 0) {
                        for (int j = 0; j < _optimizer.RunBestCount * 2; j++)
                        {
                            _optimizer.BestNetworkIsRunning = true;

                            float timeScale = Time.timeScale;
                            IBlackBox bestBlackBox = _optimizer.GetBestPhenome();
                            if (bestBlackBox != null)
                            {
                                TPhenome bestPhenome = (TPhenome)bestBlackBox;
                                yield return Coroutiner.StartCoroutine(_phenomeEvaluator.Evaluate(bestPhenome));
                                Evaluator.RunCount++;
                                Time.timeScale = timeScale;
                            }
                        }
                        _optimizer.RunBestNetwork = false;
                    }

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

						if (parallel != maxParallel - 1) {
							Evaluator.RunCount++;
							Coroutiner.StartCoroutine (_phenomeEvaluator.Evaluate (phenome));
							parallel++;
						} else {
							Evaluator.RunCount++;
							Coroutiner.StartCoroutine (_phenomeEvaluator.Evaluate (phenome));
							while(Evaluator.RunCount != 0){
								yield return null;
							}
							parallel = 0;
							NEATArena.ResetYOffset ();
						}
					}
				}
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
                    double auxFitness = 0;

                    for (int i = 0; i < _optimizer.Trials; i++) {
						fitness += fitnessDict [genome] [i]._fitness;
                        auxFitness += fitnessDict[genome][i]._auxFitnessArr[0]._value;
                    }
                    fitness /= _optimizer.Trials; // Averaged fitness
                    auxFitness /= _optimizer.Trials;

                    AuxFitnessInfo[] aux = new AuxFitnessInfo[1];
                    aux[0] = new AuxFitnessInfo("Running fitness", auxFitness);

                    genome.EvaluationInfo.SetFitness (fitness);
					genome.EvaluationInfo.AuxFitnessArr = aux;
				}
			}
        }

        public void Reset ()
		{
			_phenomeEvaluator.Reset ();
		}
	}
}