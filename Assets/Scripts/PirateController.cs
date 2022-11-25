using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateController : MonoBehaviour
{
    public float moveSpeed = 0.1f;

    public float jumpForce = 6.0f;

    private Rigidbody2D rigidBody;

    public LayerMask groundLayer;

    const float rayLength = 1.00f;

    private Collider2D _collider;

    private bool active = true;

    private Vector3 startPosition;

    private Animator animator;

    private bool isWalking = false;

    private bool isFacingRight = true;

    int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider2D>();
        SetStartingPosition();

    }
    // Update is called once per frame
    void Update()
    {
        if (!active)
        {
            return;
        }

        isWalking = false;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        { 
            transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
            isWalking = true;

            if(!isFacingRight) Flip();
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        { 
            transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
            isWalking = true;

            if (isFacingRight) Flip();
        }
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (transform.position.y < -5)
        {
            Die();
        }

        /*
        Debug.DrawRay(transform.position, rayLength * Vector3.down, Color.white, 1, false);
        Debug.DrawRay(transform.position - new Vector3(0.3f, 0, 0), rayLength * Vector3.down, Color.white, 1, false);
        Debug.DrawRay(transform.position + new Vector3(0.3f, 0, 0), rayLength * Vector3.down, Color.white, 1, false);
        */

        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isGrounded", isGrounded());
    }
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();
    }

    bool isGrounded() 
    {
        return (Physics2D.Raycast(transform.position - new Vector3(0.3f, 0, 0), Vector2.down, rayLength, groundLayer.value) ||
            Physics2D.Raycast(transform.position + new Vector3(0.3f, 0, 0), Vector2.down, rayLength, groundLayer.value) ||
            Physics2D.Raycast(this.transform.position, Vector2.down, rayLength, groundLayer.value));
}

    void Jump()
    {
        if (isGrounded())
        {
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            Debug.Log("jumping");
        }
    }

    private void MiniJump()
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce/2);
    }

    public void Die()
    {
        active = false;
        _collider.enabled = false;
        MiniJump();
        Debug.Log("death");
        /* TODO respienie monet nie dziala bo find nie znajduje nieaktywnych obiektow
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Bonus");
        foreach (GameObject coin in coins)
        {
            coin.SetActive(true);
            Debug.Log(coin);
        }
        */
        StartCoroutine(RespawnPlayer());
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(1.5f);
        transform.position = startPosition;
        active = true;
        _collider.enabled = true;
        MiniJump();
    }

    public void SetStartingPosition()
    {
        startPosition = transform.position;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.CompareTag("Bonus");
        Debug.Log("Score: " + ++score);
        other.gameObject.SetActive(false);
    }
}

