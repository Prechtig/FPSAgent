using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    public CharacterValues cv;
    public Weapon wep;
    public Animation anim;
    public AnimationClip idle;
    public AnimationClip run;
    public Vector3 idlePos;
    public Vector3 idleRot;
    public Vector3 runPos;
    public Vector3 runRot;

    void Start()
    {
        anim.wrapMode = WrapMode.Loop;
    }

    void Update()
    {
        if (cv.running)
        {
            anim.Play(run.name);
            if (wep.reloading)
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(idleRot), Time.deltaTime * 8);
                transform.localPosition = Vector3.Lerp(transform.localPosition, idlePos, Time.deltaTime * 8);
            }
            else
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(runRot), Time.deltaTime * 8);
                transform.localPosition = Vector3.Lerp(transform.localPosition, runPos, Time.deltaTime * 8);
            }
        }
        else
        {
            anim.CrossFade(idle.name);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(idleRot), Time.deltaTime * 8);
            transform.localPosition = Vector3.Lerp(transform.localPosition, idlePos, Time.deltaTime * 8);
        }
    }
}
