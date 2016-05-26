using UnityEngine;
using System.Collections;

public class LockCursor : MonoBehaviour
{
    void Start()
    {
        if (!GetComponent<NetworkView>().isMine)
        {
            this.enabled = false;
        }
    }

    void Update()
    {
        if (GetComponent<NetworkView>().isMine)
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
