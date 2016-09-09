package org.mma.imagerecognition.dataobjects;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.HashMap;
import java.util.Map;

public class TrainingData {
	private int height, width;
	private byte[] pixelData;
	private Map<String, Double> features = new HashMap<String, Double>();
	
	public static String horizontalAngleKey = "horizontalAngle";
	public static String verticalAngleKey = "verticalAngle";
	public static String distanceKey = "distance";
	public static String withinSightKey = "withinsight";
	
	public static int numberOfBots = 5;
	
	
	public TrainingData(ResultSet rs) throws SQLException {
		pixelData = rs.getBytes("pixelData");
		height = rs.getInt("height");
		width = rs.getInt("width");
		
		for(int i = 1; i <= numberOfBots; i++) {
			addFeature(rs, horizontalAngleKey, i);
			addFeature(rs, verticalAngleKey, i);
			addFeature(rs, distanceKey, i);
			addFeature(rs, withinSightKey, i);
		}
	}
	
	private void addFeature(ResultSet rs, String key, int suffix) throws SQLException {
		String keyNumerated = key + suffix;
		features.put(keyNumerated, rs.getDouble(keyNumerated));
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
}
