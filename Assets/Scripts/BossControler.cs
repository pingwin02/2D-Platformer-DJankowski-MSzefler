using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossControler : MonoBehaviour
{

    public int health;

    public int damage;

    public LayerMask groundLayer;

    private Collider2D _collider;

    private float rayLength = 2.25f;

    private float timeBtwDamage = 1.5f;

    public Canvas healthBarCanvas;

    public Slider healthBar;

    public bool isDead;

    private Transform playerPos;

    public float speed;

    private Rigidbody2D rigidBody;

    private SpriteRenderer spriteRenderer;

    public float jumpForce;

    private float startYPosition;

    private Animator anim;

    public GameObject key;

    private void Start()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        startYPosition = rigidBody.transform.position.y;

        healthBarCanvas.enabled = false;

        anim = GetComponent<Animator>();

        _collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if ( Vector2.Distance(rigidBody.transform.position, playerPos.position) <= 20f)
        {
            healthBarCanvas.enabled = true;

            Flip();

            if (health > 5)
            {
                Vector2 target = new Vector2(playerPos.position.x, rigidBody.transform.position.y);
                rigidBody.transform.position = Vector2.MoveTowards(rigidBody.transform.position, target, speed * Time.deltaTime);
            }

            if (health <= 5)
            {
                //rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);
                Vector2 target = new Vector2(playerPos.position.x, rigidBody.transform.position.y);
                rigidBody.transform.position = Vector2.MoveTowards(rigidBody.transform.position, target, speed * Time.deltaTime);
                Jump();
            }

            if (health <= 0)
            {
                _collider.enabled = false;
                anim.SetBool("isDead", true);
                StartCoroutine(KillOnAnimationEnd());
            }

            key.transform.position = new Vector2(transform.position.x, transform.position.y + 1.0f);

            // give the player some time to recover before taking more damage !
            if (timeBtwDamage > 0)
            {
                timeBtwDamage -= Time.deltaTime;
            }
            if (timeBtwDamage <= 0)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f);
            }

            healthBar.value = health;

            Debug.DrawRay(transform.position, rayLength * Vector3.down, Color.white, 1, false);

        }
        else
        {
            healthBarCanvas.enabled = false;
        }
    }

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Flip()
    {
        if (playerPos.position.x > rigidBody.transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    bool isGrounded()
    {
        return (Physics2D.Raycast(this.transform.position, Vector2.down, rayLength, groundLayer.value));
    }

    private void Jump()
    {
        if (isGrounded())
        {
            Debug.Log(rigidBody.transform.position.y);
            Debug.Log(startYPosition);
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // deal the player damage ! 
        if (other.CompareTag("Player") && isDead == false)
        {
            if (transform.position.y < other.gameObject.transform.position.y - 0.8 && timeBtwDamage <= 0)
            {
                health--;
                timeBtwDamage = 2.0f;
                spriteRenderer.color = new Color(1f, 0.4f, 0.4f);
            }
        }
    }
    private IEnumerator KillOnAnimationEnd()
    {
        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);
        key.GetComponent<Collider2D>().enabled = true;
        healthBarCanvas.enabled = false;
    }
}