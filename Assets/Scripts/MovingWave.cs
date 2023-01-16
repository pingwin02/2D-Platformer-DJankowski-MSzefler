using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWave : MonoBehaviour
{

    public int moveSpeed = 10;

    public bool movingDown = false;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentGameState == GameState.GS_GAME)
        {
            if (transform.position.x > 170)
                movingDown = true;

            if(!movingDown)
                transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
            else
                transform.Translate(0.0f, -moveSpeed * Time.deltaTime, 0.0f, Space.World);
        }
    }
}
