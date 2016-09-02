using UnityEngine;
using System.Collections;

public class LockCursor : MonoBehaviour
{
    void Start()
    {
		
    }

    void Update()
	{
		if (Input.GetKeyDown (KeyCode.B) || Input.GetKeyDown (KeyCode.Escape)) {
			Screen.lockCursor = !Screen.lockCursor;
		}
	}
}
