using UnityEngine;
using System.Collections;

public class Screenshot {

	private readonly Color32[] pixels;
	private readonly int width, height;

	public Screenshot(Color32[] pixels, int width, int height) {
		this.pixels = pixels;
		this.width = width;
		this.height = height;
	}

	public byte[] GetRGB() {
		return ArrayHelper.ToRGB (pixels, width);
	}

	public Color32[] GetPixels() {
		return pixels;
	}

	public int GetWidth() {
		return width;
	}

	public int GetHeight() {
		return height;
	}
}