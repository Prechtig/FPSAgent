using UnityEngine;
using System.Collections;

public class RotateAroundVector : MonoBehaviour
{
    public Vector3 rotateAround;
    public float speed;

    void Start()
    {
        rotateAround.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotateAround * speed * Time.deltaTime);
    }
}
