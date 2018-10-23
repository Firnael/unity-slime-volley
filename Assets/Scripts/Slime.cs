using System.Collections;
using UnityEngine;

public class Slime : MonoBehaviour {

    private Rigidbody2D body;
    private AudioSource source;

    public AudioClip sound;
    public float speed;
    public float fallSpeed;
    public float jumpForce;
    public float jumpForceInitialBoost;
    public float startJumpTime;
    public float dashForce;
    public float dashDuration;
    public float dashCooldown;
    public bool canIncreaseAirTime = false;
    public bool grounded = false;

    public string dashLeftInput;
    public string dashRightInput;
    public string jumpInput;
    public string horizontalInput;

    private bool canDash = true;
    public bool isDashing = false;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();
    }

    public void Init(Vector3 position)
    {
        transform.position = position;
        grounded = false;
        canIncreaseAirTime = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            grounded = false;
        }
    }

    void FixedUpdate() {

        if(!isDashing)
        {
            MoveUpdate();
            JumpUpdate();
        }
        DashUpdate();
    }

    void DashUpdate()
    {
        if(canDash)
        {
            if (Input.GetAxis(dashLeftInput) != 0)
            {
                isDashing = true;
                StartCoroutine(Dash(dashDuration, -dashForce, body.velocity.x));
            }
            else if (Input.GetAxis(dashRightInput) != 0)
            {
                isDashing = true;
                StartCoroutine(Dash(dashDuration, dashForce, body.velocity.x));
            }
        }
    }

    IEnumerator Dash(float duration, float directedDashForce, float currentHorizontalVelocity)
    {
        canDash = false;
        float time = 0;
        source.PlayOneShot(sound, 1);
        while(duration > time)
        {
            time += Time.deltaTime;
            body.MovePosition(transform.position + new Vector3(directedDashForce * Time.deltaTime, 0, 0));
            yield return 0;
        }

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void JumpUpdate()
    {
        if (Input.GetButton(jumpInput))
        {
            if (grounded)
            {
                body.AddForce(new Vector2(0, jumpForce * jumpForceInitialBoost), ForceMode2D.Impulse);
                startJumpTime = 0;
            }
            else
            {
                startJumpTime += Time.deltaTime;
                // Debug.Log("time spent: " + startJumpTime.ToString());
                float forceToApply = jumpForce / (100 * startJumpTime);
                // Debug.Log("force to apply: " + forceToApply.ToString());
                body.AddForce(new Vector2(0, forceToApply), ForceMode2D.Impulse);
            }
        } 
    }

    void MoveUpdate()
    {
        float moveHorizontal = Input.GetAxis(horizontalInput);
        if (!grounded)
        {
            moveHorizontal = moveHorizontal / 2;

            // Fall faster
            if (body.velocity.y < 2)
            {
                body.AddForce(new Vector2(0, -fallSpeed), ForceMode2D.Impulse);
            }
        }

        body.AddForce(new Vector2(moveHorizontal * speed, 0), ForceMode2D.Impulse);
    }

}
