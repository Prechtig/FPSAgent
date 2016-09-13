package org.mma.imagerecognition.configuration;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.util.Arrays;

import org.apache.commons.io.FilenameUtils;
import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.util.ModelSerializer;
import org.mma.imagerecognition.tools.INDArrayTool;
import org.nd4j.linalg.api.ndarray.INDArray;

public class TestSaver {
	public static void main(String[] args) throws IOException {
		
		File tempFile = new File(FilenameUtils.concat(System.getProperty("java.io.tmpdir"), "MNISTTest/bestModel.bin"));
		FileInputStream fis = new FileInputStream(tempFile);
		MultiLayerNetwork network = ModelSerializer.restoreMultiLayerNetwork(fis);
		
		INDArray ndArray = INDArrayTool.generateRandomVector(28*28);
		INDArray output = network.output(ndArray);

		System.out.println(Arrays.toString(INDArrayTool.toFlatDoubleArray(output)));
	}
}
