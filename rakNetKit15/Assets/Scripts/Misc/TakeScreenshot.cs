using UnityEngine;
using System.Collections;

public class TakeScreenshot : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        Application.CaptureScreenshot("Minimap.png", 10);
    }


}
