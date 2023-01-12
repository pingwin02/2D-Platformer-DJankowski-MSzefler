using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering.Universal;

public class FoxController : MonoBehaviour
{
    public float moveSpeed = 0.1f;

    public float jumpForce = 6.0f;

    private Rigidbody2D rigidBody;

    public LayerMask groundLayer;

    const float rayLength = 1.50f;

    const float rayLength2 = 0.7f;

    private Collider2D _collider;

    private SpriteRenderer _renderer;

    private bool active = true;

    private Vector3 startPosition;

    private Animator animator;

    private bool isWalking = false;

    private bool isFacingRight = true;

    private bool doubleJumped = false;

    private bool immortalMode = false;

    private bool isStartMode = false;

    [SerializeField] AudioClip bonusSound;

    [SerializeField] AudioClip deathSound;

    [SerializeField] AudioClip killSound;

    [SerializeField] AudioClip heartSound;

    [SerializeField] AudioClip gameOverMusic;

    [SerializeField] AudioClip levelCompletedMusic;

    private AudioSource source;

    public Light2D DayLight;

    public Light2D DungeonLight;

    private bool inDungeon = false;

    private bool skullsWarning = false;

    private Vector3 dungeonRespPoint;

    public GameObject[] startObjects;

    public GameObject[] doorObject;

    Vector3 currentPosition;


    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider2D>();
        _renderer = GetComponent<SpriteRenderer>();
        SetStartingPosition();
    
