using UnityEngine;
using System.Collections;

public class TiltGun : MonoBehaviour
{
    public CharacterValues cv;
    public float tilt;
    public float smooth = 5;
    public Quaternion curRot;

    void Update()
    {
        curRot = Quaternion.Euler(0, 0, -cv.hor * tilt);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, curRot, Time.deltaTime * smooth);
    }
}
