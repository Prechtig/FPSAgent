package org.mma.imagerecognition.executables;

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

public class LayerActivator {
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
}
