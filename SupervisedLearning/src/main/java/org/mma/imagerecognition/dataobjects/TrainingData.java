package org.mma.imagerecognition.dataobjects;

import java.sql.ResultSet;
import java.sql.SQLException;

import org.mma.imagerecognition.partitioner.PartitionId;
import org.mma.imagerecognition.partitioner.VisualPartitionClassifier;

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
		return VisualPartitionClassifier.GetInstance().getNumberOfPartitions() + 1;
	}
	
	public TrainingData(ResultSet rs) throws SQLException {
		id = rs.getInt("id");
		pixelData = rs.getBytes("pixelData");
		height = rs.getInt("height");
		width = rs.getInt("width");
		
		double withinSightValue = rs.getDouble(withinSightKey);
		withinSight = withinSightValue == 1d;
		horizontalAngle = withinSight ? rs.getDouble(horizontalAngleKey) : 0d;
		verticalAngle = withinSight ? rs.getDouble(verticalAngleKey) : 0d;
		distance = rs.getDouble(distanceKey);
		
		if(withinSight) {
			partitionId = VisualPartitionClassifier.GetInstance().GetVisualPartition(horizontalAngle, verticalAngle);
		} else {
			partitionId = null;
		}
		
		features = calculateFeatures();
	}
	
	public TrainingData(int id, int width, int height, byte[] pixelData, double horizontalAngle, double verticalAngle, double distance, boolean withinSight) {
		this.id = id;
		this.width = width;
		this.height = height;
		this.pixelData = pixelData;
		this.horizontalAngle = horizontalAngle;
		this.verticalAngle = verticalAngle;
		this.distance = distance;
		this.withinSight = withinSight;
		
		partitionId = withinSight ? VisualPartitionClassifier.GetInstance().GetVisualPartition(horizontalAngle, verticalAngle) : null;
		this.features = calculateFeatures();
	}
	
//	public TrainingData(int id, int width, int height, byte[] pixelData, double[] features) {
//		this.id = id;
//		this.width = width;
//		this.height = height;
//		this.pixelData = pixelData;
//		horizontalAngle	= features[horizontalAngleIndex];
//		verticalAngle	= features[verticalAngleIndex];
//		distance		= features[distanceIndex];
//		withinSight		= features[withinSightIndex] == 1d;
//		
//		partitionId = VisualPartitionClassifier.GetInstance().GetVisualPartition(horizontalAngle, verticalAngle);
//		this.features = calculateFeatures();
//	}
	
	private double[] calculateFeatures() {
		int numberOfPartitions = VisualPartitionClassifier.GetInstance().getNumberOfPartitions();
		
		double[] result = new double[numberOfPartitions + 1];
		// Default to last index
		int index = numberOfPartitions;
		if(withinSight) {
			index = VisualPartitionClassifier.GetInstance().calculateFeatureIndexFromPartitionId(partitionId);
		}
		result[index] = 1d;
		return result;
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
