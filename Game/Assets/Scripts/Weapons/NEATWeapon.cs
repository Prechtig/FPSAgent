using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NEATWeapon : MonoBehaviour
{
    public Animation anim;
    public AnimationClip fireAnim;
    public AnimationClip reloadAnim;
    public AnimationClip reloadEmptyAnim;
    public AnimationClip drawAnim;

    #region bools

    public bool reloading;
    //public bool[] canAims;
    //private bool canAim;
    //public bool[] canReloads;
    //private bool canReload;
    //public bool[] canFires;
    private bool canFire;

    #endregion

    #region stats

    public float fireRate = 0.1f;
    public float timer = 0;
    [SerializeField]
    protected int bulletsLeft = 30;
    [SerializeField]
    protected int bulletsPerMag = 30;
    [SerializeField]
    protected int magsLeft = 4;
    public float range = 2000;
    public float damageMin = 10;
    public float damageMax = 20;
    public Transform bulletGo;
    public LayerMask hitLayers;
    public GameObject blood;
    public GameObject concrete;
    public GameObject wood;
    public GameObject metal;
    public GameObject dirt;

    #endregion

    #region readOnly

    public int bulletsLeftRead = 30;
    public int bulletsPerMagRead = 30;
    public int magsLeftRead = 10;

    #endregion


    #region components

    public CharacterValues cv;
    public PlayerAnimations pa;

    #endregion

    #region ads

    public Camera cam;
    public bool aiming;
    public float hipFov = 75;
    public float aimFov = 55;
    private float curFov = 75;
    public Vector3 hipPos;
    public Vector3 crouchPos;
    public Vector3 aimPos;
    private Vector3 curPos;

    #endregion

    #region recoil

    public Transform camKB;
    public Transform wepKB;
    public float minKB;
    public float maxKB;
    public float minKBSide;
    public float maxKBSide;
    public float returnSpeed = 5f;

    #endregion

    #region muzzle

    public GameObject muzzle;

    #endregion

    #region crosshair

    public float sizeMultiplier = 1f;
    public float aimSpread;
    public float basicSpread = 30;
    public float maximumSpread = 100;
    public float spreadReturnTime = 5;
    public float spreadAddPerShot = 5;
    public float spreadTemp;
    private float spread = 30;

    //Crosshair Textures
    public Texture2D crosshairFirstModeHorizontal;
    public Texture2D crosshairFirstModeVertical;

    #endregion

    #region private

    private Vector2 pivot;

    #endregion

    #region hitMark

    public Texture2D tex;
    public float size = 32;
    //private float hitAlpha;
    //public AudioClip hitMarkerSound;

    #endregion


    public Text ShotsLeftText;
    private System.Random rng = new System.Random();

    public int WrongReloads;
    public int Shots;
    public int Misses;
    public static bool recoil;

    public uint boxId { get; internal set; }

    void Start()
    {
        muzzle.SetActive(false);
        //spreadTemp = basicSpread;
        //spread = basicSpread;
        //StartCoroutine (CheckBools ());
        canFire = true;
        //StartCoroutine (Draw ());
        //shotsLeftText = GameObject.FindWithTag("UIText").GetComponent<Text>() as Text;
    }

    void Update()
    {
        /*if (hitAlpha > 0)
			hitAlpha -= Time.deltaTime;
		
		if (aiming)
		spread = aimSpread;*/
        /*
        //Debug.Log("First: " + spread);
        spread = Mathf.Lerp(spread, spreadTemp + cv.velMag * 2, Time.fixedDeltaTime * 8);
        //Debug.Log("Secon: " + spread);
        if (spreadTemp > basicSpread)
            spreadTemp -= Time.fixedDeltaTime * spreadReturnTime;
        if (spreadTemp < 0)
            spreadTemp = 0;
            */
        //Debug.Log("SpreadTemp: " + spread);
        /*
		pivot = new Vector2 (Screen.width / 2, Screen.height / 2);
		bulletsLeftRead = bulletsLeft;
		bulletsPerMagRead = bulletsPerMag;
		magsLeftRead = magsLeft;
		camKB.localRotation = Quaternion.Lerp (camKB.localRotation, Quaternion.identity, Time.deltaTime * returnSpeed);
		wepKB.localRotation = Quaternion.Lerp (wepKB.localRotation, Quaternion.identity, Time.deltaTime * returnSpeed);
		cam.fieldOfView = Mathf.Lerp (cam.fieldOfView, curFov, Time.deltaTime * 10);
		transform.localPosition = Vector3.Lerp (transform.localPosition, curPos, Time.deltaTime * 10);
		//CheckInput ();
		canReloads [1] = true;
		canAims [1] = !cv.running;
		canFires [1] = !cv.running;
		if (aiming) {
			curFov = aimFov;
			curPos = aimPos;
		} else {
			curFov = hipFov;
			if (cv.state == 0) {
				curPos = hipPos;
			} else if (cv.state == 1) {
				curPos = crouchPos;
			}
		}

		if (!canAim)
			aiming = false;
		 */
        ShotsLeftText.text = bulletsLeft + " / " + magsLeft;
    }

    /*
	void OnGUI ()
	{
		float w = crosshairFirstModeHorizontal.width;
		float h = crosshairFirstModeHorizontal.height;
		Rect position1 = new Rect ((Screen.width + w) / 2 + (spread * sizeMultiplier), (Screen.height - h) / 2, w, h);
		Rect position2 = new Rect ((Screen.width - w) / 2, (Screen.height + h) / 2 + (spread * sizeMultiplier), w, h);
		Rect position3 = new Rect ((Screen.width - w) / 2 - (spread * sizeMultiplier) - w, (Screen.height - h) / 2, w, h);
		Rect position4 = new Rect ((Screen.width - w) / 2, (Screen.height - h) / 2 - (spread * sizeMultiplier) - h, w, h);
	}
	*/

    /*
	void CheckInput ()
	{

		aiming = (canAim && Input.GetKey (KeyCode.Mouse1));
		if (!reloading && Time.time > timer && canFire && Input.GetKey (KeyCode.Mouse0) && bulletsLeft > 0) {// && Screen.lockCursor)
			FireOneShot ();
		}
		if (!reloading && canReload && magsLeft > 0 && Input.GetKeyDown (KeyCode.R)) {// && Screen.lockCursor)
			reloading = true;
			StartCoroutine (Reload ());
		}

	}
	*/

    public float GetRandomFloat(double min, double max)
    {
        return (float)(min + (rng.NextDouble() * (max - min)));
    }

    float recoilViolence = 2.5f;
    float recoilCoolOffTime = 0.6f;
    private float SpreadModification(float deltaTime)
    {
        if (deltaTime < fireRate)
        {
            throw new System.Exception("Too fast fire");
        }
        if (deltaTime > recoilCoolOffTime)
        {
            return 0;
        }
        return (recoilCoolOffTime - deltaTime) * recoilViolence;
    }

    public void FireOneShot()
    {
        if (!reloading && Time.time >= timer && canFire && bulletsLeft > 0)
        {// && Screen.lockCursor)
            //spreadTemp += spreadAddPerShot;
            if (recoil)
            {
                spread = (spread + 1) * SpreadModification(Time.time - (timer - fireRate));
                spread = Mathf.Clamp(spread, 0, maximumSpread);
            }
            timer = Time.time + fireRate;
            anim.Rewind(fireAnim.name);
            anim.Play(fireAnim.name);
            //localSource.clip = fireSound;
            //localSource.PlayOneShot (fireSound);
            StartCoroutine(MuzzleFlash());

            //Weapon kick
            //StartCoroutine(Kick3(camKB, new Vector3(-Random.Range(minKB, maxKB), Random.Range(minKBSide, maxKBSide), 0), 0.1f));
            //StartCoroutine(Kick3(wepKB, new Vector3(-Random.Range(minKB, maxKB), Random.Range(minKBSide, maxKBSide), 0), 0.1f));

            //float actualSpread = Random.Range (-spread, spread);
            //Vector3 position = new Vector3 (bulletGo.position.x - actualSpread, bulletGo.position.y - actualSpread, bulletGo.position.z);

            //Bullet spread
            //Vector3 direction = gameObject.transform.TransformDirection(new Vector3(Random.Range(-0.01f, 0.01f) * spread, Random.Range(0, 0.01f) * spread * 3, 1));
            Vector3 direction;
            if (recoil)
            {
                direction = gameObject.transform.TransformDirection(new Vector3(GetRandomFloat(-0.01d, 0.01d) * spread, GetRandomFloat(0, 0.01d) * spread * 3, 1));
            }
            else
            {
                direction = gameObject.transform.TransformDirection(0, 0, 1);
            }

            RaycastHit hit2;
            if (Physics.Raycast(bulletGo.position, direction, out hit2, range, hitLayers))
            {
                OnHit(hit2);
            }
            bulletsLeft--;
            /*
            if (spreadTemp == 0)
            {
                spreadTemp += spreadAddPerShot;
            }
            else
            {
                spreadTemp *= spreadAddPerShot;
            }

            if (spreadTemp > maximumSpread)
            {
                spreadTemp = maximumSpread;
            }
            */
            //Debug.Log("Temp: " + spreadTemp);
        }
    }

    void DoHitMark()
    {
        //hitAlpha = 2;
        //GetComponent<AudioSource> ().PlayOneShot (hitMarkerSound, 1f);
    }

    void OnHit(RaycastHit hit)
    {
        Shots++;
        if (hit.transform.tag == "Bot")
        {
            Instantiate(blood, hit.point, Quaternion.identity);
            DoHitMark();
            if (hit.transform.GetComponent<BotVitals>())
            {
                hit.transform.GetComponent<BotVitals>().ApplyDamage(Random.Range(damageMin, damageMax), 1);
            }
        }
        else
        {
            Misses++;
            if (hit.transform.tag == "Wood")
            {
                GameObject theObj = Instantiate(wood, hit.point + hit.normal * 0.01f, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                theObj.transform.parent = hit.transform;
            }
            else if (hit.transform.tag == "Metal")
            {
                GameObject theObj = Instantiate(metal, hit.point + hit.normal * 0.01f, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                theObj.transform.parent = hit.transform;
            }
            else if (hit.transform.tag == "Dirt")
            {
                GameObject theObj = Instantiate(dirt, hit.point + hit.normal * 0.01f, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                theObj.transform.parent = hit.transform;
            }
            else
            {
                GameObject theObj = Instantiate(concrete, hit.point + hit.normal * 0.01f, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                theObj.transform.parent = hit.transform;
            }
        }
    }

    IEnumerator Kick3(Transform goTransform, Vector3 kbDirection, float time)
    {
        Quaternion startRotation = goTransform.localRotation;
        Quaternion endRotation = goTransform.localRotation * Quaternion.Euler(kbDirection);
        float rate = 1.0f / time;
        var t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.fixedDeltaTime * rate;
            goTransform.localRotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }
    }

    public void Reload()
    {
        if (!reloading && magsLeft > 0)
        {// && Screen.lockCursor)
            reloading = true;
            StartCoroutine(RunReload());
        }
    }

    private IEnumerator RunReload()
    {
        reloading = true;
        //canAims [0] = false;
        //canFires [0] = false;
        //canReloads [0] = false;
        anim[reloadAnim.name].speed = 3;
        anim[reloadEmptyAnim.name].speed = 3;

        if (bulletsLeft == bulletsPerMag)
        {
            WrongReloads++;
        }

        //StartCoroutine (ReloadingSound (reloadSoundsEmpty));
        anim.Play(reloadEmptyAnim.name);
        yield return new WaitForSeconds(reloadEmptyAnim.length / 3);
        bulletsLeft = bulletsPerMag;
        magsLeft--;

        //canAims [0] = true;
        //canFires [0] = true;
        //canReloads [0] = true;
        reloading = false;
    }

    /*
	IEnumerator ReloadingSound (ReloadSound[] theSound)
	{
		foreach (ReloadSound lol in theSound) {
			yield return new WaitForSeconds (lol.length);
			localSource.clip = lol.clip;
			localSource.Play ();
		}
	}
	*/

    /*
	IEnumerator CheckBools ()
	{
		CheckAim ();
		CheckReload ();
		CheckFire ();
		yield return new WaitForSeconds (0.1f);
		StartCoroutine (CheckBools ());
	}
	*/

    IEnumerator MuzzleFlash()
    {
        muzzle.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        muzzle.SetActive(false);
    }

    /*
	void CheckAim ()
	{
		canAim = false;
		foreach (bool lol in canAims) {
			if (!lol)
				return;
		}
		canAim = true;
	}
	*/

    /*
	void CheckReload ()
	{
		canReload = false;
		foreach (bool lol in canReloads) {
			if (!lol)
				return;
		}
		canReload = true;
	}
	*/

    /*
	void CheckFire ()
	{

		canFire = false;
		foreach (bool lol in canFires) {
			if (!lol)
				return;
		}
		canFire = true;

	}
	*/

    IEnumerator Draw()
    {
        //canAims [0] = false;
        //canFires [0] = false;
        //canReloads [0] = false;
        //localSource.clip = drawSound;
        //localSource.Play();
        //StartCoroutine (ReloadingSound (drawSound));
        anim[drawAnim.name].speed = 3;
        anim.Play(drawAnim.name);
        yield return new WaitForSeconds(drawAnim.length / 3);
        //canAims [0] = true;
        //canFires [0] = true;

        //canReload = true;
        //canReloads [0] = true;
    }
}
