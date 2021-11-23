using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DeadEventHandler();

public class Player : Character
{
    private static Player instance;

    public event DeadEventHandler Dead;

    public static Player Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<Player>();
            }
            return instance;
        }
    }
    
    [SerializeField]
    private Transform[] groundPoints;

    [SerializeField]
    private float groundRadius;

    [SerializeField]
    private LayerMask whatIsGround;

    [SerializeField]
    private bool airControl;

    [SerializeField]
    private float jumpForce;

    private bool immortal = false;

    private SpriteRenderer spriteRenderer;
    
    [SerializeField]
    private float immortalTime;
    private float direction;
    private bool move;
    AudioSource audioSrc;

    private float btnHorizontal;

    public Rigidbody2D MyRigidbody { get; set; }

    public bool Slide { get; set; }
    public bool Jump { get; set; }
    public bool OnGround { get; set; }

    AudioSource jumpsound;

    public override bool IsDead
    {
        get
        {
            if (healthStat.CurrentValue <= 0)
            {
                OnDead();
            }
            return healthStat.CurrentValue <= 0;
        }
    }

    private Vector2 startPos;

    public override void Start()
    {
        base.Start();
        startPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        MyRigidbody = GetComponent<Rigidbody2D>();
        audioSrc = GetComponent<AudioSource> ();
        
    }

    void Update()
    {
        if (!TakingDamage && !IsDead)
        {
            if (transform.position.y <= -14f)
            {
                Death();
            }
            HandleInput();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!TakingDamage && !IsDead)
        {
            float horizontal = Input.GetAxis("Horizontal");

            OnGround = IsGrounded();

            if (move)
            {
                this.btnHorizontal = Mathf.Lerp(btnHorizontal, direction, Time.deltaTime * 2);
                HandleMovement(direction);
                Flip(direction);
                {
                if (!audioSrc.isPlaying)
                    audioSrc.Play ();
                    }
                }
                else
                {
                    HandleMovement(horizontal);
                    Flip(horizontal);
                    audioSrc.Stop ();
                }

            HandleLayers();
        }  
    }

    public void OnDead()
    {
        if (Dead != null)
        {
            Dead();
        }
    }

    private void HandleMovement(float horizontal)
    {
        if (MyRigidbody.velocity.y < 0)
        {
            MyAnimator.SetBool("land", true);
        }
        if (!Attack && !Slide && (OnGround || airControl))
        {
            MyRigidbody.velocity = new Vector2(horizontal * movementSpeed, MyRigidbody.velocity.y);
        }
        if (Jump && MyRigidbody.velocity.y == 0)
        {
            MyRigidbody.AddForce(new Vector2(0, jumpForce));
            SoundManagerScript.PlaySound("jumpSound");
        }

        MyAnimator.SetFloat("speed", Mathf.Abs(horizontal));
    }

    private void HandleInput()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            MyAnimator.SetTrigger("jump");
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            MyAnimator.SetTrigger("attack");
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
           MyAnimator.SetTrigger("slide");
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
           MyAnimator.SetTrigger("throw");
        }
    }

    private void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
           ChangeDirection();
        }
    }

    private bool IsGrounded()
    {
        if(MyRigidbody.velocity.y <=0)
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);

                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != gameObject)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void HandleLayers()
    {
        if (!OnGround)
        {
            MyAnimator.SetLayerWeight(1, 1);
        }
        else
        {
            MyAnimator.SetLayerWeight(1, 0);
        }
    }

    public override void ThrowKnife(int value)
    {
        if (!OnGround && value == 1 || OnGround && value == 0)
        {
            base.ThrowKnife(value);
        }
    }

    private IEnumerator IndicateImmortal()
    {
        while (immortal)
        {
            spriteRenderer.enabled = false;

            yield return new WaitForSeconds(.1f);

            spriteRenderer.enabled = true;

            yield return new WaitForSeconds(.1f);
        }
    }

    public override IEnumerator TakeDamage()
    {
        if (!immortal)
        {
             healthStat.CurrentValue -= 10;

            if (!IsDead)
            {
                MyAnimator.SetTrigger("damage");
                immortal = true;
                StartCoroutine(IndicateImmortal());
                yield return new WaitForSeconds(immortalTime);
            
                immortal = false;
            }
            else
            {
                MyAnimator.SetLayerWeight(1, 0);
                MyAnimator.SetTrigger("die");
            }
        }
    }

        public override IEnumerator TakeDamageArea()
    {
        if (!immortal)
        {
             healthStat.CurrentValue -= 2;

            if (!IsDead)
            {
                StartCoroutine(IndicateImmortal());
                yield return new WaitForSeconds(immortalTime);
            }
            else
            {
                MyAnimator.SetLayerWeight(1, 0);
                MyAnimator.SetTrigger("die");
            }
        }
    }

    public override IEnumerator Heal()
    {
        if (!immortal)
        {
             healthStat.CurrentValue += 15;

            if (!IsDead)
            {
                StartCoroutine(IndicateImmortal());
                yield return new WaitForSeconds(immortalTime);
            }
            else
            {
                MyAnimator.SetLayerWeight(1, 0);
                MyAnimator.SetTrigger("die");
            }
        }
    }

    public override IEnumerator Heal2()
    {
        if (!immortal)
        {
             healthStat.CurrentValue += 10;

            if (!IsDead)
            {
                StartCoroutine(IndicateImmortal());
                yield return new WaitForSeconds(immortalTime);
            }
            else
            {
                MyAnimator.SetLayerWeight(1, 0);
                MyAnimator.SetTrigger("die");
            }
        }
    }

    public override void Death()
    {
        MyRigidbody.velocity = Vector2.zero;
        MyAnimator.SetTrigger("idle");
        healthStat.CurrentValue = healthStat.MaxVal;
        transform.position = startPos;
    }

    public void BtnJump()
    {
        MyAnimator.SetTrigger("jump");
        Jump = true;
    }

    public void BtnSlide()
    {
        MyAnimator.SetTrigger("slide");
    }

    public void BtnThrow()
    {
        MyAnimator.SetTrigger("throw");
    }

    public void BtnAttack()
    {
        MyAnimator.SetTrigger("attack");
    }

    public void BtnMove(float direction)
    {
        this.direction = direction;
        this.move = true;
    }

    public void BtnStopMove()
    {
        this.direction = 0;
        this.btnHorizontal = 0;
        this.move = false;
    }
}
