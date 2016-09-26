package org.mma.imagerecognition.configuration;

import java.io.File;
import java.io.IOException;

import org.deeplearning4j.eval.Evaluation;
import org.deeplearning4j.nn.conf.MultiLayerConfiguration;
import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.optimize.listeners.ScoreIterationListener;
import org.deeplearning4j.ui.weights.HistogramIterationListener;
import org.deeplearning4j.util.ModelSerializer;
import org.mma.imagerecognition.dao.TrainingDbDao;
import org.mma.imagerecognition.tools.PropertiesReader;
import org.nd4j.linalg.api.ndarray.INDArray;
import org.nd4j.linalg.dataset.DataSet;
import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;

public class ContinuousTraining implements Trainable {

	@Override
	public void train(DataSetIterator trainIterator, DataSetIterator testIterator) {
		String modelFilePath = PropertiesReader.getProjectProperties().getProperty("training.persistence.savePath") + File.separator + "continuous" + File.separator;
		String modelFileName = "model";
		int width = TrainingDbDao.getWidth();
		int height = TrainingDbDao.getHeight();
		int featureCount = TrainingDbDao.getNumberOfGroundTruths();
        int nEpochs = 100;

		MultiLayerConfiguration configuration = BuilderFactory.getDeepConvNet(height, width, featureCount).build();
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
        
        System.out.println("Evaluate model....");
        Evaluation eval = new Evaluation(featureCount);
        while(testIterator.hasNext()){
            DataSet ds = testIterator.next();
            INDArray output = model.output(ds.getFeatureMatrix(), false);
            eval.eval(ds.getLabels(), output);
        }
	}
}
