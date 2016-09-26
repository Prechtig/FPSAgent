package org.mma.imagerecognition.configuration;

import java.io.File;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Map;
import java.util.concurrent.TimeUnit;

import org.deeplearning4j.earlystopping.EarlyStoppingConfiguration;
import org.deeplearning4j.earlystopping.EarlyStoppingModelSaver;
import org.deeplearning4j.earlystopping.EarlyStoppingResult;
import org.deeplearning4j.earlystopping.saver.LocalFileModelSaver;
import org.deeplearning4j.earlystopping.scorecalc.DataSetLossCalculator;
import org.deeplearning4j.earlystopping.termination.MaxEpochsTerminationCondition;
import org.deeplearning4j.earlystopping.termination.MaxTimeIterationTerminationCondition;
import org.deeplearning4j.earlystopping.trainer.EarlyStoppingTrainer;
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
import org.mma.imagerecognition.dao.TrainingDbDao;
import org.mma.imagerecognition.tools.PropertiesReader;
import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;
import org.nd4j.linalg.lossfunctions.LossFunctions;

public class EarlyStoppingTraining implements Trainable {

	public void train(DataSetIterator trainIterator, DataSetIterator testIterator) {
		int width = TrainingDbDao.getWidth();
		int height = TrainingDbDao.getHeight();
		int numberOfGroundTruths = TrainingDbDao.getNumberOfGroundTruths();
		
		// Configuration
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
		
		//Training
        EarlyStoppingModelSaver<MultiLayerNetwork> saver = new LocalFileModelSaver(PropertiesReader.getProjectProperties().getProperty("training.persistence.savePath") + File.separator);
        EarlyStoppingConfiguration<MultiLayerNetwork> esConf = new EarlyStoppingConfiguration.Builder<MultiLayerNetwork>()
                .epochTerminationConditions(new MaxEpochsTerminationCondition(50))
                .evaluateEveryNEpochs(1)
                .iterationTerminationConditions(new MaxTimeIterationTerminationCondition(10, TimeUnit.MINUTES)) //Max of 20 minutes
                .scoreCalculator(new DataSetLossCalculator(testIterator, true))     //Calculate test set score
                .modelSaver(saver)
                .build();

        EarlyStoppingTrainer trainer = new EarlyStoppingTrainer(esConf,configuration,trainIterator);

        //Conduct early stopping training:
        long startTime = System.currentTimeMillis();
        EarlyStoppingResult<MultiLayerNetwork> result = trainer.fit();
        System.out.println(String.format("The run took %d milliseconds", System.currentTimeMillis() - startTime));
        System.out.println("Termination reason: " + result.getTerminationReason());
        System.out.println("Termination details: " + result.getTerminationDetails());
        System.out.println("Total epochs: " + result.getTotalEpochs());
        System.out.println("Best epoch number: " + result.getBestModelEpoch());
        System.out.println("Score at best epoch: " + result.getBestModelScore());

        //Print score vs. epoch
        Map<Integer,Double> scoreVsEpoch = result.getScoreVsEpoch();
        List<Integer> list = new ArrayList<>(scoreVsEpoch.keySet());
        Collections.sort(list);
        System.out.println("Score vs. Epoch:");
        for( Integer i : list){
            System.out.println(i + "\t" + scoreVsEpoch.get(i));
        }
	}
}
