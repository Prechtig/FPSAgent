using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour
{
    public GameObject player;
    public Transform[] spawnPoints;
    public Camera spawnCam;
    public AudioListener spawnListener;
    public bool spawned = false;

    private int curMenu = 0;

    private string[] settings;
    private int curIndex;

    void Start()
    {
        settings = QualitySettings.names;
    }


    void Update()
    {
        if (spawned) if (Input.GetKeyDown(KeyCode.Escape)) Screen.lockCursor = !Screen.lockCursor;
    }

    void OnGUI()
    {
        if (Network.isServer || Network.isClient)
        {
            if (curMenu == 0)
            {
                if (!spawned)
                {
                    if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 80, 200, 50), "Spawn"))
                    {
                        SpawnPlayer();
                    }
                    if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), "Options"))
                    {
                        curMenu = 1;
                    }
                    if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 30, 200, 50), "Quit"))
                    {
                        Application.Quit();
                    }
                }
                else
                {
                    if (!Screen.lockCursor)
                    {
                        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 80, 200, 50), "Resume"))
                        {
                            Screen.lockCursor = true;
                        }
                        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), "Options"))
                        {
                            curMenu = 1;
                        }
                        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 30, 200, 50), "Quit"))
                        {
                            Application.Quit();
                        }
                    }
                }
            }
            else if (curMenu == 1)
            {
                if (!spawned || !Screen.lockCursor)
                {
                    if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 80, 200, 50), "Audio"))
                    {
                        curMenu = 2;
                    }
                    if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), "Graphics"))
                    {
                        curMenu = 3;
                    }
                    if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 30, 200, 50), "Back"))
                    {
                        curMenu = 0;
                    }
                }
            }
            else if (curMenu == 2)
            {
                AudioListener.volume = GUI.HorizontalSlider(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), AudioListener.volume, 0f, 1f);
                GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 45, 200, 50), "Volume: " + (AudioListener.volume * 100).ToString("F0") + " %");
                if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 30, 200, 50), "Back"))
                {
                    curMenu = 0;
                }
            }
            else if (curMenu == 3)
            {
                if (GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height / 2 - 25, 150, 50), settings[curIndex]))
                {
                    QualitySettings.SetQualityLevel(curIndex);
                }
                if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 20, 50), "←"))
                {
                    if (curIndex != 0) curIndex--;
                    else curIndex = settings.Length - 1;
                }

                if (GUI.Button(new Rect(Screen.width / 2 + 80, Screen.height / 2 - 25, 20, 50), "→"))
                {
                    if (curIndex != settings.Length - 1) curIndex++;
                    else curIndex = 0;
                }

                //I only made switching between quality settings. You can make it much more complex if you want.
                if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 30, 200, 50), "Back"))
                {
                    curMenu = 1;
                }
            }
        }
    }

    void SpawnPlayer()
    {
        spawned = true;
        int random = Random.Range(0, spawnPoints.Length);
        Network.Instantiate(player, spawnPoints[random].position, spawnPoints[random].rotation, 0);
        spawned = true;
        spawnCam.enabled = false;
        spawnListener.enabled = false;
        spawned = true;
        Screen.lockCursor = true;
    }

    public void Die()
    {
        Screen.lockCursor = false;
        spawned = false;
        spawnCam.enabled = true;
        spawnListener.enabled = true;
    }
}
