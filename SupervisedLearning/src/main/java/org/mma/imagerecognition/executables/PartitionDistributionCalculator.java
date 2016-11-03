package org.mma.imagerecognition.executables;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Map;
import java.util.function.Function;
import java.util.stream.Collectors;
import java.util.stream.IntStream;

import org.mma.imagerecognition.dao.FileSystemDAO;

public class PartitionDistributionCalculator {
	
	public static void main(String[] args) {
		int samples = 100000;
		
		Map<Integer, Long> distribution = IntStream.range(1, samples).boxed()
			.map(id -> FileSystemDAO.load(id).calculateFeatureIndexFromPartitionId())
			.collect(Collectors.groupingBy(Function.identity(), Collectors.counting()));
		
		List<Integer> keys = new ArrayList<Integer>(distribution.keySet());
		Collections.sort(keys);
		for(Object visualPartitionIndex : keys) {
			Long count = distribution.get(visualPartitionIndex);
			double percentage = (((double) count) / ((double) samples)) * 100;
			System.out.format("% 3d \t % 5d \t % 2.4f%s", visualPartitionIndex, count, percentage, "%");
			System.out.println();
		}
	}
}
