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
            return;
        }

        if (player == null) return;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector2.Angle(new Vector2(transform.localScale.x, 0), directionToPlayer);
        float distance = Vector2.Distance(transform.position, player.position);

        bool canSeePlayer = (distance < visionDistance && angleToPlayer < 45f);

        if (canSeePlayer)
        {
            isChasing = true;
            currentMemoryTime = chaseMemoryTime;
        }
        else if (isChasing)
        {
            currentMemoryTime -= Time.deltaTime;
            if (currentMemoryTime <= 0f)
            {
                isChasing = false;
            }
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isChasing = true;
            currentMemoryTime = chaseMemoryTime; 
        }
    }
}