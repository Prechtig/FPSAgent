package org.mma.imagerecognition.dataobjects;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.HashMap;
import java.util.Map;

import org.mma.imagerecognition.tools.PropertiesReader;
import org.mma.imagerecognition.tools.ScaleTool;

public class TrainingData {
	private int height, width, id;
	private byte[] pixelData;
	private Map<String, Double> features = new HashMap<String, Double>();
	private double[] featureDoubles;
	
	public static final String horizontalAngleKey = "horizontalAngle";
	public static final String verticalAngleKey = "verticalAngle";
	public static final String distanceKey = "distance";
	public static final String withinSightKey = "withinsight";
	
	public static final int numberOfBots = Integer.parseInt(PropertiesReader.getProjectProperties().getProperty("game.bots"));
	
	public static int getFeatureCount() {
		int featuresPerBot = Integer.parseInt(PropertiesReader.getProjectProperties().getProperty("training.featuresPerBot"));
		return numberOfBots * featuresPerBot;
	}
	
	public TrainingData(ResultSet rs) throws SQLException {
		id = rs.getInt("id");
		pixelData = rs.getBytes("pixelData");
		height = rs.getInt("height");
		width = rs.getInt("width");
		
		featureDoubles = new double[numberOfBots * 4];
		int featureCounter = 0;
		for(int i = 1; i <= numberOfBots; i++) {
			featureDoubles[featureCounter++] = ScaleTool.scaleAngle(rs.getDouble(getNumeratedKey(horizontalAngleKey, i)));
			featureDoubles[featureCounter++] = ScaleTool.scaleAngle(rs.getDouble(getNumeratedKey(verticalAngleKey, i)));
			featureDoubles[featureCounter++] = ScaleTool.scaleDistance(rs.getDouble(getNumeratedKey(distanceKey, i)));
			featureDoubles[featureCounter++] = rs.getDouble(getNumeratedKey(withinSightKey, i));
			
			addFeature(rs, horizontalAngleKey, i);
			addFeature(rs, verticalAngleKey, i);
			addFeature(rs, distanceKey, i);
			addFeature(rs, withinSightKey, i);
		}
	}
	
	public TrainingData(int id, int width, int height, byte[] pixelData, double[] features) {
		this.id = id;
		this.width = width;
		this.height = height;
		this.pixelData = pixelData;
		this.featureDoubles = features;
	}
	
	public double[] getFeatureDoubles() {
		return featureDoubles;
	}
	
	public double[] getFeatureForAllBots(String key) {
		double[] values = new double[numberOfBots];
		for(int i = 1; i <= numberOfBots; i++) {
			values[i] = features.get(key + i);
		}
		return values;
	}
	
	private void addFeature(ResultSet rs, String key, int suffix) throws SQLException {
		String keyNumerated = getNumeratedKey(key, suffix);
		features.put(keyNumerated, rs.getDouble(keyNumerated));
	}
	
	private String getNumeratedKey(String key, int suffix) {
		return key + suffix;
	}
	
	public TrainingData() {
		super();
	}

	public int getHeight() {
		return height;
	}

	public void setHeight(int height) {
		this.height = height;
	}

	public int getWidth() {
		return width;
	}

	public void setWidth(int width) {
		this.width = width;
	}

	public byte[] getPixelData() {
		return pixelData;
	}

	public void setPixelData(byte[] pixelData) {
		this.pixelData = pixelData;
	}

	public Map<String, Double> getFeatures() {
		return features;
	}

	public void setFeatures(Map<String, Double> features) {
		this.features = features;
	}
	
	public void putFeature(String name, Double value) {
		features.put(name, value);
	}
	
	public int getId() {
		return id;
	}
}
