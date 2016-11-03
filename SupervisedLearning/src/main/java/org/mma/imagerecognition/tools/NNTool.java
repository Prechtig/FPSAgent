package org.mma.imagerecognition.tools;

import java.util.Arrays;

import org.deeplearning4j.nn.api.Layer;
import org.deeplearning4j.nn.layers.convolution.ConvolutionLayer;
import org.deeplearning4j.nn.layers.convolution.subsampling.SubsamplingLayer;
import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;

public class NNTool {
	public static int numberOfConvolutionalLayers(MultiLayerNetwork network) {
		int numberOfConvolutionalLayers = 0;
		for(Layer layer : network.getLayers()) {
			if(layer instanceof ConvolutionLayer || layer instanceof SubsamplingLayer) {
				numberOfConvolutionalLayers++;
			}
		}
		return numberOfConvolutionalLayers;
	}
	
	public static double[] toBinaryVector(double[] probabilityVector) {
		double max = Arrays.stream(probabilityVector).reduce(0.0, (curMax, cur) -> Math.max(curMax, cur));
		int maxProbabilityIndex = 0;
		for(int i = 0; i < probabilityVector.length; i++) {
			if(probabilityVector[i] == max) maxProbabilityIndex = i;
		}
		double[] result = new double[probabilityVector.length];
		result[maxProbabilityIndex] = 1d;
		return result;
	}
}
