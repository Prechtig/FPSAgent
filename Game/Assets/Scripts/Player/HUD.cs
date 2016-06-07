using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour
{
    public PlayerVitals pv;
    public Weapon wep;


    // Use this for initialization
    void Start()
    {
        if (!GetComponent<NetworkView>().isMine)
        {
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void OnGUI()
    {
        if (GetComponent<NetworkView>().isMine)
        {
            GUI.Label(new Rect(20, Screen.height - 40, 100, 40), "Health: " + pv.hitPoints.ToString("F0"));
            GUI.Label(new Rect(20, Screen.height - 20, 150, 40), "Ammo: " + wep.bulletsLeftRead + " / " + wep.bulletsPerMagRead + " | " + wep.magsLeftRead);
        }
        else
        {
            this.enabled = false;
        }
    }
}
