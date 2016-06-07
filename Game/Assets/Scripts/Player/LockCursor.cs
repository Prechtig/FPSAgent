using UnityEngine;
using System.Collections;

public class LockCursor : MonoBehaviour
{
    void Start()
    {
        if (!networkView.isMine)
        {
            this.enabled = false;
        }
    }

    void Update()
    {
        if (networkView.isMine)
        {
            if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Escape))
            {
                Screen.lockCursor = !Screen.lockCursor;
            }
        }
        else
        {
            this.enabled = false;
        }
    }
}
