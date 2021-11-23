using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour, ICollisionHandler
{

    [SerializeField]
    protected Transform knifePos;

    [SerializeField]
    protected float movementSpeed;

    protected bool facingRight;

    [SerializeField]
    private GameObject knifePrefab;

    [SerializeField]
    protected Stat healthStat;

    [SerializeField]
    private EdgeCollider2D swordCollider;

    [SerializeField]
    private List<string> damageSources;

    public abstract bool IsDead { get; }

    public bool Attack { get; set; }

    public bool TakingDamage { get; set; }

    public Animator MyAnimator { get; private set; }

    public EdgeCollider2D SwordCollider
    {
        get
        {
            return swordCollider;
        }
    }
    
    public virtual void Start()
    {
        facingRight = true;    

        MyAnimator = GetComponent<Animator>();

        healthStat.Initialize();
    }

    void Update()
    {
        
    }

    public abstract IEnumerator TakeDamage();

    public abstract IEnumerator TakeDamageArea();

    public abstract IEnumerator Heal();

    public abstract IEnumerator Heal2();

    public abstract void Death();

    public void ChangeDirection()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
    }

    public virtual void ThrowKnife(int value)
    {
        if (facingRight)
        {
            GameObject tmp = (GameObject)Instantiate(knifePrefab, knifePos.position, Quaternion.Euler(new Vector3(0,0,-90)));
            tmp.GetComponent<Knife>().Initialize(Vector2.right); 
        }
        else
        {
            GameObject tmp = (GameObject)Instantiate(knifePrefab, knifePos.position, Quaternion.Euler(new Vector3(0,0,90)));
            tmp.GetComponent<Knife>().Initialize(Vector2.left); 
        }
    }

    public void MeleeAttack()
    {
        SoundManagerScript.PlaySound("attackSound");
        SwordCollider.enabled = true;
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (damageSources.Contains(other.tag))
        {
            StartCoroutine(TakeDamage());
        }
    }

        public void CollisionEnter(string colliderName, GameObject other)
    {
        if (colliderName == "DmgArea" && other.tag == "Enemy")
        {
            StartCoroutine(TakeDamageArea());
        }
    }

    public void CollisionEnter2(string colliderName, GameObject other)
    {
        if (colliderName == "Healer" && other.tag == "Heal")
        {
            StartCoroutine(Heal());
        }
    }

    public void CollisionEnter3(string colliderName, GameObject other)
    {
        if (colliderName == "Sehat" && other.tag == "Heal2")
        {
            StartCoroutine(Heal2());
        }
    }
}
