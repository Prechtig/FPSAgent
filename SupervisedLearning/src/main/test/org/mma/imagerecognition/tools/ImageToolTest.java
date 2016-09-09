package org.mma.imagerecognition.tools;


import static org.junit.Assert.*;

import org.junit.Test;

import org.mma.imagerecognition.tools.ImageTool;

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
		
		float[][][] imageData3D = ImageTool.ConvertFlattenedTo3D(flattened, 2, 3);
		
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

}
