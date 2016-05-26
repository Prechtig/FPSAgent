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

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {

        // Send data to server
        if (stream.isWriting)
        {
            stream.Serialize(ref velMag);
            stream.Serialize(ref grounded);
            stream.Serialize(ref height);
            stream.Serialize(ref center);
            stream.Serialize(ref hor);
            stream.Serialize(ref ver);
            stream.Serialize(ref state);
            stream.Serialize(ref running);
            stream.Serialize(ref aiming);
            stream.Serialize(ref speed);
        }
        else
        {
            stream.Serialize(ref velMag);
            stream.Serialize(ref grounded);
            stream.Serialize(ref height);
            stream.Serialize(ref center);
            stream.Serialize(ref hor);
            stream.Serialize(ref ver);
            stream.Serialize(ref state);
            stream.Serialize(ref running);
            stream.Serialize(ref aiming);
            stream.Serialize(ref speed);
        }
    }

    void Update()
    {
        if (GetComponent<NetworkView>().isMine)
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
        else
        {
            ml.mouseX = mouseX;
            ml.mouseY = mouseY;
            m.hor = hor;
            m.ver = ver;
            m.state = state;
            m.running = running;
            wep.aiming = aiming;
            velPercent = velMag / speed;
        }
    }
}
