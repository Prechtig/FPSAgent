using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class MinimapAdjustSize : MonoBehaviour
{
    public NetworkView nv;
    public float minimapSize = 2.0f;
    public float offsetX = 10.0f;
    public float offsetY = 10.0f;
    private float adjustSize = 0.0f;
    public Texture borderTexture;
    public Texture effectTexture;
    public Camera minimapCamera;

    void Update()
    {
        if (nv.isMine)
        {
            if (minimapCamera == null) return;

            adjustSize = Mathf.RoundToInt(Screen.width / 10);
            minimapCamera.pixelRect = new Rect(offsetX, (Screen.height - (minimapSize * adjustSize)) - offsetY, minimapSize * adjustSize, minimapSize * adjustSize);
        }
        else
        {
            minimapCamera.enabled = false;
            this.enabled = false;
        }

    }
    void OnGUI()
    {
        if (nv.isMine)
        {
            if (borderTexture != null)
            {
                minimapCamera.Render();
                GUI.DrawTexture(new Rect(offsetX, offsetY, minimapSize * adjustSize, minimapSize * adjustSize), effectTexture);
                GUI.DrawTexture(new Rect(offsetX, offsetY, minimapSize * adjustSize, minimapSize * adjustSize), borderTexture);

            }
        }
        else
        {
            minimapCamera.enabled = false;
            this.enabled = false;
        }
    }


}