package org.mma.imagerecognition.tools;

import java.util.List;
import java.util.Random;

import org.mma.imagerecognition.dataobjects.TrainingData;
import org.nd4j.linalg.api.ndarray.INDArray;
import org.nd4j.linalg.dataset.DataSet;
import org.nd4j.linalg.factory.Nd4j;

public class INDArrayTool {
	
	public static Random rng = new Random();
	
	/***
	 * Only works on arrays of rank 1
	 */
	public static double[] toFlatDoubleArray(INDArray array) {
		double[] result = new double[array.length()];
		
		for(int i = 0; i < array.length(); i++) {
			result[i] = array.getDouble(i);
		}
		
		return result;
	}
	
	public static INDArray generateRandomVector(int size) {
		float[] result = new float[size];
		
		for(int i = 0; i < size; i++) {
			result[i] = (float)rng.nextDouble();
		}
		
		return Nd4j.create(result);
	}
	
	public static DataSet toDataSet(List<TrainingData> trainingData, int batchSize) {
		double[][] pixelData			= new double[batchSize][];
		double[][] groundTruthValues	= new double[batchSize][];
		
		for(int i = 0; i < trainingData.size(); i++) {
			TrainingData td = trainingData.get(i);
			pixelData[i] = ImageTool.toScaledDoubles(td.getPixelData());
			groundTruthValues[i] = td.getFeatures();
		}
		return new DataSet(Nd4j.create(pixelData), Nd4j.create(groundTruthValues));
	}
}
