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

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        extraJumps = numExtrajumps;
        dashTime = startDashTime;
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
                animator.SetBool("isJumping", false);
                extraJumps = numExtrajumps;
                canDash = true;
            }
            else
            {
                animator.SetBool("isJumping", true);
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


            //Jumping
            if (Input.GetKeyDown(KeyCode.Space) && extraJumps > 0 && !isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, extraJumpForce);
                extraJumps--;
            }
            else if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            //Dashing
            if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && !isGrounded)
            {
                if (canDash)
                {
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

    IEnumerator DieCoroutine()
    {
        rb.Sleep();
        isDead = true;
        animator.SetBool("isDead", true);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Level 1");
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
            Die();
           
        }

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}
