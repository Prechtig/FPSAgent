package org.mma.imagerecognition.executables;

import java.io.IOException;
import java.util.List;

import org.mma.imagerecognition.configuration.ContinuousTraining;
import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dao.TrainingDbDao;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.iterator.DatabaseIterator;
import org.mma.imagerecognition.iterator.FileSystemIterator;
import org.mma.imagerecognition.tools.PropertiesReader;
import org.nd4j.linalg.api.buffer.DataBuffer;
import org.nd4j.linalg.api.buffer.util.DataTypeUtil;
import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;

public class Trainer {
	
	private static final long GIGABYTE = 1024 * 1024 * 1024;
	
	public static void main(String[] args) throws IOException {
		DataTypeUtil.setDTypeForContext(DataBuffer.Type.FLOAT);
		
//		CudaEnvironment.getInstance().getConfiguration()
//			.allowMultiGPU(true)
//		    .setMaximumDeviceCacheableLength(GIGABYTE * 1)
//		    .setMaximumDeviceCache			(GIGABYTE * 12)
//		    .setMaximumHostCacheableLength	(GIGABYTE * 1)
//		    .setMaximumHostCache			(GIGABYTE * 16);
		
		int batchSize = Integer.parseInt(PropertiesReader.getProjectProperties().getProperty("training.persistence.batchSize"));;
		String trainingPersistenceType = PropertiesReader.getProjectProperties().getProperty("training.persistence.type");
		FileSystemDAO.createFolders();
		if(trainingPersistenceType == null) {
			System.out.println("Please specify the persistence type of training data");
			System.exit(1);
		}
		if(trainingPersistenceType.equals("filesystem")) {
			int maxImagesToPersist = Integer.MAX_VALUE;
			persistImagesToDisk(batchSize, maxImagesToPersist);
			persistMissingImages(maxImagesToPersist);
		}
		
		DataSetIterator trainIterator, testIterator;
		
		int testSize = Integer.parseInt(PropertiesReader.getProjectProperties().getProperty("training.testSize"));
		int trainSize = Integer.parseInt(PropertiesReader.getProjectProperties().getProperty("training.trainSize"));
		
		if(trainingPersistenceType.equals("filesystem")) {
			testIterator = new FileSystemIterator(batchSize, testSize);
			trainIterator = new FileSystemIterator(batchSize, trainSize);
		} else {
			testIterator = new DatabaseIterator(batchSize, testSize);
			trainIterator = new DatabaseIterator(batchSize, trainSize);
		}
		
		if(PropertiesReader.getProjectProperties().getProperty("training.sli").equals("true")) {
			new ContinuousTraining().trainParallel(trainIterator, testIterator);
		} else {
			new ContinuousTraining().train(trainIterator, testIterator);
		}
	}
	
	private static void persistImagesToDisk(int batchSize ,int maxNumberOfImagesToPersist) {
		int maxSavedId = FileSystemDAO.findLatestTrainingDataId();
		int maxDbId = TrainingDbDao.getTotalNumberOfImages();
		
		if(maxDbId > maxNumberOfImagesToPersist) {
			maxDbId = maxNumberOfImagesToPersist;
		}
		
		if(maxSavedId < maxDbId) {
			System.out.println("Downloading " + (maxDbId - maxSavedId) + " instances of training data");
		}
		
		for(int i = maxSavedId + 1; i <= maxDbId; i += batchSize) {
			List<TrainingData> images = TrainingDbDao.getTrainingData(i, i + batchSize - 1);
			FileSystemDAO.persist(images);
		}
	}
	
	private static void persistMissingImages(int maxNumberOfImagesToPersist) {
		int maxDbId = TrainingDbDao.getTotalNumberOfImages();
		
		if(maxDbId > maxNumberOfImagesToPersist) {
			maxDbId = maxNumberOfImagesToPersist;
		}
		
		for(int i = 1; i <= maxDbId; i++) {
			if(!FileSystemDAO.exists(i)) {
				List<TrainingData> images = TrainingDbDao.getTrainingData(i, i);
				FileSystemDAO.persist(images);
				System.out.format("Downloaded training data with id %d", i);
			}
		}
	}
}
