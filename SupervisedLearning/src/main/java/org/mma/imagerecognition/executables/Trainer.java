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
//import org.nd4j.jita.conf.CudaEnvironment;
import org.nd4j.linalg.api.buffer.DataBuffer;
import org.nd4j.linalg.api.buffer.util.DataTypeUtil;
import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;

public class Trainer {
	
	private static final long GIGABYTE = 1024 * 1024 * 1024;
	
	public static void main(String[] args) throws IOException {
		DataTypeUtil.setDTypeForContext(DataBuffer.Type.FLOAT);
		
		boolean sliEnabled = PropertiesReader.getProjectProperties().getProperty("training.sli").equals("true");
		
//		CudaEnvironment.getInstance().getConfiguration()
//		    .setMaximumDeviceCacheableLength(GIGABYTE * 1)
//		    .setMaximumDeviceCache			(GIGABYTE * 12)
//		    .setMaximumHostCacheableLength	(GIGABYTE * 1)
//		    .setMaximumHostCache			(GIGABYTE * 16)
//			.allowMultiGPU(sliEnabled);
		
		int batchSize = Integer.parseInt(PropertiesReader.getProjectProperties().getProperty("training.persistence.batchSize"));
		String trainingPersistenceType = PropertiesReader.getProjectProperties().getProperty("training.persistence.type");
		FileSystemDAO.createFolders();
		if(trainingPersistenceType == null) {
			System.out.println("Please specify the persistence type of training data");
			System.exit(1);
		}
		if(trainingPersistenceType.equals("filesystem")) {
			int maxNumberOfImagesToPersist = Integer.parseInt(PropertiesReader.getProjectProperties().getProperty("training.persistence.maxImagesToPersist"));
			persistImagesToDisk(batchSize, maxNumberOfImagesToPersist);
			if(PropertiesReader.getProjectProperties().getProperty("training.persistence.checkIntegrity").equals("true")) {
				persistMissingImages(maxNumberOfImagesToPersist);
			}
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
		
		if(sliEnabled) {
			new ContinuousTraining().trainParallel(trainIterator, testIterator);
		} else {
			new ContinuousTraining().train(trainIterator, testIterator);
		}
	}
	
	private static void persistImagesToDisk(int batchSize, int maxNumberOfImagesToPersist) {
		int maxSavedId = FileSystemDAO.findLatestTrainingDataId();
		int maxDbId = TrainingDbDao.getTotalNumberOfImages();
		
		if(maxNumberOfImagesToPersist < maxSavedId) {
			maxNumberOfImagesToPersist = maxSavedId;
		}
		
		if(maxDbId > maxNumberOfImagesToPersist) {
			maxDbId = maxNumberOfImagesToPersist;
		}
		
		if(maxSavedId != maxDbId) {
			System.out.println("Downloading " + (maxDbId - maxSavedId) + " instances of training data");
		}
		
		for(int i = maxSavedId + 1; i <= maxDbId; i += batchSize) {
			if((i - maxSavedId - 1)%(10*batchSize) == 0 && i != maxSavedId + 1) {
				System.out.format("Downloaded %d items \n", i - maxSavedId - 1);
			}
			List<TrainingData> images = TrainingDbDao.getTrainingData(i, i + batchSize - 1);
			FileSystemDAO.persist(images);
		}
	}
	
	private static void persistMissingImages(int maxNumberOfImagesToPersist) {
		int maxSavedId = FileSystemDAO.findLatestTrainingDataId();
		int maxDbId = TrainingDbDao.getTotalNumberOfImages();
		
		if(maxNumberOfImagesToPersist < maxSavedId) {
			maxNumberOfImagesToPersist = maxSavedId;
		}
		
		if(maxDbId > maxNumberOfImagesToPersist) {
			maxDbId = maxNumberOfImagesToPersist;
		}
		
		System.out.println("Checking integrity of locally stored data");
		
		for(int i = 1; i <= maxDbId; i++) {
			if(!FileSystemDAO.exists(i)) {
				List<TrainingData> images = TrainingDbDao.getTrainingData(i, i);
				FileSystemDAO.persist(images);
				if(FileSystemDAO.exists(i)) {
					System.out.format("Successfully downloaded locally missing training data with id %d \n", i);
				} else {
					System.out.format("Tried to download locally missing training data with id %d, but the data was not persisted to file system. Try reassigning database ids. \n", i);
				}
			}
		}
	}
}
