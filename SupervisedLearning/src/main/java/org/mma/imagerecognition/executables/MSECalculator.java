package org.mma.imagerecognition.executables;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.nio.file.Paths;
import java.util.Properties;
import java.util.stream.DoubleStream;

import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.util.ModelSerializer;
import org.mma.imagerecognition.iterator.DatabaseIterator;
import org.mma.imagerecognition.tools.INDArrayTool;
import org.mma.imagerecognition.tools.PropertiesReader;
import org.nd4j.linalg.api.ndarray.INDArray;

public class MSECalculator {
	
	public static void main(String[] args) throws FileNotFoundException, IOException {
		Properties projectProperties = PropertiesReader.getProjectProperties();
		int trainSize 	= Integer.parseInt(projectProperties.getProperty("training.trainSize"));
		int testSize	= Integer.parseInt(projectProperties.getProperty("training.testSize"));
		int batchSize	= Integer.parseInt(projectProperties.getProperty("training.persistence.batchSize"));
		
		Trainer.init(trainSize+1, trainSize+testSize);
		
		File networkFile = Paths.get("models", "angular", "nolight", "deep", "model12.bin").toFile();
		MultiLayerNetwork network = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(networkFile));
		
		INDArray scores = network.scoreExamples(new DatabaseIterator(batchSize, testSize), false);
		double[] scoresAsDouble = INDArrayTool.toFlatDoubleArray(scores);
		double scoreSum = DoubleStream.of(scoresAsDouble).sum();
		System.out.println(String.format("Avg score: %f", scoreSum/scoresAsDouble.length));
	}

}
