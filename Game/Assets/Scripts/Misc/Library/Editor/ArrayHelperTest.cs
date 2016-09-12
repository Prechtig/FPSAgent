using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System;
using System.Linq;

public class ArrayHelperTest {

	private readonly double DELTA = 1E-6;

	[Test]
	public void TestSingleColorConversion() {
		byte r = 10;
		byte g = 20;
		byte b = 30;
		byte a = 40;
		Color32 color = new Color32 (r, g, b, a);

		byte[] rgba = ArrayHelper.ColorToRGB (color);

		Assert.AreEqual (3, rgba.Length);
		Assert.AreEqual (r, rgba [0]);
		Assert.AreEqual (g, rgba [1]);
		Assert.AreEqual (b, rgba [2]);
	}

	[Test]
	public void TestImageConversion() {
		Color32[] flattened = new Color32[4];

		byte alpha = 11;

		byte colorCounter = 17;

		flattened [0] = new Color32 (colorCounter++, colorCounter++, colorCounter++, alpha);
		flattened [1] = new Color32 (colorCounter++, colorCounter++, colorCounter++, alpha);
		flattened [2] = new Color32 (colorCounter++, colorCounter++, colorCounter++, alpha);
		flattened [3] = new Color32 (colorCounter++, colorCounter++, colorCounter++, alpha);

		byte[] colors = ArrayHelper.ToRGB (flattened);

		Assert.AreEqual (12, colors.Length);

		byte newColorCounter = 17;
		for (byte i = 0; i < colors.Length; i++) {
			// Skip the alpha channel
			if (colors [i] != 11) {
				Assert.AreEqual (newColorCounter++, colors [i]);
			}
		}
	}

	[Test]
	public void TestScaling() {
		for (int i = 0; i < 256; i++) {
			Assert.AreEqual ((double)i / 255d, ArrayHelper.Scale ((byte)i), 0);
		}
	}

	[Test]
	public void TestByteDoubleConversion() {
		Assert.AreEqual(123.456d, ArrayHelper.ToDouble(ArrayHelper.ToByteArray(123.456d), 0), 0);
	}

	[Test]
	public void TestScalingColorToByteArray() {
		Color32 col = new Color32 (50, 100, 150, 200);

		byte[] bytes = ArrayHelper.ColorToScaledByteArray (col);

		double[] doubles = ArrayHelper.ColorToScaledDoubles (col);

		Assert.AreEqual(doubles[0], ArrayHelper.ToDouble(bytes, 0));
		Assert.AreEqual(doubles[1], ArrayHelper.ToDouble(bytes, 8));
		Assert.AreEqual(doubles[2], ArrayHelper.ToDouble(bytes, 16));
	}

	[Test]
	public void TestImageScalingToByteArray() {
		Color32 col1 = new Color32 (0, 25, 50, 75);
		Color32 col2 = new Color32 (100, 125, 150, 175);
		Color32[] flattened = new Color32[] { col1, col2 };

		byte[] bytes = ArrayHelper.ToScaledDoublesAsByteArray (flattened);

		double[] doubles1 = ArrayHelper.ColorToScaledDoubles (col1);

		Assert.AreEqual(doubles1[0], ArrayHelper.ToDouble(bytes, 0));
		Assert.AreEqual(doubles1[1], ArrayHelper.ToDouble(bytes, 8));
		Assert.AreEqual(doubles1[2], ArrayHelper.ToDouble(bytes, 16));

		double[] doubles2 = ArrayHelper.ColorToScaledDoubles (col2);

		Assert.AreEqual(doubles2[0], ArrayHelper.ToDouble(bytes, 24));
		Assert.AreEqual(doubles2[1], ArrayHelper.ToDouble(bytes, 32));
		Assert.AreEqual(doubles2[2], ArrayHelper.ToDouble(bytes, 40));

	}
}
