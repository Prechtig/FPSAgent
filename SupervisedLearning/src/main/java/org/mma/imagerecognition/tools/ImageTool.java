package org.mma.imagerecognition.tools;

import java.awt.Point;
import java.awt.Transparency;
import java.awt.image.BufferedImage;
import java.awt.image.ColorModel;
import java.awt.image.ComponentColorModel;
import java.awt.image.DataBuffer;
import java.awt.image.DataBufferByte;
import java.awt.image.Raster;
import java.awt.image.WritableRaster;
import java.io.File;
import java.io.IOException;

import javax.imageio.ImageIO;

public class ImageTool {
	/**
	 * Prints an image as a .png image. Assumes three color channels
	 * @param flattened The pixel data
	 * @param width The width of the image
	 * @param file The file to save the image to
	 * @throws IOException @see {@link javax.imageio.ImageIO#write(java.awt.image.RenderedImage, String, File)}
	 */
	public static void printPngImage(byte[] flattened, int width, File file) throws IOException {
		int height = flattened.length / width / 3;
		
		byte[] flipped = flipImageBytes(flattened, width, 3);
		
		DataBuffer buffer = new DataBufferByte(flipped, flattened.length);
		
		//3 bytes per pixel: red, green, blue
		WritableRaster raster = Raster.createInterleavedRaster(buffer, width, height, 3 * width, 3, new int[] {0, 1, 2}, (Point)null);
		ColorModel cm = new ComponentColorModel(ColorModel.getRGBdefault().getColorSpace(), false, true, Transparency.OPAQUE, DataBuffer.TYPE_BYTE);
		BufferedImage image = new BufferedImage(cm, raster, true, null);

		ImageIO.write(image, "png", file);
	}
	
	/**
	 * Flips a flattened image vertically
	 * @param flattened The pixel data
	 * @param width The width of the image
	 * @param colorChannels The amount of color channels used
	 * @return The image flipped vertically
	 */
	public static byte[] flipImageBytes(byte[] flattened, int width, int colorChannels) {
		byte[] result = new byte[flattened.length];
		
		int height = flattened.length / width / colorChannels;
		int rowWidth = width * colorChannels;
		int offset;
		
		for(int row = 0; row < height; row++) {
			offset = row * rowWidth;
			System.arraycopy(flattened, offset, result, flattened.length - (rowWidth * (row+1)), rowWidth);
		}
		return result;
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
	public static float[][][] convertFlattenedTo3D(byte[] flattened, int width, int colorChannels) {
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
