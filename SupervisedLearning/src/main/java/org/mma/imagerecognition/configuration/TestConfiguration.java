package org.mma.imagerecognition.configuration;

import org.deeplearning4j.nn.conf.MultiLayerConfiguration.Builder;
import org.deeplearning4j.nn.conf.layers.ConvolutionLayer;
import org.deeplearning4j.nn.conf.layers.DenseLayer;
import org.deeplearning4j.nn.conf.layers.OutputLayer;
import org.deeplearning4j.nn.conf.layers.SubsamplingLayer;
import org.deeplearning4j.nn.conf.layers.setup.ConvolutionLayerSetup;
import org.deeplearning4j.nn.api.OptimizationAlgorithm;
import org.deeplearning4j.nn.conf.MultiLayerConfiguration;
import org.deeplearning4j.nn.conf.NeuralNetConfiguration;
import org.deeplearning4j.nn.conf.Updater;
import org.deeplearning4j.nn.weights.WeightInit;
import org.nd4j.linalg.lossfunctions.LossFunctions;

public class TestConfiguration {

	private static int seed = 98;

	private static int iterations = 1;
	private static double learningRate = 0.02;
	private static String activationFunction = "relu";
	private static WeightInit weightInit = WeightInit.XAVIER;
	private static OptimizationAlgorithm optimizationAlgorithm = OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT;
	private static Updater updater = Updater.NESTEROVS;
	private static double momentum = 0.9;

	// Regularization
	private static double l2regularization = 0.0005;

	public static void main(String[] args) {
		train();
	}

	public static void train() {
		Builder builder = new NeuralNetConfiguration.Builder()
				.seed(seed)
				.iterations(iterations)
				.regularization(true)
				.l2(l2regularization)
				.learningRate(learningRate)
				.weightInit(weightInit)
				.activation(activationFunction)
				.optimizationAlgo(optimizationAlgorithm)
				.updater(Updater.NESTEROVS).momentum(momentum)
				.list()
				.layer(0, new ConvolutionLayer.Builder(5, 5)
						.nIn(1)
						.stride(1, 1)
						.nOut(20)
						.dropOut(0.5)
						.build())
				.layer(1,
						new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
						.kernelSize(2, 2)
						.stride(2, 2)
						.build())
				.layer(2, new DenseLayer.Builder()
						.nOut(20)
						.build())
				.layer(3, new OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)
						.nOut(10)
						.activation("softmax").build())
				.backprop(true).pretrain(false);
		new ConvolutionLayerSetup(builder, 28, 28, 1);
		MultiLayerConfiguration configuration = builder.build();
		
		String json = configuration.toJson();
	}
}
