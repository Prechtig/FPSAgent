using UnityEngine;
using System.Collections;

public class CharacterValues : MonoBehaviour
{
    public float velMag;
    public bool grounded;
    public float height;
    public Vector3 center;
    public float hor;
    public float ver;
    public float mouseX;
    public float mouseY;
    public int state;
    public bool running;
    public bool aiming;
    public bool reloading;
    public float speed;
    public float velPercent;

    public MouseLook ml;
    public Movement m;
    public Weapon wep;

    void Update()
    {
        mouseX = ml.mouseX;
        mouseY = ml.mouseY;
        hor = m.hor;
        ver = m.ver;
        state = m.state;
        running = m.running;
        grounded = m.controller.isGrounded;
        velMag = m.controller.velocity.magnitude;
        aiming = wep.aiming;
        speed = m.speed;
        velPercent = velMag / speed;
    }
}
