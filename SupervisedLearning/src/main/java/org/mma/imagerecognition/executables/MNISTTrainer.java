package org.mma.imagerecognition.executables;

import java.io.IOException;
import java.util.Properties;

import org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator;
import org.deeplearning4j.nn.conf.MultiLayerConfiguration;
import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.optimize.listeners.ScoreIterationListener;
import org.mma.imagerecognition.configuration.BuilderFactory;
import org.mma.imagerecognition.tools.PropertiesReader;
import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;

public class MNISTTrainer {
	
	private static int height, width, featureCount, nEpochs, batchSize;
	
	static {
		init();
	}
	
	public static void main(String[] args) throws IOException {
		
		MultiLayerConfiguration conf = BuilderFactory.getShallowConvNet(height, width, featureCount).build();
		MultiLayerNetwork model = new MultiLayerNetwork(conf);
		model.init();
		
		model.setListeners(new ScoreIterationListener(1));
		
		DataSetIterator mnistTrain1024 = new MnistDataSetIterator(batchSize,1024,false,true,true,12345);
//        DataSetIterator mnistTest512 = new MnistDataSetIterator(batchSize,512,false,false,true,12345);
		
		for(int i = 1; i <= nEpochs; i++) {
			
            model.fit(mnistTrain1024);
            System.out.println(String.format("*** Completed epoch %d ***", i));
            DeadNeuronDetector.getDeadNeurons(model, 100);
//            try {
//				ModelSerializer.writeModel(model, modelFilePath + modelFileName + i +".bin", true);
//			} catch (IOException e) {
//				e.printStackTrace();
//			}
        }
		
		
	}
	
	private static void init() {
		Properties projectProperties = PropertiesReader.getProjectProperties();
		width = Integer.parseInt(projectProperties.getProperty("training.image.width"));
		height = Integer.parseInt(projectProperties.getProperty("training.image.height"));
		featureCount = 10;
        nEpochs = Integer.parseInt(projectProperties.getProperty("training.epochs"));
        batchSize = Integer.parseInt(projectProperties.getProperty("training.persistence.batchSize"));
	}
}
