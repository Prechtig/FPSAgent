package org.mma.imagerecognition.tools;

public class ScaleTool {
	private static final double ANGLE_OFFSET = 90;
	private static final double ANGLE_SCALE_FACTOR = 180;
	private static final double DISTANCE_SCALE_FACTOR = 0.05;
	
	public static double scaleAngle(double angle) {
		return (angle + ANGLE_OFFSET) / ANGLE_SCALE_FACTOR;
	}
	
	public static double scaleDistance(double distance) {
		return distance*DISTANCE_SCALE_FACTOR;
	}
}
