using UnityEngine;
using System.Collections;

[System.Serializable]
public class ReloadSound
{
    public string name = "Mag out";
    public AudioClip clip;
    public float length;
}

public class Weapon : MonoBehaviour
{
    public Animation anim;
    public AnimationClip fireAnim;
    public AnimationClip reloadAnim;
    public AnimationClip reloadEmptyAnim;
    public AnimationClip drawAnim;

    #region bools
    public bool reloading;
    public bool[] canAims;
    private bool canAim;
    public bool[] canReloads;
    private bool canReload;
    public bool[] canFires;
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
    protected int magsLeft = 10;
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

    #region sound
    public AudioSource localSource;
    public AudioClip fireSound;
    public ReloadSound[] drawSound;
    public ReloadSound[] reloadSounds;
    public ReloadSound[] reloadSoundsEmpty;
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
    private float hitAlpha;
    public AudioClip hitMarkerSound;
    #endregion


    void Start()
    {
        muzzle.SetActive(false);
        if (GetComponent<NetworkView>().isMine)
        {
            spreadTemp = basicSpread;
            spread = basicSpread;
            StartCoroutine(CheckBools());
            StartCoroutine(Draw());
        }
        else
        {
            this.enabled = false;
        }
    }

    void Update()
    {
        if (GetComponent<NetworkView>().isMine)
        {
            if (hitAlpha > 0) hitAlpha -= Time.deltaTime;
            spread = Mathf.Clamp(spread, 0, maximumSpread);
            if (aiming) spread = aimSpread;
            else spread = Mathf.Lerp(spread, spreadTemp + cv.velMag * 2, Time.deltaTime * 8);
            if (spreadTemp > basicSpread) spreadTemp -= Time.deltaTime * spreadReturnTime;
            pivot = new Vector2(Screen.width / 2, Screen.height / 2);
            bulletsLeftRead = bulletsLeft;
            bulletsPerMagRead = bulletsPerMag;
            magsLeftRead = magsLeft;
            camKB.localRotation = Quaternion.Lerp(camKB.localRotation, Quaternion.identity, Time.deltaTime * returnSpeed);
            wepKB.localRotation = Quaternion.Lerp(wepKB.localRotation, Quaternion.identity, Time.deltaTime * returnSpeed);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, curFov, Time.deltaTime * 10);
            transform.localPosition = Vector3.Lerp(transform.localPosition, curPos, Time.deltaTime * 10);
            if (Screen.lockCursor)
                CheckInput();
            canReloads[1] = true;
            canAims[1] = !cv.running;
            canFires[1] = !cv.running;
            if (aiming)
            {
                curFov = aimFov;
                curPos = aimPos;
            }
            else
            {
                curFov = hipFov;
                if (cv.state == 0)
                {
                    curPos = hipPos;
                }
                else if (cv.state == 1)
                {
                    curPos = crouchPos;
                }
            }

            if (!canAim) aiming = false;
        }
        else
        {
            this.enabled = false;
        }
    }

    void OnGUI()
    {
        if (GetComponent<NetworkView>().isMine)
        {

            float w = crosshairFirstModeHorizontal.width;
            float h = crosshairFirstModeHorizontal.height;
            Rect position1 = new Rect((Screen.width + w) / 2 + (spread * sizeMultiplier), (Screen.height - h) / 2, w, h);
            Rect position2 = new Rect((Screen.width - w) / 2, (Screen.height + h) / 2 + (spread * sizeMultiplier), w, h);
            Rect position3 = new Rect((Screen.width - w) / 2 - (spread * sizeMultiplier) - w, (Screen.height - h) / 2, w, h);
            Rect position4 = new Rect((Screen.width - w) / 2, (Screen.height - h) / 2 - (spread * sizeMultiplier) - h, w, h);
            if (!aiming)
            {
                GUI.DrawTexture(position1, crosshairFirstModeHorizontal); 	//Right
                GUI.DrawTexture(position2, crosshairFirstModeVertical); 	//Up
                GUI.DrawTexture(position3, crosshairFirstModeHorizontal); 	//Left
                GUI.DrawTexture(position4, crosshairFirstModeVertical);		//Down
            }

            GUI.color = new Color(1, 1, 1, hitAlpha);
            GUI.DrawTexture(new Rect((Screen.width - size) / 2, (Screen.height - size) / 2, size, size), tex);

        }
    }

    void CheckInput()
    {
        aiming = (canAim && Input.GetKey(KeyCode.Mouse1));
        if (!reloading && Time.time > timer && canFire && Input.GetKey(KeyCode.Mouse0) && bulletsLeft > 0 && Screen.lockCursor)
        {
            FireOneShot();
        }
        if (!reloading && canReload && magsLeft > 0 && Input.GetKeyDown(KeyCode.R) && Screen.lockCursor)
        {
            reloading = true;
            StartCoroutine(Reload());
        }
    }

    void FireOneShot()
    {
        spreadTemp += spreadAddPerShot;
        timer = Time.time + fireRate;
        anim.Rewind(fireAnim.name);
        anim.Play(fireAnim.name);
        localSource.clip = fireSound;
        localSource.PlayOneShot(fireSound);
        StartCoroutine(MuzzleFlash());
        StartCoroutine(Kick3(camKB, new Vector3(-Random.Range(minKB, maxKB), Random.Range(minKBSide, maxKBSide), 0), 0.1f));
        StartCoroutine(Kick3(wepKB, new Vector3(-Random.Range(minKB, maxKB), Random.Range(minKBSide, maxKBSide), 0), 0.1f));

        float actualSpread = Random.Range(-spread, spread);
        //Vector3 position = new Vector3(bulletGo.position.x - actualSpread, bulletGo.position.y - actualSpread, bulletGo.position.z);
        Vector3 direction = gameObject.transform.TransformDirection(new Vector3(Random.Range(-0.01f, 0.01f) * spread, Random.Range(-0.01f, 0.01f) * spread, 1));
        RaycastHit hit2;
        if (Physics.Raycast(bulletGo.position, direction, out hit2, range, hitLayers))
        {
            OnHit(hit2);
        }
        bulletsLeft--;
    }

    void DoHitMark()
    {
        hitAlpha = 2;
        GetComponent<AudioSource>().PlayOneShot(hitMarkerSound, 1f);
    }

    void OnHit(RaycastHit hit)
    {
        if (hit.rigidbody)
        {
            hit.rigidbody.AddForceAtPosition(2000 * bulletGo.forward, hit.point);
        }
        if (hit.transform.tag == "Player")
        {
            Instantiate(blood, hit.point, Quaternion.identity);
            DoHitMark();
            if (hit.transform.root.GetComponent<NetworkView>())
                hit.transform.root.GetComponent<NetworkView>().RPC("ApplyDamage", RPCMode.AllBuffered, Random.Range(damageMin, damageMax), 1);
        }
        else
        {
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
            t += Time.deltaTime * rate;
            goTransform.localRotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }
    }

    IEnumerator Reload()
    {
        reloading = true;
        canAims[0] = false;
        canFires[0] = false;
        canReloads[0] = false;
        if (bulletsLeft > 0)
        {
            StartCoroutine(ReloadingSound(reloadSounds));
            anim.Play(reloadAnim.name);
            yield return new WaitForSeconds(reloadAnim.length);
            bulletsLeft = bulletsPerMag + 1;
            magsLeft--;
        }
        else
        {
            StartCoroutine(ReloadingSound(reloadSoundsEmpty));
            anim.Play(reloadEmptyAnim.name);
            yield return new WaitForSeconds(reloadEmptyAnim.length);
            bulletsLeft = bulletsPerMag;
            magsLeft--;
        }
        canAims[0] = true;
        canFires[0] = true;
        canReloads[0] = true;
        reloading = false;
    }

    IEnumerator ReloadingSound(ReloadSound[] theSound)
    {
        foreach (ReloadSound lol in theSound)
        {
            yield return new WaitForSeconds(lol.length);
            localSource.clip = lol.clip;
            localSource.Play();
        }
    }

    IEnumerator CheckBools()
    {
        CheckAim();
        CheckReload();
        CheckFire();
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(CheckBools());
    }

    IEnumerator MuzzleFlash()
    {
        muzzle.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        muzzle.SetActive(false);
    }

    void CheckAim()
    {
        canAim = false;
        foreach (bool lol in canAims)
        {
            if (!lol) return;
        }
        canAim = true;
    }

    void CheckReload()
    {
        canReload = false;
        foreach (bool lol in canReloads)
        {
            if (!lol) return;
        }
        canReload = true;
    }

    void CheckFire()
    {
        canFire = false;
        foreach (bool lol in canFires)
        {
            if (!lol) return;
        }
        canFire = true;
    }

    IEnumerator Draw()
    {
        canAims[0] = false;
        canFires[0] = false;
        canReloads[0] = false;
        //localSource.clip = drawSound;
        //localSource.Play();
        StartCoroutine(ReloadingSound(drawSound));
        anim.Play(drawAnim.name);
        yield return new WaitForSeconds(drawAnim.length);
        canAims[0] = true;
        canFires[0] = true;
        canReloads[0] = true;
    }
}
