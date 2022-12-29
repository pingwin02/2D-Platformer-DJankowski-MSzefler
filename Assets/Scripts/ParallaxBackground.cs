using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform cam;
    public float relativeMove = 0.3f;

    // Update is called once per frame
    void Update()
    {

        transform.position = new Vector2(cam.position.x * relativeMove,
        cam.position.y * relativeMove);
    }
}
