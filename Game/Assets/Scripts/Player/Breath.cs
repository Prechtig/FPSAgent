using UnityEngine;
using System.Collections;

public class Breath : MonoBehaviour
{
    public CharacterValues cv;
    public Animation anim;
    public AnimationClip breath;
    public AnimationClip noBreath;

    void Start()
    {
        anim.wrapMode = WrapMode.Loop;
    }

    void Update()
    {
        if (cv.aiming) anim.CrossFade(noBreath.name);
        else anim.CrossFade(breath.name);
    }
}
