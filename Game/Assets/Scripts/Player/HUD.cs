using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour
{
    public PlayerVitals pv;
    public Weapon wep;
	public GameObject player;


    // Use this for initialization
    void Start()
    {
		
    }

    // Update is called once per frame
    void OnGUI()
    {
    	GUI.Label(new Rect(20, Screen.height - 40, 100, 40), "Health: " + pv.hitPoints.ToString("F0"));
		GUI.Label(new Rect(20, Screen.height - 20, 150, 40), "Ammo: " + wep.bulletsLeftRead + " / " + wep.bulletsPerMagRead + " | " + wep.magsLeftRead);
    }
}
