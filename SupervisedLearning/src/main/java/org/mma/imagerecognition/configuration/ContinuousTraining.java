package org.mma.imagerecognition.configuration;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.Properties;

import org.deeplearning4j.nn.conf.MultiLayerConfiguration;
import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.optimize.listeners.ScoreIterationListener;
import org.deeplearning4j.parallelism.ParallelWrapper;
import org.deeplearning4j.util.ModelSerializer;
import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.executables.DeadNeuronDetector;
import org.mma.imagerecognition.tools.PropertiesReader;
import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;

public class ContinuousTraining implements Trainable {
	
	private MultiLayerNetwork model;
	private MultiLayerConfiguration configuration;
	private int height, width, featureCount, nEpochs;
	private int latestEpoch = 0;
	
	public ContinuousTraining() throws FileNotFoundException, IOException {
		init();
	}

	@Override
	public void train(DataSetIterator trainIterator, DataSetIterator testIterator) {
        for(int i = latestEpoch+1; i <= nEpochs; i++) {
            model.fit(trainIterator);
            System.out.println("*** Completed epoch {} ***" + i);
            try {
				ModelSerializer.writeModel(model, FileSystemDAO.getPathOfLatestModelFile(i).toString(), true);
			} catch (IOException e) {
				e.printStackTrace();
			}
            DeadNeuronDetector.getDeadNeurons(model, 100);
            testIterator.reset();
        }
	}
	
	public void trainParallel(DataSetIterator trainIterator, DataSetIterator testIterator) {
		ParallelWrapper wrapper = new ParallelWrapper.Builder(model)
	        .prefetchBuffer(4)
	        .workers(4)
	        .averagingFrequency(1)
	        .reportScoreAfterAveraging(true)
	        .useLegacyAveraging(true)
	        .build();
		
		for(int i = latestEpoch; i <= nEpochs; i++) {
            wrapper.fit(trainIterator);
            System.out.println("*** Completed epoch {} ***" + i);
            DeadNeuronDetector.getDeadNeurons(model, 100);
            try {
				ModelSerializer.writeModel(model, FileSystemDAO.getPathOfLatestModelFile(i).toString(), true);
			} catch (IOException e) {
				e.printStackTrace();
			}
            testIterator.reset();
        }
	}
	
	private void init() throws FileNotFoundException, IOException {
		Properties projectProperties = PropertiesReader.getProjectProperties();
		width = Integer.parseInt(projectProperties.getProperty("training.image.width"));
		height = Integer.parseInt(projectProperties.getProperty("training.image.height"));
		featureCount = TrainingData.getFeatureCount();
        nEpochs = Integer.parseInt(projectProperties.getProperty("training.epochs"));
        initConfig();
        initNetwork();
	}
	
	private void initConfig() {
		configuration = BuilderFactory.getDeepConvNet(height, width, featureCount).build();
	}
	
	private void initNetwork() throws FileNotFoundException, IOException {
		if(PropertiesReader.getProjectProperties().getProperty("training.continueFromLatestModel").equals("true")) {
			model = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(FileSystemDAO.getPathOfLatestModelFile().toString()));
			latestEpoch = FileSystemDAO.findLatestModelId();
		} else {
			model = new MultiLayerNetwork(configuration);
	        model.init();
		}
		model.setListeners(new ScoreIterationListener(1));
	}
}
