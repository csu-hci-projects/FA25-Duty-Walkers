using UnityEngine;
using UnityEngine.AI;
public class ZombieController : MonoBehaviour
{
    NavMeshAgent nav;
    Transform player;
    //Animator controller;
    Animator animator;
    private float health;
    private float baseHealth = 20f;
    private float baseSpeed;
    ZombieSystem zSystem;
    CapsuleCollider capsuleCollider;
    private bool isDead = false;

    private int bodyShot = 10;
    private int killBodyShot = 70;
    //private float headShot;

    public float timeToAttack;
    private float attackTimer = 0f;
    public AudioClip zombieAttackSound;

    
    public int damage = 25;

    private int speedHash;
    private int attackHash;
    private int isDeadHash;
    void Awake()
    {
        nav = GetComponent <NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //animator = GetComponentInParent<Animator>();
        animator = GetComponent<Animator>();
        zSystem = FindAnyObjectByType<ZombieSystem>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        baseSpeed = nav.speed;

        ZombieProgression();

        speedHash = Animator.StringToHash("Speed");
        attackHash = Animator.StringToHash("Attack");
        isDeadHash = Animator.StringToHash("IsDead");
    }

    void Update()
    {
        Debug.Log("Script Running");
        if (!isDead)
        {
            nav.SetDestination(player.position);
            float speed = nav.velocity.magnitude;
            animator.SetFloat(speedHash, speed);
            float distance = Vector3.Distance(transform.position, player.position);
            if(distance < 3)
            {
                attackTimer += Time.deltaTime / 3;
                if(attackTimer >= timeToAttack)
                {
                    Attack();
                }
            }
            else
            {
                if(attackTimer > 0) attackTimer -= Time.deltaTime * 2;
                else attackTimer = 0;
            }
        }
    }
    private void ZombieProgression()
    {
        health = baseHealth * Mathf.Pow(1.1f, zSystem.round -1);

        if(zSystem.round == 2)
        {
            nav.speed = baseSpeed + 0.3f;
        }

        if(zSystem.round % 3 == 0)
        {
            nav.speed = baseSpeed * 3f;
        }
    }

    private bool Attack()
    {
        if(attackTimer > timeToAttack)
        {
            Debug.Log("Attack() fired, setting trigger");
            //sound
            attackTimer = 0;
            animator.SetTrigger(attackHash);

            if(zombieAttackSound) AudioSource.PlayClipAtPoint(zombieAttackSound, transform.position);

            var health = player.GetComponent<PlayerHealth>();
            if(health != null) health.SendMessage("PlayerDamage", damage, SendMessageOptions.DontRequireReceiver);
            return true;
        }
        return false;
    }
    public void TakeDamage(float damageTaken)
        {
            health -= damageTaken;
        if (!isDead)
        {
            ZombieSystem.AddPoints(bodyShot);
            if (health <= 0)
            {
                ZombieSystem.AddPoints(killBodyShot);
                Death();
            }
        }
        
        }
    
    private void Death()
    {
        isDead = true;
        nav.isStopped = true;
        capsuleCollider.isTrigger = true;

        animator.SetBool(isDeadHash, true);

        if(zSystem != null)
        {
            zSystem.OnZombieKilled();
        }
        Destroy(gameObject, 4f);
    }
}
