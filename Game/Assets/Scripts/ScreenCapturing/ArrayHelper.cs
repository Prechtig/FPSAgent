using UnityEngine;
using System;
using System.Linq;

public class ArrayHelper {

	public static byte[] ToRGB(Color32[] flattened) {
		return flattened.Select (color => ColorToRGB (color)).SelectMany(color => color).ToArray();
	}

	public static byte[] ToScaledDoublesAsByteArray(Color32[] flattened) {
		return flattened.Select (color => ColorToScaledByteArray(color)).SelectMany(color => color).ToArray();
	}

	/// <summary>
	/// Converts a double to byte array.
	/// Uses Big Endian.
	/// </summary>
	/// <returns>The doubled represented as a byte array</returns>
	/// <param name="d">The double to convert</param>
	public static byte[] ToByteArray(double d) {
		byte[] arr = BitConverter.GetBytes (d);
		if (BitConverter.IsLittleEndian) {
			Array.Reverse (arr);
		}
		return arr;
	}

	/// <summary>
	/// Converts a double to byte array.
	/// Expects the byte array to be in Big Endian form
	/// </summary>
	/// <returns>The resulting double</returns>
	/// <param name="bs">The doubled represented as a byte array</param>
	/// <param name="startIndex">The index to start reading at</param>
	public static double ToDouble(byte[] bs, int startIndex) {
		byte[] arr = new byte[8];
		Array.Copy (bs, startIndex, arr, 0, 8);
		if (BitConverter.IsLittleEndian) {
			Array.Reverse (arr);
		}
		return BitConverter.ToDouble (arr, 0);
	}

	public static double Scale(byte b) {
		return Convert.ToDouble (b) / Byte.MaxValue;
	}

	public static double[] ColorToScaledDoubles(Color32 col) {
		double[] result = new double[3];
		result [0] = Scale (col.r);
		result [1] = Scale (col.g);
		result [2] = Scale (col.b);
		return result;
	}

	public static byte[] ColorToScaledByteArray(Color32 col) {
		byte[] result = new byte[24]; // 3*8
		Array.Copy(ToByteArray(Scale(col.r)), 0, result, 0, 8);
		Array.Copy(ToByteArray(Scale(col.g)), 0, result, 8, 8);
		Array.Copy(ToByteArray(Scale(col.b)), 0, result, 16, 8);
		return result;

	}

	public static byte[] ColorToRGB(Color32 color) {
		byte[] rgba = new byte[3];
		rgba [0] = color.r;
		rgba [1] = color.g;
		rgba [2] = color.b;
		return rgba;
	}
}
