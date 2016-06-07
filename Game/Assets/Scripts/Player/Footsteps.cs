using UnityEngine;
using System.Collections;

public class Footsteps : MonoBehaviour
{
    public CharacterValues cv;
    public AudioClip[] concrete;
    public AudioClip[] snow;

    public float runLength = 0.25f;
    public float walkLength = 0.4f;
    public float crouchLength = 0.65f;

    public AudioSource theSource;

    public float timer;

    public string curMat = "";
    public RaycastHit hit;


    void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2))
        {
            curMat = hit.transform.tag;
        }
        if (cv.grounded)
        {
            if (cv.velMag > 1)
            {
                if (timer <= 0)
                {
                    if (cv.running)
                    {
                        if (curMat == "Snow")
                            OneStep(snow, runLength, 1);
                        else
                            OneStep(concrete, runLength, 1);
                    }
                    else if (cv.state == 0)
                    {
                        if (curMat == "Snow")
                            OneStep(snow, walkLength, 0.6f);
                        else
                            OneStep(concrete, walkLength, 0.6f);
                    }
                    else if (cv.state == 1)
                    {
                        if (curMat == "Snow")
                            OneStep(snow, crouchLength, 0.3f);
                        else
                            OneStep(concrete, crouchLength, 0.3f);
                    }
                }
            }
        }
        if (timer > 0) timer -= Time.deltaTime * cv.velPercent;
    }

    void OneStep(AudioClip[] clips, float length, float volume)
    {
        int random = Random.Range(0, clips.Length);
        theSource.clip = clips[random];
        theSource.volume = volume;
        theSource.Play();
        timer = length;
    }
}
