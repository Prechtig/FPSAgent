using UnityEngine;
using System;

public class ScreenSnapper {

	/// <summary>
	/// Snaps a screenshot from a given camera.
	/// Uses the cameras target texture as the screen.
	/// </summary>
	/// <returns>The screenshot as a flattened array</returns>
	/// <param name="camera">The camera that should be used when capturing the screenshot</param>
	public static Color32[] SnapScreenshot(Camera camera) {
		RenderTexture cameraRenderTexture = camera.targetTexture;
		if (cameraRenderTexture == null) {
			throw new InvalidOperationException ("The camera did not have any target texture. Please assign one to the camera");
		}

		RenderTexture currentRT = RenderTexture.active;
		RenderTexture.active = cameraRenderTexture;

		int width = cameraRenderTexture.width;
		int height = cameraRenderTexture.height;

		Texture2D tex = new Texture2D (width, height, TextureFormat.RGB24, false);

		// Read screen contents into the texture
		tex.ReadPixels (new Rect (0, 0, width, height), 0, 0);
		tex.Apply ();

		RenderTexture.active = currentRT;

		return tex.GetPixels32 ();
	}
}