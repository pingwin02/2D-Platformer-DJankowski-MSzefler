using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crusher : MonoBehaviour
{

    public float downSpeed;

    public float upSpeed;

    public Transform up;

    public Transform down;

    public float delayTime;

    private float startTime;

    private bool chop;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentGameState != GameState.GS_GAME
            && GameManager.instance.currentGameState != GameState.GS_START) return;

        if (Time.time - startTime >= delayTime)
        {
            if (transform.position.y >= up.position.y)
            {
                chop = true;
            }

            if (transform.position.y <= down.position.y)
            {
                chop = false;
            }
            if (chop)
            {
                transform.position = Vector2.MoveTowards(transform.position, down.position, downSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, up.position, upSpeed * Time.deltaTime);
            }
        }
    }
}
