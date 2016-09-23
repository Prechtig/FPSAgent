package org.mma.imagerecognition.controller;

import java.io.File;
import java.io.IOException;
import java.util.List;

import org.mma.imagerecognition.configuration.TestConfiguration;
import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dao.TrainingDbDao;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.iterator.DatabaseIterator;
import org.mma.imagerecognition.iterator.FileSystemIterator;
import org.mma.imagerecognition.tools.PropertiesReader;
import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;

public class Controller {
	public static void main(String[] args) throws IOException {
		int batchSize = 0;
		String trainingPersistenceType = PropertiesReader.getProjectProperties().getProperty("training.persistence.type");
		if(trainingPersistenceType == null) {
			System.out.println("Please specify the persistence type of training data");
			System.exit(1);
		}
		if(trainingPersistenceType.equals("filesystem")) {
			FileSystemDAO.createFolders();
			batchSize = Integer.parseInt(PropertiesReader.getProjectProperties().getProperty("training.persistence.batchSize"));
			persistImagesToDisk(batchSize);
		}
		
		DataSetIterator trainIterator, testIterator;
		
		if(trainingPersistenceType.equals("filesystem")) {
			testIterator = new FileSystemIterator(batchSize, 100);
			trainIterator = new FileSystemIterator(batchSize, 100);
		} else {
			testIterator = new DatabaseIterator(batchSize, 40);
			trainIterator = new DatabaseIterator(batchSize, 80);
		}
		
		TestConfiguration.train(trainIterator, testIterator, "models" + File.separator);
	}
	
	private static void persistImagesToDisk(int batchSize) {
		int maxSavedId = FileSystemDAO.findLatestTrainingDataId();
		int maxDbId = TrainingDbDao.getTotalNumberOfImages();
		for(int i = maxSavedId + 1; i <= maxDbId; i += batchSize) {
			List<TrainingData> images = TrainingDbDao.getTrainingData(i, i + batchSize - 1);
			FileSystemDAO.persist(images);
		}
	}
}
