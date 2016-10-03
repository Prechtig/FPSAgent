package org.mma.imagerecognition.controller;

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
import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;

public class Controller {
	public static void main(String[] args) throws IOException {
//		CudaEnvironment.getInstance().getConfiguration().allowMultiGPU(true);
		
		int batchSize = Integer.parseInt(PropertiesReader.getProjectProperties().getProperty("training.persistence.batchSize"));;
		String trainingPersistenceType = PropertiesReader.getProjectProperties().getProperty("training.persistence.type");
		if(trainingPersistenceType == null) {
			System.out.println("Please specify the persistence type of training data");
			System.exit(1);
		}
		if(trainingPersistenceType.equals("filesystem")) {
			FileSystemDAO.createFolders();
			persistImagesToDisk(batchSize);
		}
		
		DataSetIterator trainIterator, testIterator;
		
		if(trainingPersistenceType.equals("filesystem")) {
			testIterator = new FileSystemIterator(batchSize, 50);
			trainIterator = new FileSystemIterator(batchSize, 50);
		} else {
			testIterator = new DatabaseIterator(batchSize, 40);
			trainIterator = new DatabaseIterator(batchSize, 80);
		}
		
		new ContinuousTraining().train(trainIterator, testIterator);
	}
	
	private static void persistImagesToDisk(int batchSize) {
		int maxSavedId = FileSystemDAO.findLatestTrainingDataId();
		int maxDbId = TrainingDbDao.getTotalNumberOfImages();
		
		if(maxSavedId != maxDbId) {
			System.out.println("Downloading " + (maxDbId - maxSavedId) + " instances of training data");
		}
		
		for(int i = maxSavedId + 1; i <= maxDbId; i += batchSize) {
			List<TrainingData> images = TrainingDbDao.getTrainingData(i, i + batchSize - 1);
			FileSystemDAO.persist(images);
		}
	}
}
