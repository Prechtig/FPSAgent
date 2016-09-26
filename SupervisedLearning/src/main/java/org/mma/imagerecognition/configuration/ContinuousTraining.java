package org.mma.imagerecognition.configuration;

import java.io.File;
import java.io.IOException;

import org.deeplearning4j.eval.Evaluation;
import org.deeplearning4j.nn.api.OptimizationAlgorithm;
import org.deeplearning4j.nn.conf.MultiLayerConfiguration;
import org.deeplearning4j.nn.conf.MultiLayerConfiguration.Builder;
import org.deeplearning4j.nn.conf.NeuralNetConfiguration;
import org.deeplearning4j.nn.conf.Updater;
import org.deeplearning4j.nn.conf.layers.ConvolutionLayer;
import org.deeplearning4j.nn.conf.layers.DenseLayer;
import org.deeplearning4j.nn.conf.layers.OutputLayer;
import org.deeplearning4j.nn.conf.layers.SubsamplingLayer;
import org.deeplearning4j.nn.conf.layers.setup.ConvolutionLayerSetup;
import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.nn.weights.WeightInit;
import org.deeplearning4j.optimize.listeners.ScoreIterationListener;
import org.deeplearning4j.ui.weights.HistogramIterationListener;
import org.deeplearning4j.util.ModelSerializer;
import org.mma.imagerecognition.dao.TrainingDbDao;
import org.mma.imagerecognition.tools.PropertiesReader;
import org.nd4j.linalg.api.ndarray.INDArray;
import org.nd4j.linalg.dataset.DataSet;
import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;
import org.nd4j.linalg.lossfunctions.LossFunctions;

public class ContinuousTraining implements Trainable {

	@Override
	public void train(DataSetIterator trainIterator, DataSetIterator testIterator) {
		int width = TrainingDbDao.getWidth();
		int height = TrainingDbDao.getHeight();
		int numberOfGroundTruths = TrainingDbDao.getNumberOfGroundTruths();
        int nEpochs = 50;

        Builder builder = new NeuralNetConfiguration.Builder()
				.seed(98)
				.iterations(1)
				.regularization(true)
				.l2(0.0005)
				.learningRate(0.02)
				.weightInit(WeightInit.XAVIER)
				.activation("relu")
				.optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT)
				.updater(Updater.NESTEROVS).momentum(0.9)
				.list()
				.layer(0, new ConvolutionLayer.Builder(3, 3)
						.nIn(1)
						.stride(1, 1)
						.nOut(20)
						.dropOut(0.5)
						.activation("relu")
						.build())
				.layer(1,
						new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
						.kernelSize(2, 2)
						.stride(2, 2)
						.build())
				.layer(2, new ConvolutionLayer.Builder(3, 3)
						.nIn(1)
						.stride(1, 1)
						.nOut(20)
						.dropOut(0.5)
						.activation("relu")
						.build())
				.layer(3,
						new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
						.kernelSize(2, 2)
						.stride(2, 2)
						.build())
				.layer(4, new ConvolutionLayer.Builder(3, 3)
						.nIn(1)
						.stride(1, 1)
						.nOut(20)
						.dropOut(0.5)
						.activation("relu")
						.build())
				.layer(5, new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
						.kernelSize(2, 2)
						.stride(2, 2)
						.build())
				.layer(6, new DenseLayer.Builder()
						.nOut(numberOfGroundTruths)
						.activation("sigmoid")
						.build())
				.layer(7, new OutputLayer.Builder(LossFunctions.LossFunction.SQUARED_LOSS)
						.nOut(numberOfGroundTruths)
						.activation("identity")
						.build())
				.backprop(true).pretrain(false);
		new ConvolutionLayerSetup(builder, width, height, 3);
		MultiLayerConfiguration configuration = builder.build();

        MultiLayerNetwork model = new MultiLayerNetwork(configuration);
        model.init();
        
        model.setListeners(new HistogramIterationListener(1));
        model.setListeners(new ScoreIterationListener(1));
        
        double bestAccuracy = Double.MAX_VALUE;
        for( int i=0; i<nEpochs; i++ ) {
            model.fit(trainIterator);
            System.out.println("*** Completed epoch {} ***" + i);

            System.out.println("Evaluate model....");
            Evaluation eval = new Evaluation(numberOfGroundTruths);
            while(testIterator.hasNext()){
                DataSet ds = testIterator.next();
                INDArray output = model.output(ds.getFeatureMatrix(), false);
                eval.eval(ds.getLabels(), output);
                if (eval.accuracy() < bestAccuracy) {
                	bestAccuracy = eval.accuracy();
                	try {
						ModelSerializer.writeModel(model, PropertiesReader.getProjectProperties().getProperty("training.persistence.savePath") + File.separator + "continuous" + File.separator + "bestModel.bin", true);
					} catch (IOException e) {
						e.printStackTrace();
					}
                }
            }
            testIterator.reset();
        }
	}
}
