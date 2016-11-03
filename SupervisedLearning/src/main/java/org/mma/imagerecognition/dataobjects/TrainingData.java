package org.mma.imagerecognition.dataobjects;

import java.sql.ResultSet;
import java.sql.SQLException;

import org.mma.imagerecognition.partitioner.PartitionId;
import org.mma.imagerecognition.partitioner.VisualPartitionClassifier;
import org.mma.imagerecognition.tools.PropertiesReader;
import org.mma.imagerecognition.tools.ScaleTool;

public class TrainingData {
	private final int height, width, id;
	private final byte[] pixelData;
	private final double[] features;
	private final double horizontalAngle, verticalAngle, distance;
	private final boolean withinSight;
	private final PartitionId partitionId;
	
	private static final int horizontalAngleIndex = 0, verticalAngleIndex = 1, distanceIndex = 2, withinSightIndex = 3;
	
	public static final String horizontalAngleKey = "horizontalAngle";
	public static final String verticalAngleKey = "verticalAngle";
	public static final String distanceKey = "distance";
	public static final String withinSightKey = "withinsight";
	
	public static int getFeatureCount() {
		return Integer.parseInt(PropertiesReader.getProjectProperties().getProperty("training.featuresPerBot"));
	}
	
	public TrainingData(ResultSet rs) throws SQLException {
		id = rs.getInt("id");
		pixelData = rs.getBytes("pixelData");
		height = rs.getInt("height");
		width = rs.getInt("width");
		
		double withinSightValue = rs.getDouble(withinSightKey);
		withinSight = withinSightValue == 1d;
		horizontalAngle = withinSight ? ScaleTool.scaleAngle(rs.getDouble(horizontalAngleKey)) : 2d;
		verticalAngle = withinSight ? ScaleTool.scaleAngle(rs.getDouble(verticalAngleKey)) : 2d;
		distance = ScaleTool.scaleDistance(rs.getDouble(distanceKey));
		
		features = new double[4];
		features[horizontalAngleIndex] = horizontalAngle;
		features[verticalAngleIndex] = verticalAngle;
		features[distanceIndex] = distance;
		features[withinSightIndex] = withinSightValue;
		
		partitionId = VisualPartitionClassifier.GetInstance().GetVisualPartition(horizontalAngle, verticalAngle);
	}
	
	public TrainingData(int id, int width, int height, byte[] pixelData, double[] features) {
		this.id = id;
		this.width = width;
		this.height = height;
		this.pixelData = pixelData;
		this.features = features;
		horizontalAngle	= features[horizontalAngleIndex];
		verticalAngle	= features[verticalAngleIndex];
		distance		= features[distanceIndex];
		withinSight		= features[withinSightIndex] == 1d;
		
		partitionId = VisualPartitionClassifier.GetInstance().GetVisualPartition(horizontalAngle, verticalAngle);
	}
	
	public double[] getFeatures() {
		return features;
	}
	
	public int getHeight() {
		return height;
	}

	public int getWidth() {
		return width;
	}

	public byte[] getPixelData() {
		return pixelData;
	}

	public int getId() {
		return id;
	}

	public double getHorizontalAngle() {
		return horizontalAngle;
	}

	public double getVerticalAngle() {
		return verticalAngle;
	}

	public double getDistance() {
		return distance;
	}

	public boolean isWithinSight() {
		return withinSight;
	}

	public PartitionId getPartitionId() {
		return partitionId;
	}
}
