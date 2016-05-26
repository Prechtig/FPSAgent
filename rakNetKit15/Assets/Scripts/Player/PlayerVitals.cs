using UnityEngine;
using System.Collections;

public class PlayerVitals : MonoBehaviour
{
    public Spawn spawn;
    public GameObject obj;
    public float hitPoints;
    public Rigidbody wep;
    public Transform hitCam;
    public Transform hitWep;

    void Start()
    {
        spawn = GameObject.FindWithTag("GameController").GetComponent<Spawn>();
        Rigidbody[] bodies = obj.GetComponentsInChildren<Rigidbody>();
        Collider[] collies = obj.GetComponentsInChildren<Collider>();
        wep.useGravity = false;
        wep.isKinematic = true;
        wep.GetComponent<Collider>().isTrigger = true;
        foreach (Rigidbody body in bodies)
        {
            body.useGravity = false;
            body.isKinematic = true;
        }
        foreach (Collider coll in collies)
        {
            coll.isTrigger = true;
        }
    }

    void Update()
    {
        hitPoints = Mathf.Clamp(hitPoints, 0, 100);
        hitCam.localRotation = Quaternion.Lerp(hitCam.localRotation, Quaternion.identity, Time.deltaTime * 5);
        hitWep.localRotation = Quaternion.Lerp(hitWep.localRotation, Quaternion.identity, Time.deltaTime * 5);
    }

    [RPC]
    public void ApplyDamage(float dmg, int isBullet)
    {
        hitPoints -= dmg;
        StartCoroutine(Kick3(hitWep, new Vector3(-3f * dmg / 10, Random.Range(-3, 3) * dmg / 10, 0), 0.1f));
        StartCoroutine(Kick3(hitCam, new Vector3(-5f * dmg / 10, Random.Range(-5, 5) * dmg / 10, 0), 0.1f));
        if (hitPoints <= 0)
        {
            GetComponent<NetworkView>().RPC("Die", RPCMode.AllBuffered);
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

    [RPC]
    protected void Die()
    {
        Destroy(obj.GetComponent<PlayerAnimations>());
        Destroy(obj.GetComponent<Animator>());
        Rigidbody[] bodies = obj.GetComponentsInChildren<Rigidbody>();
        Collider[] collies = obj.GetComponentsInChildren<Collider>();
        foreach (Rigidbody body in bodies)
        {
            body.useGravity = true;
            body.isKinematic = false;
        }
        foreach (Collider coll in collies)
        {
            coll.isTrigger = false;
        }
        wep.useGravity = true;
        wep.isKinematic = false;
        wep.GetComponent<Collider>().isTrigger = false;
        wep.transform.parent = null;
        if (GetComponent<NetworkView>().isMine)
        {
            spawn.Die();
        }
        obj.transform.parent = null;
        Destroy(wep, 10);
        Destroy(obj, 10);
        Destroy(this.gameObject);
    }

}
