package org.mma.imagerecognition.configuration;

import org.deeplearning4j.nn.api.OptimizationAlgorithm;
import org.deeplearning4j.nn.conf.MultiLayerConfiguration.Builder;
import org.deeplearning4j.nn.conf.NeuralNetConfiguration;
import org.deeplearning4j.nn.conf.Updater;
import org.deeplearning4j.nn.conf.layers.ConvolutionLayer;
import org.deeplearning4j.nn.conf.layers.DenseLayer;
import org.deeplearning4j.nn.conf.layers.OutputLayer;
import org.deeplearning4j.nn.conf.layers.SubsamplingLayer;
import org.deeplearning4j.nn.conf.layers.setup.ConvolutionLayerSetup;
import org.deeplearning4j.nn.weights.WeightInit;
import org.nd4j.linalg.lossfunctions.LossFunctions;

public class BuilderFactory {
	public static Builder getDeepConvNet(int height, int width, int featureCount) {
		Builder builder = new NeuralNetConfiguration.Builder()
		.seed(98)
		.iterations(1)
		.regularization(true)
		.l2(0.0005)
		.learningRate(0.008)
		.weightInit(WeightInit.XAVIER)
		.activation("relu")
		.optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT)
		.updater(Updater.NESTEROVS).momentum(0.9)
		.list()
		.layer(0, new ConvolutionLayer.Builder(3, 3)
				.nIn(1)
				.stride(1, 1)
				.nOut(20)
				.padding(1,1)
				.dropOut(0.5)
				.activation("relu")
				.build())
		.layer(1,
				new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
				.kernelSize(2, 2)
				.stride(2, 2)
				.build())
		.layer(2, new ConvolutionLayer.Builder(3, 3)
				.nIn(1)
				.stride(1, 1)
				.nOut(20)
				.padding(1,1)
				.dropOut(0.5)
				.activation("relu")
				.build())
		.layer(3,
				new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
				.kernelSize(2, 2)
				.stride(2, 2)
				.build())
		.layer(4, new ConvolutionLayer.Builder(3, 3)
				.nIn(1)
				.stride(1, 1)
				.nOut(20)
				.padding(1,1)
				.dropOut(0.5)
				.activation("relu")
				.build())
		.layer(5, new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
				.kernelSize(2, 2)
				.stride(2, 2)
				.build())
		.layer(6, new DenseLayer.Builder()
				.nOut(featureCount)
				.activation("sigmoid")
				.build())
		.layer(7, new OutputLayer.Builder(LossFunctions.LossFunction.SQUARED_LOSS)
				.nOut(featureCount)
				.activation("identity")
				.build())
		.backprop(true).pretrain(false);
		new ConvolutionLayerSetup(builder, height, width, 3);
		return builder;
	}
	
	public static Builder getConvNet(int height, int width, int featureCount) {
		Builder builder = new NeuralNetConfiguration.Builder()
		.seed(98)
		.iterations(1)
		.regularization(true)
		.l2(0.0005)
		.learningRate(0.008)
		.weightInit(WeightInit.XAVIER)
		.activation("relu")
		.optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT)
		.updater(Updater.NESTEROVS).momentum(0.9)
		.list()
		.layer(0, new ConvolutionLayer.Builder(5, 5)
				.nIn(1)
				.stride(2, 2)
				.nOut(5)
				.padding(1,1)
				.dropOut(0.5)
				.activation("relu")
				.build())
		.layer(1,
				new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
				.kernelSize(2, 2)
				.stride(2, 2)
				.build())
		.layer(2, new ConvolutionLayer.Builder(3, 3)
				.nIn(1)
				.stride(2, 2)
				.nOut(5)
				.padding(1,1)
				.dropOut(0.5)
				.activation("relu")
				.build())
		.layer(3,
				new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
				.kernelSize(2, 2)
				.stride(2, 2)
				.build())
		.layer(6, new DenseLayer.Builder()
				.nOut(200)
				.activation("sigmoid")
				.build())
		.layer(7, new OutputLayer.Builder(LossFunctions.LossFunction.SQUARED_LOSS)
				.nOut(featureCount)
				.activation("identity")
				.build())
		.backprop(true).pretrain(false);
		new ConvolutionLayerSetup(builder, height, width, 3);
		return builder;
	}
}
