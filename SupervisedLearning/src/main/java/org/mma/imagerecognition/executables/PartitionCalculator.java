package org.mma.imagerecognition.executables;

import java.util.Map;
import java.util.stream.Collectors;
import java.util.stream.IntStream;

import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dataobjects.TrainingData;

public class PartitionCalculator {
	
	public static void main(String[] args) {
		int samples = 32;
		
		Map<Object, Object> distribution = IntStream.range(1, samples).boxed()
			.collect(Collectors.toMap(id -> id, id -> getPartitionId(id)));
		
		for(Object o : distribution.keySet()) {
			System.out.format("% 3d \t % 3d\n", o, distribution.get(o));
		}
	}
	
	public static int getPartitionId(int id) {
		TrainingData td = FileSystemDAO.load(id);
		double[] features = td.getFeatures();
		for(int i = 0; i < features.length; i++) {
			if(features[i] == 1d) {
				return i;
			}
		}
		throw new RuntimeException();
	}
}
