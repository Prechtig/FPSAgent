using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DefaultHorizontalPlayerSpawn : MonoBehaviour, IPlayerSpawn
{
	public GameObject Player{ get; set;}
	public Transform[] SpawnPoints{ get; set;}
	public float X{ get; set;}
	public float Z{ get; set;}

	private static Text _shots;


	//Canvas components
	private static Canvas _hud;
	private static Canvas HUD{
		get {
			if (_hud == null) {
				_hud = GameObject.FindWithTag("PlayerHUD").GetComponent<Canvas>() as Canvas;
			}
			return _hud;
		}
	}

	private static Text _shotsLeftText;
	private static Text ShotsLeftText{
		get {
			if (_shotsLeftText == null) {
				_shotsLeftText = GameObject.FindWithTag("UIText").GetComponent<Text>() as Text;
			}
			return _shotsLeftText;
		}
	}

	public void OnEnable(){
		//SpawnPlayer ();
	}

	// Use this for initialization
	/*
	void Start ()
	{
		SpawnPlayer ();
	}
	*/

	// Update is called once per frame
	void Update ()
	{

	}

	public void SpawnPlayer()
	{
		/*
		int random = Random.Range(0, spawnPoints.Length);
		Instantiate (player, spawnPoints [random].position, spawnPoints [random].rotation);
		*/
		//Object[] obs = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object));
		Object a = Resources.Load ("Player");
		Player = (GameObject)Instantiate(a, SpawnPoints[0].position, SpawnPoints[0].rotation);
		//HUD.worldCamera = player.GetComponentInChildren<Camera> ();
		Player.GetComponentInChildren<NEATWeapon> ().ShotsLeftText = ShotsLeftText;
		//Instantiate (GameObject., spawnPoints.position, spawnPoints.rotation);
	}

	/*
	public void OnDestroy(){
		Destroy (spawnPoints);
	}
	*/
}