package org.mma.imagerecognition.configuration;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.util.Arrays;

import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.util.ModelSerializer;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.dbaccess.TrainingDbDao;
import org.mma.imagerecognition.tools.INDArrayTool;
import org.mma.imagerecognition.tools.ImageTool;
import org.nd4j.linalg.api.ndarray.INDArray;

public class TestSaver {
	public static void main(String[] args) throws IOException {
		
//		File tempFile = new File(FilenameUtils.concat(System.getProperty("java.io.tmpdir"), "MNISTTest/bestModel.bin"));
//		FileInputStream fis = new FileInputStream(tempFile);
//		MultiLayerNetwork network = ModelSerializer.restoreMultiLayerNetwork(fis);
//		
//		INDArray ndArray = INDArrayTool.generateRandomVector(28*28);
//		INDArray output = network.output(ndArray);
//
//		System.out.println(Arrays.toString(INDArrayTool.toFlatDoubleArray(output)));
//		
//		
//		TrainingDbDao.initializeConnection();
		
		
		File model = new File("models\\bestModel.bin");
		FileInputStream fis = new FileInputStream(model);
		MultiLayerNetwork network = ModelSerializer.restoreMultiLayerNetwork(fis);
		
		TrainingData td = TrainingDbDao.getImages(1).get(0);
		INDArray indArray = ImageTool.convertToINDArray(td.getPixelData(), td.getWidth());
		INDArray output = network.output(indArray);
		
		System.out.println(Arrays.toString(INDArrayTool.toFlatDoubleArray(output)));
		System.out.println(Arrays.toString(td.getFeatureDoubles()));
		ImageTool.printPngImage(td.getPixelData(), td.getWidth(), new File("testimagefromnetwork.png"));
		
		
		
	}
}
