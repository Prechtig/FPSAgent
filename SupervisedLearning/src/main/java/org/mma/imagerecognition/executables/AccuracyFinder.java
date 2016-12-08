package org.mma.imagerecognition.executables;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.List;

import org.apache.commons.lang.ArrayUtils;
import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.util.ModelSerializer;
import org.mma.imagerecognition.dao.DbConnector;
import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.tools.INDArrayTool;
import org.mma.imagerecognition.tools.ImageTool;
import org.mma.imagerecognition.tools.NNTool;
import org.nd4j.linalg.api.ndarray.INDArray;
import org.nd4j.linalg.factory.Nd4j;

public class AccuracyFinder {
	
	private static final int BATCH_SIZE = 250;
	
	public static void main(String[] args) throws FileNotFoundException, IOException {
		Trainer.init();
		FileSystemDAO.createFolders();
		
		Path modelPath;
		MultiLayerNetwork network;
		
		System.out.println(String.format("Training set - using db table: %s", DbConnector.getTableName()));
		
		modelPath = Paths.get("models", "nolight-shallow", "model2.bin");
		network = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(modelPath.toFile()));
		System.out.format("Evaluating model %s \n", modelPath.toString());
		double accuracy = getAccuracy(network, 1, 10_000);
		System.out.format("Hit rate: %f%%\n", accuracy*100);
		
		modelPath = Paths.get("models", "nolight-deep", "model2.bin");
		network = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(modelPath.toFile()));
		System.out.format("Evaluating model %s \n", modelPath.toString());
		accuracy = getAccuracy(network, 1, 10_000);
		System.out.format("Hit rate: %f%%\n", accuracy*100);
		
		DbConnector.setTableName("trainingDataNoLightTestSet");
		System.out.println(String.format("Test set - using db table: %s", DbConnector.getTableName()));
		
		modelPath = Paths.get("models", "nolight-shallow", "model2.bin");
		network = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(modelPath.toFile()));
		System.out.format("Evaluating model %s \n", modelPath.toString());
		accuracy = getAccuracy(network, 1, 10_000);
		System.out.format("Hit rate: %f%%\n", accuracy*100);
		
		modelPath = Paths.get("models", "nolight-deep", "model2.bin");
		network = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(modelPath.toFile()));
		System.out.format("Evaluating model %s \n", modelPath.toString());
		accuracy = getAccuracy(network, 1, 10_000);
		System.out.format("Hit rate: %f%%\n", accuracy*100);
	}
	
	public static double getAccuracy(MultiLayerNetwork network, int firstId, int lastId) throws IOException {
		if(lastId-firstId < 0) {
			throw new IllegalArgumentException("Test set is empty");
		}
		
		@SuppressWarnings("unused")
		int hits = 0, misses = 0;
		
		List<TrainingData> testSet;
		for(int fromId = firstId; fromId <= lastId; fromId += BATCH_SIZE) {
			int toId = Math.min(fromId + BATCH_SIZE-1, lastId);
			testSet = FileSystemDAO.load(fromId, toId);
			for(TrainingData td : testSet) {
				double[] groundTruths = td.getFeatures();
				INDArray output = network.output(Nd4j.create(ImageTool.toScaledDoubles(td.getPixelData()), new int[] { 1, 3, td.getWidth(), td.getHeight()}), false);
				double[] binaryVector = NNTool.toBinaryVector(INDArrayTool.toFlatDoubleArray(output));
				if(ArrayUtils.isEquals(binaryVector, groundTruths)) {
					hits++;
				} else {
					misses++;
				}
			}
			System.out.println(String.format("Evaluated %d images", toId-firstId+1));
		}
			
		return ((double) hits) / ((double) lastId-firstId+1);
	}
}
