package org.mma.imagerecognition.executables;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.nio.file.Path;
import java.util.List;

import org.apache.commons.lang.ArrayUtils;
import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.util.ModelSerializer;
import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.tools.INDArrayTool;
import org.mma.imagerecognition.tools.ImageTool;
import org.mma.imagerecognition.tools.NNTool;
import org.nd4j.linalg.api.ndarray.INDArray;
import org.nd4j.linalg.factory.Nd4j;

public class Evaluator {
	public static void main(String[] args) throws FileNotFoundException, IOException {
		Trainer.init();
		FileSystemDAO.createFolders();
		Path modelPath;
		if(args.length != 1) {
			modelPath = FileSystemDAO.getPathOfLatestModelFile();
		} else {
			modelPath = FileSystemDAO.getPathOfModelFile(Integer.parseInt(args[0]));
		}
		
		MultiLayerNetwork network = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(modelPath.toFile()));
		double accuracy = getAccuracy(network);
		System.out.format("Hit rate: %f", accuracy);
	}
	
	private static double getAccuracy(MultiLayerNetwork network) {
		List<TrainingData> testSet = FileSystemDAO.load(Trainer.lastValidationIndex()+1, Trainer.lastTestIndex());
		int hits = 0, misses = 0;
		
		for(TrainingData td : testSet) {
			double[] groundTruths = td.getFeatures();
			INDArray output = network.output(Nd4j.create(ImageTool.toScaledDoubles(td.getPixelData()), new int[] { 1, 3, td.getWidth(), td.getHeight()}), false);
			double[] binaryVector = NNTool.toBinaryVector(INDArrayTool.toFlatDoubleArray(output));
			if(ArrayUtils.isEquals(binaryVector, groundTruths)) {
				hits++;
			} else {
				misses++;
			}
		}
		
		return ((double) hits) / ((double) testSet.size());
	}
}
