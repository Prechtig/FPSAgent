package org.mma.imagerecognition.executables;

import java.util.ArrayList;
import java.util.List;

import org.mma.imagerecognition.dao.DbDao;
import org.mma.imagerecognition.dao.TrainingDbDao;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.partitioner.VisualPartitionClassifier;

public class CreateEqualDistributionSet {
	
	private static final int samplesPerClass = 50;
	private static final int batchSize = 25;
	private static final VisualPartitionClassifier vpc = VisualPartitionClassifier.GetInstance();
	private static final int numberOfPartitions = vpc.getNumberOfPartitions() + 1;
	
	public static void main(String[] args) {
		List<List<TrainingData>> classLists = new ArrayList<List<TrainingData>>(numberOfPartitions);
		
		for(int i = 0; i < numberOfPartitions; i++) {
			classLists.add(new ArrayList<TrainingData>(samplesPerClass));
		}
		
		TrainingDbDao.setTableName("trainingData");
		int picturesFound = 0;
		for(int i = 1; true; i += batchSize) {
			List<TrainingData> trainingData = TrainingDbDao.getTrainingData(i, i + batchSize - 1);
			
			for(TrainingData td : trainingData) {
				int featureIndex;
				if(td.isWithinSight()) {
					featureIndex = vpc.calculateFeatureIndexFromPartitionId(td.getPartitionId());
				} else {
					featureIndex = numberOfPartitions - 1;
				}
				List<TrainingData> trainingDataList = classLists.get(featureIndex);
				if(trainingDataList.size() < samplesPerClass) {
					trainingDataList.add(td);
					picturesFound++;
				}
			}
			if(picturesFound == samplesPerClass * numberOfPartitions) {
				break;
			}
		}
		
		DbDao writer = new DbDao("trainingDataLightEqualDistribution");
		for(List<TrainingData> trainingData : classLists) {
			writer.persist(trainingData);
		}
	}

}
