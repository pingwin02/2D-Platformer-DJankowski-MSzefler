using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class FoxController : MonoBehaviour
{
    public float moveSpeed = 0.1f;

    public float jumpForce = 6.0f;

    private Rigidbody2D rigidBody;

    public LayerMask groundLayer;

    const float rayLength = 1.50f;

    private Collider2D _collider;

    private bool active = true;

    private Vector3 startPosition;

    private Animator animator;

    private bool isWalking = false;

    private bool isFacingRight = true;

    private bool doubleJumped = false;

    private bool immortalMode = false;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider2D>();
        SetStartingPosition();

    }
    // Update is called once per frame
    void Update()
    {
        if (!active || GameManager.instance.currentGameState != GameState.GS_GAME) return;

        isWalking = false;

        if (Input.GetKeyDown(KeyCode.G))
        {
            immortalMode = !immortalMode;
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
            isWalking = true;

            if (!isFacingRight) Flip();
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

        if (!isGrounded() && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !doubleJumped)
        {
            // Double jump
            DoubleJump();
        }

        /*
        Debug.DrawRay(transform.position, rayLength * Vector3.down, Color.white, 1, false);
        Debug.DrawRay(transform.position - new Vector3(0.3f, 0, 0), rayLength * Vector3.down, Color.white, 1, false);
        Debug.DrawRay(transform.position + new Vector3(0.3f, 0, 0), rayLength * Vector3.down, Color.white, 1, false);
        */

        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isGrounded", isGrounded());
        animator.SetBool("isDead", false);
        if (isGrounded())
        {
            animator.SetBool("didKill", false);
        }
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
            //rigidBody.velocity = Vector3.zero;
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);
            //rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            doubleJumped = false;
        }
    }

    void DoubleJump()
    {
        // Apply a force upwards to the player
        //rigidBody.AddForce(Vector2.up * jumpForce/2, ForceMode2D.Impulse);
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);

        // Set the doubleJumped flag to true
        doubleJumped = true;
    }

    private void MiniJump()
    {
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpForce / 2);
    }

    public void Die()
    {
        if (!immortalMode)
        {
            active = false;
            animator.SetBool("isDead", true);
            _collider.enabled = false;
            rigidBody.velocity = Vector3.zero;
            MiniJump();
            GameManager.instance.AddHealth(-1);

            if (GameManager.instance.currentGameState == GameState.GS_GAME)
                StartCoroutine(RespawnPlayer());
        }
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(1.5f);
        transform.position = startPosition;
        active = true;
        _collider.enabled = true;
        MiniJump();
    }
    private IEnumerator WinAnimation()
    {
        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(0.5f);
            MiniJump();
        }
        yield return new WaitForSeconds(1.0f);
        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(0.5f);
            MiniJump();
        }

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
        if (other.CompareTag("Bonus"))
        {
            GameManager.instance.AddPoints(1);
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("Finish"))
        {
            //if (GameManager.instance.LevelCompleted())
            //{
            //    StartCoroutine(WinAnimation());
            //    other.GetComponent<BoxCollider2D>().enabled = false;
            //    animator.SetBool("didKill", true);
            //}

            GameManager.instance.LevelCompleted();
        }
        else if (other.CompareTag("Enemy"))
        {
            if (transform.position.y > other.gameObject.transform.position.y + 1.15f)
            {
                animator.SetBool("didKill", true);
                MiniJump();
                GameManager.instance.AddKilledEnemy();
            }
            else
            {
                Die();
            }
        }
        else if (other.CompareTag("Danger"))
        {
            Die();
        }
        else if (other.CompareTag("Key"))
        {
            Color color = other.GetComponent<SpriteRenderer>().color;

            GameManager.instance.AddKeys(color);
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("Heart"))
        {
            GameManager.instance.AddHealth(2);
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("MovingPlatform"))
        {
            transform.SetParent(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("MovingPlatform"))
        {
            transform.SetParent(null);
        }
    }
}
