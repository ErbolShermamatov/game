using UnityEngine;

public class EnemyArcherAI : MonoBehaviour
{
    public float visionRange = 7f;
    public float aimDelay = 1.5f;
    public float arrowSpeed = 15f;
    
    public float runAwayRange = 4f;
    public float runSpeed = 3f;
    public float groundCheckDistance = 1.2f; 

    public GameObject arrowPrefab;
    public Transform firePoint;

    private Transform player;
    private Animator anim;
    
    private bool isAiming = false;
    private bool isShooting = false;
    private float aimTimer = 0f;
    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null || isDead) return;

        float distance = Vector2.Distance(transform.position, player.position);

        bool isGrounded = false;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, groundCheckDistance);
        
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Ground"))
            {
                isGrounded = true;
                break; 
            }
        }

        if (isGrounded && distance <= runAwayRange)
        {
            if (isAiming || isShooting)
            {
                isAiming = false;
                isShooting = false;
                anim.SetTrigger("CancelAim");
            }
            
            anim.SetBool("IsRunning", true);

            float runDir = Mathf.Sign(transform.position.x - player.position.x);
            transform.localScale = new Vector3(runDir, 1, 1);
            transform.position += new Vector3(runDir * runSpeed * Time.deltaTime, 0, 0);
            
            return; 
        }
        else
        {
            anim.SetBool("IsRunning", false);
        }

        if (distance <= visionRange + 2f && !isShooting && !isAiming) 
        {
            float dir = Mathf.Sign(player.position.x - transform.position.x);
            if (dir != 0) transform.localScale = new Vector3(dir, 1, 1);
        }

        if (distance <= visionRange)
        {
            if (!isAiming && !isShooting) 
            {
                isAiming = true;
                anim.SetTrigger("Aim");
                aimTimer = aimDelay;
            }
            else if (isAiming)
            {
                aimTimer -= Time.deltaTime;
                if (aimTimer <= 0f)
                {
                    isAiming = false;
                    isShooting = true; 
                    anim.SetTrigger("Shoot");
                }
            }
        }
        else
        {
            if (isAiming || isShooting)
            {
                isAiming = false;
                isShooting = false;
                anim.SetTrigger("CancelAim");
            }
        }
    }

    public void SpawnArrowEvent()
    {
        if (player == null) return;

        GameObject arrowGO = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Arrow arrow = arrowGO.GetComponent<Arrow>();
        
        Collider2D archerCollider = GetComponent<Collider2D>();
        Collider2D arrowCollider = arrowGO.GetComponent<Collider2D>();
        if (archerCollider != null && arrowCollider != null)
        {
            Physics2D.IgnoreCollision(archerCollider, arrowCollider);
        }

        if (arrow != null)
        {
            Vector3 targetPosition = player.position + Vector3.up * 0.7f;
            Vector2 displacement = targetPosition - firePoint.position;
            
            float time = displacement.magnitude / arrowSpeed;
            if (time < 0.05f) time = 0.05f;
            
            float vX = displacement.x / time;
            float vY = (displacement.y - 0.5f * Physics2D.gravity.y * time * time) / time;

            arrow.LaunchBallistic(new Vector2(vX, vY));
        }
    }

    public void FinishShootEvent()
    {
        isShooting = false;
    }

    public void SetDead()
    {
        isDead = true;
        isAiming = false;
        isShooting = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}