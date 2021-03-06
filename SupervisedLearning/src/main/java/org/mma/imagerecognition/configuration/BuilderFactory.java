package org.mma.imagerecognition.configuration;

import java.util.Properties;

import org.deeplearning4j.nn.api.OptimizationAlgorithm;
import org.deeplearning4j.nn.conf.MultiLayerConfiguration.Builder;
import org.deeplearning4j.nn.conf.NeuralNetConfiguration;
import org.deeplearning4j.nn.conf.Updater;
import org.deeplearning4j.nn.conf.inputs.InputType;
import org.deeplearning4j.nn.conf.layers.ConvolutionLayer;
import org.deeplearning4j.nn.conf.layers.DenseLayer;
import org.deeplearning4j.nn.conf.layers.OutputLayer;
import org.deeplearning4j.nn.conf.layers.SubsamplingLayer;
import org.deeplearning4j.nn.weights.WeightInit;
import org.mma.imagerecognition.tools.PropertiesReader;
import org.nd4j.linalg.lossfunctions.LossFunctions;

public class BuilderFactory {
	private static double learningRate = 0.0;
	private static double l2 = 0.0;
	private static int seed = 0;
	private static boolean regularization = false;
	
	static {
		Properties pp = PropertiesReader.getProjectProperties();
		learningRate = Double.parseDouble(pp.getProperty("training.learningRate"));
		l2 = Double.parseDouble(pp.getProperty("training.l2"));
		seed = Integer.parseInt(pp.getProperty("training.seed"));
		regularization = Boolean.valueOf(pp.getProperty("training.regularization"));
	}
	
