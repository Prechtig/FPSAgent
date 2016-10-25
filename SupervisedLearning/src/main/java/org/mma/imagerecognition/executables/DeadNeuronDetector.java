package org.mma.imagerecognition.executables;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import org.apache.commons.lang.math.NumberUtils;
import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.util.ModelSerializer;
import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dao.TrainingDbDao;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.tools.ImageTool;
import org.nd4j.linalg.api.ndarray.INDArray;
import org.nd4j.linalg.factory.Nd4j;

public class DeadNeuronDetector {
	public static void main(String[] args) throws IOException {
		FileSystemDAO.createFolders();
		if (args.length != 2) {
			System.err.println("Expected 2 arguments, the model number, and the sample size to evaluate on.");
			System.exit(1);
		}

		File networkFile = null;
		int sampleSize = Integer.parseInt(args[1]);

		if (NumberUtils.isDigits(args[0])) {
			int modelNumber = Integer.parseInt(args[0]);
			networkFile = FileSystemDAO.getContinuousFolder().resolve(String.format("model%d.bin",modelNumber)).toFile();
		} else {
			String path = args[0];
			networkFile = new File(path);
		}

		getDeadNeurons(networkFile, sampleSize);
	}
	
	public static void getDeadNeurons(MultiLayerNetwork network, int sampleSize) {
		Map<Integer, DeadMap> deadMaps = new HashMap<Integer, DeadMap>();
		List<TrainingData> sample = TrainingDbDao.getRandomImages(sampleSize);

		for (TrainingData traningData : sample) {
			network.output(Nd4j.create(ImageTool.toScaledDoubles(traningData.getPixelData()), new int[] { 1, 3, traningData.getWidth(), traningData.getHeight() }), false);

			for (int currentLayer = 0; currentLayer < network.getnLayers(); currentLayer++) {
				INDArray activations = network.getLayer(currentLayer).activate();
				if(activations.shape().length == 4) {
					//Conv output
					activations = activations.slice(0);
				}
				if (!deadMaps.containsKey(currentLayer)) {
					deadMaps.put(currentLayer, new DeadMap(activations, currentLayer));
				}
				deadMaps.get(currentLayer).setAll(activations);
			}
		}
		
		for (Entry<Integer, DeadMap> deadMap : deadMaps.entrySet()) {
			System.out.println(deadMap.getValue().toString());
		}
	}

	public static void getDeadNeurons(File networkFile, int sampleSize) throws FileNotFoundException, IOException {
		MultiLayerNetwork network = ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(networkFile));
		getDeadNeurons(network, sampleSize);
	}

	private static class DeadMap {
		int layer;
		int[][][] deadMap;
		final static double zeroThreshold = 0.0;
		boolean flat;

		public DeadMap(INDArray map, int layer) {
			if (map.shape().length != 2 && map.shape().length != 3) {
				throw new IllegalStateException();
			}

			this.layer = layer;

			if (map.shape().length == 2) {
				flat = true;
				deadMap = new int[map.shape()[1]][1][1];
			}

			if (map.shape().length == 3) {
				deadMap = new int[map.shape()[0]][map.shape()[1]][map.shape()[2]];
			}
		}

		public void setAll(INDArray map) {
			int height, width, depth;
			if (flat) {
				height = map.shape()[1];
				width = 1;
				depth = 1;
			} else {
				height = map.shape()[0];
				width = map.shape()[1];
				depth = map.shape()[2];
			}

			for (int i = 0; i < height; i++) {
				for (int j = 0; j < width; j++) {
					for (int k = 0; k < depth; k++) {
						if (flat) {
							set(map.getDouble(0, i), i, j, k);
						} else {
							set(map.getDouble(i, j, k), i, j, k);
						}
					}
				}
			}
		}

		private void set(double activation, int height, int width, int depth) {
			if (activation > zeroThreshold) {
				deadMap[height][width][depth] = 1;
			}
		}

		private double percentageDeadNeurons() {
			double totalNeurons = (double) deadMap.length * deadMap[0].length
					* deadMap[0][0].length;
			double deadNeurons = 0;
			for (int height = 0; height < deadMap.length; height++) {
				for (int width = 0; width < deadMap[0].length; width++) {
					for (int depth = 0; depth < deadMap[0][0].length; depth++) {
						if (deadMap[height][width][depth] == 0) {
							deadNeurons += 1;
						}
					}
				}
			}
			return deadNeurons / totalNeurons;
		}

		@Override
		public String toString() {
			return (flat ? "Flat l" : "L") + "ayer " + layer + " has "
					+ percentageDeadNeurons() * 100 + "% dead neurons.";
		}
	}
}
