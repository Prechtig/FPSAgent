using UnityEngine;
using System.Collections;

public class Delay : MonoBehaviour
{
    public CharacterValues cv;
    public float amount = 0.02f;
    public float maxAmount = 0.03f;
    public float smooth = 3f;
    private Vector3 def;

    private float factorX;
    private float factorY;

    // Use this for initialization
    void Start()
    {
        def = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        factorX = -cv.mouseX * amount;
        factorY = -cv.mouseY * amount;

        if (factorX > maxAmount)
            factorX = maxAmount;

        if (factorX < -maxAmount)
            factorX = -maxAmount;

        if (factorY > maxAmount)
            factorY = maxAmount;

        if (factorY < -maxAmount)
            factorY = -maxAmount;

        Vector3 Final = new Vector3(def.x + factorX, def.y + factorY, def.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, Final, Time.deltaTime * smooth);
    }
}
