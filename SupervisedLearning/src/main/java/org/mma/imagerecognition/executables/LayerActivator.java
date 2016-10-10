package org.mma.imagerecognition.executables;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.Arrays;

import org.deeplearning4j.nn.api.Layer;
import org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor;
import org.deeplearning4j.nn.layers.convolution.ConvolutionLayer;
import org.deeplearning4j.nn.layers.convolution.subsampling.SubsamplingLayer;
import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.util.ModelSerializer;
import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.tools.INDArrayTool;
import org.mma.imagerecognition.tools.ImageTool;
import org.nd4j.linalg.api.ndarray.INDArray;
import org.nd4j.linalg.factory.Nd4j;

public class LayerActivator {
	public static void main(String[] args) throws IOException {
		FileSystemDAO.createFolders();
		if(args.length != 2) {
			System.err.println("Expected 2 arguments, the model number, and the id of the image to evaluate on.");
			System.exit(1);
		}
		
		File networkFile = null;
		int imageId = Integer.parseInt(args[1]);
		
		if(isInteger(args[0])) {
			int modelNumber = Integer.parseInt(args[0]);
			networkFile = FileSystemDAO.getModelsFolder().resolve("untrainedModel.bin").toFile();
		} else {
			String path = args[0];
			networkFile = new File(path);
		}
		getActivationOfLayer(networkFile, imageId, 100);
	}
	
	public static void getActivationOfLayer(File networkFile, int imageId, int maxPrint) throws FileNotFoundException, IOException {
		MultiLayerNetwork network = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(networkFile));
		TrainingData image = FileSystemDAO.getRandomImages(1).get(0);
		ImageTool.printColoredPngImage(image.getPixelData(), image.getWidth(), new File("image.png"));
		
		INDArray latestOutput = Nd4j.create(ImageTool.toDoubleStream(image.getPixelData()).toArray(), new int[] { 1, 3, image.getWidth(), image.getHeight()});
		int convLayers = numberOfConvolutionalLayers(network);
		
		for(int currentLayer = 0; currentLayer < convLayers; currentLayer++) {
			network.getLayer(currentLayer).setInput(latestOutput);
			latestOutput = network.getLayer(currentLayer).activate();
			persistFeatureMap(latestOutput, currentLayer);
		}
		
		CnnToFeedForwardPreProcessor processor = new CnnToFeedForwardPreProcessor(image.getHeight(), image.getWidth(), latestOutput.shape()[0]);
		INDArray preProcessed = processor.preProcess(latestOutput, 1);
		
		Layer denseLayer = network.getLayer(convLayers);
		denseLayer.setInput(preProcessed);
		latestOutput = denseLayer.activate();
		System.out.println(Arrays.toString(INDArrayTool.toFlatDoubleArray(latestOutput)));
	}
	
	private static void persistFeatureMap(INDArray featureMaps, int layer) throws IOException {
		featureMaps = featureMaps.slice(0);
		for(int featureMap = 0; featureMap < featureMaps.shape()[0]; featureMap++) {
			ImageTool.printGreyScalePngImage(INDArrayTool.toFlatDoubleArray(featureMaps.slice(featureMap)), featureMaps.shape()[1], FileSystemDAO.getFeatureMapsFolderForLayer(layer).resolve("featureMap" + featureMap + ".png").toFile());
		}
	}
	
	private static int numberOfConvolutionalLayers(MultiLayerNetwork network) {
		int numberOfConvolutionalLayers = 0;
		for(Layer layer : network.getLayers()) {
			if(layer instanceof ConvolutionLayer || layer instanceof SubsamplingLayer) {
				numberOfConvolutionalLayers++;
			}
		}
		return numberOfConvolutionalLayers;
	}
	
	public static boolean isInteger(String s) {
	    return isInteger(s,10);
	}

	public static boolean isInteger(String s, int radix) {
	    if(s.isEmpty()) return false;
	    for(int i = 0; i < s.length(); i++) {
	        if(i == 0 && s.charAt(i) == '-') {
	            if(s.length() == 1) return false;
	            else continue;
	        }
	        if(Character.digit(s.charAt(i),radix) < 0) return false;
	    }
	    return true;
	}
}
