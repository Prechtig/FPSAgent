package org.mma.imagerecognition.tools;

import java.awt.Color;
import java.awt.image.BufferedImage;
import java.io.IOException;
import java.net.URL;

import javax.imageio.ImageIO;

public class ImageTool {
	public static int[][][] toInputVolume(URL url) {
		BufferedImage bufferedImage = getBufferedImage(url);
		int width = bufferedImage.getWidth();
		int height = bufferedImage.getHeight();
		int[][][] colors = new int[width][height][3];
		for(int i = 0; i < width; i++) {
			for(int j = 0; j < height; j++) {
				Color c = new Color(bufferedImage.getRGB(i, j));
				colors[i][j][0] = c.getRed();
				colors[i][j][1] = c.getGreen();
				colors[i][j][2] = c.getBlue();
			}
		}
		return colors;
	}
	
	public static int getImageType(URL url) {
		return getBufferedImage(url).getType();
	}
	
	private static BufferedImage getBufferedImage(URL url) {
		BufferedImage bi = null;
		try {
			bi = ImageIO.read(url);
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return bi;
	}
	
	/**
	 * Converts a flattened array of pixel data into a 3D array of pixel data.
	 * 
	 * <pre>
	 * For instance given three color channels (R, G, B) and a 2x2 image
	 * the flattened array must be in the form of:
	 * 	[R1, B1, G1, R2, B2, G2, R3, B3, G3, R4, B4, G4]
	 * where the original image is
	 * 	-----------
	 * 	| P3 | P4 |
	 * 	-----------
	 * 	| P1 | P2 |
	 * 	-----------
	 * 
	 * The resulting 3D array are of the form:
	 * [] is a row of pixels
	 * [][] is a specific pixel
	 * [][][] is a specific color value
	 * </pre>
	 * @param flattened The flattened array of pixel data
	 * @param width The width of the original image
	 * @param colorChannels The number of color channels used per pixel
	 * @return A 3D representation of the flattened pixel data
	 */
	public static float[][][] ConvertFlattenedTo3D(byte[] flattened, int width, int colorChannels) {
		int height = flattened.length / width / colorChannels;
		
		float[][][] result = new float[height][width][colorChannels];
		
		for(int row = 0; row < height; row++) {
			for(int pixel = 0; pixel < width; pixel++) {
				for(int colorChannel = 0; colorChannel < colorChannels; colorChannel++) {
					int index = (row * width * colorChannels) + (pixel * colorChannels) + colorChannel; 
					result[row][pixel][colorChannel] = flattened[index];
				}
			}
		}
		return result;
	}
}
