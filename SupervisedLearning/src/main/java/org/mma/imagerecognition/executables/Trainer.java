package org.mma.imagerecognition.executables;

import java.io.FileNotFoundException;
import java.io.IOException;

import org.mma.imagerecognition.configuration.ContinuousParallelTraining;
import org.mma.imagerecognition.configuration.ContinuousSequentialTraining;
import org.mma.imagerecognition.configuration.EarlyStoppingTraining;
import org.mma.imagerecognition.configuration.Trainable;
import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.iterator.DatabaseIterator;
import org.mma.imagerecognition.iterator.FileSystemIterator;
import org.mma.imagerecognition.tools.PropertiesReader;
//import org.nd4j.jita.conf.CudaEnvironment;
import org.nd4j.linalg.api.buffer.DataBuffer;
import org.nd4j.linalg.api.buffer.util.DataTypeUtil;
import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;

public class Trainer {
	
	private static int testSize, validationSize, trainSize, batchSize;
	private static String trainingPersistenceType, trainingType;
	
	static {
		readProperties();
	}

	public static void main(String[] args) throws IOException {
		init();
		doTrain();
	}
	
	private static void configureND4J() {
		DataTypeUtil.setDTypeForContext(DataBuffer.Type.FLOAT);
		/*
		final long GIGABYTE = 1024 * 1024 * 1024;
		CudaEnvironment.getInstance().getConfiguration()
	    .setMaximumDeviceCacheableLength(GIGABYTE * 1)
	    .setMaximumDeviceCache			(GIGABYTE * 12)
	    .setMaximumHostCacheableLength	(GIGABYTE * 1)
	    .setMaximumHostCache			(GIGABYTE * 12)
		.allowMultiGPU(boolFromProperty("training.sli"));
		 */
	}
	
	private static void doTrain() throws FileNotFoundException, IOException {
		DataSetIterator trainIterator = getIterator(batchSize, trainSize);
		DataSetIterator testIterator = getIterator(batchSize, testSize);
		
		getTrainer().train(trainIterator, testIterator);
	}
	
	private static DataSetIterator getIterator(int batchSize, int numExamples) {
		switch (trainingPersistenceType) {
		case "filesystem":
			return new FileSystemIterator(batchSize, numExamples);
		case "database":
			return new DatabaseIterator(batchSize, numExamples);
		default:
			return null;
		}
	}
	
	private static Trainable getTrainer() throws FileNotFoundException, IOException {
		switch (trainingType) {
		case "parallel":
			return new ContinuousParallelTraining();
		case "sequential":
			return new ContinuousSequentialTraining();
		case "early-stopping":
			return new EarlyStoppingTraining();
		default:
			return null;
		}
	}
	
	public static void init() throws IOException {
		init(0, trainSize+validationSize+testSize);
	}
	
	public static void init(int fromId, int toId) throws IOException {
		configureND4J();
		
		checkProperties();
		
		FileSystemDAO.createFolders();
		if(trainingPersistenceType.equals("filesystem")) {
			FileSystemDAO.persist(fromId, toId, batchSize);
		}
	}
	
	private static void readProperties() {
		testSize = intFromProperty("training.testSize");
		validationSize = intFromProperty("training.validationSize");
		trainSize = intFromProperty("training.trainSize");
		batchSize = intFromProperty("training.persistence.batchSize");
		
		trainingPersistenceType = stringFromProperty("training.persistence.type");
		trainingType = stringFromProperty("training.type");
	}
	
	private static void checkProperties() {
		boolean exit = false;
		
		if(trainingPersistenceType == null) {
			System.out.println("Please provide an option for \"training.persistence.type\" in the properties.");
			System.out.println("Currently \"filesystem\" and \"database\" is supported");
			exit = true;
		}
		if(trainingType == null) {
			System.out.println("Please provide an option for \"training.type\" in the properties.");
			System.out.println("Currently \"parallel\", \"sequential\" and \"early-stopping\" is supported");
			exit = true;
		}
		
		if(exit) {
			System.exit(12321);
		}
	}
	
	private static int intFromProperty(String property) {
		return Integer.parseInt(PropertiesReader.getProjectProperties().getProperty(property));
	}
	
	private static String stringFromProperty(String property) {
		return PropertiesReader.getProjectProperties().getProperty(property);
	}
	
	public static int lastTrainIndex() {
		return trainSize;
	}
	
	public static int lastValidationIndex() {
		return lastTrainIndex()+validationSize;
	}
	
	public static int lastTestIndex() {
		return lastValidationIndex() + testSize;
	}
}
