package org.mma.imagerecognition.configuration;

import java.io.File;
import java.io.IOException;
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
import org.deeplearning4j.nn.weights.WeightInit;
import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dao.TrainingDbDao;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.iterator.DatabaseIterator;
import org.mma.imagerecognition.iterator.FileSystemIterator;
import org.mma.imagerecognition.tools.PropertiesReader;
import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;

public class TestConfiguration {

	private static int seed = 98;

	private static int iterations = 1;
	private static double learningRate = 0.02;
	private static String activationFunction = "relu";
	private static WeightInit weightInit = WeightInit.XAVIER;
	private static OptimizationAlgorithm optimizationAlgorithm = OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT;
	private static Updater updater = Updater.NESTEROVS;
	private static double momentum = 0.9;

	// Regularization
	private static double l2regularization = 0.0005;

	public static void main(String[] args) throws IOException {
		String trainingPersistenceType = PropertiesReader.getProjectProperties().getProperty("training.persistence.type");
		if(trainingPersistenceType == null) {
			System.out.println("Please specify the persistence type of training data");
			System.exit(1);
		}
		if(trainingPersistenceType.equals("filesystem")) {
			FileSystemDAO.createFolders();
			String batchSize = PropertiesReader.getProjectProperties().getProperty("training.persistence.batchSize");
			persistImagesToDisk(Integer.parseInt(batchSize));
		}
		
		train();
	}

	public static void train() {
		int width = TrainingDbDao.getWidth();
		int height = TrainingDbDao.getHeight();
		int numberOfGroundTruths = TrainingDbDao.getNumberOfGroundTruths();
		
		// Configuration
		Builder builder = new NeuralNetConfiguration.Builder()
				.seed(seed)
				.iterations(iterations)
				.regularization(true)
				.l2(l2regularization)
				.learningRate(learningRate)
				.weightInit(weightInit)
				.activation(activationFunction)
				.optimizationAlgo(optimizationAlgorithm)
				.updater(updater).momentum(momentum)
				.list()
				.layer(0, new ConvolutionLayer.Builder(5, 5)
						.nIn(1)
						.stride(1, 1)
						.nOut(20)
						.dropOut(0.5)
						.build())
				.layer(1,
						new SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)
						.kernelSize(2, 2)
						.stride(2, 2)
						.build())
				.layer(2, new DenseLayer.Builder()
						.nOut(numberOfGroundTruths)
						.build())
				.layer(3, new OutputLayer.Builder().nOut(numberOfGroundTruths).build())
				.backprop(true).pretrain(false);
		new ConvolutionLayerSetup(builder, width, height, 3);
		MultiLayerConfiguration configuration = builder.build();
		
		DataSetIterator testIterator, trainIterator;
		
		// TODO: Clean up
		String trainingPersistenceType = PropertiesReader.getProjectProperties().getProperty("training.persistence.type");
		if(trainingPersistenceType.equals("filesystem")) {
			testIterator = new FileSystemIterator(25, 200);
			trainIterator = new FileSystemIterator(25, 400);
		} else {
			testIterator = new DatabaseIterator(25, 200);
			trainIterator = new DatabaseIterator(25, 400);
		}
		
		//Training
        String exampleDirectory = "models" + File.separator;
        EarlyStoppingModelSaver saver = new LocalFileModelSaver(exampleDirectory);
        EarlyStoppingConfiguration esConf = new EarlyStoppingConfiguration.Builder()
                .epochTerminationConditions(new MaxEpochsTerminationCondition(1)) //Max of 50 epochs
                .evaluateEveryNEpochs(1)
                .iterationTerminationConditions(new MaxTimeIterationTerminationCondition(30, TimeUnit.MINUTES)) //Max of 20 minutes
                .scoreCalculator(new DataSetLossCalculator(testIterator, true))     //Calculate test set score
                .modelSaver(saver)
                .build();

        EarlyStoppingTrainer trainer = new EarlyStoppingTrainer(esConf,configuration,trainIterator);

        //Conduct early stopping training:
        long startTime = System.currentTimeMillis();
        EarlyStoppingResult result = trainer.fit();
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
	
	private static void persistImagesToDisk(int batchSize) {
		int maxSavedId = FileSystemDAO.findLatestTrainingDataId();
		int maxDbId = TrainingDbDao.getTotalNumberOfImages();
		for(int i = maxSavedId + 1; i <= maxDbId; i += batchSize) {
			List<TrainingData> images = TrainingDbDao.getTrainingData(i, i + batchSize - 1);
			FileSystemDAO.persist(images);
		}
	}
}
