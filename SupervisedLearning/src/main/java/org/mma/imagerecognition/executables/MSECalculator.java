package org.mma.imagerecognition.executables;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.nio.file.Paths;
import java.util.List;
import java.util.stream.DoubleStream;

import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.util.ModelSerializer;
import org.mma.imagerecognition.dao.TrainingDbDao;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.tools.INDArrayTool;
import org.nd4j.linalg.api.ndarray.INDArray;
import org.nd4j.linalg.dataset.DataSet;

public class MSECalculator {
	
	/*
	 * Calculate avg absolute score of, horizontal, vertical and distance
	 * Calculate accuracy of withinSight
	 * 
	 */
	
//	public static void main(String[] args) throws FileNotFoundException, IOException {
//		Properties projectProperties = PropertiesReader.getProjectProperties();
//		int trainSize 	= Integer.parseInt(projectProperties.getProperty("training.trainSize"));
//		int testSize	= Integer.parseInt(projectProperties.getProperty("training.testSize"));
//		int batchSize	= Integer.parseInt(projectProperties.getProperty("training.persistence.batchSize"));
//		
//		Trainer.init(trainSize+1, trainSize+testSize);
//		
//		
//		
//		//network.out
//		
//		INDArray scores = network.scoreExamples(new DatabaseIterator(batchSize, testSize), false);
//		double[] scoresAsDouble = INDArrayTool.toFlatDoubleArray(scores);
//		double scoreSum = DoubleStream.of(scoresAsDouble).sum();
//		System.out.println(String.format("Avg score: %f", scoreSum/scoresAsDouble.length));
//	}
	
	public static void main(String[] args) throws FileNotFoundException, IOException {
		File networkFile = Paths.get("models", "angular", "nolight", "deep", "model12.bin").toFile();
		MultiLayerNetwork network = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(networkFile));
		
		int samples = 10000;
		int fromId = 1;//130_001;
		int toId = fromId + samples - 1;
		int batchSize = 25;
		
		Trainer.init(fromId, toId);
		
		double[] truthValuesHorizontal		= new double[samples];
		double[] truthValuesVertical		= new double[samples];
		double[] truthValuesDistance		= new double[samples];
		double[] truthValuesWithinSight		= new double[samples];
		
		double[] networkValuesHorizontal	= new double[samples];
		double[] networkValuesVertical		= new double[samples];
		double[] networkValuesDistance		= new double[samples];
		double[] networkValuesWithinSight	= new double[samples];
		
		for(int id = fromId; id <= toId; id += batchSize*4) {
			int endId = Math.min(toId, id + batchSize - 1);
			List<TrainingData> trainingData = TrainingDbDao.getTrainingData(id, endId);
			DataSet dataSet = INDArrayTool.toDataSet(trainingData, batchSize);
			
			double[] tmpTruthValues = INDArrayTool.toFlatDoubleArray(dataSet.getLabels());
			
			INDArray output = network.output(dataSet.getFeatures());
			double[] tmpNetworkValues = INDArrayTool.toFlatDoubleArray(output);
			
			for(int i = 0; i < tmpTruthValues.length; i += 4) {
				System.arraycopy(tmpTruthValues, i+0, truthValuesHorizontal, i/4, 1);
				System.arraycopy(tmpTruthValues, i+1, truthValuesVertical, i/4, 1);
				System.arraycopy(tmpTruthValues, i+2, truthValuesDistance, i/4, 1);
				System.arraycopy(tmpTruthValues, i+3, truthValuesWithinSight, i/4, 1);
				
				System.arraycopy(tmpNetworkValues, i+0, networkValuesHorizontal, i/4, 1);
				System.arraycopy(tmpNetworkValues, i+1, networkValuesVertical, i/4, 1);
				System.arraycopy(tmpNetworkValues, i+2, networkValuesDistance, i/4, 1);
				System.arraycopy(tmpNetworkValues, i+3, networkValuesWithinSight, i/4, 1);
			}
		}
		double[] horizontalError = calculateError(truthValuesHorizontal, networkValuesHorizontal);
		double meanHorizontalError = calculateMeanError(horizontalError);
		System.out.println(String.format("Mean horizontal error is: %f", meanHorizontalError));
		
		double[] verticalError = calculateError(truthValuesVertical, networkValuesVertical);
		double meanVerticalError = calculateMeanError(verticalError);
		System.out.println(String.format("Mean vertical error is: %f", meanVerticalError));
		
		double[] distanceError = calculateError(truthValuesDistance, networkValuesDistance);
		double meanDistanceError = calculateMeanError(distanceError);
		System.out.println(String.format("Mean distance error is: %f", meanDistanceError));
		
		double accuracy = calculateAccuracy(truthValuesWithinSight, networkValuesWithinSight);
		System.out.println(String.format("Accuracy is: %f", accuracy));
	}
	
	private static double calculateAccuracy(double[] truthValues, double[] networkValues) {
		if(truthValues.length != networkValues.length) {
			throw new RuntimeException("Arrays was not of equal length");
		}
		int hits = 0;
		for(int i = 0; i < truthValues.length; i++) {
			double networkValue = networkValues[i];
			double truthValue = truthValues[i];
			if(	(networkValue < 0.5 && truthValue == 0d) ||
				(networkValue >= 0.5 && truthValue == 1d)) {
				hits++;
			}
		}
		return (double)hits/(double)truthValues.length;
	}
	
	private static double[] calculateError(double[] truthValues, double[] networkValues) {
		if(truthValues.length != networkValues.length) {
			throw new RuntimeException("Arrays was not of equal length");
		}
		double[] error = new double[truthValues.length];
		
		for(int i = 0; i < truthValues.length; i++) {
			error[i] = Math.abs(truthValues[i] - networkValues[i]);
		}
		return error;
	}
	
	private static double calculateMeanError(double[] arr) {
		return DoubleStream.of(arr).average().getAsDouble();
	}

}
