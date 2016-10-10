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
import java.util.function.Function;
import java.util.stream.DoubleStream;
import java.util.stream.IntStream;

import javax.imageio.ImageIO;

import org.apache.commons.lang3.ArrayUtils;
import org.nd4j.linalg.api.ndarray.INDArray;
import org.nd4j.linalg.factory.Nd4j;

import com.google.common.primitives.Bytes;

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
		double cutOff = 30.0;
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
		int height = calculateHeight(flattened.length, width, colorChannels);
		
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
	
	private static int calculateHeight(int arrayLength, int width, int colorChannels) {
		return arrayLength / width / colorChannels;
	}
	
	private static int calculateHeight(int arrayLength, int width) {
		return calculateHeight(arrayLength, width, 3);
	}
	
	public static INDArray convertToINDArray(byte[] flattened, int width) {
		return Nd4j.create(toScaledDoubleStream(flattened).toArray());
	}
	
	public static INDArray convertToINDArray(DoubleStream ds, int width) {
		double[] array = ds.toArray();
		int height = calculateHeight(array.length, width);
		return Nd4j.create(array, new int[] {width, height, 3}, 'c');
	}
	
	public static INDArray convertToINDArray(double[][] values) {
		return Nd4j.create(values);
	}
	
	public static INDArray convertToINDArray(int width, int height, double[][] values) {
		return Nd4j.create(flatten(values), new int[] {width, height, 3, values.length}, 'c');
	}
	
	public static double[] flatten(double[][] values) {
		int arrLength = values[0].length;
		double[] result = new double[values.length * arrLength];
		for(int i = 0; i < values.length; i++) {
			System.arraycopy(values[i], 0, result, i * arrLength, arrLength);
		}
		return result;
	}

	public static Float[] mapToFloats(byte[] bs, Function<? super Byte, ? extends Float> mapper) {
		IntStream.range(0, bs.length).mapToDouble(i -> scale(bs[i])).toArray();
		return Bytes.asList(bs).parallelStream().map(mapper).toArray(Float[]::new);
	}
	
	public static DoubleStream toScaledDoubleStream(byte[] bs) {
		return toDoubleStream(bs).map(d -> scale(d));
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
	
	public static DoubleStream toDoubleStream(byte[] bs) {
		return DoubleStream.of(toDoubleArray(bs));
	}
	
	public static double[] toDoubleArray(byte[] bs) {
		double[] ds = new double[bs.length];
		for(int i = 0; i < ds.length; i++) {
			ds[i] = toDouble(bs[i]);
		}
		return ds;
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
