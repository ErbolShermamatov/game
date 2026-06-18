using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 1.5f;
    public Transform[] patrolPoints;
    private int currentPoint = 0;

    public float visionDistance = 4f;
    public LayerMask playerLayer;
    public float chaseMemoryTime = 2f;
    private float currentMemoryTime;

    public LayerMask groundLayer;
    public float wallCheckDistance = 0.6f;

    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
    private float nextAttackTime = 0f;
    private bool isAttacking = false;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isChasing = false;
    private Transform player;

    [HideInInspector]
    public float knockbackTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
            isAttacking = false;
            return; 
        }

        if (player == null) return;

        if (isAttacking)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            if (Time.time >= nextAttackTime)
            {
                StartAttack();
            }
            rb.velocity = new Vector2(0, rb.velocity.y);
            anim.SetFloat("Speed", 0f);
            return; 
        }

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector2.Angle(new Vector2(transform.localScale.x, 0), directionToPlayer);
        
        bool canSeePlayer = (distance < visionDistance && angleToPlayer < 45f);

        if (canSeePlayer)
        {
            isChasing = true;
            currentMemoryTime = chaseMemoryTime;
        }
        else if (isChasing)
        {
            currentMemoryTime -= Time.deltaTime;
            if (currentMemoryTime <= 0f) isChasing = false;
        }

        Vector2 facingDir = new Vector2(transform.localScale.x, 0);
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, facingDir, wallCheckDistance, groundLayer);

        if (wallHit.collider != null && isChasing)
        {
            isChasing = false;
            currentMemoryTime = 0f;
        }

        if (isChasing) Chase();
        else Patrol();
    }

    void StartAttack()
    {
        isAttacking = true;
        
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        transform.localScale = new Vector3(dir, 1, 1);

        int randomAttack = Random.Range(0, 2);
        if (randomAttack == 0) anim.SetTrigger("Attack1");
        else anim.SetTrigger("Attack2");
    }


   public void StrikePlayer()
    {
        if (player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange + 0.3f)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1, transform);
            }
        }
    }

    public void FinishAttack()
    {
        isAttacking = false;
        nextAttackTime = Time.time + attackCooldown;
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPoint];
        float dir = Mathf.Sign(target.position.x - transform.position.x);
        transform.localScale = new Vector3(dir, 1, 1);

        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
        anim.SetFloat("Speed", speed);

        if (Mathf.Abs(transform.position.x - target.position.x) < 0.2f)
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
        }
    }

    void Chase()
    {
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        transform.localScale = new Vector3(dir, 1, 1);
        rb.velocity = new Vector2(dir * speed * 1.5f, rb.velocity.y);
        anim.SetFloat("Speed", speed * 1.5f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 direction = new Vector2(transform.localScale.x, 0);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + direction * visionDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + direction * wallCheckDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isChasing = true;
            currentMemoryTime = chaseMemoryTime; 

            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1, transform);
            }
        }
    }
}