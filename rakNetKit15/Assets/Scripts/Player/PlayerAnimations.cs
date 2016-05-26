using UnityEngine;
using System.Collections;

public class PlayerAnimations : MonoBehaviour
{
    public CharacterValues cv;
    public Animator anim;
    public Transform leftHandIK;

    void Update()
    {
        if (cv && anim)
        {
            anim.SetFloat("ver", cv.ver);
            anim.SetFloat("hor", cv.hor);
            anim.SetFloat("velMag", cv.velMag);
            anim.SetFloat("look", 0);
            anim.SetBool("grounded", cv.grounded);
            anim.SetBool("running", cv.running);
            anim.SetBool("reloading", false);
            anim.SetBool("firing", false);
            anim.SetBool("aiming", cv.aiming);
            anim.SetInteger("state", cv.state);
        }
    }

    //PRO ONLY
    void OnAnimatorIK()
    {
        if (anim)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (!cv.reloading)
            {

                //weight = 1.0 for the right hand means position and rotation will be at the IK goal (the place the character wants to grab)
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);


                    anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIK.position);
                    anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIK.rotation);
                

            }

                        //if the IK is not active, set the position and rotation of the hand back to the original position
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            }
        }
    }    
}
