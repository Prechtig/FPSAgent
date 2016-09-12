package org.mma.imagerecognition.tools;

import static org.junit.Assert.assertArrayEquals;
import static org.junit.Assert.assertEquals;
import static org.junit.Assert.fail;

import java.io.File;

import org.junit.Test;

public class ImageToolTest {
	
	private static final float DELTA = 1E-6f;

	/**
	 * Tests that a flattened image can be
	 * translated correctly into its 3D representation
	 */
	@Test
	public void testConvertFlattenedTo3D() {
		/*
		 * Flattened representation of the following image
		 * -----------
		 * | P3 | P4 |
		 * -----------
		 * | P1 | P2 |
		 * -----------
		 * Where P = R, G, B:
		 * P1 = 11, 12, 13
		 * P2 = 21, 22, 23
		 * P3 = 31, 32, 33
		 * P4 = 41, 42, 43
		 */
		byte[] flattened = new byte[] { 11, 12, 13,
										21, 22, 23,
										31, 32, 33,
										41, 42, 43 };
		
		float[][][] imageData3D = ImageTool.convertFlattenedTo3D(flattened, 2, 3);
		
		assertEquals(11f, imageData3D[0][0][0], DELTA);
		assertEquals(12f, imageData3D[0][0][1], DELTA);
		assertEquals(13f, imageData3D[0][0][2], DELTA);
		
		assertEquals(21f, imageData3D[0][1][0], DELTA);
		assertEquals(22f, imageData3D[0][1][1], DELTA);
		assertEquals(23f, imageData3D[0][1][2], DELTA);
		
		assertEquals(31f, imageData3D[1][0][0], DELTA);
		assertEquals(32f, imageData3D[1][0][1], DELTA);
		assertEquals(33f, imageData3D[1][0][2], DELTA);
		
		assertEquals(41f, imageData3D[1][1][0], DELTA);
		assertEquals(42f, imageData3D[1][1][1], DELTA);
		assertEquals(43f, imageData3D[1][1][2], DELTA);
	}
	
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
	public void testPrintPngImage() {
		byte[] flattened = new byte[] { 0, 0, 0,
										(byte)0xFF, 0, 0,
										0, (byte)0xFF, 0,
										0, 0, (byte)0xFF };
		try {
		ImageTool.printPngImage(flattened, 2, new File("test.png"));
		} catch (Exception e) {
			fail("Should not happen");
		}
	}
	
	@Test
	public void testByteToFloatConversion() {
		assertEquals(0f,	ImageTool.toFloat((byte) 0),	DELTA);
		assertEquals(0f,	ImageTool.toFloat((byte) 0x00),	DELTA);
		assertEquals(17f,	ImageTool.toFloat((byte) 17),	DELTA);
		assertEquals(255f,	ImageTool.toFloat((byte) 255),	DELTA);
		assertEquals(255f,	ImageTool.toFloat((byte) 0xFF),	DELTA);
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
//	public void testPrintImageFromDatabase() {
//		TrainingDbDao.initializeConnection("uid", "pwd");
//		TrainingData trainingData = TrainingDbDao.getImages(1).get(0);
//		ImageTool.printPngImage(trainingData.getPixelData(), trainingData.getWidth());
//	}

}
