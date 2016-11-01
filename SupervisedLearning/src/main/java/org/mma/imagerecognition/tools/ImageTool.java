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
import java.util.stream.DoubleStream;

import javax.imageio.ImageIO;

import org.apache.commons.lang3.ArrayUtils;

public class ImageTool {
	/**
	 * Prints an image as a .png image. Assumes three color channels
	 * @param flattened The pixel data
	 * @param width The width of the image
	 * @param file The file to save the image to
	 * @throws IOException @see {@link javax.imageio.ImageIO#write(java.awt.image.RenderedImage, String, File)}
	 */
	public static void printColoredPngImage(byte[] flattened, int width, File file) throws IOException {
		int height = flattened.length / width / 3;
		
		byte[] flipped = flipImageBytes(flattened, width, 3);
		
		DataBuffer buffer = new DataBufferByte(flipped, flattened.length);
		
		//3 bytes per pixel: red, green, blue
		WritableRaster raster = Raster.createInterleavedRaster(buffer, width, height, 3 * width, 3, new int[] {0, 1, 2}, (Point)null);
		ColorModel cm = new ComponentColorModel(ColorModel.getRGBdefault().getColorSpace(), false, true, Transparency.OPAQUE, DataBuffer.TYPE_BYTE);
		BufferedImage image = new BufferedImage(cm, raster, true, null);

		ImageIO.write(image, "png", file);
	}
	
	public static void printGreyScalePngImage(double[] flattened, int width, File file) throws IOException {
		byte[] greyScale = DoubleStream.of(flattened)
			.boxed()
			.map(d -> toGreyScale(d))
			.reduce(new byte[0], (cumArr, curArr) -> ArrayUtils.addAll(cumArr, curArr));
		printColoredPngImage(greyScale, width, file);
	}
	
	private static byte toByte(double d) {
		double cutOff = 5.0;
		if(d < 0.0) {
			d = 0.0;
		}
		if(d > cutOff) {
			d = cutOff;
		}
		int colorValue = (int) Math.round((255.0/cutOff) * d);
		if(colorValue > 255 || colorValue < 0) {
			throw new IllegalStateException();
		}
		return (byte) colorValue;
	}
	
	private static byte[] toGreyScale(double d) {
		return new byte[] { toByte(d), toByte(d), toByte(d) };
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
	
	public static double[] toScaledDoubles(byte[] bs) {
		final int channelLength = bs.length / 3;
		
		double[] r = new double[channelLength];
		double[] g = new double[channelLength];
		double[] b = new double[channelLength];
		
		for(int i = 0; i < channelLength; i++) {
			r[i] = scale(bs[i * 3 + 0]);
			g[i] = scale(bs[i * 3 + 1]);
			b[i] = scale(bs[i * 3 + 2]);
		}
		
		double[] result = new double[bs.length];
		System.arraycopy(r, 0, result, channelLength * 0, channelLength);
		System.arraycopy(g, 0, result, channelLength * 1, channelLength);
		System.arraycopy(b, 0, result, channelLength * 2, channelLength);
		return result;
	}
	
	public static double scale(double d) {
		return d / (double) 0xFF;
	}
	
	public static double scale(byte b) {
		return scale(toDouble(b));
	}
	
	public static double toDouble(byte b) {
		return (b & 0xFF);
	}
}
