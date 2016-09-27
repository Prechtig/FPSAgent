package org.mma.imagerecognition.configuration;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.Arrays;

import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.util.ModelSerializer;
import org.mma.imagerecognition.dao.TrainingDbDao;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.tools.INDArrayTool;
import org.mma.imagerecognition.tools.ImageTool;
import org.nd4j.linalg.api.ndarray.INDArray;

public class Evaluator {
	public static void main(String[] args) throws FileNotFoundException, IOException {
		File networkFile = new File("models" + File.separator + "model99.bin");
		getActivationOfLayer(networkFile, 0);
		//evaluateNetworkOnRandomImage(networkFile);
	}
	
	public static void evaluateNetworkOnRandomImage(File networkFile) throws FileNotFoundException, IOException {
		MultiLayerNetwork network = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(networkFile));
		TrainingData randomImage = TrainingDbDao.getRandomImages(1).get(0);
		
		ImageTool.printPngImage(randomImage.getPixelData(), 640, new File("image.png"));
		System.out.println(Arrays.toString(randomImage.getFeatureDoubles()));
		System.out.println(Arrays.toString(INDArrayTool.toFlatDoubleArray(network.output(ImageTool.convertToINDArray(randomImage.getPixelData(), 640)))));
	}
	
	public static void getActivationOfLayer(File networkFile, int layer) throws FileNotFoundException, IOException {
		MultiLayerNetwork network = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(networkFile));
		TrainingData randomImage = TrainingDbDao.getRandomImages(1).get(0);
		
		INDArray input = ImageTool.convertToINDArray(randomImage.getPixelData(), 640);
		
		network.getLayer(0).setInput(input);
		INDArray activate = network.getLayer(0).activate();
		System.out.println(Arrays.toString(INDArrayTool.toFlatDoubleArray(activate)));
		
	}
}
