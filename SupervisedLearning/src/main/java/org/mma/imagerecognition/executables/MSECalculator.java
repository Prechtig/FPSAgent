package org.mma.imagerecognition.executables;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;

import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.util.ModelSerializer;
import org.mma.imagerecognition.dao.DbConnector;
import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.tools.INDArrayTool;
import org.mma.imagerecognition.tools.ImageTool;
import org.nd4j.linalg.api.ndarray.INDArray;
import org.nd4j.linalg.factory.Nd4j;

public class MSECalculator {
	
	/*
	 * Calculate avg absolute score of, horizontal, vertical and distance
	 * Calculate accuracy of withinSight
	 * 
	 */
	
	public static void main(String[] args) throws FileNotFoundException, IOException {
		File networkFile = Paths.get("models", "angular", "light", "deep", "model3.bin").toFile();
		MultiLayerNetwork network = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(networkFile));
		
		int samples = 10000;
		int fromId = 1;
		
		System.out.println(String.format("Evaluating model: %s", networkFile.toString()));
		System.out.println(String.format("Using db table: %s", DbConnector.getTableName()));
		calculateMeanErrors(network, fromId, samples);
	}
	
	private static void calculateMeanErrors(MultiLayerNetwork network, int fromId, int samples) throws IOException {
		int toId = fromId + samples - 1;
		
		Trainer.init(fromId, toId);
		
		List<Double> horizontalErrors = new ArrayList<Double>(samples);
		List<Double> verticalErrors = new ArrayList<Double>(samples);
		List<Double> distanceErrors = new ArrayList<Double>(samples);
		int hits = 0, counter = 0;

		for(int id = fromId; id <= toId; id++) {
			TrainingData trainingData = FileSystemDAO.load(id);
			
			double[] truthValues = trainingData.getFeatures();
			
			INDArray input = Nd4j.create(ImageTool.toScaledDoubles(trainingData.getPixelData()));
			INDArray output = network.output(input);
			double[] networkValues = INDArrayTool.toFlatDoubleArray(output);
			
			double horizontalTruthValue		= truthValues[0];
			double verticalTruthValue		= truthValues[1];
			double distanceTruthValue		= truthValues[2];
			double withinSightTruthValue	= truthValues[3];
			
			double horizontalNetworkValue	= networkValues[0];
			double verticalNetworkValue		= networkValues[1];
			double distanceNetworkValue		= networkValues[2];
			double withinSightNetworkValue	= networkValues[3];
			
			horizontalErrors.add(Math.abs(horizontalTruthValue - horizontalNetworkValue));
			verticalErrors.add(Math.abs(verticalTruthValue - verticalNetworkValue));
			distanceErrors.add(Math.abs(distanceTruthValue - distanceNetworkValue));
			
			if(	(withinSightNetworkValue < 0.5 && withinSightTruthValue == 0d) ||
				(withinSightNetworkValue >= 0.5 && withinSightTruthValue == 1d)) {
				hits++;
			}
			if(++counter % 250 == 0) {
				System.out.println(String.format("Evaluated %d samples", counter));
			}
		}
		System.out.println(String.format("Mean horizontal error is: %f", calculateMeanError(horizontalErrors)));
		System.out.println(String.format("Mean vertical error is: %f", calculateMeanError(verticalErrors)));
		System.out.println(String.format("Mean distance error is: %f", calculateMeanError(distanceErrors)));
		System.out.println(String.format("Accuracy is: %f", ((double)hits)/((double)samples)));
	}
	
	private static double calculateMeanError(List<Double> ds) {
		return ds.stream().mapToDouble(d -> d).average().getAsDouble();
	}
}
