using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class ArrayHelperTest {

	[Test]
	public void testSingleColorConversion() {
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
	public void testImageConversion() {
		Color32[] flattened = new Color32[4];

		byte alpha = 11;

		byte colorCounter = 17;

		flattened [0] = new Color32 (colorCounter++, colorCounter++, colorCounter++, alpha);
		flattened [1] = new Color32 (colorCounter++, colorCounter++, colorCounter++, alpha);
		flattened [2] = new Color32 (colorCounter++, colorCounter++, colorCounter++, alpha);
		flattened [3] = new Color32 (colorCounter++, colorCounter++, colorCounter++, alpha);

		byte[] colors = ArrayHelper.ToRGB (flattened, 2);

		Assert.AreEqual (12, colors.Length);

		byte newColorCounter = 17;
		for (byte i = 0; i < colors.Length; i++) {
			// Skip the alpha channel
			if (colors [i] != 11) {
				Assert.AreEqual (newColorCounter++, colors [i]);
			}
		}
	}
}
