using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{

    public float moveSpeed = 0.1f;

    float startpositionX;

    public float moveRange = 1.0f;

    bool isMovingRight = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentGameState != GameState.GS_GAME) return;

        if (isMovingRight)
        {
            if (this.transform.position.x < startpositionX + moveRange)
            {
                MoveRight();
            }
            else
            {
                isMovingRight = !isMovingRight;
                MoveLeft();
            }
        }
        else
        {
            if (this.transform.position.x > startpositionX - moveRange)
            {
                MoveLeft();
            }
            else
            {
                isMovingRight = !isMovingRight;
                MoveRight();
            }
        }
    }
    void Awake()
    {

        startpositionX = this.transform.position.x;

    }

    private void MoveRight()
    {
        transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
    }

    private void MoveLeft()
    {
        transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
    }
}
