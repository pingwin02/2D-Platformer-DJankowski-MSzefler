using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public float moveSpeed = 0.1f;

    float startpositionX;

    public float moveRange = 1.0f;

    private Animator animator;

    private bool isFacingRight = false;

    bool isMovingRight = false;

    private Collider2D _collider;

    public bool isMoving = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isMoving", isMoving);

        if (!isMoving) return;

        if (isMovingRight)
        {
            if (this.transform.position.x < startpositionX + moveRange)
            {
                MoveRight();
            } 
            else
            {
                Flip();
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
                Flip();
                MoveRight();
            }
        }
    }
    void Awake()
    {
        animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();

        startpositionX = this.transform.position.x;

    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        isMovingRight = !isMovingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void MoveRight()
    {
        transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
    }

    private void MoveLeft()
    {
        transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (transform.position.y + 1.15f < other.gameObject.transform.position.y)
            {
                isMoving = false;
                _collider.enabled = false;
                animator.SetBool("isDead", true);
                StartCoroutine(KillOnAnimationEnd());
            }
        }

    }

    private IEnumerator KillOnAnimationEnd()
    {
        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);
    }
}
