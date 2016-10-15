package org.mma.imagerecognition.tools;

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
}
