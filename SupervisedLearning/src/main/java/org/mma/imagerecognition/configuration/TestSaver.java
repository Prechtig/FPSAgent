package org.mma.imagerecognition.configuration;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;

import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.util.ModelSerializer;
import org.nd4j.linalg.cpu.nativecpu.NDArray;

public class TestSaver {
	public static void main(String[] args) throws IOException {
		MultiLayerNetwork network = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(new File("models\\bestModel.bin")));
		network.output(new NDArray());
	}
}
