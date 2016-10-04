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
import org.nd4j.linalg.factory.Nd4j;

public class Evaluator {
	public static void main(String[] args) throws FileNotFoundException, IOException {
		if(args.length != 2) {
			System.err.println("Expected 2 arguments, the model number, and the id of the image to evaluate on.");
			System.exit(1);
		}
		
		File networkFile = null;
		int imageId = Integer.parseInt(args[1]);
		
		if(isInteger(args[0])) {
			int modelNumber = Integer.parseInt(args[0]);
			networkFile = new File("models" + File.separator + "continuous" + File.separator + "model" + modelNumber + ".bin");
		} else {
			String path = args[0];
			networkFile = new File(path);
		}
		
		evaluateNetworkOnRandomImage(networkFile, imageId);
	}
	
	public static void evaluateNetworkOnRandomImage(File networkFile, int imageId) throws FileNotFoundException, IOException {
		MultiLayerNetwork network = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(networkFile));
		
		TrainingData randomImage = TrainingDbDao.getImages("(" + imageId + ")").get(0);
		
		ImageTool.printPngImage(randomImage.getPixelData(), 640, new File("image.png"));
		System.out.println(Arrays.toString(randomImage.getFeatureDoubles()));
		System.out.println(Arrays.toString(INDArrayTool.toFlatDoubleArray(network.output(ImageTool.convertToINDArray(randomImage.getPixelData(), 640)))));
	}
	
	public static void getActivationOfLayer(File networkFile, int layer) throws FileNotFoundException, IOException {
		MultiLayerNetwork network = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(networkFile));
		TrainingData randomImage = TrainingDbDao.getRandomImages(1).get(0);
		//INDArray input = ImageTool.convertToINDArray(randomImage.getPixelData(), 640);
		
		int layers = network.getLayers().length;
		INDArray latestOutput = Nd4j.create(ImageTool.toDoubleStream(randomImage.getPixelData()).toArray(), new int[] { 1, 3, 360, 640});
		for(int currentLayer = 0; currentLayer < 4; currentLayer++) {
			network.getLayer(currentLayer).setInput(latestOutput);
			latestOutput = network.getLayer(currentLayer).activate();
			System.out.println(Arrays.toString(Arrays.copyOf(INDArrayTool.toFlatDoubleArray(latestOutput), 100)));
		}
		
		network.getLayer(4).setInput(latestOutput);
		INDArray result4 = network.getLayer(4).activate();
		
//		network.getLayer(0).setInput(input);
//		INDArray activate = network.getLayer(0).activate();
//		
//		network.getLayer(1).setInput(activate);
//		INDArray activate1 = network.getLayer(1).activate();
//		
//		network.getLayer(2).setInput(activate1);
//		INDArray activate2 = network.getLayer(2).activate();
//		
//		System.out.println(Arrays.toString(INDArrayTool.toFlatDoubleArray(activate2)));
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
