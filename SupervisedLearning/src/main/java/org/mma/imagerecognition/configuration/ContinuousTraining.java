package org.mma.imagerecognition.configuration;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.Properties;

import org.deeplearning4j.nn.conf.MultiLayerConfiguration;
import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.optimize.listeners.ScoreIterationListener;
import org.deeplearning4j.util.ModelSerializer;
import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.executables.DeadNeuronDetector;
import org.mma.imagerecognition.tools.PropertiesReader;
import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;

public abstract class ContinuousTraining implements Trainable {
	
	protected MultiLayerNetwork model;
	protected MultiLayerConfiguration configuration;
	protected int height, width, featureCount, nEpochs, latestEpoch = 0;
	private boolean outputDeadNeurons, saveModel;
	
	public ContinuousTraining() throws FileNotFoundException, IOException {
		init();
	}

	@Override
	public void train(DataSetIterator trainIterator, DataSetIterator testIterator) {
        for(int i = latestEpoch; i <= nEpochs; i++) {
            model.fit(trainIterator);
            System.out.println(String.format("*** Completed epoch %d ***", i));
            testIterator.reset();
            
            saveModel(model, i);
            outputDeadNeurons(model);
        }
	}
	
	protected void saveModel(MultiLayerNetwork model, int numeration) {
        if(saveModel) {
            try {
				ModelSerializer.writeModel(model, FileSystemDAO.getPathOfModelFile(numeration).toString(), true);
			} catch (IOException e) {
				e.printStackTrace();
			}
        }
	}
	
	protected void outputDeadNeurons(MultiLayerNetwork model) {
		if(outputDeadNeurons) {
			DeadNeuronDetector.getDeadNeurons(model, 100);
		}
	}
	
	protected void init() throws FileNotFoundException, IOException {
		Properties projectProperties = PropertiesReader.getProjectProperties();
		width = Integer.parseInt(projectProperties.getProperty("training.image.width"));
		height = Integer.parseInt(projectProperties.getProperty("training.image.height"));
		featureCount = TrainingData.getFeatureCount();
        nEpochs = Integer.parseInt(projectProperties.getProperty("training.epochs"));
        outputDeadNeurons = projectProperties.getProperty("training.outputDeadNeurons").equals("true");
        saveModel = projectProperties.getProperty("training.saveModel").equals("true");
        initConfig();
        initNetwork();
	}
	
	protected void initConfig() {
		configuration = BuilderFactory.getDeepConvNet(height, width, featureCount).build();
	}
	
	protected void initNetwork() throws FileNotFoundException, IOException {
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
