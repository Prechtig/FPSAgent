package org.mma.imagerecognition.configuration;

import java.io.File;
import java.io.IOException;
import java.util.Properties;

import org.deeplearning4j.nn.conf.MultiLayerConfiguration;
import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.optimize.listeners.ScoreIterationListener;
import org.deeplearning4j.ui.weights.HistogramIterationListener;
import org.deeplearning4j.util.ModelSerializer;
import org.mma.imagerecognition.dao.TrainingDbDao;
import org.mma.imagerecognition.tools.PropertiesReader;
import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;

public class ContinuousTraining implements Trainable {
	
	private String modelFilePath, modelFileName;
	private int height, width, featureCount, nEpochs;
	
	public ContinuousTraining() {
		init();
	}

	@Override
	public void train(DataSetIterator trainIterator, DataSetIterator testIterator) {
		MultiLayerConfiguration configuration = BuilderFactory.getConvNet(height, width, featureCount).build();
        MultiLayerNetwork model = new MultiLayerNetwork(configuration);
        model.init();
        
        model.setListeners(new HistogramIterationListener(1));
        model.setListeners(new ScoreIterationListener(1));
        
        for( int i=0; i<nEpochs; i++ ) {
            model.fit(trainIterator);
            System.out.println("*** Completed epoch {} ***" + i);
            try {
				ModelSerializer.writeModel(model, modelFilePath + modelFileName + i +".bin", true);
			} catch (IOException e) {
				e.printStackTrace();
			}
            testIterator.reset();
        }
	}
	
	private void init() {
		Properties projectProperties = PropertiesReader.getProjectProperties();
		modelFilePath = projectProperties.getProperty("training.persistence.savePath") + File.separator + "continuous" + File.separator;
		modelFileName = "model";
		width = TrainingDbDao.getWidth();
		height = TrainingDbDao.getHeight();
		featureCount = TrainingDbDao.getNumberOfGroundTruths();
        nEpochs = Integer.parseInt(projectProperties.getProperty("training.epochs"));
	}
}
