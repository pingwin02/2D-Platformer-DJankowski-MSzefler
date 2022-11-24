using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxController : MonoBehaviour
{
    public float moveSpeed = 0.1f;

    public float jumpForce = 6.0f;

    private Rigidbody2D rigidBody;

    public LayerMask groundLayer;

    const float rayLength = 1.00f;

    private Collider2D _collider;

    private bool active = true;

    private Vector3 startPosition;

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


        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (transform.position.y < -5)
        {
            Die();
        }

        /*
        Debug.DrawRay(transform.position, rayLength * Vector3.down, Color.white, 1, false);
        Debug.DrawRay(transform.position - new Vector3(0.5f, 0, 0), rayLength * Vector3.down, Color.white, 1, false);
        Debug.DrawRay(transform.position + new Vector3(0.5f, 0, 0), rayLength * Vector3.down, Color.white, 1, false);
        */
    }
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    bool isGrounded() 
    {
        return (Physics2D.Raycast(transform.position - new Vector3(0.5f, 0, 0), Vector2.down, rayLength, groundLayer.value) ||
            Physics2D.Raycast(transform.position + new Vector3(0.5f, 0, 0), Vector2.down, rayLength, groundLayer.value) ||
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
        StartCoroutine(RespawnPlayer());
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(1f);
        transform.position = startPosition;
        active = true;
        _collider.enabled = true;
        MiniJump();
    }

    public void SetStartingPosition()
    {
        startPosition = transform.position;
    }
}

