package org.mma.imagerecognition.executables;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Map;
import java.util.function.Function;
import java.util.stream.Collectors;
import java.util.stream.IntStream;

import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dataobjects.TrainingData;

public class PartitionDistributionCalculator {
	
	public static void main(String[] args) {
		int samples = 10000;
		
		Map<Integer, Long> distribution = IntStream.range(1, samples).boxed()
			.map(id -> getPartitionId(id))
			.collect(Collectors.groupingBy(Function.identity(), Collectors.counting()));
		
		List<Integer> keys = new ArrayList<Integer>(distribution.keySet());
		Collections.sort(keys);
		for(Object visualPartitionIndex : keys) {
			Long count = distribution.get(visualPartitionIndex);
			double percentage = (((double) count) / ((double) samples)) * 100;
			System.out.format("% 3d \t % 6d \t % 2.4f%s", visualPartitionIndex, count, percentage, "%");
			System.out.println();
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
