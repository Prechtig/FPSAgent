using UnityEngine;
using System;
using System.Linq;

public class ArrayHelper {

	public static byte[] ToRGB(Color32[] flattened, int width) {
		return flattened.Select (color => ColorToRGB (color)).SelectMany(color => color).ToArray();

	}

	public static byte[] ColorToRGB(Color32 color) {
		byte[] rgba = new byte[3];
		rgba [0] = color.r;
		rgba [1] = color.g;
		rgba [2] = color.b;
		return rgba;
	}

}

