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
}
