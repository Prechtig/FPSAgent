package org.mma.imagerecognition.tools;

import static org.junit.Assert.assertArrayEquals;
import static org.junit.Assert.assertEquals;

import org.junit.Test;

public class ImageToolTest {
	
	private static final float DELTA = 1E-6f;

	/**
	 * Tests that a flattened image can be
	 * translated correctly into its 3D representation
	 */
//	@Test
//	public void testConvertFlattenedTo3D() {
//		/*
//		 * Flattened representation of the following image
//		 * -----------
//		 * | P3 | P4 |
//		 * -----------
//		 * | P1 | P2 |
//		 * -----------
//		 * Where P = R, G, B:
//		 * P1 = 11, 12, 13
//		 * P2 = 21, 22, 23
//		 * P3 = 31, 32, 33
//		 * P4 = 41, 42, 43
//		 */
//		byte[] flattened = new byte[] { 11, 12, 13,
//										21, 22, 23,
//										31, 32, 33,
//										41, 42, 43 };
//		
//		float[][][] imageData3D = ImageTool.convertFlattenedTo3D(flattened, 2, 3);
//		
//		assertEquals(11f, imageData3D[0][0][0], DELTA);
//		assertEquals(12f, imageData3D[0][0][1], DELTA);
//		assertEquals(13f, imageData3D[0][0][2], DELTA);
//		
//		assertEquals(21f, imageData3D[0][1][0], DELTA);
//		assertEquals(22f, imageData3D[0][1][1], DELTA);
//		assertEquals(23f, imageData3D[0][1][2], DELTA);
//		
//		assertEquals(31f, imageData3D[1][0][0], DELTA);
//		assertEquals(32f, imageData3D[1][0][1], DELTA);
//		assertEquals(33f, imageData3D[1][0][2], DELTA);
//		
//		assertEquals(41f, imageData3D[1][1][0], DELTA);
//		assertEquals(42f, imageData3D[1][1][1], DELTA);
//		assertEquals(43f, imageData3D[1][1][2], DELTA);
//	}
	
	@Test
	public void testFlipFlattenedImage() {
		byte[] flattened = new byte[] { 11, 12, 13,
										21, 22, 23,
										31, 32, 33,
										41, 42, 43 };
		
		byte[] expected = new byte[] { 	31, 32, 33,
										41, 42, 43,
										11, 12, 13,
										21, 22, 23 };
		
		byte[] flipped = ImageTool.flipImageBytes(flattened, 2, 3);
		assertArrayEquals(expected, flipped);
	}
	
	@Test
	public void testByteToDoubleConversion() {
		assertEquals(0d,	ImageTool.toDouble((byte) 0),		DELTA);
		assertEquals(0d,	ImageTool.toDouble((byte) 0x00),	DELTA);
		assertEquals(17d,	ImageTool.toDouble((byte) 17),		DELTA);
		assertEquals(255d,	ImageTool.toDouble((byte) 255),		DELTA);
		assertEquals(255d,	ImageTool.toDouble((byte) 0xFF),	DELTA);
	}
	
	@Test
	public void testByteScaling() {
		assertEquals(0f,			ImageTool.scale((byte) 0),		DELTA);
		assertEquals(0f,			ImageTool.scale((byte) 0x00),	DELTA);
		assertEquals((17f/255f),	ImageTool.scale((byte) 17),		DELTA);
		assertEquals((52f/255f),	ImageTool.scale((byte) 52),		DELTA);
		assertEquals((128/255f),	ImageTool.scale((byte) 128),	DELTA);
		assertEquals(1f,			ImageTool.scale((byte) 255),	DELTA);
		assertEquals(1f,			ImageTool.scale((byte) 0xFF),	DELTA);
	}
	
//	@Test
//	public void testByteArrayToScaledDoubleArray() {
//		byte[] bytes = new byte[] {(byte) 0, (byte) 17, (byte) 128, (byte) 255};
//		double[] expectedFloats = new double[] {0d, (17d/255d), (128d/255d), 1d};
//		
//		assertArrayEquals(expectedFloats, ImageTool.toScaledDoubleStream(bytes).toArray(), DELTA);
//	}
	
//	@Test
//	public void testToINDArryConversion() {
//		byte[] flattened = new byte[] { (byte) 000, (byte) 128, (byte) 255,
//										(byte) 010, (byte) 020, (byte) 030,
//										(byte) 110, (byte) 120, (byte) 130,
//										(byte) 210, (byte) 220, (byte) 230 };
//		INDArray arr = ImageTool.convertToINDArray(flattened, 2);
//		
//		assertEquals(scale((byte)000), arr.getDouble(0, 0, 0), DELTA);
//		assertEquals(scale((byte)128), arr.getDouble(0, 0, 1), DELTA);
//		assertEquals(scale((byte)255), arr.getDouble(0, 0, 2), DELTA);
//		
//		assertEquals(scale((byte)010), arr.getDouble(0, 1, 0), DELTA);
//		assertEquals(scale((byte)020), arr.getDouble(0, 1, 1), DELTA);
//		assertEquals(scale((byte)030), arr.getDouble(0, 1, 2), DELTA);
//		
//		assertEquals(scale((byte)110), arr.getDouble(1, 0, 0), DELTA);
//		assertEquals(scale((byte)120), arr.getDouble(1, 0, 1), DELTA);
//		assertEquals(scale((byte)130), arr.getDouble(1, 0, 2), DELTA);
//		
//		assertEquals(scale((byte)210), arr.getDouble(1, 1, 0), DELTA);
//		assertEquals(scale((byte)220), arr.getDouble(1, 1, 1), DELTA);
//		assertEquals(scale((byte)230), arr.getDouble(1, 1, 2), DELTA);
//	}
//	
//	@Test
//	public void asdasd() {
//		TrainingDbDao.getRandomImages(1).get(0);
//	}
	
//	@Test
//	public void testPrintPngImage() {
//		byte[] flattened = new byte[] { 0, 0, 0,
//										(byte)0xFF, 0, 0,
//										0, (byte)0xFF, 0,
//										0, 0, (byte)0xFF };
//		try {
//		ImageTool.printPngImage(flattened, 2, new File("test.png"));
//		} catch (Exception e) {
//			fail("Should not happen");
//		}
//	}
}
