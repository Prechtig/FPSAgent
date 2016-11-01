package org.mma.imagerecognition.executables;

import java.io.IOException;

import org.mma.imagerecognition.configuration.ContinuousParallelTraining;
import org.mma.imagerecognition.configuration.ContinuousSequentialTraining;
import org.mma.imagerecognition.configuration.ContinuousTraining;
import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.iterator.DatabaseIterator;
import org.mma.imagerecognition.iterator.FileSystemIterator;
import org.mma.imagerecognition.tools.PropertiesReader;
//import org.nd4j.jita.conf.CudaEnvironment;
import org.nd4j.linalg.api.buffer.DataBuffer;
import org.nd4j.linalg.api.buffer.util.DataTypeUtil;
import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;

public class Trainer {
	
	@SuppressWarnings("unused")
	private static final long GIGABYTE = 1024 * 1024 * 1024;
	
	public static void main(String[] args) throws IOException {
		int testSize = intFromProperty("training.testSize");
		int validationSize = intFromProperty("training.validationSize");
		int trainSize = intFromProperty("training.trainSize");
		int batchSize = intFromProperty("training.persistence.batchSize");
		
		boolean checkIntegrity = boolFromProperty("training.persistence.checkIntegrity");
		@SuppressWarnings("unused")
		boolean sliEnabled = boolFromProperty("training.sli");
		
		String trainingPersistenceType = stringFromProperty("training.persistence.type");
		String trainingType = stringFromProperty("training.type");
		
		DataTypeUtil.setDTypeForContext(DataBuffer.Type.FLOAT);
		
//		CudaEnvironment.getInstance().getConfiguration()
//		    .setMaximumDeviceCacheableLength(GIGABYTE * 1)
//		    .setMaximumDeviceCache			(GIGABYTE * 6)
//		    .setMaximumHostCacheableLength	(GIGABYTE * 1)
//		    .setMaximumHostCache			(GIGABYTE * 5)
//			.allowMultiGPU(sliEnabled);
		
		FileSystemDAO.createFolders();
		if(trainingPersistenceType == null) {
			System.out.println("Please specify the persistence type of training data");
			System.exit(1);
		}
		
		ContinuousTraining trainer = null;
		switch (trainingType) {
		case "parallel":
			trainer = new ContinuousParallelTraining();
			break;
		case "sequential":
			trainer = new ContinuousSequentialTraining();
			break;
		default:
			System.out.println("Please provide an option for \"training.type\" in the properties.");
			System.out.println("Currently \"parallel\" and \"sequential\" is supported");
			System.exit(12321);
		}
		
		DataSetIterator trainIterator, testIterator;
		if(trainingPersistenceType.equals("filesystem")) {
			Persistance.persist(trainSize, validationSize, testSize, batchSize, checkIntegrity);
			
			testIterator = new FileSystemIterator(batchSize, testSize);
			trainIterator = new FileSystemIterator(batchSize, trainSize);
		} else {
			testIterator = new DatabaseIterator(batchSize, testSize);
			trainIterator = new DatabaseIterator(batchSize, trainSize);
		}
		
		trainer.train(trainIterator, testIterator);
	}
	
	private static int intFromProperty(String property) {
		return Integer.parseInt(PropertiesReader.getProjectProperties().getProperty(property));
	}
	
	private static boolean boolFromProperty(String property) {
		return "true".equals(PropertiesReader.getProjectProperties().getProperty(property));
	}
	
	private static String stringFromProperty(String property) {
		return PropertiesReader.getProjectProperties().getProperty(property);
	}
}
