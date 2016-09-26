package org.mma.imagerecognition.configuration;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;

import org.apache.commons.lang3.ArrayUtils;
import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.util.ModelSerializer;
import org.mma.imagerecognition.bridge.SingleThreadedBridge;
import org.mma.imagerecognition.tools.INDArrayTool;
import org.mma.imagerecognition.tools.ImageTool;
import org.mma.imagerecognition.tools.PropertiesReader;
import org.nd4j.linalg.api.ndarray.INDArray;

public class TestSaver {
	
	public static void main(String[] args) throws IOException {
		int width = Integer.parseInt(PropertiesReader.getProjectProperties().getProperty("image.width"));
		
		MultiLayerNetwork network = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(new File("models" + File.separator + "bestModel.bin")));
		
		SingleThreadedBridge bridge = new SingleThreadedBridge();
		bridge.listen();
		
		while(true) {
			byte[] flattenedImage = bridge.readData();
			if(ArrayUtils.isEmpty(flattenedImage)) {
				break;
			}
			INDArray input = ImageTool.convertToINDArray(flattenedImage, width);
			INDArray output = network.output(input);
			bridge.sendData(INDArrayTool.toFlatDoubleArray(output));
		}
		bridge.closeServer();
	}
}