	public static Builder getVeryShallowConvNet(int height, int width, int featureCount) {
		int layerId = 0;
		
		Builder builder = new NeuralNetConfiguration.Builder()
		.seed(seed)
		.iterations(1)
		.regularization(regularization)
		.l2(l2)
		.learningRate(learningRate)
		.weightInit(WeightInit.XAVIER)
		.activation("leakyrelu")
		.optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT)
		.updater(Updater.NESTEROVS).momentum(0.9)
		.list()
		.layer(layerId++, new ConvolutionLayer.Builder(16, 16)
				.nIn(3)
				.stride(4, 4)
				.nOut(40)
				.padding(6,6)
				.build())
		.layer(layerId++,
				new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
				.kernelSize(2, 2)
				.stride(2, 2)
				.build())
		.layer(layerId++, new DenseLayer.Builder()
				.nOut(100)
				.build())
		.layer(layerId++, new OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)
				.activation("softmax")
				.nOut(featureCount)
				.build())
		.backprop(true).pretrain(false)
		.setInputType(InputType.convolutionalFlat(height, width, 3));
		return builder;
	}
	
	public static Builder getShallowConvNet(int height, int width, int featureCount) {
		int layerId = 0;
		
		Builder builder = new NeuralNetConfiguration.Builder()
		.seed(seed)
		.iterations(1)
		.regularization(regularization)
		.l2(l2)
		.learningRate(learningRate)
		.weightInit(WeightInit.XAVIER)
		.activation("relu")
		.optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT)
		.updater(Updater.NESTEROVS).momentum(0.9)
		.list()
		.layer(layerId++, new ConvolutionLayer.Builder(8, 8)
				.nIn(3)
				.stride(2, 2)
				.nOut(20)
				.padding(3,3)
				.build())
		.layer(layerId++,
				new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
				.kernelSize(2, 2)
				.stride(2, 2)
				.build())
		.layer(layerId++, new ConvolutionLayer.Builder(4, 4)
				.nIn(20)
				.stride(2, 2)
				.nOut(40)
				.padding(1,1)
				.build())
		.layer(layerId++,
				new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
				.kernelSize(2, 2)
				.stride(2, 2)
				.build())
		.layer(layerId++, new DenseLayer.Builder()
				.nOut(100)
				.build())
		.layer(layerId++, new OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)
				.activation("softmax")
				.nOut(featureCount)
				.build())
		.backprop(true).pretrain(false)
		.setInputType(InputType.convolutionalFlat(height, width, 3));
		return builder;
	}
	
	public static Builder getDeepConvNet(int height, int width, int featureCount) {
		int layerId = 0;
		
		Builder builder = new NeuralNetConfiguration.Builder()
		.seed(seed)
		.iterations(1)
		.regularization(regularization)
		.l2(l2)
		.learningRate(learningRate)
		.weightInit(WeightInit.XAVIER)
		.activation("relu")
		.optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT)
		.updater(Updater.NESTEROVS).momentum(0.9)
		.list()
		.layer(layerId++, new ConvolutionLayer.Builder(3, 3)
				.nIn(3)
				.stride(1, 1)
				.nOut(60)
				.padding(1,1)
				.build())
		.layer(layerId++,
				new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
				.kernelSize(2, 2)
				.stride(2, 2)
				.build())
		.layer(layerId++, new ConvolutionLayer.Builder(3, 3)
				.nIn(60)
				.stride(1, 1)
				.nOut(60)
				.padding(1,1)
				.build())
		.layer(layerId++,
				new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
				.kernelSize(2, 2)
				.stride(2, 2)
				.build())
		.layer(layerId++, new ConvolutionLayer.Builder(3, 3)
				.nIn(60)
				.stride(1, 1)
				.nOut(80)
				.padding(1,1)
				.build())
		.layer(layerId++,
				new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
				.kernelSize(2, 2)
				.stride(2, 2)
				.build())
		.layer(layerId++, new ConvolutionLayer.Builder(3, 3)
				.nIn(80)
				.stride(1, 1)
				.nOut(120)
				.padding(1,1)
				.build())
		.layer(layerId++, new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
				.kernelSize(2, 2)
				.stride(2, 2)
				.build())
		.layer(layerId++, new DenseLayer.Builder()
				.nOut(40)
				.build())
		.layer(layerId++, new DenseLayer.Builder()
				.nOut(40)
				.build())
		.layer(layerId++, new DenseLayer.Builder()
				.nOut(40)
				.build())
		.layer(layerId++, new OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)
				.activation("softmax")
				.nOut(featureCount)
				.build())
		.backprop(true).pretrain(false)
		.setInputType(InputType.convolutionalFlat(height, width, 3));
		return builder;
	}
	
	public static Builder getConvNet(int height, int width, int featureCount) {
		Builder builder = new NeuralNetConfiguration.Builder()
		.seed(seed)
		.iterations(1)
		.regularization(regularization)
		.l2(l2)
		.learningRate(learningRate)
		.weightInit(WeightInit.XAVIER)
		.activation("relu")
		.optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT)
		.updater(Updater.NESTEROVS).momentum(0.9)
		.list()
		.layer(0, new ConvolutionLayer.Builder()
				.name("layer0")
				.kernelSize(4, 4)
				.nIn(3)
				.stride(2, 2)
				.nOut(20)
				.padding(1, 1)
				.dropOut(0.5)
				.activation("relu")
				.build())
		.layer(1,
				new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
				.name("layer1")
				.kernelSize(2, 2)
				.stride(2, 2)
				.build())
		.layer(2, new ConvolutionLayer.Builder()
				.name("layer2")
				.kernelSize(3, 3)
				.nIn(20)
				.stride(1, 1)
				.nOut(20)
				.padding(1,1)
				.dropOut(0.5)
				.activation("relu")
				.build())
		.layer(3,
				new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
				.name("layer3")
				.kernelSize(2, 2)
				.stride(2, 2)
				.build())
		.layer(4, new DenseLayer.Builder()
				.name("layer4")
				.activation("relu")
				.nOut(200)
				.build())
		.layer(5, new OutputLayer.Builder(LossFunctions.LossFunction.SQUARED_LOSS)
				.name("layer5")
				.nOut(featureCount)
				.activation("identity")
				.build())
		.backprop(true).pretrain(false)
		.setInputType(InputType.convolutionalFlat(height, width, 3));
		return builder;
	}
	
	public static Builder getReducingConvNet(int height, int width, int featureCount) {
		Builder builder = new NeuralNetConfiguration.Builder()
		.seed(seed)
		.iterations(1)
		.regularization(regularization)
		.l2(l2)
		.learningRate(learningRate)
		.weightInit(WeightInit.XAVIER)
		.activation("relu")
		.optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT)
		.updater(Updater.NESTEROVS).momentum(0.9)
		.list()
		.layer(0, new ConvolutionLayer.Builder()
				.name("layer0")
				.kernelSize(1, 1)
				.nIn(3)
				.stride(1, 1)
				.nOut(20)
				.padding(0, 0)
				.dropOut(0.5)
				.activation("relu")
				.build())
		.layer(1,
				new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
				.name("layer1")
				.kernelSize(2, 2)
				.stride(2, 2)
				.build())
		.layer(2, new ConvolutionLayer.Builder()
				.name("layer2")
				.kernelSize(3, 3)
				.nIn(20)
				.stride(1, 1)
				.nOut(20)
				.padding(1,1)
				.dropOut(0.5)
				.activation("relu")
				.build())
		.layer(3, new ConvolutionLayer.Builder()
				.name("layer2")
				.kernelSize(2, 2)
				.nIn(20)
				.stride(2, 2)
				.nOut(20)
				.padding(0,0)
				.dropOut(0.5)
				.activation("relu")
				.build())
		.layer(4, new ConvolutionLayer.Builder()
				.name("layer3")
				.kernelSize(2, 2)
				.nIn(20)
				.stride(2, 2)
				.nOut(20)
				.padding(0,0)
				.dropOut(0.5)
				.activation("relu")
				.build())
		.layer(5, new ConvolutionLayer.Builder()
				.name("layer4")
				.kernelSize(2, 2)
				.nIn(20)
				.stride(2, 2)
				.nOut(20)
				.padding(0,0)
				.dropOut(0.5)
				.activation("relu")
				.build())
		.layer(6, new DenseLayer.Builder()
				.name("layer5")
				.activation("relu")
				.nOut(200)
				.build())
		.layer(7, new OutputLayer.Builder(LossFunctions.LossFunction.SQUARED_LOSS)
				.name("layer6")
				.nOut(featureCount)
				.activation("identity")
				.build())
		.backprop(true).pretrain(false)
		.setInputType(InputType.convolutionalFlat(height, width, 3));
		return builder;
	}
}
