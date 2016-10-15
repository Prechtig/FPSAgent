package org.mma.imagerecognition.executables;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.Arrays;

import org.apache.commons.lang.math.NumberUtils;
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
		
		File networkFile = null;
		int imageId = Integer.parseInt(args[1]);
		
		if(NumberUtils.isDigits(args[0])) {
			int modelNumber = Integer.parseInt(args[0]);
			networkFile = FileSystemDAO.getContinuousFolder().resolve(String.format("model%d.bin",modelNumber)).toFile();
		} else {
			String path = args[0];
			networkFile = new File(path);
		}
		getActivationOfLayers(networkFile, imageId);
	}
	
	public static void getActivationOfLayers(File networkFile, int imageId) throws FileNotFoundException, IOException {
		MultiLayerNetwork network = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(networkFile));
		TrainingData image = TrainingDbDao.getImages(TrainingDbDao.getIndices(imageId, imageId)).get(0);
		ImageTool.printColoredPngImage(image.getPixelData(), image.getWidth(), new File("image.png"));
		
		INDArray output = network.output(Nd4j.create(ImageTool.toScaledDoubles(image.getPixelData()), new int[] { 1, 3, image.getWidth(), image.getHeight()}), false);
		int convLayers = NNTool.numberOfConvolutionalLayers(network);
		
		//Save convolutional activations as images
		for(int currentLayer = 0; currentLayer < convLayers; currentLayer++) {
			persistFeatureMap(network.getLayer(currentLayer).activate().slice(0), currentLayer);
		}
		
		//Print dense layer and output layer activation
		for(int currentLayer = convLayers; currentLayer < convLayers+2; currentLayer++) {
			System.out.println("Output of layer: " + currentLayer);
			System.out.println(Arrays.toString(INDArrayTool.toFlatDoubleArray(network.getLayer(currentLayer).activate())));
		}
		
	}
	
	private static void persistFeatureMap(INDArray featureMaps, int layer) throws IOException {
		featureMaps = featureMaps;
		for(int featureMap = 0; featureMap < featureMaps.shape()[0]; featureMap++) {
			ImageTool.printGreyScalePngImage(INDArrayTool.toFlatDoubleArray(featureMaps.slice(featureMap)), featureMaps.shape()[1], FileSystemDAO.getFeatureMapsFolderForLayer(layer).resolve("featureMap" + featureMap + ".png").toFile());
		}
	}
}
