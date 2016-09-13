package org.mma.imagerecognition.tools;

import java.util.Random;

import org.nd4j.linalg.api.ndarray.INDArray;
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
}
