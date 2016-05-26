using UnityEngine;
using System.Collections;

[System.Serializable]
public class AutoLoad
{
    public bool autoLoad;
    public string LevelToLoad;
}

public class LoadLevelAsync : MonoBehaviour
{
    private bool loading;
    public AutoLoad autoLoad;
    public AsyncOperation async;
    public float progress;
    public bool isDone;

    public Texture2D loadingBar;

    public string map = "Test";
    public string year = "2014";
    public string location = "Unknown";
    public string gameMode = "Free testing";

    public string[] intels;
    private string curIntel = "DICK";



    void Start()
    {
        curIntel = intels[Random.Range(0, intels.Length)];
        DontDestroyOnLoad(transform.gameObject);
        if (autoLoad.autoLoad)
            StartCoroutine("LoadLevel", autoLoad.LevelToLoad);
    }
    void Update()
    {
        if (loading)
        {
            progress = async.progress * 100;
            isDone = async.isDone;
        }
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 60;
        GUI.Label(new Rect(20, 20, 200, 200), map + ", " + year, style);
        GUIStyle style2 = new GUIStyle();
        style2.fontSize = 40;
        GUI.Label(new Rect(30, 90, 200, 200), location, style2);
        GUIStyle style3 = new GUIStyle();
        style3.fontSize = 20;
        GUI.Label(new Rect(40, 140, 200, 200), gameMode, style3);
        GUI.Label(new Rect(20, Screen.height - 80, Screen.width - 20, 20), curIntel);
        GUI.DrawTexture(new Rect(20, Screen.height - 60, progress * Screen.width / 100, 40), loadingBar);
    }

    IEnumerator LoadLevel(string levelname)
    {
        loading = true;
        async = Application.LoadLevelAsync(levelname);
        yield return async;
        Debug.Log("Done loading level named: " + levelname);
        loading = false;
        Destroy(this.gameObject);
    }
}