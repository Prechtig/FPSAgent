package org.mma.imagerecognition.executables;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.nio.file.Paths;
import java.util.Arrays;

import org.apache.commons.lang3.tuple.ImmutablePair;
import org.apache.commons.lang3.tuple.Pair;
import org.deeplearning4j.nn.api.Layer;
import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.util.ModelSerializer;
import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dao.TrainingDbDao;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.tools.INDArrayTool;
import org.mma.imagerecognition.tools.ImageTool;
import org.mma.imagerecognition.tools.NNTool;
import org.nd4j.linalg.api.ndarray.INDArray;
import org.nd4j.linalg.factory.Nd4j;

public class LayerActivator {
	public static void main(String[] args) throws IOException {
		FileSystemDAO.createFolders();
		if(args.length != 2) {
			System.err.println("Expected 2 arguments, the model number, and the id of the image to evaluate on.");
			System.exit(1);
		}
		
		File networkFile = Paths.get("models", "continuous", "model2.bin").toFile();
		int[] imageIds = new int[] { 47, 52, 55, 139, 146, 192, 193, 225, 242 };
		
		for(int imageId : imageIds) {
			getActivationOfLayers(networkFile, imageId);
		}
	}
	
	public static void getActivationOfLayers(File networkFile, int imageId) throws FileNotFoundException, IOException {
		MultiLayerNetwork network = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(networkFile));
		TrainingData image = TrainingDbDao.getImages(TrainingDbDao.getIndices(imageId, imageId)).get(0);
		ImageTool.printColoredPngImage(image.getPixelData(), image.getWidth(), new File("image"+ imageId +".png"));
		
		network.output(Nd4j.create(ImageTool.toScaledDoubles(image.getPixelData()), new int[] { 1, 3, image.getWidth(), image.getHeight()}), false);
		int convLayers = NNTool.numberOfConvolutionalLayers(network);
		
		//Save convolutional activations as images
		for(int currentLayer = 0; currentLayer < convLayers; currentLayer++) {
			persistFeatureMap(network.getLayer(currentLayer).activate().slice(0), currentLayer);
		}
		
		System.out.println();
		System.out.println("Evaluating image with id: " + imageId);
		
		//Print dense layer and output layer activation
		double[] output = null;
		for(int currentLayer = convLayers; currentLayer < network.getLayers().length; currentLayer++) {
			Layer layer = network.getLayer(currentLayer);
			INDArray activate = layer.activate();
			if(currentLayer+1 == network.getLayers().length) {
				// Output layer
				output = INDArrayTool.toFlatDoubleArray(activate);
			}
		}
		
		System.out.println("Output of output layer:");
		System.out.println(Arrays.toString(output));
		System.out.println("Output of grount truths:");
		System.out.println(Arrays.toString(image.getFeatures()));
		
		Pair<Integer, Double> featureInfo = findHighestIdAndValue(image.getFeatures());
		System.out.println("Correct index = " + featureInfo.getLeft());
		
		Pair<Integer, Double> networkInfo = findHighestIdAndValue(output);
		System.out.println("Guessed index = " + networkInfo.getLeft());
		System.out.println("Guessed percentage = " + networkInfo.getRight());
		System.out.println("Percentage of correct index = " + output[featureInfo.getLeft()]);
		
		System.out.println();
	}
	
	private static void persistFeatureMap(INDArray featureMaps, int layer) throws IOException {
		for(int featureMap = 0; featureMap < featureMaps.shape()[0]; featureMap++) {
			ImageTool.printGreyScalePngImage(INDArrayTool.toFlatDoubleArray(featureMaps.slice(featureMap)), featureMaps.shape()[1], FileSystemDAO.getFeatureMapsFolderForLayer(layer).resolve("featureMap" + featureMap + ".png").toFile());
		}
	}
	
	private static Pair<Integer, Double> findHighestIdAndValue(double[] arr) {
		int idx = 0;
		double val = 0d;
		for(int i = 0; i < arr.length; i++) {
			if(val < arr[i]) {
				val = arr[i];
				idx = i;
			}
		}
		return new ImmutablePair<Integer, Double>(idx, val);
	}
}










