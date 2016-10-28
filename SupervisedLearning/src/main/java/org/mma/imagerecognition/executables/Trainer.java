package org.mma.imagerecognition.executables;

import java.io.IOException;

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
	
	private static final long GIGABYTE = 1024 * 1024 * 1024;
	
	public static void main(String[] args) throws IOException {
		int testSize = intFromProperty("training.testSize");
		int validationSize = intFromProperty("training.validationSize");
		int trainSize = intFromProperty("training.trainSize");
		int batchSize = intFromProperty("training.persistence.batchSize");
		
		boolean checkIntegrity = PropertiesReader.getProjectProperties().getProperty("training.persistence.checkIntegrity").equals("true");
		boolean sliEnabled = PropertiesReader.getProjectProperties().getProperty("training.sli").equals("true");
		String trainingPersistenceType = PropertiesReader.getProjectProperties().getProperty("training.persistence.type");
		DataTypeUtil.setDTypeForContext(DataBuffer.Type.FLOAT);
		
//		CudaEnvironment.getInstance().getConfiguration()
//		    .setMaximumDeviceCacheableLength(GIGABYTE * 1)
//		    .setMaximumDeviceCache			(GIGABYTE * 12)
//		    .setMaximumHostCacheableLength	(GIGABYTE * 1)
//		    .setMaximumHostCache			(GIGABYTE * 16)
//			.allowMultiGPU(sliEnabled);
		
		FileSystemDAO.createFolders();
		if(trainingPersistenceType == null) {
			System.out.println("Please specify the persistence type of training data");
			System.exit(1);
		}
		if(trainingPersistenceType.equals("filesystem")) {
			Downloader.download(trainSize, validationSize, testSize, batchSize, checkIntegrity);
		}
		
		DataSetIterator trainIterator, testIterator;
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
	
	private static int intFromProperty(String property) {
		return Integer.parseInt(PropertiesReader.getProjectProperties().getProperty(property));
	}
}
