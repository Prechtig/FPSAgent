using UnityEngine;
using System.Collections;

public class WalkAnimation : MonoBehaviour
{
    public CharacterValues cv;
    public Animation anim;
    public AnimationClip idle;
    public AnimationClip walk;

    void Start()
    {
        anim.wrapMode = WrapMode.Loop;
    }

    void Update()
    {
        if (cv.grounded && cv.velMag > 1 && !cv.running && !cv.aiming)
        {
            anim[walk.name].speed = cv.velMag / 6;
            anim.CrossFade(walk.name);
        }
        else
        {
            anim.CrossFade(idle.name);
        }
    }
}
