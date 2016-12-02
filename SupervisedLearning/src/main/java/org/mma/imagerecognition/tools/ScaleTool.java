package org.mma.imagerecognition.tools;

public class ScaleTool {
	private static final double ANGLE_OFFSET = 90;
	private static final double ANGLE_SCALE_FACTOR = 180;
	private static final double DISTANCE_SCALE_FACTOR = 0.05;
	private static double FOV = 0.0;
	
	static {
		FOV = Double.parseDouble(PropertiesReader.getProjectProperties().getProperty("topology.fov"));
	}
	
	public static double scaleAngle(double angle) {
		return angle / (FOV/2);
//		return (angle + ANGLE_OFFSET) / ANGLE_SCALE_FACTOR;
	}
	
	public static double scaleDistance(double distance) {
		return distance*DISTANCE_SCALE_FACTOR;
	}
}
