package org.mma.imagerecognition.executables;

import java.io.IOException;
import java.util.Properties;

import org.deeplearning4j.nn.conf.MultiLayerConfiguration;
import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.util.ModelSerializer;
import org.mma.imagerecognition.configuration.BuilderFactory;
import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.tools.PropertiesReader;

public class UntrainedModelSaver {
	public static void main(String[] args) {
		Properties projectProperties = PropertiesReader.getProjectProperties();
		int featureCount = TrainingData.getFeatureCount();
		int width = Integer.parseInt(projectProperties.getProperty("training.image.width"));
		int height = Integer.parseInt(projectProperties.getProperty("training.image.height"));
		
		MultiLayerConfiguration configuration = BuilderFactory.getReducingConvNet(height, width, featureCount).build();
        MultiLayerNetwork model = new MultiLayerNetwork(configuration);
        model.init();
		
		try {
			ModelSerializer.writeModel(model, FileSystemDAO.getModelsFolder().resolve("untrainedModel.bin").toFile(), true);
		} catch (IOException e) {
			e.printStackTrace();
		}
		
		System.out.println("Saved untrained model");
	}
}