        StartCoroutine(ShowObjects(startObjects));
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isStartMode)
        {
            StopAllCoroutines();
            GameManager.instance.InGame();
            this.transform.position = currentPosition;
            _renderer.enabled = true;
            _collider.enabled = true;
            rigidBody.gravityScale = 2f;
            active = true;
            isStartMode = false;

            if (skullsWarning)
            {
                GameManager.instance.DungeonWarning();
                skullsWarning = false;
            }
            return;
        }

        if (!active) return;

        isWalking = false;

        if (GameManager.instance.currentGameState == GameState.GS_GAME)
        {

            #if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.G))
                {
                    immortalMode = !immortalMode;
                }
            #endif

            if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && !isWall(Vector2.right))
            {
                transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
                isWalking = true;

                if (!isFacingRight) Flip();
            }
            if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && !isWall(Vector2.left))
            {
                transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
                isWalking = true;

                if (isFacingRight) Flip();
            }
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
        
        Debug.DrawRay(transform.position, rayLength * Vector3.down, Color.white, 1, false);
        Debug.DrawRay(transform.position - new Vector3(0.3f, 0, 0), rayLength * Vector3.down, Color.white, 1, false);
        Debug.DrawRay(transform.position + new Vector3(0.3f, 0, 0), rayLength * Vector3.down, Color.white, 1, false);
        Debug.DrawRay(transform.position, Vector2.right * rayLength2, Color.white, 1, false);
        Debug.DrawRay(transform.position, Vector2.left * rayLength2, Color.white, 1, false);
        

        StartCoroutine(switchLight());

        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isGrounded", isGrounded());
        animator.SetBool("isDead", false);
        if (isGrounded())
        {
            animator.SetBool("didKill", false);
            doubleJumped = false;
            rigidBody.velocity *= new Vector2(0, 1);
        }
    }
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        source = GetComponent<AudioSource>();

        DayLight.intensity = 1f;
        DungeonLight.intensity = 0f;

        source.volume = 0.1f;

    }

    bool isGrounded()
    {
        return (Physics2D.Raycast(transform.position - new Vector3(0.3f, 0, 0), Vector2.down, rayLength, groundLayer.value) ||
            Physics2D.Raycast(transform.position + new Vector3(0.3f, 0, 0), Vector2.down, rayLength, groundLayer.value) ||
            Physics2D.Raycast(this.transform.position, Vector2.down, rayLength, groundLayer.value));
    }

    bool isWall(Vector2 direction)
    {
        return (Physics2D.Raycast(transform.position, direction, rayLength2, groundLayer.value));
    }

    void Jump()
    {
        if (isGrounded())
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);
        }
        else if (!doubleJumped && !inDungeon)
        {
            DoubleJump();
        }
    }

    void DoubleJump()
    {
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
            animator.SetBool("didKill", false);
            source.PlayOneShot(deathSound, 25);
            _collider.enabled = false;
            rigidBody.velocity = Vector3.zero;
            MiniJump();
            GameManager.instance.AddHealth(-1);

            if (GameManager.instance.currentGameState == GameState.GS_GAME)
                StartCoroutine(RespawnPlayer());
            if (GameManager.instance.currentGameState == GameState.GS_GAME_OVER)
            {
                source.clip = null;
                source.PlayOneShot(gameOverMusic, 50);
            }
        }
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(1.5f);
        if (inDungeon)
        {
            transform.position = dungeonRespPoint;
        }
        else
        {
            transform.position = startPosition;
        }
        active = true;
        _collider.enabled = true;
        MiniJump();
    }

    public void SetStartingPosition()
    {
        startPosition = transform.position;
        dungeonRespPoint = new Vector3(-35, 2, -2);
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
            source.PlayOneShot(bonusSound, 2);
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("Finish"))
        {
            GameManager.instance.LevelCompleted();
            if (GameManager.instance.currentGameState == GameState.GS_LEVELCOMPLETED)
            {
                source.clip = null;
                source.PlayOneShot(levelCompletedMusic, 50);
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            if (transform.position.y - 0.8 > other.gameObject.transform.position.y)
            {
                animator.SetBool("didKill", true);
                MiniJump();
                GameManager.instance.AddKilledEnemy();
                source.PlayOneShot(killSound, 25);
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
            source.PlayOneShot(bonusSound, 2);
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("Heart"))
        {
            GameManager.instance.AddHealth(2);
            source.PlayOneShot(heartSound, 2);
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("MovingPlatform"))
        {
            transform.SetParent(other.transform);
        }
        else if (other.CompareTag("DoorKey"))
        {
            source.PlayOneShot(bonusSound, 2);
            StartCoroutine(ShowObjects(doorObject));
            other.gameObject.SetActive(false);

        }
        else if (other.CompareTag("Crusher"))
        {
            Die();
        }
        else if (other.CompareTag("DungeonTrap"))
        {
            skullsWarning = true;
            StartCoroutine(ShowObjects(doorObject));
            other.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("MovingPlatform"))
        {
            transform.SetParent(null);
        }
    }

    public bool GetActive()
    {
        return active;
    }

    private IEnumerator switchLight()
    {

        if (this.transform.position.x < -30)
        {
            inDungeon = true;
        }
        else
        {
            inDungeon = false;
        }

        if (inDungeon && DayLight.intensity == 1f )
        {
            for (float i = 0; i < 1; i += 0.01f)
            {
                DungeonLight.intensity = i;
                DayLight.intensity = 1 - i;
                yield return new WaitForSeconds(0.01f);
            }
            DayLight.intensity = 0f;
            DungeonLight.intensity = 1f;
        }
        else if (!inDungeon && DayLight.intensity == 0f )
        {
            for (float i = 1; i >= 0 ; i -= 0.01f)
            {
                DungeonLight.intensity = i;
                DayLight.intensity = 1 - i;
                yield return new WaitForSeconds(0.01f);
            }

            DayLight.intensity = 1f;
            DungeonLight.intensity = 0f;
        }

        yield return 0;
    }

    private IEnumerator ShowObjects(GameObject[] keys)
    {
        isStartMode = true;
        GameManager.instance.StartScreen();
        foreach (GameObject key in keys)
        {
            if (key.transform.position.x < -28.0f)
            {
                DayLight.intensity = 0f;
                DungeonLight.intensity = 1f;
            }
            else
            {
                DayLight.intensity = 1f;
                DungeonLight.intensity = 0f;
            }
            StartCoroutine(showObject(key.transform.position));
            yield return new WaitForSeconds(3f);
        }
        GameManager.instance.InGame();
        isStartMode = false;
    }

    private IEnumerator showObject(Vector3 position)
    {
        _renderer.enabled = false;
        active = false;
        _collider.enabled = false;
        rigidBody.gravityScale = 0f;
        rigidBody.velocity = Vector2.zero;
        currentPosition = this.transform.position;
        this.transform.position = position;
        yield return new WaitForSeconds(3f);
        this.transform.position = currentPosition;
        _renderer.enabled = true;
        _collider.enabled = true;
        rigidBody.gravityScale = 2f;
        active = true;
        if (skullsWarning)
        {
            GameManager.instance.DungeonWarning();
        }
    }

}
