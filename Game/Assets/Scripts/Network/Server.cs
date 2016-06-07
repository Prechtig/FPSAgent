using UnityEngine;
using System.Collections;

public class Server : MonoBehaviour
{
    public float dif = 1;
    HostData[] datas;
    public Vector2 scroll;
    private string gameName = "Server 666";

    //You have to change this!
    public string uniqueGameName = "MyUniqueGameName";

    //After which time will the server list be refreshed ?
    public float refreshTime = 10;

    void Start()
    {
        InvokeRepeating("GetHostList", 0, refreshTime);
        Network.isMessageQueueRunning = true;
    }

    void GetHostList()
    {
        MasterServer.RequestHostList(uniqueGameName);
    }

    void Update()
    {
        dif = (Screen.width / 12.8f) / 100;
        datas = MasterServer.PollHostList();
    }

    void OnGUI()
    {
        if (!Network.isServer && !Network.isClient)
        {
            GUI.Box(new Rect(20 * dif, 20 * dif, 200 * dif, 200 * dif), "");
            GUILayout.BeginArea(new Rect(25 * dif, 25 * dif, 190 * dif, 190 * dif));
            gameName = GUILayout.TextField(gameName);
            if (GUILayout.Button("Start Server"))
            {
                Network.InitializeSecurity();
                Network.InitializeServer(17, 25001, /*!Network.HavePublicAddress()*/ true);
                MasterServer.RegisterHost(uniqueGameName, gameName);
            }
            if (GUILayout.Button("Quit"))
            {
                Application.Quit();
            }
            if (GUILayout.Button("Direct Connect"))
            {
                Network.Connect("127.0.0.1", 25001);
            }
            GUILayout.EndArea();
            GUI.Box(new Rect(Screen.width - 400 * dif, 0, 400 * dif, Screen.height), "");
            GUILayout.BeginArea(new Rect(Screen.width - 400 * dif, 0, 400 * dif, Screen.height));
            GUILayout.Label("Avaiable Servers: " + datas.Length);
            scroll = GUILayout.BeginScrollView(scroll);
            foreach (HostData data in datas)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(data.gameName + " Players: " + data.connectedPlayers + " / " + data.playerLimit);
                if (GUILayout.Button("Connect"))
                {
                    Network.Connect(data);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
}
