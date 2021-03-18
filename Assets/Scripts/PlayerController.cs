using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float extraJumpForce;
    private float moveInput;

    public float hangTime; //Short time to jump after walking off a surface
    private float hangCounter;

    public float jumpBufferLength; //Short Time to jump before hitting ground
    private float jumpBufferCount;

    public ParticleSystem footSteps;
    private ParticleSystem.EmissionModule footEmission;
    public ParticleSystem impactEffect;
    private bool wasOnGround;

    private Rigidbody2D rb;

    private bool facingRight = true;

    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    private int extraJumps;
    public int numExtrajumps;

    public float dashSpeed;
    public float startDashTime;
    private float dashTime;
    private bool isDashing;
    private int direction = 1;
    private bool canDash = true;

    public Animator animator;

    private bool isDead = false;

    public Transform spawnPos;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        extraJumps = numExtrajumps;
        dashTime = startDashTime;

        footEmission = footSteps.emission;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!isDead)
        {
            //Flip Sprite Direction
            if (!facingRight && moveInput > 0)
            {
                Flip();
            }
            else if (facingRight && moveInput < 0)
            {
                Flip();
            }
        }
    }

    private void Update()
    {
        if (!isDead)
        {
            //Check if on ground
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

            if (isGrounded)
            {
                hangCounter = hangTime;
                animator.SetBool("isJumping", false);
                extraJumps = numExtrajumps;
                canDash = true;
            }
            else
            {
                animator.SetBool("isJumping", true);
                hangCounter -= Time.deltaTime;
            }

            //Horiz Movement
            moveInput = Input.GetAxisRaw("Horizontal");

            animator.SetFloat("speed", Mathf.Abs(moveInput * speed));
            if (moveInput > 0)
            {
                direction = 1;
            }
            else if (moveInput < 0)
            {
                direction = -1;
            }
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

            //Footstep Particle Effect
            if (moveInput != 0 && isGrounded)
            {
                footEmission.rateOverTime = 25f;
            }
            else
            {
                footEmission.rateOverTime = 0f;
            }

            //Impact Effect
            if(!wasOnGround && isGrounded)
            {
                impactEffect.gameObject.SetActive(true);
                impactEffect.Stop();
                impactEffect.transform.position = footSteps.transform.position;
                impactEffect.Play();
            }

            wasOnGround = isGrounded;

            //Jump Buffer
            if (Input.GetKeyDown(KeyCode.Space)){
                jumpBufferCount = jumpBufferLength;
            }
            else
            {
                jumpBufferCount -= Time.deltaTime;
            }

            

            //Jumping
            if (jumpBufferCount >= 0 && extraJumps > 0 && !(isGrounded || hangCounter > 0))
            {
                FindObjectOfType<AudioManager>().Play("Player Jump");
                rb.velocity = new Vector2(rb.velocity.x, extraJumpForce);
                extraJumps--;
                jumpBufferCount = 0;
            }
            else if (jumpBufferCount >= 0 && (isGrounded || hangCounter > 0))
            {
                FindObjectOfType<AudioManager>().Play("Player Jump");
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpBufferCount = 0;
            }

            //Dashing
            if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && !isGrounded)
            {
                if (canDash)
                {
                    FindObjectOfType<AudioManager>().Play("Player Dash");
                    animator.SetBool("isDashing", true);
                    isDashing = true;
                }
            }

            if (isDashing)
            {
                if (dashTime > 0)
                {
                    rb.velocity = new Vector2(direction * dashSpeed, 0);
                    dashTime -= Time.deltaTime;
                }
                else
                {
                    animator.SetBool("isDashing", false);
                    isDashing = false;
                    canDash = false;
                    dashTime = startDashTime;
                }
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
        
    }

    public void Die()
    {
        StartCoroutine(DieCoroutine());        
    }

    public void Respawn()
    {
        transform.position = spawnPos.position;
        rb.WakeUp();
        animator.SetBool("isDead", false);
        isDead = false;
    }

    IEnumerator DieCoroutine()
    {
        rb.Sleep();
        isDead = true;
        animator.SetBool("isDead", true);
        yield return new WaitForSeconds(0.5f);
        Respawn();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Check if touching wall
        if (collision.gameObject.layer.ToString().Equals("Ground"))
        {
            isDashing = false;
            canDash = false;
            dashTime = startDashTime;
        }
        if (collision.gameObject.CompareTag("Spike"))
        {
            FindObjectOfType<AudioManager>().Play("Player Spiked");
            Die();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            if (!collision.gameObject.transform.Equals(spawnPos))
            {
                FindObjectOfType<AudioManager>().Play("Checkpoint");
                spawnPos = collision.gameObject.transform;
            }
           
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}
