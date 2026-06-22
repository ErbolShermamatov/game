using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 1.5f;
    public Transform[] patrolPoints;
    
    public float visionDistance = 4f;
    public LayerMask playerLayer;
    public float chaseMemoryTime = 2f;
    public LayerMask groundLayer;
    public float wallCheckDistance = 0.6f;

    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;

    [HideInInspector] public float knockbackTimer = 0f;

    private int currentPoint = 0;
    private float currentMemoryTime;
    private float nextAttackTime = 0f;
    private bool isAttacking = false;
    private bool isChasing = false;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;

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
            StopMovement();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            HandleAttack();
            return; 
        }

        CheckVisibility(distanceToPlayer);
        CheckWalls();

        if (isChasing) Chase();
        else Patrol();
    }

    private void HandleAttack()
    {
        StopMovement();
        
        if (Time.time >= nextAttackTime)
        {
            isAttacking = true;
            
            float dir = Mathf.Sign(player.position.x - transform.position.x);
            transform.localScale = new Vector3(dir, 1, 1);

            int randomAttack = Random.Range(0, 2);
            anim.SetTrigger(randomAttack == 0 ? "Attack1" : "Attack2");
        }
    }

    private void CheckVisibility(float distanceToPlayer)
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector2.Angle(new Vector2(transform.localScale.x, 0), directionToPlayer);
        
        bool canSeePlayer = (distanceToPlayer < visionDistance && angleToPlayer < 45f);

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
    }

    private void CheckWalls()
    {
        Vector2 facingDir = new Vector2(transform.localScale.x, 0);
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, facingDir, wallCheckDistance, groundLayer);

        if (wallHit.collider != null && isChasing)
        {
            isChasing = false;
            currentMemoryTime = 0f;
        }
    }

    private void StopMovement()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetFloat("Speed", 0f);
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
        
        float chaseSpeed = speed * 1.5f;
        rb.velocity = new Vector2(dir * chaseSpeed, rb.velocity.y);
        anim.SetFloat("Speed", chaseSpeed);
    }

    public void StrikePlayer()
    {
        if (player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange + 0.3f)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.TakeDamage(1, transform);
        }
    }

    public void FinishAttack()
    {
        isAttacking = false;
        nextAttackTime = Time.time + attackCooldown;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isChasing = true;
            currentMemoryTime = chaseMemoryTime; 

            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.TakeDamage(1, transform);
        }
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
}