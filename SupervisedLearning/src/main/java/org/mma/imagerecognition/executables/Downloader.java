package org.mma.imagerecognition.executables;

import java.util.List;

import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dao.TrainingDbDao;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.tools.PropertiesReader;

public class Downloader {
	
	public static void download(int trainSize, int validationSize, int testSize, int batchSize, boolean checkIntegrity) {
		int maxNumberOfImagesToPersist = trainSize + validationSize + testSize;
		persistImagesToDisk(batchSize, maxNumberOfImagesToPersist);
		if(PropertiesReader.getProjectProperties().getProperty("training.persistence.checkIntegrity").equals("true")) {
			persistMissingImages(maxNumberOfImagesToPersist);
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
